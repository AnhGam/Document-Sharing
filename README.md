# Document Sharing Manager

Document Sharing Manager là hệ thống chia sẻ tài liệu ngang hàng, được thiết kế theo mô hình Client-Server với giao diện Desktop WinForms dành cho người quản trị (Admin) và người dùng cuối, kết hợp cùng Server API (ASP.NET Core) ẩn bên trong để quản lý kết nối.

<div align="center">

![Document Sharing Manager](docs/assets/hero-banner.png)

[![CI/CD Status](https://img.shields.io/badge/CI%2FCD-Enterprise_Pipeline_2026-success?style=for-the-badge&logo=github-actions)](https://github.com/AnhGam/Document-Sharing/actions)

</div>

## Tính năng chính

- **Mô hình Client-Server Tích hợp**:
  - Giao diện WinForms (Client) giúp người dùng quản lý tài liệu, đồng bộ thư mục nội bộ.
  - Tích hợp tính năng Server Administration ngay trên Client UI, cho phép người dùng đóng vai trò máy chủ Host quản lý mọi request mà không cần thao tác qua dòng lệnh (CLI).
  - Tự động lưu vết bảo mật thông qua Audit Log (Nhật ký hệ thống).

- **Hệ thống Invite Link (Link Mời)**:
  - Chia sẻ kết nối với người ngoài LAN thông qua Invite Code (ví dụ: `docshare://join/1234abcd`).
  - Hỗ trợ *Custom URI Scheme*: Nhấp vào link trên trình duyệt sẽ tự động gọi ứng dụng lên.
  - Quản lý lời mời linh hoạt: Giới hạn số lần dùng, thời hạn sử dụng, và tùy chọn "Bắt buộc duyệt" để tăng cường bảo mật.

- **Dashboard CI/CD Web**:
  - Các bản build được tự động phân tích và tạo trang Web Dashboard trên GitHub Pages.
  - Theo dõi DORA Metrics: Tỉ lệ thành công, thời gian build trung bình, dung lượng Repo.
  - **AI Diagnostic**: Tự động chẩn đoán và đưa ra lý do, đề xuất sửa lỗi cho các bản Build thất bại (Failed Builds) hiển thị trực quan.

## Công nghệ sử dụng
- **WinForms (.NET Framework 4.8)**: Xây dựng giao diện ứng dụng Desktop (Client).
- **ASP.NET Core (.NET 8.0)**: Cung cấp API nội bộ và quản lý kết nối ngang hàng.
- **SQLite / Entity Framework Core**: Lưu trữ dữ liệu cấu hình, Document, Audit Logs.
- **GitHub Actions**: Đường ống CI/CD tự động (Build, Test, Report, Web Deploy).
- **HTML/CSS/JS thuần**: Cho Dashboard Report giao diện đẹp mắt (Neon theme).

## Giới hạn hệ thống
- Hỗ trợ upload/sync tệp tin lớn lên đến **10GB**.

## Xem thêm
- [SETUP_GUIDE.md](SETUP_GUIDE.md): Hướng dẫn cài đặt và thiết lập.
- [CONTRIBUTING.md](CONTRIBUTING.md): Hướng dẫn tham gia phát triển và luồng CI/CD.
