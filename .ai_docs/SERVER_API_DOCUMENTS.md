# PHẦN 6: SERVER API - DOCUMENT MANAGEMENT & SYNC LOGIC

**Mục đích:** API cho Client tương tác với tài liệu, đảm bảo RESTful và xử lý chuẩn các tình huống xung đột đồng bộ.

## 1. RESTful API Best Practices
- Tuân thủ định dạng Route: `/api/documents` (số nhiều).
- Sử dụng đúng HTTP Verbs: `GET` (đọc), `POST` (tạo mới/upload), `PUT` (cập nhật toàn bộ), `DELETE` (xóa).
- Trả về đúng HTTP Status Codes:
  - `200 OK`: Truy xuất/Cập nhật thành công.
  - `201 Created`: Upload file mới thành công (Kèm Header Location).
  - `400 Bad Request`: Validation lỗi.
  - `401 Unauthorized`: Lỗi Token.
  - `403 Forbidden`: Truy cập file của người khác (Dù có Token hợp lệ).
  - `404 Not Found`: File không tồn tại.
  - `409 Conflict`: Xung đột phiên bản đồng bộ.

## 2. Xử lý Logic Đồng Bộ & Xung Đột (Conflict Resolution)
Đây là cốt lõi của hệ thống Cloud:
- Client (WinForms) gọi `POST /api/documents/sync` kèm tham số `LocalVersion`.
- Server query Database lấy `CurrentVersion`.
- Nếu `LocalVersion < CurrentVersion`: Báo cho Client biết file trên Server đã bị thay đổi bởi thiết bị khác -> Trả về `409 Conflict`.
- Nếu `LocalVersion == CurrentVersion`: Upload an toàn, Server cập nhật `Version = Version + 1`.

## 3. DTO Validation
- Validate đầu vào bằng Data Annotations (VD: `[Required]`, `[MaxLength(255)]`) hoặc thư viện `FluentValidation`.
- File Upload dùng interface `IFormFile` (chú ý limit dung lượng tối đa qua cấu hình `[RequestSizeLimit]`).

## 4. Tách bạch Logic
- Controller chỉ thực hiện: Get Token Claims (UserId), Bind Parameters, gọi DocumentService, trả về `IActionResult`.
- Không viết logic kiểm tra DB hay Hash file trực tiếp trong Controller.
