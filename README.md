# Document Sharing Manager

Đây là ứng dụng Document Sharing Manager (Chia sẻ tài liệu mạng ngang hàng / tự Host).
Phần mềm bao gồm 2 thành phần:
1. **Client (UI)**: Chạy trên WinForms (Thư mục `document-sharing-manager`).
2. **Server (API)**: Chạy trên ASP.NET Core (Thư mục `document-sharing-manager-api`).

Khi bạn mở UI, nó sẽ tự động chạy Server ngầm bên trong.

## Yêu cầu hệ thống bắt buộc (Prerequisites)

Do phần mềm dùng kiến trúc tự Host Server ngay trên máy tính của bạn, máy bạn **BẮT BUỘC PHẢI CÀI ĐẶT CƠ SỞ DỮ LIỆU POSTGRESQL** để API có thể lưu trữ dữ liệu (User, Link chia sẻ, File,...).

Nếu bạn không cài PostgreSQL, API sẽ bị Crash ngay lập tức khi mở lên, dẫn đến nút "Tạo link" báo lỗi.

### Hướng dẫn cài đặt PostgreSQL trên Windows:
1. Tải **PostgreSQL 15** tại đây: [Download PostgreSQL for Windows](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads)
2. Chạy file cài đặt.
3. **CẤU HÌNH BẢO MẬT:** Trong quá trình cài đặt, trình cài đặt sẽ hỏi bạn nhập Password cho tài khoản siêu quản trị `postgres`. Hãy nhập mật khẩu của bạn (ví dụ: một mật khẩu mạnh bất kỳ).
4. **Cấu hình file môi trường (.env):** Hãy tạo file `.env` ở thư mục gốc (bằng cách copy và đổi tên file `.env.example`) và cập nhật dòng `POSTGRES_PASSWORD=mật_khẩu_của_bạn` trùng khớp với mật khẩu PostgreSQL bạn vừa cài đặt.
5. **VÔ CÙNG QUAN TRỌNG:** Ở bước cấu hình Port (cổng kết nối), bạn **BẮT BUỘC phải giữ nguyên số `5432`** (đây là port mặc định). Nếu bạn đổi số này, Code sẽ không tìm thấy Database và báo lỗi. Cứ nhấn Next đến khi hoàn tất cài đặt.
6. Mở phần mềm Document Sharing Manager của bạn lên và Tận hưởng!

## Về Cloudflare Tunnel

Phần mềm dùng Cloudflare Tunnel để đưa server nội bộ của bạn ra Internet (cho phép người khác truy cập qua tên miền do Cloudflare cấp phát ngẫu nhiên, ví dụ như `xyz.trycloudflare.com`).
- File `cloudflared.exe` đã được tải sẵn và nhúng vào thư mục `document-sharing-manager/Resources/`.
- Khi bạn Build dự án, file sẽ tự động đi kèm phần mềm.
- Bạn không cần phải cài đặt thêm Cloudflare Tunnel bằng tay nữa!
