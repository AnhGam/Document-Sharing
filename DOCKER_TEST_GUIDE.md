# HƯỚNG DẪN TEST ĐỒNG BỘ ĐA MÁY VỚI DOCKER

Tài liệu này giúp bạn thực hiện bài test: **"Máy A upload, Máy B (sau khi xóa data) có thấy và tải lại được không?"**.

## 1. Khởi động Server (Docker)
1. Mở Terminal tại thư mục gốc của dự án.
2. Chạy lệnh:
   ```powershell
   docker-compose up -d
   ```
3. Đợi các container `docshare_db` và `docshare_api` sẵn sàng.

## 2. Lấy Token truy cập (Authentication)
Vì API có bảo mật JWT, bạn cần một Token để App có thể đồng bộ. Hãy thực hiện đăng ký và đăng nhập nhanh bằng 2 lệnh sau:

### Bước 2.1: Đăng ký tài khoản test
```powershell
curl -X POST http://localhost:5000/api/Auth/register `
     -H "Content-Type: application/json" `
     -d '{"username": "testuser", "password": "Password123!", "email": "test@example.com"}'
```

### Bước 2.2: Đăng nhập để lấy Token
```powershell
curl -X POST http://localhost:5000/api/Auth/login `
     -H "Content-Type: application/json" `
     -d '{"username": "testuser", "password": "Password123!"}'
```
**Kết quả**: Bạn sẽ nhận được một chuỗi `accessToken`. Hãy copy chuỗi này.

## 3. Cấu hình App (Máy A)
1. Chạy ứng dụng WinForms.
2. Click icon **Cài đặt (Bánh răng)**.
3. **URL**: Nhập `http://localhost:5000/api/documents` (Nếu test cùng máy) hoặc IP máy (`http://192.168.1.x:5000/api/documents`).
4. **Token**: Dán chuỗi `accessToken` vừa copy vào ô **JWT Access Token**.
5. Nhấn **Lưu cấu hình**.

## 4. Thực hiện bài Test Sync

### Giai đoạn 1: Upload (Máy A)
1. Trên giao diện chính, nhấn **Import** (Ctrl+N) và chọn vài file (ảnh, pdf, txt...).
2. Đợi vài giây để App tự động upload (Bạn có thể xem log Docker: `docker-compose logs -f api`).
3. Khi file hiện trạng thái "Synced" (hoặc không còn icon pending), nghĩa là đã lên Server.

### Giai đoạn 2: Giả lập Máy B (Xóa dữ liệu local)
1. Tắt ứng dụng WinForms.
2. Truy cập thư mục chạy app (thường là `document-sharing-manager/bin/Debug/`).
3. **Xóa sạch**:
   - File `document_sharing.db` (Xóa DB SQLite local).
   - Folder `documents/` (Xóa file local đã import).
4. Mở lại ứng dụng. **Lúc này App sẽ trắng trơn.**

### Giai đoạn 3: Kiểm chứng (Máy B tải lại)
1. Vào lại **Cài đặt**, nhập lại URL và Token (vì file config cũng bị ảnh hưởng hoặc bạn muốn chắc chắn).
2. Nhấn nút **Làm mới (F5)**.
3. **Kết quả**: 
   - App sẽ tự động gọi API lấy danh sách file bạn đã úp từ Máy A.
   - Các file sẽ xuất hiện trong danh sách.
   - App sẽ tự động tải (Download) file vật lý từ Server về folder `documents/` mới.
   - Bạn có thể nhấn đúp vào file để mở xem -> **Thành công!**

## 5. Các lệnh hỗ trợ
- **Xóa sạch toàn bộ Server để làm lại từ đầu**: `docker-compose down -v`
- **Xem log API**: `docker-compose logs -f api`
