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

## 4. Managed Storage Implementation (Current Version)
Currently, the system implements a simplified version of the Virtual Drive concept via `StorageService.cs`:
- **Managed Folder:** A `storage/` directory is created in the app base path.
- **Relocation:** Files imported via `BatchImportForm` are copied into this folder.
- **Path Resolution:** The database stores relative paths (e.g., `storage/file_timestamp.pdf`), which are resolved at runtime using `StorageService.ResolvePath()`. This ensures that even if source files are deleted or the app is moved, links remain intact as long as the `storage/` folder is preserved.
- **Legacy Fallback:** If a stored path is absolute and exists, the system still supports direct opening.
