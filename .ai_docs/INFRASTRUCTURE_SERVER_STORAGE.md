# PHẦN 4: INFRASTRUCTURE - SERVER FILE STORAGE

**Mục đích:** Xử lý luồng dữ liệu nhị phân (Binary Streams) an toàn và tối ưu cho Server.

## 1. Nguyên tắc Xử lý Stream (Best Practices)
- **Không bao giờ đọc toàn bộ file vào RAM:** Tuyệt đối không dùng `IFormFile.ReadAllBytes()` hoặc `MemoryStream` cho các file dung lượng lớn. Bắt buộc dùng `FileStream` và copy stream qua từng chunk nhỏ (VD: 8192 bytes buffer).
- **Dispose Resources:** Sử dụng `using` hoặc `await using` cho mọi đối tượng Stream để đảm bảo không bị memory leak (rò rỉ bộ nhớ) và giải phóng File handle của OS.

## 2. Bảo mật Hệ thống File (Security)
- **Ngăn chặn Path Traversal:**
  - Tên file do Client gửi lên (VD: `../../etc/passwd`) KHÔNG ĐƯỢC dùng trực tiếp để lưu.
  - Bắt buộc phải hash tên file (ví dụ lưu trên đĩa bằng `Guid.NewGuid().ToString()`) hoặc dùng `Path.GetFileName()` và whitelist ký tự an toàn. Tên file gốc chỉ lưu ở metadata trong Database.
- **Giới hạn dung lượng:** Check file size trước khi copy stream (VD: không cho phép file quá 500MB qua cấu hình appsettings).

## 3. LocalFileStorageService Implementation
- Khởi tạo service nhận đường dẫn `BaseDirectory` từ cấu hình (IConfiguration).
- Nếu thư mục chưa tồn tại, tự động `Directory.CreateDirectory()`.
- Method `UploadFileAsync`: Trả về đường dẫn tương đối (Relative path) để lưu vào DB.

## 4. Mở rộng (S3/MinIO Ghost Code)
- Để dự án có thể scale lên Cloud thực sự, chuẩn bị sẵn class `MinIOStorageService : IStorageService`.
- Throw `NotImplementedException`, nhưng khai báo đầy đủ các package cần thiết trong comment (VD: `AWSSDK.S3` hoặc `Minio`). Việc chia Interface này giúp dễ dàng chuyển đổi hệ thống lưu trữ chỉ với 1 dòng đổi DI trong `Program.cs`.
