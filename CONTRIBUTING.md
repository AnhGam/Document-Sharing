# Đóng góp cho Study Document Manager

Cảm ơn bạn đã quan tâm đến việc đóng góp cho dự án! Mọi đóng góp đều được chào đón và đánh giá cao.

## Mục lục

- [Quy tắc ứng xử](#quy-tắc-ứng-xử)
- [Cách đóng góp](#cách-đóng-góp)
- [Báo cáo lỗi](#báo-cáo-lỗi)
- [Đề xuất tính năng](#đề-xuất-tính-năng)
- [Pull Request](#pull-request)
- [Quy ước Code](#quy-ước-code)
- [Cấu trúc dự án](#cấu-trúc-dự-án)

---

## Quy tắc ứng xử

Dự án này tuân theo các nguyên tắc tôn trọng và chuyên nghiệp. Vui lòng:

- Tôn trọng mọi người tham gia
- Sử dụng ngôn ngữ lịch sự và chuyên nghiệp
- Chấp nhận phản hồi mang tính xây dựng
- Tập trung vào những gì tốt nhất cho cộng đồng

---

## Cách đóng góp

### 1. Fork repository

```bash
# Fork trên GitHub, sau đó clone về máy
git clone https://github.com/YOUR_USERNAME/study-document-manager.git
cd study-document-manager
```

### 2. Tạo branch mới

```bash
# Tạo branch từ main
git checkout -b feature/ten-tinh-nang

# Hoặc cho bug fix
git checkout -b fix/ten-bug
```

### 3. Thực hiện thay đổi

- Viết code theo [quy ước](#quy-ước-code)
- Test kỹ trước khi commit
- Commit với message rõ ràng

### 4. Push và tạo Pull Request

```bash
git push origin feature/ten-tinh-nang
```

Sau đó tạo Pull Request trên GitHub.

---

## Báo cáo lỗi

Khi báo cáo lỗi, vui lòng cung cấp:

1. **Tiêu đề ngắn gọn** mô tả vấn đề
2. **Môi trường**:
   - Phiên bản Windows
   - Phiên bản .NET Framework
   - Phiên bản SQL Server
3. **Các bước tái hiện**:
   - Bước 1...
   - Bước 2...
4. **Kết quả mong đợi** vs **Kết quả thực tế**
5. **Screenshot** (nếu có)

### Mẫu Issue

```markdown
## Mô tả lỗi
[Mô tả ngắn gọn vấn đề]

## Môi trường
- Windows: 11
- .NET Framework: 4.8
- SQL Server: 2019

## Các bước tái hiện
1. Mở ứng dụng
2. Click vào...
3. Lỗi xuất hiện

## Kết quả mong đợi
[Mô tả]

## Kết quả thực tế
[Mô tả]

## Screenshot
[Đính kèm nếu có]
```

---

## Đề xuất tính năng

Trước khi đề xuất tính năng mới:

1. Kiểm tra [FEATURES.md](FEATURES.md) xem đã có trong roadmap chưa
2. Tìm trong Issues xem đã có ai đề xuất chưa
3. Nếu chưa, tạo Issue mới với label `enhancement`

### Mẫu đề xuất

```markdown
## Tính năng đề xuất
[Mô tả tính năng]

## Lý do cần tính năng này
[Giải thích use case]

## Giải pháp đề xuất
[Mô tả cách thực hiện nếu có ý tưởng]

## Alternatives
[Các giải pháp thay thế đã cân nhắc]
```

---

## Pull Request

### Checklist trước khi tạo PR

- [ ] Code tuân theo quy ước coding của dự án
- [ ] Đã test trên môi trường local
- [ ] Không có lỗi build
- [ ] Đã cập nhật documentation (nếu cần)
- [ ] Commit message rõ ràng

### Quy trình review

1. Tạo PR với mô tả chi tiết
2. Chờ review từ maintainer
3. Sửa theo feedback (nếu có)
4. Sau khi approved, PR sẽ được merge

### Mẫu PR

```markdown
## Loại thay đổi
- [ ] Bug fix
- [ ] Tính năng mới
- [ ] Refactor
- [ ] Documentation

## Mô tả
[Mô tả thay đổi]

## Related Issues
Fixes #123

## Screenshots (nếu có UI thay đổi)
[Đính kèm]

## Checklist
- [ ] Đã test local
- [ ] Không có lỗi build
- [ ] Code tuân theo quy ước
```

---

## Quy ước Code

### Naming Convention

```csharp
// Classes: PascalCase
public class DatabaseHelper { }

// Methods: PascalCase
public void LoadData() { }

// Variables: camelCase
private string connectionString;

// Constants: UPPER_CASE hoặc PascalCase
private const string DEFAULT_CONNECTION = "...";

// Controls: prefix + PascalCase
private Button btnSave;
private TextBox txtName;
private DataGridView dgvDocuments;
private ComboBox cboSubject;
private Label lblStatus;
```

### Code Style

```csharp
// Dùng var khi type rõ ràng
var document = new Document();

// Explicit type khi cần rõ ràng
DataTable dt = DatabaseHelper.GetData();

// Braces luôn trên dòng mới (Allman style)
if (condition)
{
    DoSomething();
}

// Không bỏ braces cho single-line if
if (condition)
{
    return;
}

// Comments bằng tiếng Việt hoặc tiếng Anh, nhất quán
/// <summary>
/// Lấy danh sách tài liệu của user hiện tại
/// </summary>
public DataTable GetDocuments() { }
```

### Màu sắc (Material Design)

```csharp
// Sử dụng màu đã định nghĩa
Color.FromArgb(33, 150, 243)   // Primary - Blue #2196F3
Color.FromArgb(76, 175, 80)    // Success - Green #4CAF50
Color.FromArgb(244, 67, 54)    // Danger - Red #F44336
Color.FromArgb(255, 152, 0)    // Warning - Orange #FF9800
Color.FromArgb(255, 202, 40)   // Star - Yellow #FFCA28
Color.FromArgb(52, 73, 94)     // Header - Dark #34495E
Color.FromArgb(245, 245, 245)  // Alternating Row #F5F5F5
```

### Toast Notification

```csharp
// Thay vì MessageBox, sử dụng ToastNotification
ToastNotification.Success("Đã lưu thành công!");
ToastNotification.Error("Lỗi: " + ex.Message);
ToastNotification.Warning("Cảnh báo!");
ToastNotification.Info("Thông tin");

// Chỉ dùng MessageBox cho confirm dialogs
DialogResult result = MessageBox.Show(
    "Bạn có chắc chắn muốn xóa?",
    "Xác nhận",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);
```

---

## Cấu trúc dự án

```
study-document-manager/
├── study-document-manager/     # Project chính
│   ├── Form1.cs               # Form chính
│   ├── DatabaseHelper.cs      # Data access layer
│   ├── UserSession.cs         # Quản lý session
│   ├── ToastNotification.cs   # Thông báo Toast
│   ├── IconHelper.cs          # Tạo icon động
│   └── ...
├── DATABASE.md                # Cấu trúc database
├── FEATURES.md                # Roadmap tính năng
├── PROJECT_STRUCTURE.md       # Kiến trúc dự án
├── README.md                  # Hướng dẫn sử dụng
├── CONTRIBUTING.md            # Hướng dẫn đóng góp (file này)
└── LICENSE                    # MIT License
```

Xem chi tiết tại [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md).

---

## Liên hệ

- **Maintainer**: Vũ Đức Dũng
- **Email**: hayatoshino05@gmail.com
- **GitHub**: [@hayato-shino05](https://github.com/hayato-shino05)

---

Cảm ơn bạn đã đóng góp! 
