using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager.Services
{
    /// <summary>
    /// Downloads and installs updates from GitHub Releases
    /// </summary>
    public static class UpdateInstaller
    {
        /// <summary>
        /// Download Setup.exe and launch it, then close the current app
        /// </summary>
        public static void DownloadAndInstall(string downloadUrl, string version, Form parentForm)
        {
            if (string.IsNullOrEmpty(downloadUrl))
            {
                MessageBox.Show(
                    "Không tìm thấy file cài đặt.\nVui lòng tải thủ công từ GitHub.",
                    "Lỗi cập nhật",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Phiên bản mới {version} đã sẵn sàng.\n\n" +
                "Ứng dụng sẽ tải bản cài đặt và khởi động lại.\n" +
                "Bạn có muốn tiếp tục?",
                "Cập nhật ứng dụng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            // Show progress dialog
            var progressForm = CreateProgressForm(parentForm);
            progressForm.Show(parentForm);

            string tempPath = Path.Combine(
                Path.GetTempPath(),
                $"StudyDocumentManager_{version}_Setup.exe");

            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", $"StudyDocumentManager/{AppVersion.Current}");

                client.DownloadProgressChanged += (s, e) =>
                {
                    if (progressForm.IsDisposed) return;
                    var progressBar = progressForm.Controls["progressBar"] as ProgressBar;
                    var lblStatus = progressForm.Controls["lblStatus"] as Label;
                    if (progressBar != null) progressBar.Value = e.ProgressPercentage;
                    if (lblStatus != null)
                    {
                        double mbReceived = e.BytesReceived / 1048576.0;
                        double mbTotal = e.TotalBytesToReceive / 1048576.0;
                        lblStatus.Text = $"Đang tải... {mbReceived:F1} / {mbTotal:F1} MB ({e.ProgressPercentage}%)";
                    }
                };

                client.DownloadFileCompleted += (s, e) =>
                {
                    if (!progressForm.IsDisposed) progressForm.Close();

                    if (e.Error != null)
                    {
                        MessageBox.Show(
                            $"Lỗi khi tải: {e.Error.Message}",
                            "Lỗi cập nhật",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    if (e.Cancelled) return;

                    try
                    {
                        // Launch installer and close app
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = tempPath,
                            UseShellExecute = true
                        });

                        Application.Exit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Không thể khởi chạy trình cài đặt:\n{ex.Message}",
                            "Lỗi cập nhật",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                };

                client.DownloadFileAsync(new Uri(downloadUrl), tempPath);
            }
        }

        private static Form CreateProgressForm(Form parent)
        {
            var form = new Form
            {
                Text = "Đang cập nhật...",
                Size = new Size(420, 160),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = AppTheme.BackgroundMain,
                ShowInTaskbar = false
            };

            if (parent != null && parent.Icon != null)
                form.Icon = parent.Icon;

            var lblTitle = new Label
            {
                Text = "Đang tải phiên bản mới...",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(20, 20),
                AutoSize = true
            };

            var progressBar = new ProgressBar
            {
                Name = "progressBar",
                Location = new Point(20, 50),
                Size = new Size(360, 25),
                Style = ProgressBarStyle.Continuous
            };

            var lblStatus = new Label
            {
                Name = "lblStatus",
                Text = "Đang kết nối...",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(20, 82),
                AutoSize = true
            };

            form.Controls.AddRange(new Control[] { lblTitle, progressBar, lblStatus });
            return form;
        }
    }
}
