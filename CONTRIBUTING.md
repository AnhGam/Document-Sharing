# Hướng dẫn đóng góp (Contributing)

Cảm ơn bạn đã quan tâm đóng góp cho dự án Document Sharing Manager. Để đảm bảo tính ổn định và sự phát triển liên tục, dự án áp dụng hệ thống CI/CD rất nghiêm ngặt.

## Quy trình làm việc (Workflow)
1. Tách nhánh mới từ `main` (ví dụ: `feature/invite-link`, `bugfix/dashboard`).
2. Viết code, thực hiện thay đổi và luôn đảm bảo đã test cục bộ (Local Testing).
3. Đẩy nhánh (Push) lên GitHub và tạo Pull Request (PR) vào `main`.
4. Xem xét hệ thống CI/CD Dashboard phản hồi.

## Hệ thống CI/CD
Mọi thao tác `push` hoặc `pull_request` vào `main` đều sẽ tự động kích hoạt GitHub Actions. Đường ống CI/CD gồm nhiều giai đoạn:
1. **Restore & Build**: Biên dịch tất cả các projects (.NET Framework 4.8 và .NET 8.0).
2. **Unit Tests**: Chạy các bài test trong dự án `.Tests` bằng NUnit/xUnit. Nếu test thất bại, build sẽ chuyển sang trạng thái "Failure".
3. **Telemetry & Audit**: Thu thập dữ liệu build (thời gian chạy, trạng thái, kích thước file installer, v.v.).
4. **Publish Report**: Tổng hợp dữ liệu vào tệp `history.json` trên nhánh `logs` và triển khai tự động lên [GitHub Pages Dashboard].
5. **AI Diagnostic**: Nếu CI/CD thất bại, hệ thống giả lập AI Analysis (hoặc API thật) sẽ tự động phân tích commit lỗi và lưu kết quả lại. Bạn có thể lên trang Dashboard Web để xem gợi ý sửa lỗi trực quan.

> [!WARNING]
> Tuyệt đối không được can thiệp thủ công vào nhánh `logs` hoặc tệp `history.json`.

## Hướng dẫn Test cục bộ
Trước khi tạo PR, vui lòng chạy lệnh sau để kiểm tra lỗi:
```powershell
dotnet test document-sharing-manager.sln
```
Nếu có lỗi, hãy sửa nó trước khi push để tiết kiệm CI minutes.
