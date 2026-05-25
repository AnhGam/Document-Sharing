# Hướng dẫn Cài đặt (Setup Guide)

Dự án Document Sharing Manager hiện tại đã loại bỏ "Server-Only mode" qua dòng lệnh (CLI). Tất cả các chức năng khởi tạo máy chủ, quản lý phê duyệt tham gia, tạo link mời đều được tích hợp thẳng vào giao diện Desktop Client.

## Yêu cầu hệ thống
- Hệ điều hành: Windows 10/11.
- .NET SDK 8.0 (cho API Server).
- .NET Framework 4.8 Developer Pack (cho WinForms Client).
- PostgreSQL Server (để lưu trữ dữ liệu).

## Cài đặt Database
1. Cài đặt và khởi chạy PostgreSQL.
2. Tạo database: `document_sharing_db`.
3. Đặt biến môi trường `POSTGRES_PASSWORD` thành mật khẩu PostgreSQL của bạn.
4. (Tùy chọn) Cài đặt chuỗi kết nối trong `appsettings.json` của thư mục `document-sharing-manager-api`.

## Build và Chạy ứng dụng

Bạn không cần phải khởi chạy Server và Client riêng biệt qua dòng lệnh nữa. Bạn chỉ cần Build toàn bộ Solution.

1. Khôi phục các gói (Restore packages):
   ```powershell
   dotnet restore document-sharing-manager.sln
   ```

2. Build ứng dụng:
   ```powershell
   dotnet build document-sharing-manager.sln
   ```

3. Chạy ứng dụng Client:
   ```powershell
   # Trong thư mục dự án WinForms
   cd document-sharing-manager
   dotnet run
   ```

Khi ứng dụng chạy lên, nó sẽ tự động chạy Server nền ở bên trong.

## Custom URI Scheme (docshare://)
Để ứng dụng có thể bắt được các link lời mời `docshare://join/{code}` từ trình duyệt, chương trình sẽ tự động đăng ký giao thức `docshare` vào Windows Registry trong lần chạy đầu tiên (Yêu cầu quyền Administrator nếu chưa được cấp).

## Giới hạn tải lên
Ứng dụng đã được nâng cấu hình giới hạn tải tài liệu lên **10GB**, đảm bảo chia sẻ dữ liệu lớn dễ dàng qua LAN và Internet (nếu đã cấu hình Port Forwarding/Ngrok).
