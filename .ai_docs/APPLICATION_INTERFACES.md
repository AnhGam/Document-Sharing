# PHẦN 2: APPLICATION INTERFACES & DTOs - BEST PRACTICES

**Mục đích:** Định nghĩa các Hợp đồng (Contracts) giao tiếp giữa các Layer. Đây là tầng quan trọng để tách biệt UI/API với Infrastructure.

## 1. Nguyên tắc Giao tiếp
- **Sử dụng CancellationToken:** Mọi phương thức `Task` đều phải có tham số `CancellationToken cancellationToken = default`.
- **Result Pattern:** Tránh throw Exception. Hãy tạo một wrapper class `Result<T>`:
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string ErrorMessage { get; }
    // Implement Success() and Failure() factories
}
```

## 2. Repository Interfaces (Generic + Specific)
- `IRepository<T> where T : BaseEntity`:
  - `Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);`
  - `Task AddAsync(T entity, CancellationToken ct = default);`
  - (Các thao tác Update/Delete nên quản lý qua UnitOfWork hoặc SaveChanges của EF Core).
- `IDocumentRepository`:
  - `Task<IEnumerable<Document>> GetFilesByOwnerAsync(Guid ownerId, CancellationToken ct = default);`
  - `Task<Document?> GetByVersionAsync(Guid docId, int version, CancellationToken ct = default);`

## 3. Service Interfaces (Nghiệp vụ)
- `IAuthService`:
  - `Task<Result<string>> LoginAsync(LoginDto request, CancellationToken ct = default);` (Trả về JWT)
  - `Task<Result<bool>> RegisterAsync(RegisterDto request, CancellationToken ct = default);`
- `IDocumentService`:
  - Xử lý logic chia sẻ, xóa mềm, kiểm tra phiên bản (Conflict detection).
- `ISyncService`:
  - Chuyên xử lý logic đồng bộ, trả về danh sách các file cần Pull/Push.

## 4. DTOs (Data Transfer Objects)
- Yêu cầu AI tự tạo thư mục `DTOs`.
- Thay vì dùng class thông thường, với .NET 8, ưu tiên dùng `record` cho DTOs vì tính bất biến (immutability).
- VD: `public record DocumentDto(Guid Id, string FileName, long FileSize, DateTime UpdatedAt);`
