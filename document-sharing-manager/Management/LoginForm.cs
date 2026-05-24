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
            var lnkRegister = new LinkLabel
            {
                Text = "Chưa có tài khoản? Đăng ký ngay",
                Location = new Point(left, 350),
                AutoSize = true,
                Font = new Font(AppTheme.FontFamily, 9F)
            };
            lnkRegister.LinkClicked += (s, e) => 
            {
                var localAuth = new document_sharing_manager.Core.Services.LocalAuthService();
                string user = txtUsername.Text.Trim();
                string pass = txtPassword.Text;

                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    lblStatus.Text = "Nhập Username và Pass để Đăng ký!";
                    lblStatus.ForeColor = AppTheme.StatusError;
                    return;
                }

                if (localAuth.RegisterLocal(user, pass, user, ""))
                {
                    lblStatus.Text = "Đăng ký thành công! Bấm Đăng nhập.";
                    lblStatus.ForeColor = AppTheme.StatusInfo;
                }
                else
                {
                    lblStatus.Text = "Đăng ký thất bại (Tên đã tồn tại).";
                    lblStatus.ForeColor = AppTheme.StatusError;
                }
            };

            this.Controls.AddRange(new Control[] { lblTitle, lblUser, txtUsername, lblPass, txtPassword, lblStatus, btnLogin, lnkRegister });
            
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

            var localAuth = new document_sharing_manager.Core.Services.LocalAuthService();
            var result = localAuth.LoginLocal(user, pass);
            
            if (result.Success)
            {
                document_sharing_manager.Core.Data.UserSession.CurrentUserId = result.UserId;
                document_sharing_manager.Core.Data.UserSession.Username = user;
                
                // Reset and Initialize DB for the specific user
                DatabaseHelper.ResetConnection();
                DatabaseHelper.InitializeDatabase();

                LoggedIn = true;
                
                // Note: We don't fetch joined servers from the API anymore because this is purely local!
                // The joined servers are ALREADY inside this user's specific document_sharing_{UserId}.db.

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblStatus.Text = "Đăng nhập thất bại.\nSai tài khoản hoặc mật khẩu.";
                lblStatus.ForeColor = AppTheme.StatusError;
                btnLogin.Enabled = true;
            }
        }
    }
}
