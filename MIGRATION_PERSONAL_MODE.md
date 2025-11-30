# Kế hoạch chuyển đổi sang chế độ cá nhân hóa

> Tài liệu mô tả việc chuyển đổi từ hệ thống **3 cấp quyền trường học** (Admin/Teacher/Student) sang hệ thống **2 cấp quyền cá nhân hóa** (Admin/User).

---

## 1. Tổng quan thay đổi

### 1.1. Mô hình hiện tại (Trường học)

```
┌─────────────────────────────────────────────────────────────┐
│                        ADMIN                                │
│  - Quản lý tất cả tài liệu                                 │
│  - Quản lý người dùng                                       │
│  - Quản lý danh mục                                         │
├─────────────────────────────────────────────────────────────┤
│                       TEACHER                               │
│  - Xem tất cả tài liệu                                     │
│  - Quản lý tài liệu của mình                               │
│  - Quản lý danh mục                                         │
├─────────────────────────────────────────────────────────────┤
│                       STUDENT                               │
│  - Chỉ xem/quản lý tài liệu của mình                       │
│  - Không quản lý danh mục                                   │
└─────────────────────────────────────────────────────────────┘
```

### 1.2. Mô hình mới (Cá nhân hóa)

```
┌─────────────────────────────────────────────────────────────┐
│                        ADMIN                                │
│  - Quản lý người dùng (thêm/sửa/xóa tài khoản)             │
│  - Xem thống kê hệ thống                                    │
│  - KHÔNG thấy dữ liệu cá nhân của user khác                │
│  - Quản lý tài liệu/danh mục CỦA RIÊNG MÌNH                │
├─────────────────────────────────────────────────────────────┤
│                         USER                                │
│  - Quản lý TẤT CẢ mọi thứ của riêng mình:                  │
│    • Tài liệu (CRUD)                                        │
│    • Danh mục môn học, loại tài liệu                       │
│    • Bộ sưu tập, ghi chú cá nhân                           │
│    • Tags, deadline                                         │
│  - Dữ liệu hoàn toàn riêng tư, không ai thấy được          │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Ma trận quyền mới

| Chức năng | Admin | User |
|-----------|:-----:|:----:|
| **Quản lý tài khoản** | ✓ | ✗ |
| Thêm user mới | ✓ | ✗ |
| Đổi mật khẩu user khác | ✓ | ✗ |
| Khóa/Mở khóa tài khoản | ✓ | ✗ |
| Xóa tài khoản | ✓ | ✗ |
| **Quản lý tài liệu** | Của mình | Của mình |
| Xem tài liệu | Của mình | Của mình |
| Thêm/Sửa/Xóa tài liệu | Của mình | Của mình |
| **Quản lý danh mục** | Của mình | Của mình |
| Môn học (riêng) | ✓ | ✓ |
| Loại tài liệu (riêng) | ✓ | ✓ |
| **Tính năng Phase 2** | Của mình | Của mình |
| Bộ sưu tập | ✓ | ✓ |
| Ghi chú cá nhân | ✓ | ✓ |
| Tags & Deadline | ✓ | ✓ |
| **Thống kê** | Hệ thống | Của mình |

---

## 3. Thay đổi Database Schema

### 3.1. Bảng `users`

```sql
-- Thay đổi cột role
ALTER TABLE users 
DROP CONSTRAINT [nếu có constraint cho role];

-- Cập nhật giá trị role
UPDATE users SET role = 'User' WHERE role IN ('Teacher', 'Student');

-- Thêm constraint mới (tùy chọn)
ALTER TABLE users 
ADD CONSTRAINT CK_users_role CHECK (role IN ('Admin', 'User'));
```

### 3.2. Bảng danh mục mới (tùy chọn - nếu muốn danh mục riêng)

**Hiện tại**: Môn học và Loại lấy DISTINCT từ bảng `tai_lieu` (dùng chung).

**Đề xuất mới**: Tạo bảng riêng để mỗi user có danh mục riêng.

```sql
-- Bảng môn học riêng cho từng user
CREATE TABLE user_subjects (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    subject_name NVARCHAR(100) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_user_subjects_users FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT UQ_user_subjects UNIQUE (user_id, subject_name)
);

-- Bảng loại tài liệu riêng cho từng user
CREATE TABLE user_document_types (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    type_name NVARCHAR(100) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_user_types_users FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT UQ_user_types UNIQUE (user_id, type_name)
);
```

**Lưu ý**: Nếu giữ nguyên cách lấy DISTINCT từ `tai_lieu`, thì tự động mỗi user chỉ thấy danh mục của mình (vì đã filter theo `user_id`).

---

## 4. Thay đổi Code

### 4.1. UserSession.cs

```csharp
// XÓA các property cũ
// public static bool IsTeacher => ...
// public static bool IsStudent => ...
// public static bool CanManageCategories => ...
// public static bool CanEditAllDocuments => ...

// GIỮ LẠI
public static bool IsAdmin => Role == "Admin";

// THÊM MỚI
public static bool IsUser => Role == "User";

// SỬA LẠI
public static bool CanManageUsers => IsAdmin;  // Chỉ Admin quản lý user
public static bool CanManageOwnData => true;   // Ai cũng quản lý data của mình

// XÓA logic cũ trong CanEditDocument
public static bool CanEditDocument(int documentUserId)
{
    // Mọi user chỉ sửa được tài liệu của mình
    return documentUserId == UserId;
}
```

### 4.2. DatabaseHelper.cs

```csharp
// GetDocumentsForCurrentUser() - GIỮ NGUYÊN logic filter theo user_id
// Xóa logic "Teacher/Admin thấy tất cả"

public static DataTable GetDocumentsForCurrentUser()
{
    // LUÔN filter theo user_id, kể cả Admin
    string query = @"
        SELECT * FROM tai_lieu 
        WHERE user_id = @userId 
        ORDER BY ngay_them DESC";
    
    return ExecuteQuery(query, 
        new SqlParameter("@userId", UserSession.UserId));
}

// GetDistinctSubjects() - Filter theo user
public static DataTable GetDistinctSubjects()
{
    string query = @"
        SELECT DISTINCT mon_hoc 
        FROM tai_lieu 
        WHERE user_id = @userId AND mon_hoc IS NOT NULL
        ORDER BY mon_hoc";
    
    return ExecuteQuery(query,
        new SqlParameter("@userId", UserSession.UserId));
}

// Tương tự cho GetDistinctTypes()
```

### 4.3. Form1.cs

```csharp
// ApplyPermissions() - Đơn giản hóa
private void ApplyPermissions()
{
    // Menu quản lý user - chỉ Admin
    if (menuManagement != null)
    {
        menuManagement.Visible = UserSession.IsAdmin;
    }
    
    // Ẩn cột "Người tạo" - không cần thiết nữa vì mọi thứ đều của mình
    if (dgvDocuments.Columns.Contains("created_by"))
    {
        dgvDocuments.Columns["created_by"].Visible = false;
    }
    
    // Ẩn filter "Người tạo"
    if (cboFilterCreator != null)
    {
        cboFilterCreator.Visible = false;
        lblFilterCreator.Visible = false;
    }
    
    // Mọi user đều có quyền quản lý danh mục (của mình)
    menuViewCategories.Enabled = true;
}
```

### 4.4. CategoryManagementForm.cs

```csharp
// Cho phép mọi user truy cập (không check IsTeacher/IsAdmin)
// Nhưng chỉ hiển thị danh mục của user đó

private void LoadSubjects()
{
    // Đã tự động filter theo user_id trong DatabaseHelper
    DataTable dt = DatabaseHelper.GetDistinctSubjects();
    // ...
}
```

### 4.5. RegisterForm.cs

```csharp
// Xóa option chọn Teacher/Student
// Mặc định tạo User

private void btnRegister_Click(object sender, EventArgs e)
{
    // ...
    string role = "User"; // Mặc định là User, không cho chọn
    DatabaseHelper.RegisterUser(username, password, fullName, email, role);
    // ...
}

// Hoặc: Chỉ Admin mới tạo được tài khoản mới
// Xóa form đăng ký công khai
```

---

## 5. Các file cần sửa

### 5.1. Danh sách file

| File | Thay đổi |
|------|----------|
| `UserSession.cs` | Xóa IsTeacher, IsStudent; đơn giản hóa logic |
| `DatabaseHelper.cs` | Luôn filter theo user_id |
| `DatabaseHelper_UserAuth.cs` | Cập nhật role validation |
| `Form1.cs` | Đơn giản hóa ApplyPermissions() |
| `Form1.Designer.cs` | Xóa/ẩn controls không cần |
| `CategoryManagementForm.cs` | Cho phép mọi user, filter theo user_id |
| `RegisterForm.cs` | Xóa dropdown chọn role |
| `UserManagementForm.cs` | Cập nhật dropdown role (chỉ Admin/User) |
| `LoginForm.cs` | Không thay đổi |
| `AddEditForm.cs` | Không thay đổi (đã gán user_id) |

### 5.2. Database Scripts

```sql
-- migration_personal_mode.sql

-- 1. Cập nhật role hiện có
UPDATE users SET role = 'User' WHERE role = 'Teacher';
UPDATE users SET role = 'User' WHERE role = 'Student';

-- 2. Đảm bảo có ít nhất 1 Admin
-- (Kiểm tra trước khi chạy)
SELECT COUNT(*) FROM users WHERE role = 'Admin';

-- 3. (Tùy chọn) Tạo bảng danh mục riêng
-- Xem phần 3.2
```

---

## 6. Lộ trình thực hiện

### Sprint 1: Chuẩn bị (1 ngày)

1. [X] Backup database
2. [X] Tạo branch mới: `feature/personal-mode`
3. [ ] Chạy script cập nhật role trong database

### Sprint 2: Backend (1-2 ngày)

1. [ ] Cập nhật `UserSession.cs`
2. [ ] Cập nhật `DatabaseHelper.cs` - filter theo user_id
3. [ ] Cập nhật `DatabaseHelper_UserAuth.cs` - role validation
4. [ ] Test các query

### Sprint 3: Frontend (1-2 ngày)

1. [ ] Cập nhật `Form1.cs` - ApplyPermissions()
2. [ ] Cập nhật `CategoryManagementForm.cs`
3. [ ] Cập nhật `RegisterForm.cs` hoặc xóa đăng ký công khai
4. [ ] Cập nhật `UserManagementForm.cs`
5. [ ] Ẩn/xóa các controls không cần

### Sprint 4: Testing & Cleanup (1 ngày)

1. [ ] Test với tài khoản Admin
2. [ ] Test với tài khoản User
3. [ ] Kiểm tra không thấy dữ liệu của user khác
4. [ ] Cập nhật tài liệu (README, PROJECT_STRUCTURE)

---

## 7. Câu hỏi cần quyết định

1. **Đăng ký tài khoản**: 
   - [ ] Cho phép tự đăng ký (mặc định là User)?
   - [ ] Chỉ Admin mới tạo được tài khoản?

2. **Danh mục (Môn học, Loại)**:
   - [ ] Dùng chung DISTINCT từ tai_lieu (tự động riêng theo user_id)?
   - [ ] Tạo bảng riêng để user quản lý danh mục?

3. **Admin có thấy dữ liệu user khác không?**
   - [ ] Không (Admin cũng là user bình thường + quyền quản lý tài khoản)
   - [ ] Có (Admin xem được tất cả để hỗ trợ khi cần)

4. **Migrate dữ liệu cũ**:
   - [ ] Giữ nguyên dữ liệu, chỉ đổi role
   - [ ] Gộp Teacher + Student thành User

---

## 8. Ưu/Nhược điểm của mô hình mới

### Ưu điểm

- ✅ Đơn giản hơn, dễ hiểu
- ✅ Phù hợp sử dụng cá nhân
- ✅ Bảo mật: mỗi người chỉ thấy của mình
- ✅ Ít code hơn, dễ maintain
- ✅ User có toàn quyền với data của mình

### Nhược điểm

- ❌ Không phù hợp nếu cần chia sẻ tài liệu
- ❌ Admin không thể hỗ trợ user nếu có vấn đề
- ❌ Mất tính năng "Teacher xem tất cả để quản lý lớp"

---

## 9. Tham khảo

- [PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE.md) - Cấu trúc hiện tại
- [FEATURES.md](./FEATURES.md) - Roadmap tính năng
- [Database.sql](./study-document-manager/Database/Database.sql) - Schema gốc

---

*Tài liệu này phục vụ việc lên kế hoạch chuyển đổi. Xác nhận các câu hỏi ở mục 7 trước khi triển khai.*
