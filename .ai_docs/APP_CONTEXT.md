# APP_CONTEXT: BỘ NHỚ ĐỊNH HƯỚNG DỰ ÁN & QUY TẮC CỐT LÕI (STRICT MODE)

## 1. BỐI CẢNH DỰ ÁN
- **Tên dự án:** Cloud-Based Document Manager.
- **Mục tiêu:** Hệ thống quản lý thư mục, file và chia sẻ dữ liệu đa dụng. Chuyển đổi từ mô hình Desktop Offline sang Client-Server (Hybrid Cloud).
- **Tính năng đặc thù:** Hỗ trợ Sync ngầm, Files On-Demand (Virtual Drive), quản lý xung đột dữ liệu (Conflict Resolution).

## 2. QUY TẮC MÃ HÓA CHO AI (VIBE CODE BEST PRACTICES)
Khi AI đọc file này và tiến hành sinh code, BẮT BUỘC phải tuân thủ các chuẩn mực sau:

### 2.1. C# & .NET 8 Best Practices
- **Async/Await All The Way:** Tuyệt đối không dùng `.Result` hay `.Wait()`. Mọi I/O bound operations phải là `Async`.
- **CancellationToken:** Bắt buộc truyền `CancellationToken` vào mọi method `Async` để hỗ trợ hủy tác vụ khi cần.
- **Nullable Reference Types:** Dự án mặc định bật `<Nullable>enable</Nullable>`. Code sinh ra phải kiểm tra null chặt chẽ, sử dụng `?` và `!` hợp lý.
- **Tránh ném Exception cho Logic:** Sử dụng pattern `Result<T>` (hoặc trả về Tuple/Record chứa thông tin lỗi) thay vì dùng `throw new Exception()` cho các luồng xử lý nghiệp vụ thông thường. Chỉ `throw` khi lỗi hệ thống nghiêm trọng.

### 2.2. Clean Architecture & SOLID
- **Dependency Inversion (DIP):** Các class không bao giờ khởi tạo (`new`) trực tiếp các Service phụ thuộc. Mọi phụ thuộc phải được truyền qua Constructor Injection.
- **Single Responsibility (SRP):** Controller chỉ nhận request và trả response. Logic nghiệp vụ phải nằm ở Application Service. Truy xuất dữ liệu phải nằm ở Repository.
- **Tách biệt Entity & DTO:** Tuyệt đối không trả `Entity` (Domain) trực tiếp ra ngoài Controller API. Bắt buộc map qua `DTO` (Data Transfer Object).

### 2.3. WinForms Client Best Practices
- **Không Block UI Thread:** Mọi tác vụ mạng, đọc/ghi file phải dùng `Task.Run` hoặc `async/await`. Cập nhật giao diện từ luồng nền phải bọc trong `Control.Invoke()` hoặc `Control.BeginInvoke()`.
- **MVP Pattern (Model-View-Presenter):** Khuyến khích tách logic ra khỏi code-behind của Form (`.Designer.cs`) bằng Presenter.

## 3. LỘ TRÌNH THỰC THI (DO NOT SKIP)
AI không được nhảy cóc. Phải code theo trình tự:
1. Core (Entities -> Interfaces).
2. Infrastructure (EF Core, Dapper, Storage).
3. Server API (Auth, Documents Controllers).
4. Client Background (Sync Engine, SQLite Cache).
5. Client UI (WinForms integration).
6. Nâng cao (Virtual Drive, CI/CD).

## 4. QUY TẮC ĐỌC CODE HIỆN TẠI (CONTEXT AWARENESS & CODE STYLE)
**CHÚ Ý CHO AI:** Không được tự ý "sáng tác" cấu trúc bừa bãi. Trước khi tạo Server hay sửa code, MÀY (AI) BẮT BUỘC phải sử dụng tool (`list_dir`, `view_file`, `grep_search`) để đọc thư mục dự án hiện tại (`study-document-manager`) và tuân thủ chặt chẽ các chuẩn mực đang có:
- **Naming Conventions & Syntax:** Hãy nhìn cách dự án đang code. Phải giữ nguyên các quy tắc như: Biến private luôn dùng `readonly` (nếu không thay đổi), sử dụng Object Initializers (Easy Initialization `{}`), Target-typed `new()`, và các Convention C# hiện tại mà dev đã cất công fix cả buổi tối.
- **Thêm Server vào Monorepo:** Khi được yêu cầu tạo Server, hãy nhìn xem folder WinForms hiện tại tên gì, đang nằm ở đâu. Server API phải được tạo thành một project ngang hàng (Sibling project) trong cùng Solution, với tên gọi tương xứng (VD: `study-document-manager-api`). Đừng tạo folder lộn xộn.
- **Tương thích:** Bất kỳ đoạn code nào chèn thêm vào WinForms hiện hành đều phải "nhập gia tùy tục", bắt chước y hệt văn phong (vibe) của các file class bên cạnh.
