# HƯỚNG DẪN TEST SERVER VỚI DOCKER

Tài liệu này giúp bạn chạy thử Server trên chính máy tính của mình nhưng vẫn đảm bảo tính chất giống như truy cập từ một mạng khác.

## 1. Chuẩn bị file docker-compose.yml
Tôi đã tạo sẵn file `docker-compose.yml` ở thư mục gốc. File này sẽ dựng lên:
- **db**: Database PostgreSQL.
- **api**: Web API của chúng ta (Server).

## 2. Cách khởi chạy
Mở Terminal tại thư mục gốc của dự án và chạy lệnh:
```bash
docker-compose up -d
```

## 3. Cách test "Giả lập mạng ngoài"
Để giả lập việc kết nối không phải qua `localhost` (như một máy khách thực thụ):

1. **Tìm IP máy tính của bạn**: 
   - Trên Windows: Chạy lệnh `ipconfig` trong CMD. Tìm dòng `IPv4 Address` (Ví dụ: `192.168.1.15`).
2. **Cấu hình Client**:
   - Mở ứng dụng WinForms của bạn.
   - Vào **Cài đặt**.
   - Nhập URL dùng IP vừa tìm được: `http://192.168.1.15:5000/api/documents`.
3. **Kiểm chứng**:
   - Mặc dù Server và Client đang chạy trên cùng một máy vật lý, nhưng khi bạn gọi qua IP `192.168.1.15`, gói tin sẽ đi qua Card mạng và Docker sẽ tiếp nhận nó như một kết nối từ thiết bị ngoại vi.

## 4. Các lệnh hữu ích
- **Xem log Server**: `docker-compose logs -f api`
- **Dừng hệ thống**: `docker-compose down`
- **Xóa sạch dữ liệu để làm mới**: `docker-compose down -v`

## 5. Lưu ý về File lưu trữ
Dữ liệu file bạn upload lên Docker sẽ được lưu vào Volume `api_uploads`. Nếu bạn muốn xem trực tiếp file trên Windows, hãy kiểm tra phần `volumes` trong file docker-compose.
