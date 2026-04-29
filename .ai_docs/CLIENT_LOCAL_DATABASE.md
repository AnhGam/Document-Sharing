# PHẦN 7: CLIENT - LOCAL DATABASE (SQLITE CACHE) - BEST PRACTICES

**Mục đích:** Tối ưu hóa SQLite để làm Local Cache cho WinForms, chịu tải tốt các tác vụ đọc/ghi nền mà không khóa luồng UI.

## 1. Lựa chọn Công nghệ
- Sử dụng thư viện **System.Data.SQLite** (phù hợp cho .NET 4.8). 
- Hiện tại dự án đang sử dụng ADO.NET truyền thống (`SQLiteCommand`, `SQLiteDataAdapter`) thông qua class `DatabaseHelper`. Tuy nhiên, có thể cân nhắc tích hợp **Dapper** (Micro ORM) để tối ưu hóa việc map dữ liệu và giảm code boilerplate trong tương lai.

## 2. Tối ưu hóa SQLite
- **WAL Mode (Write-Ahead Logging):** Bắt buộc bật `PRAGMA journal_mode=WAL;` khi khởi tạo DB. Điều này cho phép nhiều tác vụ đọc và một tác vụ ghi diễn ra ĐỒNG THỜI mà không bị "Database is locked" (Rất hay gặp nếu UI đang đọc danh sách mà Sync Engine đang ghi dữ liệu mới).
- **Pooling:** Sử dụng Connection String có `Pooling=True` để tái sử dụng kết nối.

## 3. Mô hình Dữ liệu Local
Bảng `LocalDocuments`:
- `Id` (GUID)
- `FileName`
- `ServerVersion` (int) - Lưu version cuối cùng lấy từ server.
- `LocalVersion` (int) - Tự tăng mỗi khi file bị sửa ở máy local.
- `SyncStatus` (Enum/Int): 
  - `0 = Synced` (Đồng bộ hoàn toàn)
  - `1 = PendingUpload` (Đã sửa local, chờ đẩy lên)
  - `2 = PendingDownload` (Server báo có bản mới, chờ tải về)
  - `3 = Conflict` (Cần user giải quyết)
- `LocalFilePath` (string) - Đường dẫn thực tế nằm trong ổ cứng ẩn.

## 4. Thread-Safety
- Đảm bảo các hàm truy vấn Dapper được đặt trong khối `using (var connection = new SqliteConnection(...))` để tự động đóng kết nối ngay sau khi dùng xong, tránh memory leak phía Client.
