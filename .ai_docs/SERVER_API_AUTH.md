# PHẦN 5: SERVER API - AUTHENTICATION & SECURITY

**Mục đích:** Thiết lập hệ thống bảo mật API đạt chuẩn Production, chống lại các cuộc tấn công phổ biến.

## 1. Password Security
- Cấm lưu Plaintext. Sử dụng thư viện `BCrypt.Net-Next`.
- Cấu hình Work Factor (chi phí băm) tối thiểu là 10 (đảm bảo cân bằng giữa bảo mật và CPU).

## 2. JWT Configuration (Best Practices)
- Cấu hình JWT Bearer trong `Program.cs`.
- **Strict Validation:** Phải validate `Issuer`, `Audience`, `Lifetime`, và `IssuerSigningKey`.
- **Token Expiration:** JWT Access Token chỉ nên có tuổi thọ ngắn (15-30 phút).
- **(Nâng cao) Refresh Token:** Thiết kế thêm cơ chế Refresh Token (lưu trong DB kèm Expiry Date) để cấp lại Access Token mà không bắt user đăng nhập lại liên tục.

## 3. Global Exception Handling (Bắt lỗi toàn cục)
- Không bao giờ để rò rỉ StackTrace ra ngoài API trả cho Client.
- Viết một `GlobalExceptionMiddleware` hoặc dùng `IExceptionHandler` (.NET 8).
- Catch tất cả exception, ghi log chi tiết (vào Console/Serilog) và trả về cho Client một JSON chuẩn theo định dạng `ProblemDetails` (RFC 7807) kèm HTTP Status Code 500.

## 4. Rate Limiting & CORS
- **Rate Limiting:** Áp dụng `Microsoft.AspNetCore.RateLimiting` cho endpoint Login/Register (VD: tối đa 5 request / 1 phút / 1 IP) để chống Brute-force.
- **CORS:** Cấu hình chính sách CORS nghiêm ngặt, chỉ cho phép các Origins (hoặc WinForms headers) hợp lệ gọi vào. Mặc định cấm `*`.
