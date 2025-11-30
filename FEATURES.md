# Danh sách tính năng đề xuất

> Tài liệu định hướng các tính năng thực sự hữu ích cho Study Document Manager.
> Mục tiêu: tập trung vào trải nghiệm người dùng, hiệu quả học tập và khả năng bảo trì.
> 
> **Lưu ý:** Ứng dụng sử dụng mô hình **cá nhân hóa** (Personal Mode) với 2 cấp quyền:
> - **User**: Quản lý hoàn toàn tài liệu, danh mục của riêng mình
> - **Admin**: Có quyền User + quản lý người dùng

---

## Phase 1 – Nâng trải nghiệm sử dụng hằng ngày (1–2 tuần) 

### 1. Context Menu trên danh sách tài liệu (ĐÃ ÁP DỤNG)
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
- Tăng tốc thao tác quản lý tài liệu cho mọi người dùng.

---

### 2. Phím tắt (Keyboard Shortcuts) cho Form chính (ĐÃ ÁP DỤNG MỘT PHẦN)
**Mục tiêu:** Cho phép power-user thao tác không cần dùng chuột nhiều.

**Đã triển khai:**
- Ctrl+N: Thêm tài liệu mới
- Ctrl+O: Mở file đã chọn
- Ctrl+U: Sửa tài liệu
- Delete: Xóa tài liệu
- Ctrl+E: Xuất dữ liệu
- Ctrl+S: Thống kê
- Ctrl+M: Quản lý danh mục
- Ctrl+L: Đăng xuất
- F5: Làm mới danh sách

**Chưa triển khai:**
- F2: Sửa tài liệu đang chọn
- Enter: Mở tài liệu đang chọn (khi focus DataGridView)
- Ctrl+F: Focus ô tìm kiếm
- Esc: Xóa từ khóa tìm kiếm

**Tác động:**
- Thao tác nhanh hơn rõ rệt cho người dùng thường xuyên.

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
- Dễ quay lại các tài liệu đang học hoặc làm việc.
- Tiết kiệm thời gian tìm kiếm lại.

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

## Phase 2 – Hỗ trợ học tập và tổ chức tài liệu (2–3 tuần) (ĐÃ HOÀN THÀNH)

### 6. Tags (nhãn) cho tài liệu (ĐÃ ÁP DỤNG)
**Mục tiêu:** Phân loại linh hoạt hơn so với chỉ dùng danh mục và loại tài liệu.

**Tính năng:**
- Thêm trường `tags` cho mỗi tài liệu, nhập dạng chuỗi phân cách bằng dấu chấm phẩy.
  - Ví dụ: `quan trong; thi cuoi ky; on tap`.
- Ô nhập/gợi ý tags trong màn hình thêm/sửa tài liệu.
- Filter theo một hoặc nhiều tag.
- Gợi ý tags hay dùng dựa trên dữ liệu hiện có.

**Tác động:**
- Rất phù hợp cho việc ôn thi theo chủ đề, phân nhóm tài liệu theo mục đích.

---

### 7. Ghi chú cá nhân cho tài liệu (ĐÃ ÁP DỤNG)
**Mục tiêu:** Cho phép người dùng ghi lại ý chính, mẹo, hoặc trạng thái học của từng tài liệu.

**Tính năng:**
- Thêm vùng ghi chú riêng cho từng người dùng trên mỗi tài liệu (không dùng chung giữa các user).
- Hiển thị nhanh ghi chú trong form chi tiết hoặc panel bên cạnh.
- Có thể đánh dấu trạng thái như: Chua doc, Dang hoc, Da on xong.

**Tác động:**
- Biến ứng dụng từ chỗ chỉ quản lý file thành công cụ hỗ trợ học tập cá nhân.

---

### 8. Bộ sưu tập tài liệu (Collections) (ĐÃ ÁP DỤNG)
**Mục tiêu:** Gom nhóm nhiều tài liệu thành một bộ cho từng kỳ thi hoặc môn học.

**Tính năng:**
- Tạo collection: ví dụ "Thi giữa kỳ Toán 1", "Luyện đề IELTS".
- Thêm nhiều tài liệu vào một collection.
- Mở nhanh toàn bộ tài liệu trong collection hoặc duyệt theo collection.

**Tác động:**
- Tạo bộ tài liệu cho từng dự án hoặc kỳ học.
- Dễ dàng gom nhóm và mở nhanh nhiều file liên quan.

---

### 9. Deadline và nhắc việc đơn giản (ĐÃ ÁP DỤNG)
**Mục tiêu:** Hỗ trợ quản lý thời hạn ôn tập, nộp bài, kiểm tra.

**Tính năng:**
- Thêm trường deadline tùy chọn cho tài liệu (ngày giờ).
- Danh sách "Sắp đến hạn" trong vài ngày tới.
- Highlight tài liệu gần deadline trong danh sách.

**Tác động:**
- Giúp người dùng không bỏ lỡ các mốc quan trọng.

---

## Phase 3 – Quản trị và độ tin cậy hệ thống (2–3 tuần)

### 10. Dashboard thống kê nâng cao
**Mục tiêu:** Cung cấp cái nhìn tổng quan về tài liệu của người dùng.

**Tính năng:**
- Thống kê số lượng tài liệu theo danh mục, loại.
- Biểu đồ tài liệu tạo mới theo thời gian (7 ngày, 30 ngày, theo tháng).
- Thống kê tài liệu quan trọng, tài liệu chưa có file, tài liệu gần deadline.

**Tác động:**
- Hỗ trợ người dùng tối ưu cách tổ chức tài liệu.

---

### 11. Activity log thân thiện hơn cho Admin
**Mục tiêu:** Dễ theo dõi hành động quan trọng (xóa nhiều file, đổi quyền...).

**Tính năng:**
- Màn hình xem log đơn giản: ai làm gì, với tài liệu nào, lúc nào.
- Filter theo user, loại hành động, khoảng thời gian.
- Export log ra CSV khi cần báo cáo.

**Tác động:**
- Tăng độ tin cậy và khả năng audit cho Admin.

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

## Tổng kết tiến độ

### ĐÃ HOÀN THÀNH 
- Context Menu trên danh sách tài liệu
- Phím tắt cơ bản (Ctrl+N, Ctrl+O, Ctrl+U, Delete, Ctrl+E, Ctrl+S, Ctrl+M, Ctrl+L, F5)
- Kiểm tra và xử lý file bị thiếu
- Tags (nhãn) cho tài liệu
- Ghi chú cá nhân cho tài liệu
- Bộ sưu tập tài liệu (Collections)
- Deadline và xem tài liệu sắp đến hạn/quá hạn
- Cài đặt tài khoản (đổi thông tin, đổi mật khẩu)
- Tái cấu trúc menu (Công cụ, Theo dõi, Tài khoản, Quản lý)
- Toast Notification hiện đại (Success/Error/Warning/Info) với hiệu ứng fade
- IconHelper mở rộng với nhiều icon UI (Add, Edit, Delete, Settings, Chart...)
- Cá nhân hóa giao diện (đổi "Môn học" thành "Danh mục")

### CHƯA TRIỂN KHAI 
- Phím tắt nâng cao (F2, Enter mở file, Ctrl+F, Esc)
- Recent & Favorites
- Lưu cấu hình người dùng đầy đủ
- Dashboard thống kê nâng cao
- Activity log cho Admin
- Backup/Restore một chạm

---

Tùy thời gian thực tế, có thể chọn 4–6 tính năng ưu tiên nhất trong danh sách chưa triển khai để làm thành bản demo hoàn chỉnh.

