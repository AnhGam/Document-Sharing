# PHẦN 10: CI/CD PIPELINES & DEVOPS - BEST PRACTICES (ENTERPRISE EDITION 2026)

**Mục đích:** Xây dựng quy trình tự động hóa đạt chuẩn Enterprise, tối ưu hóa chi phí (FinOps) và đảm bảo tính quan sát toàn diện (Observability).

## 1. Cấu trúc GitHub Actions (dotnet-ci.yml)
Pipeline được chia thành các Job chuyên biệt để đảm bảo tốc độ và độ tin cậy:
- **Job 1: Security & Quality Gates:**
    - Secret Scanning (Gitleaks).
    - Static Analysis (SonarCloud/CodeQL).
    - Dependency Audit (NuGet Vulnerability Scan).
- **Job 2: Build & Unit Tests:** Chạy `dotnet test` với Coverage report.
- **Job 3: Mutation Testing (Stryker):** Đánh giá chất lượng của chính bộ Test.
- **Job 4: Capacity & Governance (FinOps):**
    - Kiểm tra dung lượng Artifact/Installer (Ngưỡng cảnh báo: 50MB).
    - Phân tích xu hướng tăng trưởng dung lượng repo.

## 2. Docker & Container Strategy (Platform Engineering)
- **Multi-Stage Build:** Giảm kích thước Image (Runtime dùng Alpine).
- **Immutable Artifacts:** Mỗi build sinh ra một Image duy nhất, không thay đổi khi đi qua các môi trường.
- **Rootless Execution:** Bảo mật tối đa cho container.

## 3. Observability - Ba Trụ Cột (Monitoring & Logging)
Theo tiêu chuẩn DevOps 2026, hệ thống phải đảm bảo giám sát theo mô hình **Four Golden Signals**:
1. **Latency:** Thời gian phản hồi của hệ thống.
2. **Traffic:** Lưu lượng truy cập (Request/sec).
3. **Errors:** Tỷ lệ lỗi (Error rate).
4. **Saturation:** Mức độ sử dụng tài nguyên (CPU, RAM, Disk I/O).

Hệ thống phải triển khai đầy đủ:
1. **Metrics:** Thu thập dữ liệu qua Prometheus/Grafana.
2. **Logs:** Tập trung log (ELK Stack hoặc Seq) để truy vết lỗi.
3. **Traces:** Sử dụng OpenTelemetry để theo dõi luồng dữ liệu (End-to-End Tracing).

## 4. CD & GitOps (Advanced Delivery)
- **GitOps với ArgoCD:** Tự động đồng bộ trạng thái mong muốn (Desired State) từ Git vào môi trường Runtime (Kubernetes).
- **Automated Client Release:** 
    - Khi push Tag `v*`, tự động build Installer (.exe).
    - Tự động tạo GitHub Release và upload Asset kèm Changelog sinh bởi AI.

## 5. Capacity Management (Quản lý dung lượng)
- **Nguyên tắc FinOps:** Giảm thiểu "Toil" và chi phí lưu trữ không cần thiết.
- **Cleanup Policy:** Tự động xóa các build cũ, nén các log lịch sử.
- **Size Guard:** Pipeline sẽ fail nếu file installer tăng đột biến (>20% so với bản trước) mà không có giải trình.

## 6. Infrastructure as Code (IaC)
- Toàn bộ hạ tầng Server (sau này) phải được định nghĩa bằng **Terraform** hoặc **Ansible** để đảm bảo tính nhất quán (Environment Parity).
