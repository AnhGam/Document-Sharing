# Cấu trúc dự án Study Document Manager

Tài liệu này mô tả kiến trúc, luồng dữ liệu và cấu trúc thư mục của ứng dụng **Study Document Manager**.

Ứng dụng được xây dựng trên **Windows Forms (.NET Framework 4.8)** và **SQL Server**, theo hướng thực dụng, dễ mở rộng và dễ bảo trì.

---

## 1. Tổng quan kiến trúc

### 1.1. Kiến trúc tầng

Ứng dụng tổ chức theo mô hình 3 tầng đơn giản:

1. **Presentation Layer (UI)**
   - Các Form WinForms: `Form1`, `AddEditForm`, `LoginForm`, `RegisterForm`, `UserManagementForm`, `CategoryManagementForm`, `Report`, `FileIntegrityCheckForm`.
   - Chịu trách nhiệm giao diện, binding dữ liệu vào `DataGridView`, điều khiển filter, hiển thị dialog.

2. **Application / Business Layer**
   - `DatabaseHelper` (và partial `DatabaseHelper_UserAuth`)
   - `UserSession`
   - `IconHelper`
   - Chứa logic nghiệp vụ: phân quyền, lựa chọn câu truy vấn, xử lý filter nâng cao, xác định tài liệu nào được phép sửa/xóa, chọn icon theo loại file.

3. **Data Layer (SQL Server)**
   - Database `quan_ly_tai_lieu` với các bảng chính: `tai_lieu`, `users`, `user_sessions`, các bảng danh mục (môn học, loại tài liệu) và các thủ tục/trigger liên quan.
   - `Database.sql` mô tả schema và default values (ví dụ: `quan_trong` mặc định = 0, `ngay_them` mặc định = GETDATE()).

### 1.2. Các khối chức năng chính

- **Quản lý tài liệu**: CRUD tài liệu, kèm metadata (môn học, loại, ghi chú, dung lượng, tác giả, quan trọng, người tạo).
- **Đăng nhập & phân quyền**: Student / Teacher / Admin với quyền xem/sửa/xóa khác nhau.
- **Filter & tìm kiếm nâng cao**: theo từ khóa, môn học, loại, ngày, dung lượng, người tạo, cờ quan trọng.
- **Context Menu trên danh sách**: thao tác nhanh trên DataGridView.
- **Kiểm tra file bị thiếu**: quét và sửa các record có đường dẫn không tồn tại.
- **Thống kê**: biểu đồ thống kê tài liệu.
- **Quản lý danh mục**: môn học, loại tài liệu.
- **Quản lý người dùng**: chỉ dành cho Admin.

---

## 2. Cấu trúc thư mục & file chính

### 2.1. Root repository

```text
study-document-manager/
│
├── study-document-manager/         # Project WinForms (.csproj)
│   ├── Form1.*                     # Form chính: danh sách tài liệu, tìm kiếm, filter nâng cao
│   ├── AddEditForm.*               # Thêm / sửa tài liệu
│   ├── FileIntegrityCheckForm.*    # Kiểm tra & xử lý file bị thiếu
│   ├── CategoryManagementForm.*    # Quản lý môn học & loại tài liệu
│   ├── Report.*                    # Thống kê tài liệu
│   ├── LoginForm.*                 # Đăng nhập
│   ├── RegisterForm.*              # Đăng ký tài khoản
│   ├── UserManagementForm.*        # Quản lý người dùng (Admin)
│   ├── DatabaseHelper.cs           # Truy vấn dữ liệu tài liệu & filter nâng cao
│   ├── DatabaseHelper_UserAuth.cs  # Đăng nhập, đăng ký, kiểm tra quyền
│   ├── UserSession.cs              # Thông tin user đăng nhập hiện tại
│   ├── IconHelper.cs               # Sinh icon theo loại tài liệu
│   ├── Program.cs                  # Entry point
│   ├── App.config                  # Cấu hình (connection string,...)
│   └── Properties/…                # Resources, Settings
│
├── Database/                       # Script database
│   └── Database.sql
│
├── README.md                       # Giới thiệu & hướng dẫn sử dụng
├── FEATURES.md                     # Roadmap tính năng
└── PROJECT_STRUCTURE_NEW.md        # (file hiện tại) mô tả kiến trúc
```

> Lưu ý: `*` bao gồm cả `.cs`, `.Designer.cs`, `.resx` tương ứng.

---

## 3. Vai trò từng Form và lớp chính

### 3.1. Form1 – Màn hình chính

**Chức năng:**

- Hiển thị danh sách tài liệu trong `DataGridView`.
- Panel tìm kiếm nhanh (từ khóa, môn học, loại).
- **Filter nâng cao**: khoảng ngày, khoảng dung lượng, chỉ tài liệu quan trọng, lọc theo người tạo (Admin/Teacher).
- Toolbar & menu:
  - Thêm, Sửa, Xóa, Mở file, Xuất dữ liệu.
  - Menu **Công cụ**: Thống kê, Quản lý danh mục, Kiểm tra file bị thiếu, Đăng xuất.
- **Context menu** trên mỗi dòng:
  - Mở file
  - Sửa
  - Xóa
  - Copy đường dẫn file
  - Mở thư mục chứa file
  - Đánh dấu / Bỏ đánh dấu quan trọng
- Hỗ trợ **Drag & Drop** file để thêm nhanh tài liệu.

**Liên kết với backend:**

- Gọi `DatabaseHelper.GetDocumentsForCurrentUser()` để load dữ liệu theo quyền.
- Gọi `DatabaseHelper.SearchDocuments`, `FilterDocuments`, `SearchDocumentsAdvanced` để áp dụng filter.
- Sử dụng `DatabaseHelper.DeleteDocument`, `CanUserEditDocument` để xóa / kiểm tra quyền.
- Sử dụng `UserSession` để biết `UserId`, `Role`, `IsStudent / IsTeacher / IsAdmin`.

### 3.2. AddEditForm – Thêm / sửa tài liệu

- Form nhập thông tin tài liệu:
  - Tên, môn học, loại, đường dẫn file, ghi chú, kích thước, tác giả.
  - Checkbox **Tài liệu quan trọng (★)**.
- Chọn file bằng `OpenFileDialog`, tự động:
  - Điền tên tài liệu từ tên file nếu để trống.
  - Tính kích thước file (MB).
- Khi lưu:
  - Thêm mới: `DatabaseHelper.InsertDocument(...)`.
  - Cập nhật: `DatabaseHelper.UpdateDocument(...)`.
- Validate: yêu cầu tên & đường dẫn, kiểm tra file tồn tại.

### 3.3. FileIntegrityCheckForm – Kiểm tra file bị thiếu

- Quét toàn bộ bảng `tai_lieu` để tìm các record có `duong_dan` không còn tồn tại trên ổ đĩa.
- Hiển thị danh sách file bị thiếu trong `DataGridView` với các cột: ID, Tên tài liệu, Đường dẫn, Hành động.
- Với từng dòng, context menu/nút **Xử lý** cho phép:
  - **Chọn file mới...** → cập nhật lại `duong_dan`.
  - **Xóa đường dẫn (giữ metadata)** → đặt `duong_dan` thành chuỗi rỗng.
  - **Xóa tài liệu** → xóa bản ghi khỏi `tai_lieu`.
- Có progress bar & label tiến trình, nút **Quét** và **Xóa tất cả**.

### 3.4. CategoryManagementForm – Quản lý danh mục

- Quản lý các bảng danh mục như Môn học, Loại tài liệu.
- Thêm / sửa / xóa danh mục, với cảnh báo khi thao tác có thể ảnh hưởng đến dữ liệu tài liệu.
- Sau khi đóng form, `Form1` reload lại dữ liệu để áp dụng thay đổi.

### 3.5. Report – Thống kê

- Sử dụng `System.Windows.Forms.DataVisualization.Charting` để:
  - Thống kê số lượng tài liệu theo môn học, loại.
  - Chọn kiểu biểu đồ: cột, tròn, đường, vùng, ...
- Lấy dữ liệu tổng hợp từ `DatabaseHelper`.

### 3.6. LoginForm, RegisterForm, UserManagementForm

- **LoginForm**
  - Người dùng nhập `username` và `password`.
  - Gọi `DatabaseHelper` (phần `UserAuth`) để xác thực.
  - Nếu thành công, thiết lập `UserSession` và mở `Form1`.

- **RegisterForm**
  - Cho phép tạo tài khoản mới (tùy rule dự án có thể giới hạn hoặc chỉ Admin được tạo).
  - Mật khẩu được hash (sử dụng helper như `BCryptTemp`).

- **UserManagementForm** (Admin only)
  - Xem, tạo, sửa, khóa tài khoản người dùng.
  - Chỉ được mở nếu `UserSession.IsAdmin == true`.

### 3.7. Lớp trợ giúp (Helpers)

- **DatabaseHelper / DatabaseHelper_UserAuth**
  - Quản lý connection string (đọc từ `App.config`).
  - Cung cấp các hàm:
    - `ExecuteQuery`, `ExecuteNonQuery`, `ExecuteScalar`.
    - Các hàm nghiệp vụ: `GetDocumentsForCurrentUser`, `InsertDocument`, `UpdateDocument`, `DeleteDocument`, `SearchDocuments`, `SearchDocumentsAdvanced`, `FilterDocuments`, `GetUsersForFilter`, `CanUserEditDocument`, các hàm đăng nhập/đăng ký.

- **UserSession**
  - Lưu thông tin user hiện tại trong suốt vòng đời ứng dụng:
    - `UserId`, `Username`, `FullName`, `Role`.
    - Các property tiện dụng: `IsStudent`, `IsTeacher`, `IsAdmin`, `CanManageCategories`.

- **IconHelper**
  - Trả về icon phù hợp với loại tài liệu (pdf, docx, pptx, xlsx, ...).
  - Được dùng trong `Form1` để hiển thị cột Icon.

---

## 4. Luồng dữ liệu chính

### 4.1. Đăng nhập và khởi động Form chính

1. Ứng dụng khởi động tại `Program.cs`, mở `LoginForm`.
2. Người dùng đăng nhập → `DatabaseHelper_UserAuth` kiểm tra credentials.
3. Nếu hợp lệ, thiết lập `UserSession` và mở `Form1`.
4. `Form1_Load`:
   - Gọi `InitializeCustomComponents()`.
   - Gọi `LoadData()` → `DatabaseHelper.GetDocumentsForCurrentUser()`.
   - Áp dụng phân quyền giao diện (ẩn/hiện menu Quản lý, filter theo người tạo,...).

### 4.2. Quản lý tài liệu

- Thêm / Sửa → `AddEditForm` sử dụng `InsertDocument` / `UpdateDocument`.
- Xóa → `Form1` gọi `DatabaseHelper.DeleteDocument` sau khi kiểm tra quyền với `CanUserEditDocument`.
- Mở file → kiểm tra `File.Exists`, nếu không tồn tại sẽ báo lỗi.

### 4.3. Tìm kiếm & Filter nâng cao

- Tìm kiếm nhanh theo từ khóa → `SearchDocuments`.
- Filter theo môn học / loại → `FilterDocuments`.
- Filter nâng cao (từ/đến ngày, min/max dung lượng, quan trọng, người tạo) → `SearchDocumentsAdvanced`.

### 4.4. Kiểm tra file bị thiếu

- `FileIntegrityCheckForm` tải tất cả tài liệu có `duong_dan` không rỗng.
- Với mỗi record, dùng `File.Exists(duong_dan)` để xác định còn file hay không.
- Kết quả hiển thị, cho phép cập nhật hoặc xóa như mô tả ở trên.

---

## 5. Hướng dẫn mở rộng

- **Thêm cột / thuộc tính mới cho tài liệu**
  - Cập nhật `Database.sql` và bảng `tai_lieu`.
  - Bổ sung cột trong `Form1` (DataGridView) và `AddEditForm` nếu cần chỉnh sửa.
  - Cập nhật các truy vấn liên quan trong `DatabaseHelper`.

- **Thêm Form mới**
  - Tạo Form WinForms, khai báo trong `.csproj` (Visual Studio làm tự động).
  - Kết nối vào menu hoặc toolbar thông qua event click.

- **Bổ sung rule phân quyền mới**
  - Mở rộng `UserSession` (role, permission flags).
  - Điều chỉnh các hàm trong `DatabaseHelper` và `Form1.ApplyPermissions()`.

---

Tài liệu này nhằm hỗ trợ việc bảo trì, nâng cấp và review kiến trúc cho đồ án/lộ trình phát triển tiếp theo.
