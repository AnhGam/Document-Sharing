# PHẦN 8: CLIENT - SYNC ENGINE (BỘ NÃO ĐỒNG BỘ)

**Mục đích:** Xử lý sự kiện đồng bộ bất đồng bộ, bền bỉ (resilient), không làm đơ giao diện người dùng.

## 1. Resilience & Retries (Sự bền bỉ)
- Mạng có thể rớt bất cứ lúc nào. Bắt buộc sử dụng thư viện **Polly** để thực thi Retry Policy.
- Cấu hình **Exponential Backoff**: Nếu gọi API lỗi mạng, chờ 2s -> gọi lại, lỗi tiếp -> chờ 4s -> lỗi tiếp -> chờ 8s -> sau đó đưa vào hàng đợi chờ lần sync sau. Tuyệt đối không retry điên cuồng làm treo máy và spam Server.

## 2. Background Processing (Xử lý nền)
- Chạy Sync Engine trên một luồng hoàn toàn độc lập (VD: sử dụng `Task.Run` hoặc `BackgroundWorker`).
- Thay vì quét định kỳ mù quáng bằng Timer, hãy kết hợp `FileSystemWatcher` (Theo dõi thay đổi file thực tế) và cơ chế **Debounce/Throttle**. (Tức là nếu user gõ phím lưu Word liên tục 10 lần trong 5s, chỉ upload 1 lần sau khi họ ngừng gõ 3s).
- Sử dụng `Channel<T>` (System.Threading.Channels) để tạo một hàng đợi (Queue) xử lý các task Upload/Download theo thứ tự, tránh mở quá nhiều kết nối mạng cùng lúc (Concurrency Limit).

## 3. Conflict Resolution Workflow
Quy trình chặt chẽ:
1. Sync Engine cố gắng Push file (LocalVersion = 2, ServerVersion = 1).
2. Server trả về `409 Conflict` (Vì Server đã là Version 3).
3. Client bắt được mã 409:
   - Copy file local hiện tại -> Đổi tên `FileName_Conflict_ComputerName.ext`.
   - Update SQLite: Đánh dấu bản record cũ là `SyncStatus = PendingDownload`.
   - Tiến hành Pull file Version 3 từ Server đè vào file gốc.
   - Bắn Event/Delegate cập nhật UI WinForms hiện icon cảnh báo màu vàng cho file Conflict.
