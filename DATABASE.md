# Database Schema

> Cấu trúc cơ sở dữ liệu cho Study Document Manager

## Tổng quan

Database **quan_ly_tai_lieu** sử dụng SQL Server, bao gồm 7 bảng chính:

| Bảng | Mô tả |
|------|-------|
| `users` | Tài khoản người dùng |
| `user_sessions` | Phiên đăng nhập |
| `tai_lieu` | Tài liệu học tập |
| `collections` | Bộ sưu tập tài liệu |
| `collection_items` | Liên kết tài liệu - bộ sưu tập |
| `personal_notes` | Ghi chú cá nhân cho tài liệu |
| `activity_logs` | Nhật ký hoạt động |

## Sơ đồ quan hệ

```
users (1) ──────┬──── (N) user_sessions
                │
                ├──── (N) tai_lieu
                │           │
                │           └──── (N) collection_items ──── (N) collections
                │           │
                │           └──── (N) personal_notes
                │
                └──── (N) activity_logs
```

---

## Chi tiết các bảng

### 1. Bảng `users` - Người dùng

Lưu thông tin tài khoản đăng nhập.

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| `id` | INT (PK, Identity) | ID tự động tăng |
| `username` | NVARCHAR(50), UNIQUE | Tên đăng nhập |
| `password_hash` | NVARCHAR(255) | Mật khẩu đã mã hóa (BCrypt) |
| `full_name` | NVARCHAR(100) | Họ tên đầy đủ |
| `email` | NVARCHAR(100) | Email |
| `role` | NVARCHAR(20) | Vai trò: 'User', 'Admin' |
| `is_active` | BIT | Trạng thái hoạt động (1=active, 0=disabled) |
| `created_date` | DATETIME | Ngày tạo tài khoản |
| `last_login` | DATETIME | Lần đăng nhập cuối |
| `failed_login_attempts` | INT | Số lần đăng nhập thất bại |
| `locked_until` | DATETIME | Thời điểm hết khóa (nếu bị khóa) |
| `created_by` | INT | ID người tạo |
| `updated_date` | DATETIME | Ngày cập nhật |
| `updated_by` | INT | ID người cập nhật |

**Giá trị mặc định:**
- `role`: 'User'
- `is_active`: 1 (true)
- `created_date`: GETDATE()
- `failed_login_attempts`: 0

```sql
CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    full_name NVARCHAR(100),
    email NVARCHAR(100),
    role NVARCHAR(20) DEFAULT 'User',
    is_active BIT DEFAULT 1,
    created_date DATETIME DEFAULT GETDATE(),
    last_login DATETIME,
    failed_login_attempts INT DEFAULT 0,
    locked_until DATETIME,
    created_by INT,
    updated_date DATETIME,
    updated_by INT
);
```

---

### 2. Bảng `user_sessions` - Phiên đăng nhập

Theo dõi các phiên đăng nhập của người dùng.

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| `id` | INT (PK, Identity) | ID tự động tăng |
| `user_id` | INT (FK) | ID người dùng |
| `session_token` | NVARCHAR(255), UNIQUE | Token phiên |
| `ip_address` | NVARCHAR(50) | Địa chỉ IP |
| `user_agent` | NVARCHAR(500) | Thông tin trình duyệt/ứng dụng |
| `login_date` | DATETIME | Thời điểm đăng nhập |
| `last_activity` | DATETIME | Hoạt động cuối |
| `is_active` | BIT | Phiên còn hoạt động |
| `logout_date` | DATETIME | Thời điểm đăng xuất |

```sql
CREATE TABLE user_sessions (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    session_token NVARCHAR(255) NOT NULL UNIQUE,
    ip_address NVARCHAR(50),
    user_agent NVARCHAR(500),
    login_date DATETIME DEFAULT GETDATE(),
    last_activity DATETIME DEFAULT GETDATE(),
    is_active BIT DEFAULT 1,
    logout_date DATETIME,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);
```

---

### 3. Bảng `tai_lieu` - Tài liệu

Lưu thông tin tài liệu học tập của từng người dùng.

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| `id` | INT (PK, Identity) | ID tự động tăng |
| `ten` | NVARCHAR(200) | Tên tài liệu |
| `mon_hoc` | NVARCHAR(100) | Danh mục |
| `loai` | NVARCHAR(100) | Loại tài liệu |
| `duong_dan` | NVARCHAR(500) | Đường dẫn file |
| `ghi_chu` | NVARCHAR(1000) | Ghi chú |
| `ngay_them` | DATETIME | Ngày thêm |
| `kich_thuoc` | FLOAT | Kích thước file (MB) |
| `tac_gia` | NVARCHAR(100) | Tác giả |
| `quan_trong` | BIT | Đánh dấu quan trọng |
| `user_id` | INT (FK) | Người sở hữu tài liệu |
| `tags` | NVARCHAR(500) | Nhãn phân loại |
| `deadline` | DATETIME | Hạn chót |

```sql
CREATE TABLE tai_lieu (
    id INT PRIMARY KEY IDENTITY(1,1),
    ten NVARCHAR(200) NOT NULL,
    mon_hoc NVARCHAR(100),            -- Danh mục
    loai NVARCHAR(100),
    duong_dan NVARCHAR(500) NOT NULL,
    ghi_chu NVARCHAR(1000),
    ngay_them DATETIME DEFAULT GETDATE(),
    kich_thuoc FLOAT,
    tac_gia NVARCHAR(100),
    quan_trong BIT DEFAULT 0,
    user_id INT,
    tags NVARCHAR(500),
    deadline DATETIME,
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

---

### 4. Bảng `collections` - Bộ sưu tập

Cho phép người dùng tạo các bộ sưu tập tài liệu theo chủ đề.

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| `id` | INT (PK, Identity) | ID tự động tăng |
| `user_id` | INT (FK) | ID người sở hữu |
| `name` | NVARCHAR(100) | Tên bộ sưu tập |
| `description` | NVARCHAR(500) | Mô tả |
| `created_at` | DATETIME | Ngày tạo |

```sql
CREATE TABLE collections (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(500),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);
```

---

### 5. Bảng `collection_items` - Tài liệu trong bộ sưu tập

Bảng trung gian liên kết tài liệu với bộ sưu tập (quan hệ N-N).

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| `collection_id` | INT (PK, FK) | ID bộ sưu tập |
| `document_id` | INT (PK, FK) | ID tài liệu |
| `added_at` | DATETIME | Thời điểm thêm vào |

```sql
CREATE TABLE collection_items (
    collection_id INT NOT NULL,
    document_id INT NOT NULL,
    added_at DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (collection_id, document_id),
    FOREIGN KEY (collection_id) REFERENCES collections(id) ON DELETE CASCADE,
    FOREIGN KEY (document_id) REFERENCES tai_lieu(id) ON DELETE CASCADE
);
```

---

### 6. Bảng `personal_notes` - Ghi chú cá nhân

Cho phép người dùng ghi chú cho từng tài liệu.

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| `id` | INT (PK, Identity) | ID tự động tăng |
| `user_id` | INT (FK) | ID người dùng |
| `document_id` | INT (FK) | ID tài liệu |
| `note_content` | NVARCHAR(MAX) | Nội dung ghi chú |
| `status` | NVARCHAR(50) | Trạng thái: 'Chưa đọc', 'Đang đọc', 'Đã đọc' |
| `updated_at` | DATETIME | Thời điểm cập nhật |

```sql
CREATE TABLE personal_notes (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    document_id INT NOT NULL,
    note_content NVARCHAR(MAX),
    status NVARCHAR(50) DEFAULT 'Chua doc',
    updated_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (document_id) REFERENCES tai_lieu(id) ON DELETE CASCADE
);
```

---

### 7. Bảng `activity_logs` - Nhật ký hoạt động

Ghi lại các hoạt động của người dùng trong hệ thống.

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| `id` | INT (PK, Identity) | ID tự động tăng |
| `user_id` | INT (FK) | ID người dùng |
| `action` | NVARCHAR(50) | Hành động: 'Login', 'Logout', 'Create', 'Update', 'Delete' |
| `entity_type` | NVARCHAR(50) | Loại đối tượng: 'Document', 'Collection', 'User' |
| `entity_id` | INT | ID đối tượng |
| `description` | NVARCHAR(500) | Mô tả chi tiết |
| `ip_address` | NVARCHAR(50) | Địa chỉ IP |
| `user_agent` | NVARCHAR(500) | Thông tin client |
| `created_date` | DATETIME | Thời điểm ghi log |

```sql
CREATE TABLE activity_logs (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    action NVARCHAR(50) NOT NULL,
    entity_type NVARCHAR(50),
    entity_id INT,
    description NVARCHAR(500),
    ip_address NVARCHAR(50),
    user_agent NVARCHAR(500),
    created_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);
```

---

## Ràng buộc khóa ngoại

| Bảng | Cột | Tham chiếu | Hành vi xóa |
|------|-----|------------|-------------|
| `user_sessions` | `user_id` | `users.id` | CASCADE |
| `tai_lieu` | `user_id` | `users.id` | NO ACTION |
| `collections` | `user_id` | `users.id` | CASCADE |
| `collection_items` | `collection_id` | `collections.id` | CASCADE |
| `collection_items` | `document_id` | `tai_lieu.id` | CASCADE |
| `personal_notes` | `user_id` | `users.id` | CASCADE |
| `personal_notes` | `document_id` | `tai_lieu.id` | CASCADE |
| `activity_logs` | `user_id` | `users.id` | CASCADE |

---

## Phân quyền (Role-based)

| Role | Mô tả | Quyền hạn |
|------|-------|-----------|
| `User` | Người dùng thường | Quản lý tài liệu, danh mục, bộ sưu tập của riêng mình |
| `Admin` | Quản trị viên | Tất cả quyền của User + Quản lý người dùng |

**Lưu ý:** Mỗi User chỉ có thể xem và chỉnh sửa dữ liệu của riêng mình (cá nhân hóa hoàn toàn).

---

## Hướng dẫn thiết lập

### 1. Tạo Database mới

```sql
CREATE DATABASE quan_ly_tai_lieu;
GO
USE quan_ly_tai_lieu;
```

### 2. Chạy script

Chạy file `Database/database.sql` trong SQL Server Management Studio.

### 3. Tạo tài khoản Admin mặc định (tùy chọn)

```sql
-- Mật khẩu: admin123 (đã hash bằng BCrypt)
INSERT INTO users (username, password_hash, full_name, email, role, is_active)
VALUES ('admin', '$2a$11$...', 'Administrator', 'admin@example.com', 'Admin', 1);
```

---

## Xem thêm

- [README.md](README.md) - Hướng dẫn sử dụng ứng dụng
- [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - Cấu trúc mã nguồn
- [FEATURES.md](FEATURES.md) - Danh sách tính năng
