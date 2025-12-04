<div align="center">

![Study Document Manager](study-document-manager/assets/logo/hero-banner.png)

# Study Document Manager

> Ứng dụng quản lý tài liệu cá nhân

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2012+-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![Windows Forms](https://img.shields.io/badge/Windows%20Forms-C%23-239120?logo=c-sharp)](https://docs.microsoft.com/dotnet/desktop/winforms/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

## Giới thiệu

**Study Document Manager** là một ứng dụng Windows Forms được phát triển bằng C# để giúp người dùng quản lý tài liệu học tập và công việc một cách hiệu quả. Ứng dụng sử dụng mô hình **cá nhân hóa** - mỗi người dùng có thể quản lý hoàn toàn tài liệu, danh mục của riêng mình.

> [Cấu trúc dự án](PROJECT_STRUCTURE.md)

### Tính năng chính

- **Quản lý tài liệu đầy đủ**: Thêm, sửa, xóa tài liệu với đầy đủ thông tin cho từng người dùng
- **Đăng nhập & phân quyền**: Hỗ trợ 2 cấp quyền User / Admin - mỗi User quản lý tài liệu của riêng mình
- **Tìm kiếm & filter nâng cao**: Tìm kiếm theo từ khóa, lọc theo danh mục, loại tài liệu, ngày thêm, dung lượng, chỉ tài liệu quan trọng
- **Toast Notification**: Thông báo kiểu web hiện đại (Success/Error/Warning/Info) với hiệu ứng fade
- **Mở file trực tiếp & Context Menu**: Double-click hoặc chuột phải để mở file, sửa, xóa, copy đường dẫn, mở thư mục chứa file
- **Đánh dấu quan trọng**: Đánh sao vàng cho tài liệu quan trọng, có thể lọc nhanh
- **Tags (nhãn)**: Gắn nhiều nhãn cho tài liệu, lọc theo tag để tìm nhanh theo chủ đề
- **Deadline & nhắc hạn**: Đặt hạn chót cho tài liệu, xem danh sách sắp đến hạn/quá hạn
- **Ghi chú cá nhân**: Ghi chú riêng cho từng tài liệu, đánh dấu trạng thái học tập
- **Bộ sưu tập (Collections)**: Gom nhóm tài liệu theo chủ đề, mở nhanh cả bộ sưu tập
- **Kiểm tra file bị thiếu**: Quét toàn bộ danh sách, phát hiện file không còn tồn tại và cho phép cập nhật/xóa nhanh
- **Icon động**: Hiển thị icon theo loại file (PDF, Word, PowerPoint, Excel)
- **Thống kê trực quan**: Biểu đồ thống kê số lượng tài liệu (Cột, Tròn, Đường...)
- **Xuất dữ liệu**: Xuất danh sách tài liệu ra file CSV
- **Drag & Drop**: Kéo thả file để thêm tài liệu nhanh chóng
- **Quản lý danh mục & người dùng**: Quản lý môn học, loại tài liệu và tài khoản người dùng tập trung
- **Cài đặt tài khoản**: Thay đổi thông tin cá nhân (họ tên, email) và đổi mật khẩu

## Giao diện

### Form chính
- Menu bar với các chức năng nhanh (Ctrl+N, Ctrl+O, F5...)
  - **Công cụ**: Thống kê, Quản lý danh mục, Kiểm tra file bị thiếu
  - **Theo dõi**: Sắp đến hạn, Quá hạn, Quản lý bộ sưu tập
  - **Tài khoản**: Cài đặt tài khoản, Đăng xuất
  - **Quản lý** (Admin): Quản lý người dùng
- Toolbar với các nút chức năng
- Panel tìm kiếm với TextBox và ComboBox lọc
- DataGridView hiển thị danh sách tài liệu
- Status bar hiển thị trạng thái và số lượng tài liệu
- Nút **Đăng xuất** ở góc phải menu bar

### Form thêm/sửa tài liệu
- Nhập đầy đủ thông tin: tên, môn học, loại, đường dẫn
- Chọn file với OpenFileDialog
- Tự động tính kích thước file
- Checkbox đánh dấu tài liệu quan trọng
- Validate dữ liệu đầu vào

### Form thống kê (Dashboard nâng cao)
- **6 Stat Cards**: Tổng tài liệu, Quan trọng, Quá hạn, Sắp đến hạn, Chưa có file, Bộ sưu tập
- **Biểu đồ 7 ngày**: Timeline tài liệu tạo mới trong 7 ngày gần nhất
- **Biểu đồ theo tháng**: Thống kê theo 12 tháng gần nhất
- Biểu đồ động với nhiều kiểu (Cột, Tròn, Đường, Vùng)
- Thống kê theo môn học hoặc loại tài liệu
- Màu sắc đẹp mắt và dễ nhìn
- Hiển thị tổng số tài liệu

### Form quản lý danh mục
- Quản lý môn học và loại tài liệu
- Thêm, sửa, xóa danh mục
- Cập nhật hàng loạt tài liệu khi đổi tên danh mục
- Xác nhận kỹ trước khi xóa

### Form cài đặt tài khoản
- Tab **Thông tin cá nhân**: Xem và sửa họ tên, email
- Tab **Đổi mật khẩu**: Đổi mật khẩu (cần xác thực mật khẩu hiện tại)
- **Nút bật/tắt hiển thị mật khẩu** (click toggle thay vì giữ)
- Hiển thị vai trò và thời gian đăng nhập

### Toast Notification
- Thông báo dạng Toast hiện đại kiểu web
- 4 loại: **Success** (xanh lá), **Error** (đỏ), **Warning** (cam), **Info** (xanh dương)
- Hiệu ứng fade in/out mượt mà
- Hiển thị ở góc trên bên phải của form
- Tự động đóng sau vài giây, có nút đóng nhanh

## Công nghệ sử dụng

- **Ngôn ngữ**: C# (.NET Framework 4.8)
- **Giao diện**: Windows Forms
- **Database**: SQL Server 2012+
- **Biểu đồ**: System.Windows.Forms.DataVisualization.Charting
- **Thư viện**: System.Data.SqlClient, System.Xml.Linq
- **UI Components**: ToastNotification (custom), IconHelper (custom icons)

## Cấu trúc Database

> Chi tiết đầy đủ xem tại [DATABASE.md](DATABASE.md)

Database **quan_ly_tai_lieu** gồm 7 bảng chính:

| Bảng | Mô tả |
|------|-------|
| `users` | Tài khoản người dùng (User/Admin) |
| `user_sessions` | Phiên đăng nhập |
| `tai_lieu` | Tài liệu học tập |
| `collections` | Bộ sưu tập tài liệu |
| `collection_items` | Liên kết tài liệu - bộ sưu tập |
| `personal_notes` | Ghi chú cá nhân |
| `activity_logs` | Nhật ký hoạt động |

## Cài đặt và Chạy

### Yêu cầu hệ thống

- Windows 7/8/10/11
- .NET Framework 4.8 trở lên
- SQL Server 2012 trở lên (hoặc SQL Server Express)
- RAM: 2GB trở lên
- Dung lượng: 100MB trống

### Hướng dẫn cài đặt

1. **Clone repository**
   ```bash
   git clone https://github.com/hayato-shino05/study-document-manager.git
   cd study-document-manager
   ```

2. **Cấu hình Database**
   - Mở SQL Server Management Studio
   - Chạy script `Database/Database.sql` để tạo database và bảng
   - Hoặc database sẽ được tạo tự động khi chạy lần đầu

3. **Cấu hình Connection String**
   - Mở file `App.config`
   - Sửa connection string phù hợp với SQL Server của bạn:
   ```xml
   <connectionStrings>
     <add name="DefaultConnection" 
          connectionString="Server=YOUR_SERVER;Database=quan_ly_tai_lieu;Integrated Security=True;" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Build và chạy**
   - Mở solution trong Visual Studio 2019+
   - Build solution (Ctrl+Shift+B)
   - Run (F5 hoặc Ctrl+F5)

## Hướng dẫn sử dụng

### Thêm tài liệu mới

1. Click nút **Thêm** hoặc nhấn `Ctrl+N`
2. Nhập thông tin tài liệu
3. Click **Chọn file...** để chọn file
4. Chọn môn học và loại tài liệu
5. Nhập ghi chú và tác giả (tùy chọn)
6. Đánh dấu quan trọng nếu cần
7. Click **Lưu**

### Tìm kiếm tài liệu

1. Nhập từ khóa vào ô tìm kiếm
2. Nhấn **Enter** hoặc click nút **Tìm kiếm**
3. Kết quả sẽ hiển thị ngay lập tức

### Lọc tài liệu

1. Chọn danh mục từ ComboBox "Danh mục"
2. Chọn loại từ ComboBox "Loại"
3. Danh sách tự động cập nhật

### Mở tài liệu

- **Cách 1**: Double-click vào dòng tài liệu trong bảng
- **Cách 2**: Chọn tài liệu và click nút **Mở file** hoặc `Ctrl+O`

### Drag & Drop

1. Kéo file từ Windows Explorer
2. Thả vào DataGridView
3. Form thêm tài liệu sẽ tự động mở với thông tin file

### Xem thống kê

1. Click nút **Thống kê** hoặc Menu > Công cụ > Thống kê (`Ctrl+S`)
2. Chọn thống kê theo **Môn học** hoặc **Loại tài liệu**
3. Chọn kiểu biểu đồ: Cột dọc, Cột ngang, Tròn, Đường, Vùng

### Xuất dữ liệu

1. Click nút **Xuất** hoặc Menu > Tệp tin > Xuất dữ liệu (`Ctrl+E`)
2. Chọn vị trí lưu file CSV
3. File sẽ mở tự động trong Windows Explorer

### Kiểm tra file bị thiếu

1. Vào menu **Công cụ > Kiểm tra file bị thiếu**.
2. Nhấn nút **Quét** để kiểm tra tất cả tài liệu có đường dẫn file.
3. Với từng dòng báo thiếu file có thể:
   - **Chọn file mới...**: cập nhật lại đường dẫn file.
   - **Xóa đường dẫn (giữ metadata)**: làm trống đường dẫn nhưng vẫn giữ thông tin tài liệu.
   - **Xóa tài liệu**: xóa bản ghi khỏi database.
4. Có thể dùng nút **Xóa tất cả** để xóa toàn bộ tài liệu đang bị thiếu file.

### Quản lý danh mục

1. Menu **Công cụ > Quản lý Danh mục và Loại** (`Ctrl+M`)
2. Chọn tab **Danh mục** hoặc **Loại tài liệu**
3. **Thêm**: Tạo danh mục mới
4. **Sửa**: Đổi tên danh mục (cập nhật tất cả tài liệu liên quan)
5. **Xóa**: Xóa danh mục (cảnh báo: có thể ảnh hưởng đến tài liệu liên quan)

### Cài đặt tài khoản

1. Menu **Tài khoản > Cài đặt tài khoản**
2. Tab **Thông tin cá nhân**: Sửa họ tên và email, nhấn **Lưu thay đổi**
3. Tab **Đổi mật khẩu**: Nhập mật khẩu hiện tại và mật khẩu mới, nhấn **Đổi mật khẩu**

### Đăng xuất

- Click nút **Đăng xuất** ở góc phải menu bar, hoặc
- Menu **Tài khoản > Đăng xuất** (`Ctrl+L`)

## Thiết kế màu sắc (Teal/Emerald Theme)
```
=== Màu chính (Teal/Emerald) ===
Primary:        #14B8A6 (Teal)       - Button chính, accent, menu highlight
PrimaryDark:    #0D9488 (Teal Dark)  - Hover state, pressed
PrimaryLight:   #2DD4BF (Teal Light) - Hover nhẹ, selection
Secondary:      #10B981 (Emerald)    - Accent, status active

=== Status Colors ===
Success:    #22C55E (Green)     - Thành công, Success Toast
Error:      #EF4444 (Red)       - Lỗi, xóa, quá hạn, Error Toast  
Warning:    #F59E0B (Amber)     - Cảnh báo, Warning Toast
Info:       #3B82F6 (Blue)      - Thông tin, Info Toast
Star:       #FFCA28 (Yellow)    - Đánh dấu quan trọng

=== Background & Layout ===
BackgroundMain: #FFFFFF (White)     - Form background
BackgroundSoft: #F8FAFC (Gray 50)   - Panel, card
BorderLight:    #E2E8F0 (Gray 200)  - Border nhẹ
BorderMedium:   #CBD5E1 (Gray 300)  - Border rõ

=== Text Colors ===
TextPrimary:   #0F172A (Slate 900) - Text chính
TextSecondary: #475569 (Slate 600) - Text phụ
TextMuted:     #94A3B8 (Slate 400) - Text mờ

=== Toast Notification ===
Toast Success: #22C55E - Xanh lá, icon ✓
Toast Error:   #EF4444 - Đỏ, icon ✕
Toast Warning: #F59E0B - Cam vàng, icon !
Toast Info:    #3B82F6 - Xanh dương, icon i
```
## Các phím tắt

| Phím tắt | Chức năng |
|----------|-----------|
| `Ctrl+N` | Thêm tài liệu mới |
| `Ctrl+O` | Mở tài liệu đã chọn |
| `Ctrl+U` | Sửa tài liệu đã chọn |
| `Delete` | Xóa tài liệu đã chọn |
| `Ctrl+E` | Xuất dữ liệu |
| `Ctrl+S` | Xem thống kê |
| `Ctrl+M` | Quản lý danh mục và loại |
| `Ctrl+L` | Đăng xuất |
| `F5` | Làm mới danh sách |
| `Alt+F4` | Thoát ứng dụng |

## Xử lý lỗi

Ứng dụng có xử lý các trường hợp lỗi phổ biến:

- Kiểm tra kết nối database trước khi thực hiện thao tác
- Validate dữ liệu đầu vào (tên, đường dẫn file bắt buộc)
- Kiểm tra file tồn tại trước khi mở
- Xác nhận trước khi xóa tài liệu/danh mục
- Hiển thị thông báo lỗi chi tiết và gợi ý khắc phục
- Log lỗi DataGridView để debug

## Đóng góp

Mọi đóng góp đều được chào đón! Vui lòng:

1. Fork repository
2. Tạo branch mới (`git checkout -b feature/AmazingFeature`)
3. Commit thay đổi (`git commit -m 'Add some AmazingFeature'`)
4. Push lên branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## License

Dự án này được phát hành dưới giấy phép MIT. Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

## Tác giả

**Vũ Đức Dũng**
- Lớp: TT601K14
- Email: [hayatoshino05@gmail.com](mailto:hayatoshino05@gmail.com)
- GitHub: [@hayato-shino05](https://github.com/hayato-shino05)

## Lời cảm ơn

- Cảm ơn giảng viên đã hướng dẫn
- Cảm ơn cộng đồng C# và Windows Forms
- Cảm ơn Microsoft về documentation tuyệt vời

---

<div align="center">
Made with ❤️ by Vũ Đức Dũng | © 2025
</div>
