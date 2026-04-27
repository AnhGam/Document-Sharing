# Tài Liệu Thiết Kế Kiến Trúc Hệ Thống (Architecture Design Document)
**Dự án:** Cloud-Based Study Document Manager
**Mô hình:** Client - Server (Self-Hosted / Cloud Ready)

---

## 1. Tóm tắt: Định hướng Nâng cấp
- **Client (Giữ nguyên WinForms):** KHÔNG CẦN phải đập bỏ WinForms để xây lại bằng framework khác (như WPF, MAUI) trừ khi bạn muốn đổi giao diện hoàn toàn. WinForms rất nhẹ và đủ tốt cho các tác vụ Desktop.
  - **Thêm:** Cơ chế chạy ngầm (System Tray), Background Worker để Sync file liên tục, và tích hợp sâu vào hệ điều hành (như Virtual Drive).
- **Server (Xây mới):** Cần một Backend API (khuyến nghị `ASP.NET Core Web API`) làm trung tâm lưu trữ, xác thực và điều phối dữ liệu.
- **Local Database (SQLite):** Vẫn giữ lại SQLite ở Client để làm **Local Cache** (bộ nhớ đệm). Nhờ SQLite, người dùng vẫn có thể xem danh sách tài liệu, mở file đã tải khi KHÔNG CÓ MẠNG (Offline Mode).

---

## 2. Thiết kế Database & Cơ chế Lưu trữ

### Database Tập Trung (Server-side)
- **Không dùng SQLite cho Server:** Phải chuyển sang dùng Database cấp Server như **PostgreSQL** hoặc **SQL Server**. Lý do: SQLite khóa toàn bộ database khi ghi (Lock), không phù hợp cho nhiều người truy cập cùng lúc.
- **Cấu trúc dữ liệu:** Kế thừa cấu trúc cũ nhưng BẮT BUỘC thêm:
  - Bảng `Users` (Quản lý tài khoản).
  - Các trường: `OwnerID`, `SharedWith`, `IsDeleted` (Soft Delete), `SyncVersion` / `LastModified` (để phục vụ đồng bộ).

### Quản lý File
- Backend không lưu file thẳng vào Database (rất nặng). Thay vào đó, lưu trên File System của Server hoặc tốt nhất là dùng **MinIO** (S3-Compatible Object Storage).

---

## 3. Mô hình Self-Hosting (Máy cá nhân làm Server)

Nếu dùng 1 PC cá nhân mạnh (như máy chơi game/editor) làm Host chạy 24/7:

- **Hệ điều hành Server:** Backend viết bằng .NET Core chạy được trên mọi OS (Linux/Windows). Nếu dùng PC ở nhà, Windows vẫn host tốt dưới dạng Windows Service hoặc thông qua Docker Desktop.
- **Bảo mật mạng (Networking):** Không cần mở Port (Port Forwarding) trên Router gây nguy hiểm rủi ro bị tấn công mạng. Khuyên dùng **Cloudflare Tunnels** hoặc **Tailscale**. Dịch vụ này tạo một đường hầm mã hóa (Tunnel) ra Internet, giúp thiết bị khác truy cập an toàn với tên miền HTTPS chuẩn chỉnh.
- **Bảo mật máy Host:** Người dùng khác truy cập vào Web API, Web API sẽ giới hạn họ chỉ có quyền đọc/ghi trong thư mục lưu trữ được chỉ định (vd: `D:\AppStorage`). Họ hoàn toàn không có quyền xem hay can thiệp vào các thư mục khác (như `C:\` hay game của Host).
- **Tối ưu tài nguyên máy Host:** 
  - Nếu chạy trực tiếp (Windows Service): Set Priority của Process xuống "Low" hoặc "Below Normal".
  - Nếu chạy Docker: Set giới hạn tài nguyên (`cpus: "2.0"`, `mem_limit: "2G"`) để đảm bảo không tranh giành RAM và CPU với game hoặc phần mềm Edit Video. Cấu hình I/O ổ cứng cũng có thể giới hạn để không gây nghẽn cổ chai.

---

## 4. Quản lý Người dùng & Xác thực (Authentication)

- **Số lượng User:** Server có thể phục vụ từ vài chục đến hàng nghìn người tùy cấu hình phần cứng (I/O, Network). Một PC cá nhân hoàn toàn gánh được gia đình hoặc 1 lớp học (50-100 người) dễ dàng.
- **Cơ chế Tài khoản:** Bắt buộc sử dụng Tài khoản/Mật khẩu riêng biệt (Users Accounts) thay vì 1 Folder chung.
- **Xác thực bằng Token (JWT - JSON Web Tokens):** 
  - Khi login thành công, Server trả về Token.
  - WinForms (Client) sẽ lưu Token này lại. Các request tải file, lấy danh sách sau đó đều phải đính kèm Token để Server kiểm tra quyền hợp lệ.

---

## 5. Cơ Chế Sync Giống OneDrive

### Quy trình Truyền File
Cơ chế **Client-Server** bắt buộc.
- **Máy 1 -> Server -> Máy 2:** Nếu Máy 1 có file, nó sẽ upload lên Server. Khi Máy 1 tắt máy, Máy 2 vẫn tải được file đó từ Server. 
- Nếu một máy vừa làm Server vừa làm Client (Host PC): Chạy Backend ngầm (Port 5000), bật App WinForms kết nối thẳng vào `localhost:5000`. Cơ chế hoạt động với nó hoàn toàn giống Máy 2, chỉ là tốc độ tức thời vì kết nối cục bộ.

### Giải quyết Xung đột (Conflict Resolution)
Nếu Máy 1 và Máy 2 cùng sửa 1 file offline, sau đó cùng có mạng và đẩy lên Server:
- **Hướng giải quyết (Khuyên dùng):** Tạo ra **Conflict Copy**. Server nhận thấy Version bị lỗi đồng bộ, nó sẽ lưu thành 2 file: `[Tên Tài Liệu].docx` và `[Tên Tài Liệu] - [Tên Máy Tính].docx`. Người dùng sẽ tự mở lên và quyết định giữ bản nào. Giống hệt OneDrive / Google Drive.

---

## 6. Chia Sẻ (Sharing) & Virtual Drive

### Cơ chế Chia sẻ
- Mỗi tài khoản có một không gian (Workspace) riêng biệt.
- **Tính năng Share:** Có thể Share cấp độ File hoặc Folder cho người khác (Read-only hoặc Edit). 
- **Nhận Share:** Client sẽ hiển thị một mục riêng là "Shared with me" (Được chia sẻ với tôi). Một người có thể nhận chia sẻ từ nhiều người khác.

### Ổ đĩa ảo trong My Computer (Virtual Drive & Files On-Demand)
*Đây là "Vũ khí hạng nặng" để ăn điểm tuyệt đối cho đồ án.*
- **Làm thế nào để không tải file mà vẫn mở được nhanh?**
  - Sử dụng API của Windows là **Windows Projected File System (ProjFS)** hoặc các thư viện mã nguồn mở như **Dokany** / **WinFsp** kết hợp với C#.
  - **Cách hoạt động:** App WinForms sẽ móc vào Windows Explorer, tạo ra một ổ đĩa ảo (ví dụ ổ `S:\`). Ổ đĩa này sẽ liên tục nhận dữ liệu Metadata từ SQLite Local (Danh sách tên file, dung lượng, icon).
  - Khi người dùng click đúp vào 1 file chưa tải về máy: Hệ điều hành (Windows) sẽ gửi tín hiệu (Event) cho App WinForms. App WinForms lập tức gọi lên Server tải file đó xuống (Tốc độ tùy mạng, nếu file nhẹ 1-2MB thì mở gần như tức thì). Sau khi tải xong, Windows tự động mở bằng phần mềm tương ứng (Word, PDF Reader). Băng thông chỉ tốn đúng dung lượng file đó.

---

## 7. Cấu trúc Source Code & Môi trường Test

### Repository Strategy
- Nên dùng **Monorepo** (Một Repository chung trên GitHub, chứa thư mục `Client_WinForms` và `Server_API`). Rất dễ quản lý CI/CD.
- **Tránh nặng Docker Image:** Trong file `Dockerfile` của Backend, bạn chỉ trỏ `COPY` thư mục `Server_API` vào để build. Docker sẽ "mù" với phần `Client_WinForms`, giúp Image cuối cùng chỉ nặng vài chục MB.

### Test khi không có Server
- Dùng **Dependency Injection (DI)** trong C#. Tạo `IStorageService`.
- Khi dev UI, thay vì chích `CloudStorageService` (gọi HTTP), bạn chích `MockStorageService` (chỉ tạo file ảo trong thư mục máy tính giả lập). Hoàn toàn test được 100% app mà không cần mạng.

---

## 8. CI/CD Pipelines (DevOps)

### Các tính năng Cần Có (Must-Have):
1. **Automated Build & Test:** Tự động build source code, chạy Unit Tests và xuất báo cáo độ phủ code (Code Coverage) mỗi khi có Pull Request.
2. **Containerization (Docker Build & Push):** Tự động đóng gói Backend API thành Docker Image và đẩy lên kho chứa (GitHub Container Registry / Docker Hub).
3. **Linter & Code Format Check:** Đảm bảo code chuẩn convention (StyleCop, EditorConfig).

### Các tính năng Gây Ấn Tượng Mạnh (Wow Factors - Điểm 10 Đồ Án):
1. **Automated Releases (Client):** Pipeline tự động đóng gói WinForms App thành file `.msi` (Bộ cài đặt) hoặc `.zip`, tự động gắn phiên bản (v1.0.1) và đính kèm vào mục **GitHub Releases** khi bạn tạo một Git Tag.
2. **Database Migration Scripting:** Trong quá trình deploy Docker, tự động chạy container `ef migrations` để cập nhật cấu trúc database PostgreSQL lên version mới nhất mà không mất dữ liệu.
3. **Security Auditing (DevSecOps):** Tích hợp `SonarCloud` hoặc `GitHub CodeQL` vào Pipeline để tự động quét mã nguồn và cảnh báo nếu có lỗ hổng bảo mật (Hardcode password, SQL Injection).
4. **Environment Promotion:** Tự động deploy lên môi trường Staging (Test thử) khi code merge vào `develop`. Có bước **Manual Approval** (Ai đó phải bấm nút duyệt) mới tiếp tục deploy lên môi trường Production (Chạy thật).
5. **Infrastructure as Code (IaC):** Dùng `Terraform` / `Ansible` để định nghĩa và tự động thiết lập hạ tầng mạng, Docker Compose. Máy trống trơn chạy 1 lệnh là có đầy đủ môi trường.
