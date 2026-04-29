# Đóng góp cho Document Sharing Manager

Chào mừng bạn đến với dự án **Document Sharing Manager**! Chúng tôi rất vui vì bạn quan tâm và muốn đóng góp để làm cho ứng dụng quản lý tài liệu cá nhân này trở nên tốt hơn. Dù là sửa lỗi nhỏ, bổ sung tính năng hay cải thiện tài liệu, mọi đóng góp đều được trân trọng.

## Mục lục

- [Chuẩn bị môi trường](#chuẩn-bị-môi-trường)
- [Quy trình đóng góp](#quy-trình-đóng-góp)
- [Quy ước Code](#quy-ước-code)
- [Báo cáo lỗi](#báo-cáo-lỗi)
- [Đề xuất tính năng](#đề-xuất-tính-năng)
- [Liên hệ](#liên-hệ)

---

## Chuẩn bị môi trường

Để bắt đầu phát triển, bạn cần chuẩn bị môi trường như sau:

1.  **IDE**: Cài đặt [Visual Studio 2019](https://visualstudio.microsoft.com/vs/older-downloads/) hoặc [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (Khuyên dùng bản 2022 Community).
2.  **Workload**: Khi cài đặt Visual Studio, hãy chọn workload **.NET Desktop Development**.
3.  **Runtime**: Đảm bảo đã cài đặt **.NET Framework 4.8** Developer Pack.
4.  **Database**: Dự án sử dụng **SQLite** (Local DB) nên bạn **KHÔNG** cần cài đặt SQL Server. Database sẽ được tự động khởi tạo khi chạy ứng dụng lần đầu.

---

## Quy trình đóng góp

Chúng tôi tuân theo quy trình GitHub Flow cơ bản:

### 1. Fork và Clone
Fork repository này về tài khoản GitHub của bạn, sau đó clone về máy:

```bash
git clone https://github.com/AnhGam/Document-Sharing.git
cd Document-Sharing
```

### 2. Tạo Branch
Luôn tạo branch mới cho mỗi tính năng hoặc bản sửa lỗi. Đặt tên branch rõ ràng theo quy ước:

- Tính năng mới: `feature/ten-tinh-nang` (ví dụ: `feature/dark-mode`, `feature/export-pdf`)
- Sửa lỗi: `fix/ten-loi` (ví dụ: `fix/login-crash`, `fix/typo-readme`)

```bash
git checkout -b feature/them-chuc-nang-moi
```

### 3. Code và Commit
- Viết code rõ ràng, tuân thủ [Quy ước Code](#quy-ước-code).
- Commit message cần ngắn gọn nhưng đầy đủ ý nghĩa (khuyên dùng tiếng Anh theo chuẩn Conventional Commits):
  - `feat: Add dark mode toggle`
  - `fix: Resolve database connection issue`
  - `docs: Update installation guide`

### 4. Push và tạo Pull Request (PR)
Đẩy branch của bạn lên GitHub:

```bash
git push origin feature/them-chuc-nang-moi
```

Sau đó truy cập repository gốc và tạo Pull Request. Vui lòng mô tả chi tiết những thay đổi bạn đã thực hiện trong PR.

---

## Quy ước Code

### Code Style (Clean Code)
- **Giữ code sạch sẽ**: Xóa các đoạn code thừa, code bị comment không dùng đến.
- **Comment**: Thêm comment cho các đoạn logic phức tạp để người khác dễ hiểu.
- **Naming**:
  - Class, Method, Property: `PascalCase` (ví dụ: `UserManager`, `GetData`)
  - Variable, Parameter: `camelCase` (ví dụ: `userList`, `documentId`)
  - Private field: `_camelCase` (ví dụ: `_connectionString`)

### UI Theme (Quan trọng)
Dự án sử dụng bộ màu sắc hiện đại (Teal/Emerald) được định nghĩa tập trung trong `UI/AppTheme.cs`. **Tuyệt đối không hardcode màu sắc** trong Form designer trừ khi cần thiết. Hãy sử dụng các biến từ `AppTheme`.

Ví dụ các màu chủ đạo:

```csharp
// Sử dụng AppTheme để đảm bảo tính nhất quán
btnSave.BackColor = AppTheme.Primary;        // Teal (#14b8a6)
btnCancel.BackColor = AppTheme.Secondary;    // Emerald (#10b981)
lblError.ForeColor = AppTheme.StatusError;   // Red (#ef4444)
this.BackColor = AppTheme.BackgroundMain;    // White (#ffffff)
```

Một số màu thường dùng:
- **Primary**: Teal (`#14b8a6`) - Dùng cho nút chính, highlight.
- **Secondary**: Emerald (`#10b981`) - Dùng cho các hành động phụ hoặc trạng thái thành công.
- **Background**: White (`#ffffff`) hoặc Soft Gray (`#f8fafc`).
- **Text**: Primary Dark (`#0f172a`) cho nội dung chính.

---

## Báo cáo lỗi

Nếu bạn phát hiện lỗi, hãy tạo Issue trên GitHub với các thông tin sau:

1.  **Mô tả lỗi**: Chuyện gì đã xảy ra?
2.  **Môi trường**: Windows version (10/11), .NET Framework version.
3.  **Các bước tái hiện**: Làm thế nào để gặp lỗi này?
4.  **Kết quả mong đợi**: Điều gì nên xảy ra?
5.  **Screenshot**: Ảnh chụp màn hình lỗi (nếu có).

---

## Đề xuất tính năng

Bạn có ý tưởng hay? Hãy kiểm tra xem tính năng đó đã có trong [Roadmap](FEATURES.md) chưa. Nếu chưa, hãy tạo Issue mới với nhãn `enhancement` và mô tả:

- Tính năng này giải quyết vấn đề gì?
- Nó hoạt động như thế nào?
- Tại sao nó hữu ích cho người dùng cá nhân?

---

## Liên hệ

Nếu cần hỗ trợ thêm, bạn có thể liên hệ với maintainer:

- **Maintainer**: AnhGam
- **GitHub**: [@AnhGam](https://github.com/AnhGam)

Cảm ơn bạn đã đóng góp cho cộng đồng!
