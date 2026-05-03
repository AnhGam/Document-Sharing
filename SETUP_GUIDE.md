# HƯỚNG DẪN CẤU HÌNH HỆ THỐNG (DEPLOYMENT GUIDE)

Tài liệu này hướng dẫn các bước cần thiết để đưa hệ thống Document Sharing vào hoạt động, bao gồm cấu hình Server và kết nối Client.

## 1. Yêu cầu hệ thống (Server Side)
Để chạy bản **Server Only** hoặc **Full Bundle (Host Mode)**, bạn cần chuẩn bị:
- **Runtime**: .NET 8.0 SDK/Runtime.
- **Database**: PostgreSQL (Khuyến nghị dùng Docker để nhanh nhất).
- **Storage**: Mặc định hệ thống dùng Local Storage (lưu file vào folder local trên server).

## 2. Các biến môi trường (Environment Variables)
Server cần các biến môi trường sau để khởi chạy an toàn. Bạn có thể thiết lập trong hệ điều hành hoặc qua file `.env` (nếu dùng Docker).

| Biến môi trường | Ý nghĩa | Ví dụ |
| :--- | :--- | :--- |
| `POSTGRES_PASSWORD` | Mật khẩu cho Database PostgreSQL | `123456` |
| `JWT_SECRET_KEY` | Mã bí mật để tạo Token bảo mật (Tối thiểu 32 ký tự) | `secret_test_key_12345678901234567890` |
| `ASPNETCORE_URLS` | Cấu hình IP và Port mà Server sẽ lắng nghe | `http://0.0.0.0:5000` |

### Cách kết nối Database (Connection String):
Trong file `appsettings.json` của API, chuỗi kết nối mặc định là:
`Host=localhost;Database=docshare;Username=postgres;Password=${POSTGRES_PASSWORD}`

## 3. Cấu hình dịch vụ lưu trữ (Storage)
Hiện tại hệ thống đang dùng `LocalFileStorageService`.
- **Thư mục lưu trữ**: File sẽ được lưu tại thư mục `uploads` nằm cùng cấp với file thực thi của API.
- **Lưu ý**: Nếu bạn deploy lên Cloud (như Azure/AWS), trong tương lai chúng ta có thể cấu hình thêm `S3StorageService` mà không cần sửa mã nguồn (chỉ cần đổi Dependency Injection).

## 4. Hướng dẫn cho người dùng Client
Người dùng chỉ cần cài bản **Client Only**, sau đó thực hiện:
1. Mở ứng dụng.
2. Click vào icon **Cài đặt (Bánh răng)** trên thanh công cụ.
3. Nhập URL của Server (Ví dụ: `http://103.x.x.x:5000/api/documents`).
4. Khởi động lại App và bắt đầu sử dụng.

## 5. Danh sách các tài liệu hướng dẫn khác
- [Hướng dẫn Test với Docker (Giả lập mạng ngoài)](DOCKER_TEST_GUIDE.md)
- [Quy trình CI/CD tự động](.ai_docs/CI_CD_PIPELINE.md)
