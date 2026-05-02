# PHẦN 3: INFRASTRUCTURE - SERVER DATABASE (EF CORE)

**Mục đích:** Cấu hình EF Core một cách chuyên nghiệp, tối ưu hiệu suất và bảo mật dữ liệu.

## 1. EF Core Best Practices
- **Fluent API thay vì Data Annotations:** Không làm "bẩn" các class ở Domain Entity bằng `[Table]`, `[Column]`, `[Required]`. Bắt buộc dùng `IEntityTypeConfiguration<T>` để cấu hình ánh xạ DB (Fluent API) trong thư mục `Configurations`.
- **Hiệu suất:**
  - Với các truy vấn chỉ đọc (GET api/documents), bắt buộc gọi `.AsNoTracking()` để giảm tải bộ nhớ và tăng tốc.
  - Sử dụng `.AsSplitQuery()` nếu có Include các bảng con phức tạp.

## 2. AppDbContext
- Khai báo các `DbSet`.
- Override `OnModelCreating` và gọi `modelBuilder.ApplyConfigurationsFromAssembly(...)`.
- Tích hợp logic tự động cập nhật `UpdatedAt`: Override `SaveChangesAsync` để tìm các Entity bị thay đổi (State = Modified) và tự set `UpdatedAt = DateTime.UtcNow`.

## 3. Triển khai Repository
- Implement `IDocumentRepository`.
- Tiêm `AppDbContext` qua Constructor.
- **Security Check:** Khi lấy danh sách Document, bắt buộc phải filter theo `OwnerId` hoặc `IsDeleted == false` ngay từ câu query (WHERE clause), không kéo toàn bộ table về server rồi mới filter.
- **Nullability:** Đảm bảo xử lý các giá trị Nullable trong các query bằng cách sử dụng toán tử `??` hoặc `HasValue` để tránh NullReferenceException trong thời gian chạy.

## 4. Database Migrations & Seeding
- Không dùng `db.Database.EnsureCreated()`. Bắt buộc dùng EF Core Migrations (`Add-Migration`, `Update-Database`).
- Trong quá trình khởi động app (`Program.cs`), gọi `context.Database.MigrateAsync()` để tự động apply các migration mới nhất một cách an toàn.
- Cấu hình Connection String từ `appsettings.json`, tuyệt đối không hardcode trong `AppDbContext`. Sử dụng `Npgsql.EntityFrameworkCore.PostgreSQL`.
- **Capacity & FinOps Governance:** 
    - Sử dụng `capacity-governance.ps1` để kiểm tra dung lượng Installer và Repository.
    - Tự động chặn build nếu vượt ngưỡng (50MB cho Installer).
    - **DORA Metrics:** Theo dõi Lead Time for Changes trực tiếp từ CI duration.
- **Hybrid Support:** Hỗ trợ song song SQLite cho các cá nhân tự host trên máy Windows (Local) và PostgreSQL cho các hệ thống máy chủ Online chuyên nghiệp.
