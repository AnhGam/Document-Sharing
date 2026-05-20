using System;
using System.Drawing;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.Core.Services;
using document_sharing_manager.Services;
using document_sharing_manager.Core.Data;

namespace document_sharing_manager.Management
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblStatus;
        private readonly AuthServiceClient _authClient;

        public bool LoggedIn { get; private set; }

        public LoginForm(AuthServiceClient authClient)
        {
            _authClient = authClient;
            InitializeComponentManual();
        }

        private void InitializeComponentManual()
        {
            this.Text = "Đăng nhập hệ thống";
            this.Size = new Size(360, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = AppTheme.BackgroundMain;

            int left = 40;
            int width = 280;

            var lblTitle = new Label
            {
                Text = "DocShare Login",
                Font = new Font(AppTheme.FontFamily, 16F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Location = new Point(left, 40),
                AutoSize = true
            };

            var lblUser = new Label
            {
                Text = "Tên đăng nhập:",
                Location = new Point(left, 100),
                AutoSize = true,
                ForeColor = AppTheme.TextPrimary
            };
            txtUsername = new TextBox { Location = new Point(left, 125), Width = width, Font = new Font(AppTheme.FontFamily, 10F) };

            var lblPass = new Label
            {
                Text = "Mật khẩu:",
                Location = new Point(left, 170),
                AutoSize = true,
                ForeColor = AppTheme.TextPrimary
            };
            txtPassword = new TextBox { Location = new Point(left, 195), Width = width, Font = new Font(AppTheme.FontFamily, 10F), UseSystemPasswordChar = true };

            lblStatus = new Label
            {
                Text = "",
                Location = new Point(left, 240),
                Width = width,
                Height = 40,
                ForeColor = AppTheme.StatusError,
                TextAlign = ContentAlignment.TopCenter
            };

            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Location = new Point(left, 290),
                Size = new Size(width, 45),
                BackColor = AppTheme.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font(AppTheme.FontFamily, 10F, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            this.Controls.AddRange(new Control[] { lblTitle, lblUser, txtUsername, lblPass, txtPassword, lblStatus, btnLogin });
            
            // Allow Enter key to login
            this.AcceptButton = btnLogin;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblStatus.Text = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            btnLogin.Enabled = false;
            lblStatus.Text = "Đang xác thực...";
            lblStatus.ForeColor = AppTheme.StatusInfo;

            bool success = await _authClient.LoginAsync(user, pass);
            if (success)
            {
                // Save tokens
                document_sharing_manager.Services.SecureStorage.SaveTokens(
                    document_sharing_manager.Core.Data.UserSession.AccessToken, 
                    document_sharing_manager.Core.Data.UserSession.RefreshToken);
                
                // Reset and Initialize DB for the specific user
                DatabaseHelper.ResetConnection();
                DatabaseHelper.InitializeDatabase();

                LoggedIn = true;
                
                // Sync servers from cloud
                try
                {
                    var servers = await _authClient.FetchJoinedServersAsync();
                    foreach (var s in servers)
                    {
                        DatabaseHelper.InsertServer(s.Name, s.BaseUrl, "", s.AccessToken);
                    }
                }
                catch { }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblStatus.Text = "Đăng nhập thất bại.\n" + (_authClient.LastError ?? "Lỗi không xác định.");
                lblStatus.ForeColor = AppTheme.StatusError;
                btnLogin.Enabled = true;
            }
        }
    }
}
