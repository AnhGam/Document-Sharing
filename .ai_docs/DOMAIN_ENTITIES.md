# PHẦN 1: DOMAIN ENTITIES (LÕI DỮ LIỆU) - BEST PRACTICES

**Mục đích:** Định nghĩa các đối tượng dữ liệu cốt lõi theo hướng **Domain-Driven Design (DDD)**. 

## 1. Nguyên tắc Thiết kế
- **Rich Domain Model (Mô hình phong phú):** Entity không chỉ là các "túi chứa dữ liệu" (Anemic Domain Model) với toàn `public { get; set; }`. Thay vào đó, Setter của các property quan trọng nên là `private` hoặc `protected`. Trạng thái của Entity phải được thay đổi thông qua các phương thức nghiệp vụ (Ví dụ: `MarkAsDeleted()`, `UpdateVersion()`).
- **Encapsulation:** Sử dụng Constructor để bắt buộc truyền vào các dữ liệu tối thiểu khi khởi tạo Entity (không cho phép tạo Entity lỗi/thiếu dữ liệu).

## 2. Base Entity (Bắt buộc)
Tạo class trừu tượng `BaseEntity` để tái sử dụng:
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
}
```

## 3. Các Entity Cốt lõi

### User
Kế thừa `BaseEntity`.
- `public string Username { get; private set; }`
- `public string PasswordHash { get; private set; }`
- Method: `UpdatePassword(string newHash)`

### Document
Kế thừa `BaseEntity`.
- `public string FileName { get; private set; }`
- `public string FilePath { get; private set; }`
- `public long FileSize { get; private set; }`
- `public Guid OwnerId { get; private set; }`
- `public int Version { get; private set; } = 1;`
- `public bool IsDeleted { get; private set; } = false;`
- Method: `IncrementVersion()`
- Method: `SoftDelete()`

### SharedLink
Kế thừa `BaseEntity`.
- `public Guid DocumentId { get; private set; }`
- `public Guid SharedWithUserId { get; private set; }`
- `public SharePermission Permission { get; private set; }` (Enum: Read=1, Write=2)

## 4. VirtualDriveItem (Ghost Concept)
- Cần định nghĩa một Interface hoặc Abstract Class `IVirtualDriveItem` để sau này tích hợp Dokan.Net mà không làm bẩn Domain.
