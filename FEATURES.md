# Danh sách tính năng đề xuất

> Tài liệu định hướng các tính năng thực sự hữu ích cho Study Document Manager.
> Mục tiêu: tập trung vào trải nghiệm người dùng, hiệu quả học tập và khả năng bảo trì.

---

## Phase 1 – Nâng trải nghiệm sử dụng hằng ngày (1–2 tuần) (ĐÃ ÁP DỤNG)

### 1. Context Menu trên danh sách tài liệu
**Mục tiêu:** Giảm số lần click, thao tác nhanh trên DataGridView.

**Tính năng:**
- Chuột phải trên dòng tài liệu để hiện menu:
  - Mở file
  - Sửa
  - Xóa
  - Copy đường dẫn file
  - Mở thư mục chứa file
  - Đánh dấu/Bỏ đánh dấu quan trọng
  - Xem thông tin chi tiết

**Tác động:**
- Tăng tốc thao tác quản lý tài liệu, đặc biệt cho giáo viên và admin.

---

### 2. Phím tắt (Keyboard Shortcuts) cho Form chính
**Mục tiêu:** Cho phép power-user thao tác không cần dùng chuột nhiều.

**Gợi ý phím tắt:**
- F2: Sửa tài liệu đang chọn
- Delete: Xóa tài liệu đang chọn
- Enter: Mở tài liệu đang chọn
- Ctrl + F: Focus ô tìm kiếm
- Ctrl + R: Refresh danh sách
- Ctrl + N: Thêm tài liệu mới
- Ctrl + E: Export dữ liệu
- Esc: Xóa từ khóa tìm kiếm hiện tại

**Tác động:**
- Phù hợp môi trường phòng máy, giáo viên dùng nhiều, thao tác nhanh hơn rõ rệt.

---

### 3. Kiểm tra và xử lý file bị thiếu (ĐÃ ÁP DỤNG)
**Mục tiêu:** Tránh tình trạng click vào tài liệu nhưng file thật đã bị xóa hoặc di chuyển.

**Tính năng:**
- Menu: Cong cu > Kiem tra file bi thieu
- Quét toàn bộ cột `duong_dan` trong bảng `tai_lieu`.
- Liệt kê các file không còn tồn tại trên ổ đĩa.
- Cho phép từng dòng:
  - Chọn file mới để cập nhật đường dẫn
  - Xóa bản ghi tài liệu
  - Đặt đường dẫn rỗng nhưng vẫn giữ metadata
- Có progress bar khi quét nhiều bản ghi.

**Tác động:**
- Làm sạch dữ liệu, tránh lỗi khi mở file, đặc biệt hữu ích khi copy/move thư mục tài liệu.

---

### 4. Gần đây và Yêu thích (Recent & Favorites)
**Mục tiêu:** Giúp truy cập nhanh các tài liệu hay dùng nhất.

**Tính năng:**
- Tự động ghi nhận danh sách khoảng 20 tài liệu mở gần đây.
- Panel nhỏ hoặc menu Recent Files để mở lại nhanh.
- Cho phép đánh dấu một số tài liệu là Yêu thích và ghim lên đầu.

**Tác động:**
- Sinh viên dễ quay lại các bộ đề/slide đang học.
- Giáo viên dễ mở lại giáo án, đề thi đang sử dụng.

---

### 5. Lưu cấu hình người dùng (User Preferences cơ bản)
**Mục tiêu:** Form mở lại vẫn giữ đúng trạng thái quen thuộc của người dùng.

**Tính năng:**
- Lưu vị trí và kích thước cửa sổ Form1.
- Lưu trạng thái cột trong DataGridView (thứ tự, độ rộng).
- Lưu bộ lọc đơn giản đã chọn lần gần nhất (môn học, loại, filter nâng cao).

**Tác động:**
- Trải nghiệm liền mạch, mở app lên là dùng được ngay với bối cảnh quen thuộc.

---

## Phase 2 – Hỗ trợ học tập và tổ chức tài liệu (2–3 tuần) (ĐÃ THỰC HIỆN)

### 6. Tags (nhãn) cho tài liệu
**Mục tiêu:** Phân loại linh hoạt hơn so với chỉ dùng môn học và loại tài liệu.

**Tính năng:**
- Thêm trường `tags` cho mỗi tài liệu, nhập dạng chuỗi phân cách bằng dấu chấm phẩy.
  - Ví dụ: `quan trong; thi cuoi ky; on tap`.
- Ô nhập/gợi ý tags trong màn hình thêm/sửa tài liệu.
- Filter theo một hoặc nhiều tag.
- Gợi ý tags hay dùng dựa trên dữ liệu hiện có.

**Tác động:**
- Rất phù hợp cho sinh viên ôn thi theo chủ đề, giáo viên phân nhóm tài liệu theo mục đích.

---

### 7. Ghi chú cá nhân cho tài liệu
**Mục tiêu:** Cho phép người dùng ghi lại ý chính, mẹo, hoặc trạng thái học của từng tài liệu.

**Tính năng:**
- Thêm vùng ghi chú riêng cho từng người dùng trên mỗi tài liệu (không dùng chung giữa các user).
- Hiển thị nhanh ghi chú trong form chi tiết hoặc panel bên cạnh.
- Có thể đánh dấu trạng thái như: Chua doc, Dang hoc, Da on xong.

**Tác động:**
- Biến ứng dụng từ chỗ chỉ quản lý file thành công cụ hỗ trợ học tập cá nhân.

---

### 8. Bộ sưu tập tài liệu (Collections)
**Mục tiêu:** Gom nhóm nhiều tài liệu thành một bộ cho từng kỳ thi hoặc môn học.

**Tính năng:**
- Tạo collection: ví dụ "Thi giữa kỳ Toán 1", "Luyện đề IELTS".
- Thêm nhiều tài liệu vào một collection.
- Mở nhanh toàn bộ tài liệu trong collection hoặc duyệt theo collection.

**Tác động:**
- Giáo viên tạo bộ tài liệu cho từng lớp.
- Sinh viên tạo bộ đề/slide cho từng kỳ thi.

---

### 9. Deadline và nhắc việc đơn giản
**Mục tiêu:** Hỗ trợ quản lý thời hạn ôn tập, nộp bài, kiểm tra.

**Tính năng:**
- Thêm trường deadline tùy chọn cho tài liệu (ngày giờ).
- Danh sách "Sắp đến hạn" trong vài ngày tới.
- Highlight tài liệu gần deadline trong danh sách.

**Tác động:**
- Giúp sinh viên và giáo viên không bỏ lỡ các mốc quan trọng.

---

## Phase 3 – Quản trị và độ tin cậy hệ thống (2–3 tuần)

### 10. Dashboard thống kê nâng cao
**Mục tiêu:** Cung cấp cái nhìn nhanh cho giáo viên/admin về việc sử dụng tài liệu.

**Tính năng:**
- Thống kê số lượng tài liệu theo môn học, loại, người tạo.
- Biểu đồ tài liệu tạo mới theo thời gian (7 ngày, 30 ngày, theo tháng).
- Thống kê tài liệu quan trọng, tài liệu chưa có file, tài liệu gần deadline.

**Tác động:**
- Hỗ trợ giáo viên/admin tối ưu cách tổ chức tài liệu và phát hiện bất thường.

---

### 11. Activity log thân thiện hơn cho Admin
**Mục tiêu:** Dễ theo dõi hành động quan trọng (xóa nhiều file, đổi quyền...).

**Tính năng:**
- Màn hình xem log đơn giản: ai làm gì, với tài liệu nào, lúc nào.
- Filter theo user, loại hành động, khoảng thời gian.
- Export log ra CSV khi cần báo cáo.

**Tác động:**
- Tăng độ tin cậy và khả năng audit, đặc biệt trong môi trường nhiều giáo viên dùng chung.

---

### 12. Backup/Restore một chạm
**Mục tiêu:** Đảm bảo an toàn dữ liệu, dễ backup trước khi nâng cấp hoặc cài lại máy.

**Tính năng:**
- Nút "Backup database" và "Restore database" ngay trong menu.
- Cho phép chọn đường dẫn file backup.
- Hiển thị trạng thái, thời gian ước tính, và thông báo kết quả.

**Tác động:**
- Giảm rủi ro mất dữ liệu, dễ hướng dẫn cho người dùng không rành SQL Server.

---

## Phase 4 – Ý tưởng dài hạn (ưu tiên sau)

Các ý tưởng dưới đây mang tính định hướng, chỉ triển khai khi các phần trên đã ổn định.

### 13. Đồng bộ đám mây đơn giản
- Đồng bộ thư mục tài liệu với một thư mục cloud (OneDrive/Google Drive đã cài sẵn trên máy).
- Không cần tích hợp API phức tạp, chỉ quản lý đường dẫn và trạng thái.

### 14. Xuất danh sách tài liệu sang Markdown/CSV cho Notion/Obsidian
- Export danh sách tài liệu đã lọc sang file Markdown hoặc CSV.
- Phù hợp với người dùng quen dùng Notion/Obsidian để ghi chú.

### 15. Hỗ trợ đa ngôn ngữ cơ bản
- Cấu trúc lại text để lấy từ file resources.
- Hỗ trợ tối thiểu tiếng Việt và tiếng Anh.

---

## Gợi ý lộ trình thực hiện ngắn hạn

- Sprint 1 (1 tuần): Context Menu, Phím tắt, Fix file thiếu.
- Sprint 2 (1 tuần): Recent/Favorites, Lưu cấu hình người dùng.
- Sprint 3 (2 tuần): Tags, Ghi chú cá nhân, Collections.
- Sprint 4 (2 tuần): Deadline, Dashboard đơn giản, Activity log cơ bản.

Tùy thời gian thực tế, có thể chọn 4–6 tính năng ưu tiên nhất trong Phase 1–2 để làm thành một bản demo mạnh cho đồ án.

