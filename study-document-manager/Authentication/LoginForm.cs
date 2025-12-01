using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager
{
    public partial class LoginForm : Form
    {
        private bool isDragging = false;
        private Point dragStart;

        public LoginForm()
        {
            InitializeComponent();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            pnlLeft.MouseDown += Form_MouseDown;
            pnlLeft.MouseMove += Form_MouseMove;
            pnlLeft.MouseUp += Form_MouseUp;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            btnShowPassword.Image = IconHelper.CreateEyeIcon(18, true);
            btnShowPassword.Text = "";
            
            if (!string.IsNullOrEmpty(Properties.Settings.Default.RememberUsername))
            {
                txtUsername.Text = Properties.Settings.Default.RememberUsername;
                chkRememberMe.Checked = true;
                txtPassword.Focus();
            }
            else
            {
                txtUsername.Focus();
            }

            lblWelcome.Text = "Chào mừng đến";
            lblAppName.Text = "Study Document\nManager";
            lblTagline.Text = "Quản lý tài liệu học tập thông minh,\nđơn giản và hiệu quả.";
            lblTitle.Text = "Đăng nhập";
            lblSubtitle.Text = "Nhập thông tin tài khoản của bạn";
            lblUsername.Text = "Tên đăng nhập";
            lblPassword.Text = "Mật khẩu";
            chkRememberMe.Text = "Ghi nhớ tài khoản";
            btnLogin.Text = "Đăng nhập";
            lnkRegister.Text = "Chưa có tài khoản? Đăng ký";
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = new Point(e.X, e.Y);
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - dragStart.X, p.Y - dragStart.Y);
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void pnlLeft_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (LinearGradientBrush brush = new LinearGradientBrush(
                pnlLeft.ClientRectangle,
                Color.FromArgb(240, 253, 250),
                Color.FromArgb(204, 251, 241),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, pnlLeft.ClientRectangle);
            }

            using (SolidBrush circleBrush = new SolidBrush(Color.FromArgb(25, 20, 184, 166)))
            {
                e.Graphics.FillEllipse(circleBrush, -80, 380, 220, 220);
                e.Graphics.FillEllipse(circleBrush, 280, -60, 180, 180);
                e.Graphics.FillEllipse(circleBrush, 150, 420, 120, 120);
            }

            using (SolidBrush accentBrush = new SolidBrush(Color.FromArgb(15, 16, 185, 129)))
            {
                e.Graphics.FillEllipse(accentBrush, 20, 450, 80, 80);
                e.Graphics.FillEllipse(accentBrush, 320, 100, 60, 60);
            }
        }

        private void btnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = AppTheme.PrimaryLight;
        }

        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = AppTheme.Primary;
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            txtUsername.BackColor = Color.White;
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            txtUsername.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                ToastNotification.Warning("Vui lòng nhập tên đăng nhập!");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ToastNotification.Warning("Vui lòng nhập mật khẩu!");
                txtPassword.Focus();
                return;
            }

            var user = DatabaseHelper.AuthenticateUser(username, password);
            
            if (user != null)
            {
                if (!Convert.ToBoolean(user["is_active"]))
                {
                    ToastNotification.Error("Tài khoản đã bị khóa!");
                    return;
                }

                UserSession.UserId = Convert.ToInt32(user["id"]);
                UserSession.Username = user["username"].ToString();
                UserSession.FullName = user["full_name"].ToString();
                UserSession.Email = user["email"] != DBNull.Value ? user["email"].ToString() : "";
                UserSession.Role = user["role"].ToString();
                UserSession.LoginTime = DateTime.Now;

                DatabaseHelper.UpdateLastLogin(UserSession.UserId);

                if (chkRememberMe.Checked)
                {
                    Properties.Settings.Default.RememberUsername = username;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.RememberUsername = "";
                    Properties.Settings.Default.Save();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ToastNotification.Error("Tên đăng nhập hoặc mật khẩu không đúng!");
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                txtUsername.Text = registerForm.RegisteredUsername;
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnLogin_Click(sender, e);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExit_MouseEnter(object sender, EventArgs e)
        {
            btnExit.ForeColor = AppTheme.StatusError;
            btnExit.BackColor = Color.FromArgb(254, 226, 226);
        }

        private void btnExit_MouseLeave(object sender, EventArgs e)
        {
            btnExit.ForeColor = AppTheme.TextMuted;
            btnExit.BackColor = Color.White;
        }

        private void btnShowPassword_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.PasswordChar = '\0';
        }

        private void btnShowPassword_MouseUp(object sender, MouseEventArgs e)
        {
            txtPassword.PasswordChar = '*';
        }
    }
}
