using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using document_sharing_manager.UI;

namespace document_sharing_manager.Documents
{
    public partial class SettingsForm : Form
    {
        private TextBox txtApiUrl;
        private TextBox txtAccessToken;
        private Button btnSave;
        private Button btnCancel;
        private Label lblStatus;

        public SettingsForm()
        {
            InitializeComponentManual();
            LoadSettings();
        }

        private void InitializeComponentManual()
        {
            this.Text = "Cài đặt kết nối";
            this.Size = new Size(450, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = AppTheme.BackgroundMain;

            var lblTitle = new Label
            {
                Text = "Cấu hình Server API",
                Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Location = new Point(20, 20),
                AutoSize = true
            };

            var lblUrl = new Label
            {
                Text = "Địa chỉ Server URL:",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(20, 60),
                AutoSize = true
            };

            txtApiUrl = new TextBox
            {
                Location = new Point(20, 85),
                Size = new Size(390, 25),
                Font = AppTheme.FontBody
            };

            var lblToken = new Label
            {
                Text = "JWT Access Token (để đồng bộ):",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(20, 120),
                AutoSize = true
            };

            txtAccessToken = new TextBox
            {
                Location = new Point(20, 145),
                Size = new Size(390, 25),
                Font = AppTheme.FontBody,
                PasswordChar = '*'
            };

            lblStatus = new Label
            {
                Text = "Lưu ý: Thay đổi URL yêu cầu khởi động lại App.",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(20, 180),
                AutoSize = true
            };

            btnSave = new Button
            {
                Text = "Lưu cấu hình",
                Size = new Size(120, 35),
                Location = new Point(170, 205),
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.Primary,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSaveClick;

            btnCancel = new Button
            {
                Text = "Hủy",
                Size = new Size(100, 35),
                Location = new Point(310, 205),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = AppTheme.TextPrimary,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnCancel.Click += (s, e) => this.Close();

            this.Size = new Size(450, 300);
            this.Controls.AddRange([lblTitle, lblUrl, txtApiUrl, lblToken, txtAccessToken, lblStatus, btnSave, btnCancel]);
        }

        private void LoadSettings()
        {
            txtApiUrl.Text = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5247/api/documents";
            txtAccessToken.Text = ConfigurationManager.AppSettings["AccessToken"] ?? "";
            
            // Apply to session immediately if exists
            if (!string.IsNullOrEmpty(txtAccessToken.Text))
            {
                document_sharing_manager.Core.Data.UserSession.AccessToken = txtAccessToken.Text;
            }
        }

        private void BtnSaveClick(object sender, EventArgs e)
        {
            string newUrl = txtApiUrl.Text.Trim();
            if (string.IsNullOrEmpty(newUrl) || !Uri.TryCreate(newUrl, UriKind.Absolute, out _))
            {
                MessageBox.Show("Vui lòng nhập URL hợp lệ (ví dụ: http://192.168.1.15:5000/api/documents).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["ApiBaseUrl"] == null)
                {
                    config.AppSettings.Settings.Add("ApiBaseUrl", newUrl);
                }
                else
                {
                    config.AppSettings.Settings["ApiBaseUrl"].Value = newUrl;
                }
                // Update token in config too
                if (config.AppSettings.Settings["AccessToken"] == null)
                    config.AppSettings.Settings.Add("AccessToken", txtAccessToken.Text.Trim());
                else
                    config.AppSettings.Settings["AccessToken"].Value = txtAccessToken.Text.Trim();
                
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // Update Session
                document_sharing_manager.Core.Data.UserSession.AccessToken = txtAccessToken.Text.Trim();

                MessageBox.Show("Đã lưu cấu hình thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu cấu hình: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
