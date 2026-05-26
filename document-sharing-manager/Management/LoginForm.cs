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
            lnkRegister.LinkClicked += async (s, ev) => 
            {
                string user = txtUsername.Text.Trim();
                string pass = txtPassword.Text;

                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    lblStatus.Text = "Nhập Username và Pass để Đăng ký!";
                    lblStatus.ForeColor = AppTheme.StatusError;
                    return;
                }

                lblStatus.Text = "Đang đăng ký...";
                lblStatus.ForeColor = AppTheme.StatusInfo;
                lnkRegister.Enabled = false;

                bool regSuccess = await _authClient.RegisterAsync(user, pass, $"{user}@domain.com");
                if (regSuccess)
                {
                    lblStatus.Text = "Đăng ký thành công! Bấm Đăng nhập.";
                    lblStatus.ForeColor = AppTheme.StatusInfo;
                }
                else
                {
                    lblStatus.Text = _authClient.LastError ?? "Đăng ký thất bại (Username đã tồn tại).";
                    lblStatus.ForeColor = AppTheme.StatusError;
                }
                lnkRegister.Enabled = true;
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

            // 1. Xác thực trực tiếp trên Server API trung tâm
            bool apiSuccess = await _authClient.LoginAsync(user, pass);
            
            if (apiSuccess)
            {
                // Khi đăng nhập thành công, các thông tin UserSession đã được tự động điền trong AuthServiceClient
                
                // 2. Khởi tạo/Kết nối tới CSDL SQLite cục bộ dành riêng cho User này
                DatabaseHelper.ResetConnection();
                DatabaseHelper.InitializeDatabase();

                // 3. Đảm bảo bản ghi tài khoản tồn tại trong CSDL SQLite cục bộ để đồng bộ hóa
                try
                {
                    string checkQuery = "SELECT COUNT(*) FROM tai_khoan WHERE id = @id";
                    var checkRes = DatabaseHelper.ExecuteScalar(checkQuery, [new System.Data.SQLite.SQLiteParameter("@id", UserSession.CurrentUserId)]);
                    if (Convert.ToInt32(checkRes) == 0)
                    {
                        string insertQuery = "INSERT INTO tai_khoan (id, ten_dang_nhap, mat_khau, ho_ten) VALUES (@id, @username, @password, @username)";
                        DatabaseHelper.ExecuteNonQuery(insertQuery, [
                            new System.Data.SQLite.SQLiteParameter("@id", UserSession.CurrentUserId),
                            new System.Data.SQLite.SQLiteParameter("@username", UserSession.Username),
                            new System.Data.SQLite.SQLiteParameter("@password", "central_auth") // Mật khẩu quản lý tập trung trên Server
                        ]);
                    }
                }
                catch { }

                LoggedIn = true;

                // 4. Tự động tải về danh sách các Kênh chia sẻ mà User này đã tham gia từ Server
                try
                {
                    var joinedServers = await _authClient.FetchJoinedServersAsync();
                    foreach (var srv in joinedServers)
                    {
                        DatabaseHelper.InsertServer(srv.Name, srv.BaseUrl, accessToken: UserSession.AccessToken ?? "", remoteId: srv.Id);
                    }
                }
                catch { }

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
