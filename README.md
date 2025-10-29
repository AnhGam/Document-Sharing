# Study Document Manager

> Ứng dụng quản lý tài liệu học tập chuyên nghiệp cho sinh viên và giáo viên

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2012+-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![Windows Forms](https://img.shields.io/badge/Windows%20Forms-C%23-239120?logo=c-sharp)](https://docs.microsoft.com/dotnet/desktop/winforms/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## Giới thiệu

**Study Document Manager** là một ứng dụng Windows Forms được phát triển bằng C# để giúp sinh viên, giáo viên và nhân viên quản lý tài liệu học tập một cách hiệu quả. Ứng dụng cung cấp đầy đủ các tính năng từ quản lý cơ bản đến nâng cao như tìm kiếm, thống kê, và xuất dữ liệu.

### Tính năng chính

- **Quản lý tài liệu đầy đủ**: Thêm, sửa, xóa tài liệu với đầy đủ thông tin
- **Tìm kiếm thông minh**: Tìm kiếm nhanh theo tên, môn học, ghi chú
- **Lọc linh hoạt**: Lọc theo môn học và loại tài liệu
- **Mở file trực tiếp**: Double-click để mở file tài liệu ngay lập tức
- **Đánh dấu quan trọng**: Đánh sao vàng cho tài liệu quan trọng
- **Icon động**: Hiển thị icon theo loại file (PDF, Word, PowerPoint, Excel)
- **Thống kê trực quan**: Biểu đồ thống kê số lượng tài liệu (Cột, Tròn, Đường...)
- **Xuất dữ liệu**: Xuất danh sách tài liệu ra file CSV
- **Drag & Drop**: Kéo thả file để thêm tài liệu nhanh chóng
- **Quản lý danh mục**: Quản lý môn học và loại tài liệu tập trung

## Giao diện

### Form chính
- Menu bar với các chức năng nhanh (Ctrl+N, Ctrl+O, F5...)
- Toolbar với các nút chức năng
- Panel tìm kiếm với TextBox và ComboBox lọc
- DataGridView hiển thị danh sách tài liệu
- Status bar hiển thị trạng thái và số lượng tài liệu

### Form thêm/sửa tài liệu
- Nhập đầy đủ thông tin: tên, môn học, loại, đường dẫn
- Chọn file với OpenFileDialog
- Tự động tính kích thước file
- Checkbox đánh dấu tài liệu quan trọng
- Validate dữ liệu đầu vào

### Form thống kê
- Biểu đồ động với nhiều kiểu (Cột, Tròn, Đường, Vùng)
- Thống kê theo môn học hoặc loại tài liệu
- Màu sắc đẹp mắt và dễ nhìn
- Hiển thị tổng số tài liệu

### Form quản lý danh mục
- Quản lý môn học và loại tài liệu
- Thêm, sửa, xóa danh mục
- Cập nhật hàng loạt tài liệu khi đổi tên danh mục
- Xác nhận kỹ trước khi xóa

## Công nghệ sử dụng

- **Ngôn ngữ**: C# (.NET Framework 4.8.1)
- **Giao diện**: Windows Forms
- **Database**: SQL Server 2012+
- **Biểu đồ**: System.Windows.Forms.DataVisualization.Charting
- **Thư viện**: System.Data.SqlClient, System.Xml.Linq

## Cấu trúc Database

### Bảng `tai_lieu`

```sql
CREATE TABLE tai_lieu (
    id INT PRIMARY KEY IDENTITY(1,1),      -- ID tự động tăng
    ten NVARCHAR(200) NOT NULL,            -- Tên tài liệu
    mon_hoc NVARCHAR(100),                 -- Môn học
    loai NVARCHAR(100),                    -- Loại tài liệu
    duong_dan NVARCHAR(500) NOT NULL,      -- Đường dẫn file
    ghi_chu NVARCHAR(1000),                -- Ghi chú
    ngay_them DATETIME DEFAULT GETDATE(),  -- Ngày thêm
    kich_thuoc FLOAT,                      -- Kích thước (MB)
    tac_gia NVARCHAR(100),                 -- Tác giả
    quan_trong BIT DEFAULT 0               -- Đánh dấu quan trọng
);
```

## Cài đặt và Chạy

### Yêu cầu hệ thống

- Windows 7/8/10/11
- .NET Framework 4.8.1 trở lên
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

1. Chọn môn học từ ComboBox "Môn học"
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

1. Click nút **Thống kê** hoặc Menu > Hiển thị > Thống kê (`Ctrl+S`)
2. Chọn thống kê theo **Môn học** hoặc **Loại tài liệu**
3. Chọn kiểu biểu đồ: Cột dọc, Cột ngang, Tròn, Đường, Vùng

### Xuất dữ liệu

1. Click nút **Xuất** hoặc Menu > Tệp tin > Xuất dữ liệu (`Ctrl+E`)
2. Chọn vị trí lưu file CSV
3. File sẽ mở tự động trong Windows Explorer

### Quản lý danh mục

1. Menu > Hiển thị > Quản lý Môn học và Loại (`Ctrl+M`)
2. Chọn tab **Môn học** hoặc **Loại tài liệu**
3. **Thêm**: Tạo danh mục mới
4. **Sửa**: Đổi tên danh mục (cập nhật tất cả tài liệu liên quan)
5. **Xóa**: Xóa danh mục (cảnh báo: xóa cả tài liệu!)

## Thiết kế màu sắc

- **Màu chủ đạo**: Material Design Colors
  - Primary: `#2196F3` (Blue)
  - Success: `#4CAF50` (Green)
  - Danger: `#F44336` (Red)
  - Warning: `#FF9800` (Orange)
  - Star: `#FFCA28` (Yellow)
  
- **Màu nền**:
  - Form chính: `#E3F2FD` (Light Blue)
  - Panel: `#FFFFFF` (White)
  - DataGridView alternating: `#F5F5F5` (Light Gray)

## Cấu trúc Project

```
study-document-manager/
│
├── AddEditForm.cs              # Form thêm/sửa tài liệu
├── AddEditForm.Designer.cs
├── CategoryManagementForm.cs   # Form quản lý danh mục
├── CategoryManagementForm.Designer.cs
├── Form1.cs                    # Form chính
├── Form1.Designer.cs
├── Report.cs                   # Form thống kê
├── Report.Designer.cs
├── DatabaseHelper.cs           # Class truy vấn database
├── IconHelper.cs               # Class tạo icon động
├── Program.cs                  # Entry point
├── App.config                  # Cấu hình ứng dụng
├── README.md                   # File này
│
├── Database/
│   └── Database.sql            # Script tạo database
│
└── Properties/
    ├── AssemblyInfo.cs
    ├── Resources.resx
    └── Settings.settings
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
| `Ctrl+M` | Quản lý môn học và loại |
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
- Email: [your-email@example.com](mailto:hayatoshino05@gmail.com)
- GitHub: [@hayato-shino05](https://github.com/hayato-shino05)

## Lời cảm ơn

- Cảm ơn giảng viên đã hướng dẫn
- Cảm ơn cộng đồng C# và Windows Forms
- Cảm ơn Microsoft về documentation tuyệt vời

---

<div align="center">
Made with ❤️ by Vũ Đức Dũng | © 2025
</div
