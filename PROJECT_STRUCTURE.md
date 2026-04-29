# Cấu trúc dự án Document Sharing Manager (Monorepo Mode)

Tài liệu này mô tả kiến trúc, luồng dữ liệu và cấu trúc thư mục của ứng dụng **Document Sharing Manager**.

Ứng dụng được xây dựng trên **Windows Forms (.NET Framework 4.8)** và **SQLite**, hỗ trợ chế độ **Managed Storage** và lộ trình nâng cấp lên Cloud-ready.

---

## 1. Tổng quan kiến trúc

### 1.1. Kiến trúc tầng

Ứng dụng tổ chức theo mô hình tách biệt Core, UI và Infrastructure:

```
┌─────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER (UI - Windows Forms)                    │
│  Project: document-sharing-manager                          │
│  ├── Documents/: Dashboard, AddEditForm, RecentFilesForm,   │
│  │       BatchImportForm, ...                               │
│  ├── UI/: AppTheme, ToastNotification, Modern Controls      │
│  └── assets/: logo, icons, screenshots                      │
├─────────────────────────────────────────────────────────────┤
│  DOMAIN & BUSINESS LAYER (Core)                             │
│  Project: document-sharing-manager.Core                     │
│  ├── Domain/: BaseEntity, Document                          │
│  ├── Interfaces/: IRepository, IStorageService              │
│  └── Services/: FileStorageService, AppVersion              │
├─────────────────────────────────────────────────────────────┤
│  INFRASTRUCTURE LAYER (Data & Storage)                      │
│  ├── Data/: DatabaseHelper (SQLite)                         │
│  └── Repositories/: DocumentRepository                      │
└─────────────────────────────────────────────────────────────┘
```

1. **Presentation Layer (UI)**
   - **document-sharing-manager**: Chứa mã nguồn WinForms, các Form giao diện và logic UI.
   - **UI/Controls**: Các control tùy chỉnh theo phong cách hiện đại.

2. **Domain & Business Layer**
   - **document-sharing-manager.Core**: Chứa các Entity cốt lõi và logic nghiệp vụ không phụ thuộc UI.
   - **FileStorageService**: Quản lý việc lưu trữ file vật lý trong thư mục `documents/` của ứng dụng.

3. **Infrastructure Layer**
   - **DatabaseHelper**: Quản lý SQLite database (`document_sharing.db`).
   - **Repositories**: Thực thi các Interface truy xuất dữ liệu.

---

## 2. Cấu trúc thư mục & file chính

### 2.1. Root repository

```text
Document-Sharing/
│
├── document-sharing-manager/           # Project WinForms (.csproj)
│   ├── Program.cs                      # Entry point
│   ├── Documents/                      # UI Forms
│   ├── UI/                             # Theme & Controls
│   └── assets/                         # Static assets
│
├── document-sharing-manager.Core/      # Business Logic Layer
│   ├── Domain/                         # Entities
│   ├── Services/                       # Application Services (Storage, etc.)
│   └── Data/                           # Database Access (SQLite)
│
├── document-sharing-manager.Tests/     # Unit Tests
│
├── .ai_docs/                           # Tài liệu thiết kế & AI Context
├── scripts/                            # CI/CD Automation scripts
├── document-sharing-manager.sln        # Solution file
└── README.md                           # Giới thiệu dự án
```

### 2.2. Database Schema (SQLite)

Database file: `data/document_sharing.db`

Các bảng chính:
- `tai_lieu`: Lưu thông tin tài liệu.
- `collections`: Quản lý bộ sưu tập.
- `collection_items`: Liên kết tài liệu - bộ sưu tập.
- `personal_notes`: Ghi chú cá nhân.
- `recent_files`: Lịch sử mở file.

---

## 3. Luồng dữ liệu chính

### 3.1. Managed Storage Flow
Khi người dùng chọn một file từ ổ cứng:
1. `FileStorageService.ImportFile()` copy file vào thư mục `documents/` của ứng dụng.
2. Đường dẫn tương đối được lưu vào DB.
3. Khi mở file, `FileStorageService.ResolvePath()` chuyển đổi về đường dẫn tuyệt đối để `FileStorageService.OpenFile()` thực thi.

---

## 4. Dependencies
- **System.Data.SQLite**: SQLite provider.
- **.NET Framework 4.8**: Nền tảng UI.
- **.NET Standard 2.0**: Nền tảng cho project Core.
