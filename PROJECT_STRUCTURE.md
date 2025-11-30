# Cấu trúc dự án Study Document Manager

Tài liệu này mô tả kiến trúc, luồng dữ liệu và cấu trúc thư mục của ứng dụng **Study Document Manager**.

Ứng dụng được xây dựng trên **Windows Forms (.NET Framework 4.8)** và **SQL Server**, theo hướng thực dụng, dễ mở rộng và dễ bảo trì.

---

## 1. Tổng quan kiến trúc

### 1.1. Kiến trúc tầng

Ứng dụng tổ chức theo mô hình 3 tầng:

```
┌─────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER (UI - Windows Forms)                    │
│  ├── Authentication/: LoginForm, RegisterForm,              │
│  │                    AccountSettingsForm                   │
│  ├── Documents/: Form1, AddEditForm, PersonalNoteForm       │
│  ├── Management/: CategoryManagementForm, UserManagementForm│
│  │               CollectionManagementForm, FileIntegrityForm│
│  ├── Reports/: Report                                       │
│  └── UI/: ToastNotification, IconHelper                     │
├─────────────────────────────────────────────────────────────┤
│  BUSINESS LAYER (Logic)                                     │
│  ├── Data/: DatabaseHelper, DatabaseHelper_UserAuth         │
│  └── Core/: UserSession, BCryptTemp                         │
├─────────────────────────────────────────────────────────────┤
│  DATA LAYER (SQL Server)                                    │
│  Database: quan_ly_tai_lieu                                 │
│  Tables: users, tai_lieu, collections, personal_notes...   │
1. **Presentation Layer (UI)**
   - **Authentication/**: `LoginForm`, `RegisterForm`, `AccountSettingsForm` - Forms xác thực và cài đặt tài khoản.
   - **Documents/**: `Form1`, `AddEditForm`, `PersonalNoteForm` - Forms quản lý tài liệu chính.
   - **Management/**: `CategoryManagementForm`, `CollectionManagementForm`, `UserManagementForm`, `FileIntegrityCheckForm` - Forms quản lý hệ thống.
   - **Reports/**: `Report` - Thống kê và báo cáo.
   - **UI/**: `ToastNotification`, `IconHelper` - UI Components dùng chung.
   - Chịu trách nhiệm giao diện, binding dữ liệu vào `DataGridView`, điều khiển filter, hiển thị dialog.

2. **Application / Business Layer**
   - **Data/**: `DatabaseHelper`, `DatabaseHelper_UserAuth` - Data Access Layer.
   - **Core/**: `UserSession`, `BCryptTemp` - Core utilities và session management.
   - Chứa logic nghiệp vụ: phân quyền, lựa chọn câu truy vấn, xử lý filter nâng cao, xác định tài liệu nào được phép sửa/xóa.

3. **Data Layer (SQL Server)**
   - Database `quan_ly_tai_lieu` với các bảng chính: `tai_lieu`, `users`, `user_sessions`, `collections`, `collection_items`, `personal_notes`, `activity_logs`.
   - `Database.sql` mô tả schema chính.
   - `Phase2_Schema.sql` mô tả schema mở rộng (tags, deadline, collections, personal_notes).

### 1.2. Các khối chức năng chính

**Phase 1 (Core):**
- **Quản lý tài liệu**: CRUD tài liệu, kèm metadata (danh mục, loại, ghi chú, dung lượng, tác giả, quan trọng).
- **Đăng nhập & phân quyền**: Chế độ cá nhân với 2 cấp quyền (Admin/User) - mỗi User chỉ quản lý tài liệu của riêng mình.
- **Filter & tìm kiếm nâng cao**: theo từ khóa, danh mục, loại, ngày, dung lượng, cờ quan trọng.
- **Context Menu trên danh sách**: thao tác nhanh trên DataGridView.
- **Kiểm tra file bị thiếu**: quét và sửa các record có đường dẫn không tồn tại.
- **Thống kê**: biểu đồ thống kê tài liệu.
- **Quản lý danh mục**: danh mục, loại tài liệu.
- **Quản lý người dùng**: chỉ dành cho Admin.
- **Phím tắt cơ bản**: một số phím tắt qua menu (xem mục 6.3).

> **Lưu ý**: Một số phím tắt trong FEATURES.md (F2, Ctrl+F, Ctrl+R, Esc, Enter mở file) chưa được triển khai.

**Chưa triển khai (Phase 1 theo FEATURES.md):**
- **Phím tắt đầy đủ**: F2 sửa, Enter mở file, Ctrl+F focus tìm kiếm, Ctrl+R refresh, Esc xóa từ khóa.
- **Recent & Favorites**: Danh sách tài liệu mở gần đây và yêu thích.
- **Lưu cấu hình người dùng đầy đủ**: Vị trí/kích thước cửa sổ, độ rộng cột, bộ lọc gần nhất (hiện chỉ có Remember Username).

**Phase 2 (Extended):**
- **Tags**: Gắn nhãn cho tài liệu, tìm kiếm theo tag.
- **Deadline**: Đặt hạn chót cho tài liệu, xem tài liệu sắp đến hạn/quá hạn.
- **Ghi chú cá nhân**: Mỗi user có ghi chú riêng cho từng tài liệu kèm trạng thái (Chưa đọc/Đang học/Đã ôn xong).
- **Bộ sưu tập (Collections)**: Gom nhóm tài liệu thành bộ sưu tập.

---

## 2. Cấu trúc thư mục & file chính

### 2.1. Root repository

```text
study-document-manager/
│
├── study-document-manager/             # Project WinForms (.csproj)
│   │
│   ├── Program.cs                      # Entry point
│   ├── App.config                      # Cấu hình (connection string,...)
│   │
│   ├── Authentication/                 # Forms xác thực
│   │   ├── LoginForm.cs / .Designer.cs / .resx       # Đăng nhập
│   │   ├── RegisterForm.cs / .Designer.cs / .resx    # Đăng ký tài khoản
│   │   └── AccountSettingsForm.cs / .Designer.cs / .resx # Cài đặt tài khoản
│   │
│   ├── Documents/                      # Forms quản lý tài liệu
│   │   ├── Form1.cs / .Designer.cs / .resx           # Form chính: danh sách, filter
│   │   ├── AddEditForm.cs / .Designer.cs / .resx     # Thêm / sửa tài liệu
│   │   └── PersonalNoteForm.cs / .Designer.cs / .resx # Ghi chú cá nhân
│   │
│   ├── Management/                     # Forms quản lý hệ thống
│   │   ├── CategoryManagementForm.cs / .Designer.cs / .resx  # Quản lý danh mục
│   │   ├── CollectionManagementForm.cs / .Designer.cs / .resx # Bộ sưu tập
│   │   ├── UserManagementForm.cs / .Designer.cs / .resx      # Quản lý người dùng
│   │   └── FileIntegrityCheckForm.cs / .Designer.cs / .resx  # Kiểm tra file thiếu
│   │
│   ├── Reports/                        # Báo cáo & thống kê
│   │   └── Report.cs / .Designer.cs / .resx          # Thống kê biểu đồ
│   │
│   ├── Data/                           # Data Access Layer
│   │   ├── DatabaseHelper.cs           # Truy vấn tài liệu, CRUD, collections
│   │   └── DatabaseHelper_UserAuth.cs  # Partial class - Auth & User management
│   │
│   ├── UI/                             # UI Components
│   │   ├── ToastNotification.cs / .resx # Thông báo Toast
│   │   └── IconHelper.cs               # Sinh icon động theo loại tài liệu
│   │
│   ├── Core/                           # Core/Shared
│   │   ├── UserSession.cs              # Thông tin user đăng nhập (static)
│   │   └── BCryptTemp.cs               # BCrypt wrapper
│   │
│   ├── Properties/                     # Resources
│   │   ├── AssemblyInfo.cs
│   │   ├── Resources.Designer.cs / Resources.resx
│   │   └── Settings.Designer.cs / Settings.settings
│   │
│   ├── assets/                         # Static assets
│   │   └── logo/                       # Logo và hình ảnh
│   │
│   ├── Database/                       # Database Scripts
│   │   ├── database.sql                # Schema chính
│   │   └── Phase2_Schema.sql           # Schema mở rộng
│   │
│   ├── packages.config                 # NuGet packages (BCrypt.Net-Next)
│   └── study-document-manager.csproj   # Project file
│
├── packages/                           # NuGet packages folder
│   └── BCrypt.Net-Next.4.0.3/
│
├── README.md                           # Giới thiệu & hướng dẫn sử dụng
├── DATABASE.md                         # Cấu trúc database chi tiết
├── FEATURES.md                         # Roadmap tính năng
├── PROJECT_STRUCTURE.md                # (file hiện tại) mô tả kiến trúc
├── CONTRIBUTING.md                     # Hướng dẫn đóng góp
├── LICENSE                             # Giấy phép MIT
└── study-document-manager.sln          # Solution file
```

### 2.2. Database Schema

> 📋 **Chi tiết schema database xem tại [DATABASE.md](DATABASE.md)**

Database `quan_ly_tai_lieu` gồm 7 bảng chính:
- `users` - Tài khoản người dùng
- `tai_lieu` - Tài liệu (kèm tags, deadline từ Phase 2)
- `collections` - Bộ sưu tập (Phase 2)
- `collection_items` - Tài liệu trong bộ sưu tập (Phase 2)
- `personal_notes` - Ghi chú cá nhân (Phase 2)
- `user_sessions` - Phiên đăng nhập
- `activity_logs` - Nhật ký hoạt động

---

## 3. Hệ thống phân quyền

### 3.1. Mô hình cá nhân hóa (Personal Mode)

Ứng dụng sử dụng mô hình **cá nhân hóa** với 2 cấp quyền:

- **User**: Quản lý hoàn toàn dữ liệu của riêng mình (tài liệu, danh mục, bộ sưu tập, ghi chú). Không thể xem/sửa/xóa dữ liệu của người khác.
- **Admin**: Có toàn quyền User + quản lý người dùng (thêm/sửa/xóa tài khoản, đổi vai trò, khóa/mở khóa).

### 3.2. Ma trận quyền

| Chức năng | User | Admin |
|-----------|:----:|:-----:|
| Xem tài liệu của mình | ✓ | ✓ |
| Xem tài liệu của người khác | ✗ | ✗ |
| Thêm tài liệu | ✓ | ✓ |
| Sửa tài liệu của mình | ✓ | ✓ |
| Sửa tài liệu của người khác | ✗ | ✗ |
| Xóa tài liệu của mình | ✓ | ✓ |
| Xóa tài liệu của người khác | ✗ | ✗ |
| Quản lý danh mục (của mình) | ✓ | ✓ |
| Quản lý người dùng | ✗ | ✓ |

### 3.3. UserSession Properties

```csharp
UserSession.UserId          // ID của user đang đăng nhập
UserSession.Username        // Tên đăng nhập
UserSession.FullName        // Họ tên đầy đủ
UserSession.Role            // "Admin" | "User"
UserSession.IsAdmin         // Role == "Admin"
UserSession.IsUser          // Role == "User"
UserSession.CanManageCategories  // true (tất cả user đều có quyền)
```

> **Lưu ý**: Các property `IsTeacher`, `IsStudent`, `CanEditAllDocuments` đã bị deprecated và luôn trả về `false`.

---

## 4. Vai trò từng Form và lớp chính

### 4.1. Documents/Form1 – Màn hình chính

**Chức năng:**

- Hiển thị danh sách tài liệu trong `DataGridView`.
- Panel tìm kiếm nhanh (từ khóa, danh mục, loại).
- **Filter nâng cao**: khoảng ngày, khoảng dung lượng, chỉ tài liệu quan trọng.
- Toolbar & menu:
  - Thêm, Sửa, Xóa, Mở file, Xuất dữ liệu.
  - Menu **Công cụ**: Thống kê, Quản lý Danh mục và Loại, Kiểm tra file bị thiếu.
  - Menu **Theo dõi**: Sắp đến hạn (7 ngày), Tài liệu quá hạn, Quản lý bộ sưu tập.
  - Menu **Tài khoản**: Cài đặt tài khoản, Đăng xuất.
  - Menu **Quản lý** (Admin only): Quản lý người dùng.
  - Nút **Đăng xuất** ở góc phải menu bar.
- **Toast Notification**: Hiển thị thông báo thành công/lỗi/cảnh báo kiểu web hiện đại.
- **Context menu** trên mỗi dòng:
  - Mở file
  - Sửa / Xóa
  - Copy đường dẫn file
  - Mở thư mục chứa file
  - Đánh dấu / Bỏ đánh dấu quan trọng
  - Ghi chú cá nhân (Phase 2)
  - Thêm vào bộ sưu tập (Phase 2)
- Hỗ trợ **Drag & Drop** file để thêm nhanh tài liệu.
- **CellFormatting**: Hiển thị icon theo loại file, highlight deadline sắp hết/quá hạn.

**Liên kết với backend:**

- Gọi `DatabaseHelper.GetDocumentsForCurrentUser()` để load dữ liệu của user hiện tại.
- Gọi `DatabaseHelper.SearchDocuments`, `FilterDocuments`, `SearchDocumentsAdvanced` để áp dụng filter.
- Gọi `DatabaseHelper.GetUpcomingDeadlines()`, `GetOverdueDocuments()` cho Phase 2.
- Sử dụng `DatabaseHelper.DeleteDocument`, `CanUserEditDocument` để xóa / kiểm tra quyền.
- Sử dụng `UserSession` để biết `UserId`, `Role`, `IsAdmin / IsUser`.

### 4.2. Documents/AddEditForm – Thêm / sửa tài liệu

- Form nhập thông tin tài liệu:
  - Tên, danh mục, loại, đường dẫn file, ghi chú, kích thước, tác giả.
  - Checkbox **Tài liệu quan trọng (★)**.
  - **Tags** (Phase 2): nhãn phân cách bởi dấu `;`.
  - **Deadline** (Phase 2): checkbox kích hoạt + DateTimePicker.
- Chọn file bằng `OpenFileDialog`, tự động:
  - Điền tên tài liệu từ tên file nếu để trống.
  - Tính kích thước file (MB).
- Khi lưu:
  - Thêm mới: `DatabaseHelper.InsertDocument(...)` - tự động gán `user_id = UserSession.UserId`.
  - Cập nhật: `DatabaseHelper.UpdateDocument(...)`.
- Validate: yêu cầu tên & đường dẫn, kiểm tra file tồn tại.

### 4.3. Management/FileIntegrityCheckForm – Kiểm tra file bị thiếu

- Quét toàn bộ bảng `tai_lieu` để tìm các record có `duong_dan` không còn tồn tại trên ổ đĩa.
- Hiển thị danh sách file bị thiếu trong `DataGridView` với các cột: ID, Tên tài liệu, Đường dẫn, Hành động.
- Với từng dòng, context menu/nút **Xử lý** cho phép:
  - **Chọn file mới...** → cập nhật lại `duong_dan`.
  - **Xóa đường dẫn (giữ metadata)** → đặt `duong_dan` thành chuỗi rỗng.
  - **Xóa tài liệu** → xóa bản ghi khỏi `tai_lieu`.
- Có progress bar & label tiến trình, nút **Quét** và **Xóa tất cả**.

### 4.4. Management/CategoryManagementForm – Quản lý danh mục

- Quản lý Danh mục và Loại tài liệu (lấy distinct từ bảng `tai_lieu` của user hiện tại).
- Thêm / sửa / xóa danh mục, với cảnh báo khi thao tác có thể ảnh hưởng đến dữ liệu tài liệu.
- Tất cả users đều có quyền quản lý danh mục của riêng mình.
- Sau khi đóng form, `Form1` reload lại dữ liệu để áp dụng thay đổi.

### 4.5. Reports/Report – Thống kê

- Sử dụng `System.Windows.Forms.DataVisualization.Charting` để:
  - Thống kê số lượng tài liệu theo danh mục, loại.
  - Chọn kiểu biểu đồ: Cột dọc, Cột ngang, Tròn, Đường, Vùng.
- Màu sắc Material Design cho từng data point.
- Lấy dữ liệu tổng hợp từ `DatabaseHelper.GetStatisticsBySubject()`, `GetStatisticsByType()`.

### 4.6. Authentication/ – LoginForm, RegisterForm, UserManagementForm

- **LoginForm**
  - Người dùng nhập `username` và `password`.
  - Gọi `DatabaseHelper.AuthenticateUser()` để xác thực với BCrypt.
  - Nếu thành công, thiết lập `UserSession` và mở `Form1`.
  - Hỗ trợ Remember Me (lưu username vào Settings).

- **RegisterForm**
  - Cho phép tạo tài khoản mới (mặc định là User).
  - Validation: username ≥3 ký tự, password ≥6 ký tự, email hợp lệ.
  - Mật khẩu được hash bằng BCrypt.

- **UserManagementForm** (Admin only)
  - Xem danh sách users với DataGridView.
  - Thêm user mới (mở RegisterForm).
  - Đổi mật khẩu (Admin không cần old password).
  - Đổi vai trò (Admin/User).
  - Khóa/Mở khóa tài khoản.
  - Xóa user (không được xóa chính mình).

### 4.7. Documents/PersonalNoteForm – Ghi chú cá nhân (Phase 2)

- Mỗi user có ghi chú riêng cho từng tài liệu.
- Nội dung ghi chú (textarea).
- Trạng thái học tập: "Chưa đọc" | "Đang học" | "Đã ôn xong".
- Lưu vào bảng `personal_notes` với `user_id` + `document_id`.

### 4.8. Management/CollectionManagementForm – Bộ sưu tập (Phase 2)

- ListView hiển thị danh sách bộ sưu tập của user.
- DataGridView hiển thị tài liệu trong bộ sưu tập được chọn.
- Tạo/Xóa bộ sưu tập.
- Xóa tài liệu khỏi bộ sưu tập.
- Mở tất cả tài liệu trong bộ sưu tập cùng lúc.
- Double-click để mở từng tài liệu.

### 4.9. Authentication/AccountSettingsForm – Cài đặt tài khoản

- **Tab Thông tin cá nhân**: Xem/sửa họ tên, email.
- **Tab Đổi mật khẩu**: Đổi mật khẩu (cần nhập mật khẩu hiện tại).
- Hiển thị vai trò và thời gian đăng nhập.
- Lưu vào bảng `users` thông qua `DatabaseHelper.UpdateUserProfile()` và `ChangePasswordSelf()`.

### 4.10. Lớp trợ giúp (Data/, Core/, UI/)

- **Data/DatabaseHelper & DatabaseHelper_UserAuth**
  - Quản lý connection string (đọc từ `App.config`).
  - **Core methods:**
    - `ExecuteQuery`, `ExecuteNonQuery`, `ExecuteScalar` - thực thi SQL với SqlParameter.
    - `TestConnection()` - kiểm tra kết nối database.
  - **Document methods:**
    - `GetDocumentsForCurrentUser()` - load theo phân quyền.
    - `InsertDocument()`, `UpdateDocument()`, `DeleteDocument()`.
    - `SearchDocuments()`, `FilterDocuments()`, `SearchDocumentsAdvanced()`.
    - `GetDocumentOwnerId()`, `CanUserEditDocument()`.
  - **Phase 2 methods:**
    - `GetUpcomingDeadlines(days)`, `GetOverdueDocuments()`.
    - `GetDistinctTags()` - cho autocomplete.
    - `GetCollections()`, `CreateCollection()`, `DeleteCollection()`.
    - `GetDocumentsInCollection()`, `AddDocumentToCollection()`, `RemoveDocumentFromCollection()`.
  - **Statistics methods:**
    - `GetStatisticsBySubject()`, `GetStatisticsByType()`, `GetTotalDocumentCount()`.
  - **Category methods:**
    - `GetDistinctSubjects()`, `GetDistinctTypes()`.
    - `UpdateSubjectName()`, `UpdateTypeName()`.
    - `DeleteDocumentsBySubject()`, `DeleteDocumentsByType()`.
  - **Auth methods (partial class):**
    - `AuthenticateUser()`, `RegisterUser()`.
    - `CheckUsernameExists()`, `CheckEmailExists()`.
    - `UpdateLastLogin()`, `ChangePassword()`, `AdminResetPassword()`.
    - `GetAllUsers()`, `ToggleUserActive()`, `DeleteUser()`, `UpdateUserRole()`.
    - `UpdateUserProfile()`, `ChangePasswordSelf()` - cho AccountSettingsForm.

- **Core/UserSession** (static class)
  - Lưu thông tin user hiện tại trong suốt vòng đời ứng dụng.
  - Properties: `UserId`, `Username`, `FullName`, `Email`, `Role`, `LoginTime`.
  - Helper properties: `IsLoggedIn`, `IsAdmin`, `IsUser`, `CanManageCategories`.
  - Method: `CanEditDocument(documentUserId)`, `Logout()`.

- **UI/IconHelper** (static class)
  - `GetDocumentIcon(loai, size)` - trả về Bitmap icon theo loại tài liệu.
  - Hỗ trợ: PDF (đỏ), Word (xanh dương), PowerPoint (cam), Excel (xanh lá), Default (xám).
  - `CreateStarIcon(size)` - tạo icon sao vàng cho đánh dấu quan trọng.
  - **UI Action Icons**:
    - `CreateEyeIcon(size, isOpen)` - icon con mắt (show/hide password)
    - `CreateCloseIcon(size, color)` - icon X (đóng)
    - `CreateChevronDownIcon(size, color)` - mũi tên xuống
    - `CreateAddIcon(size, color)` - dấu cộng (thêm)
    - `CreateEditIcon(size, color)` - bút chì (sửa)
    - `CreateDeleteIcon(size, color)` - thùng rác (xóa)
    - `CreateOpenIcon(size, color)` - folder mở
    - `CreateExportIcon(size, color)` - xuất dữ liệu
    - `CreateRefreshIcon(size, color)` - làm mới
    - `CreateRoleIcon(size, color)` - quản lý vai trò
    - `CreateChartIcon(size, color)` - biểu đồ thống kê
    - `CreateSettingsIcon(size, color)` - cài đặt

- **UI/ToastNotification** (Form class)
  - Thông báo Toast hiện đại kiểu web.
  - 4 loại: `Success` (xanh lá), `Error` (đỏ), `Warning` (cam), `Info` (xanh dương).
  - Hiệu ứng fade in/out mượt mà.
  - Hiển thị ở góc trên bên phải của form cha.
  - Static methods: `Show()`, `Success()`, `Error()`, `Warning()`, `Info()`, `CloseAll()`.
  - Tự động đóng sau vài giây, có nút X để đóng nhanh.

---

## 5. Luồng dữ liệu chính

### 5.1. Đăng nhập và khởi động Form chính

```
Program.Main()
    └── LoginForm.ShowDialog()
            └── btnLogin_Click()
                    └── DatabaseHelper.AuthenticateUser(username, password)
                            └── BCrypt.Verify(password, storedHash)
                    └── UserSession (set static props)
                    └── DatabaseHelper.UpdateLastLogin()
    └── Form1 (if login success)
            └── Form1_Load()
                    └── CreateManagementMenu() - tạo menu động
                    └── InitializeCustomComponents()
                    └── LoadData() → GetDocumentsForCurrentUser()
                    └── ApplyPermissions() - ẩn/hiện theo role
```

### 5.2. Quản lý tài liệu

```
THÊM:
Form1.btn_them_Click()
    └── AddEditForm() [new]
            └── btn_luu_Click()
                    └── DatabaseHelper.InsertDocument(..., UserSession.UserId)
    └── Form1.LoadData() [refresh]

SỬA:
Form1.btn_sua_Click()
    └── DatabaseHelper.CanUserEditDocument(id, userId, role)
    └── AddEditForm(id) [edit mode]
            └── LoadDocumentData()
            └── btn_luu_Click()
                    └── DatabaseHelper.UpdateDocument(...)
    └── Form1.LoadData() [refresh]

XÓA:
Form1.btn_xoa_Click()
    └── DatabaseHelper.CanUserEditDocument()
    └── MessageBox.Confirm()
    └── DatabaseHelper.DeleteDocument(id)
    └── Form1.LoadData() [refresh]

MỞ FILE:
Form1.OpenSelectedFile()
    └── File.Exists(duong_dan)
    └── Process.Start(duong_dan)
```

### 5.3. Tìm kiếm & Filter nâng cao

```
Form1.ApplyAdvancedFilter()
    └── Collect filter values từ UI controls
    └── DatabaseHelper.SearchDocumentsAdvanced(
            keyword, danh_muc, loai, 
            fromDate, toDate, 
            minSize, maxSize, 
            isImportant
        )
            └── Build SQL với WHERE clauses động
            └── Luôn filter theo user_id (mỗi user chỉ thấy tài liệu của mình)
    └── dgvDocuments.DataSource = result
    └── SetupDataGridView()
```

### 5.4. Kiểm tra file bị thiếu

```
FileIntegrityCheckForm.ScanForMissingFiles()
    └── SELECT id, ten, duong_dan FROM tai_lieu WHERE duong_dan != ''
    └── foreach row:
            └── File.Exists(duong_dan)
            └── if (!exists) → add to missingFilesData
    └── Display in DataGridView

Xử lý từng file:
    ├── SelectNewFile() → UPDATE duong_dan
    ├── ClearFilePath() → SET duong_dan = ''
    └── DeleteDocument() → DELETE FROM tai_lieu
```

### 5.5. Phase 2: Collections & Personal Notes

```
THÊM VÀO BỘ SƯU TẬP:
Form1.ctxMenuAddToCollection_Click()
    └── DatabaseHelper.GetCollections()
    └── Show dialog chọn collection
    └── DatabaseHelper.AddDocumentToCollection(collectionId, docId)

GHI CHÚ CÁ NHÂN:
Form1.ctxMenuPersonalNote_Click()
    └── PersonalNoteForm(docId, docName)
            └── LoadNote() → SELECT FROM personal_notes
            └── btnSave_Click() → INSERT/UPDATE personal_notes
```

---

## 6. UI/UX Conventions

> **Chi tiết màu sắc Material Design xem tại [README.md](README.md#bảng-màu-material-design)**

### 6.1. DataGridView Styling

- Alternating rows: `#F5F5F5`
- Selection: `#2196F3` với white text  
- Header: `#34495E` với white text, font bold
- Row height: 32px
- Column Icon: 30px width, ImageLayout.Zoom

### 6.2. Phím tắt

> **Danh sách phím tắt đầy đủ xem tại [README.md](README.md#phím-tắt)**

#### Chưa triển khai (theo FEATURES.md Phase 1)

| Phím | Chức năng dự kiến |
|------|-------------------|
| `F2` | Sửa tài liệu đang chọn (thay thế Ctrl+U) |
| `Enter` | Mở tài liệu đang chọn (khi focus ở DataGridView) |
| `Ctrl+F` | Focus ô tìm kiếm |
| `Ctrl+R` | Refresh danh sách (alternative cho F5) |
| `Esc` | Xóa từ khóa tìm kiếm hiện tại |

---

## 7. Bảo mật

1. **Password hashing**: BCrypt.Net-Next với salt tự động
2. **SQL injection protection**: Tất cả queries sử dụng `SqlParameter`
3. **Session management**: `UserSession` static class
4. **Role-based access control**: Check quyền trước mỗi action
5. **Account lockout**: `failed_login_attempts`, `locked_until` (schema sẵn)

---

## 8. Hướng dẫn mở rộng

### 8.1. Thêm cột / thuộc tính mới cho tài liệu

1. Cập nhật `Database.sql` hoặc chạy `ALTER TABLE tai_lieu ADD [column] ...`
2. Cập nhật `DatabaseHelper.InsertDocument()` và `UpdateDocument()` - thêm parameter mới
3. Cập nhật `AddEditForm` - thêm control nhập liệu
4. Cập nhật `Form1.SetupDataGridView()` - cấu hình hiển thị cột

### 8.2. Thêm Form mới

1. Add → Windows Form trong Visual Studio
2. Tạo event handler để mở form từ menu/toolbar
3. Sử dụng `DatabaseHelper` để truy vấn dữ liệu
4. Kiểm tra quyền với `UserSession` nếu cần

### 8.3. Bổ sung rule phân quyền mới

1. Thêm property mới trong `UserSession.cs`:
   ```csharp
   public static bool CanDoSomething => IsAdmin; // hoặc logic khác
   ```
2. Cập nhật `Form1.ApplyPermissions()` để ẩn/hiện UI
3. Thêm check trong `DatabaseHelper` nếu cần filter data (luôn filter theo `user_id`)

### 8.4. Thêm loại biểu đồ mới

1. Mở `Report.cs`
2. Thêm item vào `InitializeChartTypes()` 
3. Xử lý trong `GetSelectedChartType()` với `SeriesChartType` tương ứng

---

## 9. Dependencies

### 9.1. NuGet Packages

- **BCrypt.Net-Next 4.0.3** - Password hashing

### 9.2. .NET Framework References

- `System.Data.SqlClient` - SQL Server connection
- `System.Windows.Forms.DataVisualization` - Charts
- `System.Xml.Linq` - Đọc App.config
- `Microsoft.VisualBasic` - InputBox dialog

### 9.3. Connection String (App.config)

```xml
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="Server=...;Database=quan_ly_tai_lieu;..." 
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

---

Tài liệu này nhằm hỗ trợ việc bảo trì, nâng cấp và review kiến trúc cho đồ án/lộ trình phát triển tiếp theo.
