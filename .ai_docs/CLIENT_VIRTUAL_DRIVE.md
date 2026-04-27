# PHẦN 9: CLIENT - VIRTUAL DRIVE (Ổ ĐĨA ẢO) - ADVANCED

**Mục đích:** Tích hợp sâu vào Windows Explorer, tạo ra trải nghiệm "Files On-Demand" cao cấp. Đây là phần dễ crash Win nhất nên code cần cực kỳ cẩn thận.

## 1. Công nghệ & Rủi ro
- API được chọn: **Dokan.Net** (Dokan library). Nó tạo ra một ổ đĩa ảo hoạt động ở mức Kernel-mode.
- **Rủi ro:** Bất kỳ Unhandled Exception nào ném ra trong class implement `IDokanOperations` đều có thể dẫn đến màn hình xanh (BSOD) hoặc treo cứng Windows Explorer.
- **Best Practice:** Mọi method trong `VirtualDriveProvider` bắt buộc phải được bọc trong khối `try-catch(Exception)` khổng lồ và trả về các mã lỗi Win32 chuẩn (như `DokanResult.Error` hoặc `DokanResult.FileExists`), tuyệt đối không được để Exception lọt ra ngoài phạm vi hàm.

## 2. Logic "Files On-Demand" (Placeholder & Hydration)
- Khi user mở ổ ảo, hàm `FindFiles` được Windows gọi. Ta chỉ query SQLite lấy `FileName`, `FileSize` và trả về danh sách ảo. File thực tế KHÔNG tồn tại trên đĩa.
- Khi user click đúp vào file:
  1. Windows gọi hàm `CreateFile` (Mở file).
  2. Ứng dụng kiểm tra file cache ẩn ở `C:\AppData\Local\AppCache\...`.
  3. Nếu chưa có -> KHÓA THREAD (Block) hàm `CreateFile`, gọi Sync Engine ưu tiên tải file đó về bằng `HttpClient`.
  4. Tải xong -> Trỏ file handle của hệ thống vào file cache vừa tải. Hệ điều hành tiếp tục mở file bình thường.

## 3. Chỉ thị cho AI
- Bắt buộc phải viết code phân rã cẩn thận, tạo một lớp Interface `IVirtualFileSystem` riêng biệt để tách logic Dokan ra khỏi Logic Sync, giúp cho quá trình Mock và Unit Testing trở nên khả thi. Phải có Ghost Code để user chạy app mà không cần cài Dokan Driver vào máy nếu chưa test xong.
