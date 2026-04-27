# PHẦN 10: CI/CD PIPELINES & DEVOPS - BEST PRACTICES

**Mục đích:** Xây dựng quy trình tự động hóa đạt chuẩn Enterprise, đảm bảo code push lên luôn sạch, chạy được và tự động phân phối.

## 1. Cấu trúc GitHub Actions (dotnet-ci.yml)
Pipeline phải được chia thành nhiều Jobs chạy song song hoặc tuần tự hợp lý:
- **Job 1: Build & Lint:** Chạy `dotnet restore`, `dotnet build --no-restore`. Bắt buộc chạy công cụ phân tích tĩnh `dotnet format --verify-no-changes` để đảm bảo code chuẩn convention.
- **Job 2: Unit Tests:** Chạy `dotnet test`. Cấu hình fail pipeline nếu coverage < 70% (dùng Coverlet).
- **Job 3: Docker Build (Server):** Chỉ trigger khi Job 1 & 2 pass.

## 2. Dockerfile Optimization (Server)
- Bắt buộc dùng **Multi-Stage Build**.
  - Stage `build`: Dùng image `mcr.microsoft.com/dotnet/sdk:8.0` chứa đầy đủ công cụ để compile.
  - Stage `runtime`: Dùng image `mcr.microsoft.com/dotnet/aspnet:8.0-alpine` (cực kỳ nhẹ, tăng bảo mật, giảm attack surface).
- Chỉ copy các file cần thiết (binaries/publish) từ Stage `build` sang Stage `runtime`.
- Chạy container bằng user non-root (không dùng quyền admin trong Docker) để tăng cường an ninh.

## 3. CD: Automated Client Release
- **Chiến lược phân phối WinForms:**
  - Cấu hình một Workflow riêng chạy khi có push tag (VD: `v1.0.0`).
  - Dùng action tạo file nén `.zip` của thư mục Output (hoặc dùng InnoSetup compiler script tạo file `.exe` cài đặt).
  - Tự động tạo Release trên GitHub và đính kèm (Upload Asset) file `.exe` vừa tạo, kèm theo Changelog tự động sinh.

## 4. SonarCloud / CodeQL Integration
- Tích hợp bước Security Scan. Nếu phát hiện lỗ hổng nghiêm trọng (Critical Vulnerability) -> Tự động block việc Merge PR.
