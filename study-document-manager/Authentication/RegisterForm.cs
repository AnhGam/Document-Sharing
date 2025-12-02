using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager
{
    public partial class RegisterForm : Form
    {
        public string RegisteredUsername { get; private set; }
        private bool _passwordVisible = false;
        private bool _confirmPasswordVisible = false;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            btnShowPassword.Image = IconHelper.CreateEyeIcon(16, true);
            btnShowPassword.Text = "";
            btnShowConfirmPassword.Image = IconHelper.CreateEyeIcon(16, true);
            btnShowConfirmPassword.Text = "";

            lblWelcome.Text = "Tạo tài khoản mới";
            lblAppName.Text = "Study Document\nManager";
            lblTagline.Text = "Đăng ký để bắt đầu quản lý tài liệu học tập của bạn một cách thông minh.";
            lblTitle.Text = "Đăng ký tài khoản";
            lblSubtitle.Text = "Điền thông tin bên dưới";
            lblUsername.Text = "Tên đăng nhập *";
            lblPassword.Text = "Mật khẩu *";
            lblConfirmPassword.Text = "Xác nhận mật khẩu *";
            lblFullName.Text = "Họ tên *";
            lblEmail.Text = "Email (tùy chọn)";
            btnRegister.Text = "Đăng ký";
            btnCancel.Text = "Hủy";

            txtUsername.Focus();
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
                e.Graphics.FillEllipse(circleBrush, -70, 420, 200, 200);
                e.Graphics.FillEllipse(circleBrush, 240, -50, 160, 160);
                e.Graphics.FillEllipse(circleBrush, 120, 500, 100, 100);
            }

            using (SolidBrush accentBrush = new SolidBrush(Color.FromArgb(15, 16, 185, 129)))
            {
                e.Graphics.FillEllipse(accentBrush, 30, 520, 70, 70);
                e.Graphics.FillEllipse(accentBrush, 280, 80, 50, 50);
            }
        }

        private void btnRegister_MouseEnter(object sender, EventArgs e)
        {
            btnRegister.BackColor = AppTheme.PrimaryLight;
        }

        private void btnRegister_MouseLeave(object sender, EventArgs e)
        {
            btnRegister.BackColor = AppTheme.Primary;
        }

        /// <summary>
        /// Button Đăng ký
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Validation
            if (!ValidateInput())
                return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string role = "User";

            // Kiểm tra username đã tồn tại
            if (DatabaseHelper.CheckUsernameExists(username))
            {
                ToastNotification.Error("Tên đăng nhập đã tồn tại!");
                txtUsername.Focus();
                return;
            }

            // Kiểm tra email đã tồn tại
            if (!string.IsNullOrEmpty(email) && DatabaseHelper.CheckEmailExists(email))
            {
                ToastNotification.Error("Email đã được sử dụng!");
                txtEmail.Focus();
                return;
            }

            // Tạo tài khoản
            bool success = DatabaseHelper.RegisterUser(username, password, fullName, email, role);

            if (success)
            {
                ToastNotification.Success("Đăng ký thành công! Bạn có thể đăng nhập ngay.");
                
                RegisteredUsername = username;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ToastNotification.Error("Đăng ký thất bại! Vui lòng thử lại.");
            }
        }

        /// <summary>
        /// Validation input
        /// </summary>
        private bool ValidateInput()
        {
            // Username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ToastNotification.Warning("Vui lòng nhập tên đăng nhập!");
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length < 3)
            {
                ToastNotification.Warning("Tên đăng nhập phải có ít nhất 3 ký tự!");
                txtUsername.Focus();
                return false;
            }

            // Password
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                ToastNotification.Warning("Vui lòng nhập mật khẩu!");
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                ToastNotification.Warning("Mật khẩu phải có ít nhất 6 ký tự!");
                txtPassword.Focus();
                return false;
            }

            // Confirm Password
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                ToastNotification.Warning("Mật khẩu xác nhận không khớp!");
                txtConfirmPassword.Focus();
                return false;
            }

            // Full Name
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ToastNotification.Warning("Vui lòng nhập họ tên!");
                txtFullName.Focus();
                return false;
            }

            // Email (optional nhưng phải hợp lệ nếu nhập)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!IsValidEmail(txtEmail.Text))
                {
                    ToastNotification.Warning("Email không hợp lệ!");
                    txtEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// Button Hủy
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Toggle hiện/ẩn mật khẩu
        /// </summary>
        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            _passwordVisible = !_passwordVisible;
            txtPassword.PasswordChar = _passwordVisible ? '\0' : '*';
            btnShowPassword.Image = IconHelper.CreateEyeIcon(16, !_passwordVisible);
        }

        /// <summary>
        /// Toggle hiện/ẩn mật khẩu xác nhận
        /// </summary>
        private void btnShowConfirmPassword_Click(object sender, EventArgs e)
        {
            _confirmPasswordVisible = !_confirmPasswordVisible;
            txtConfirmPassword.PasswordChar = _confirmPasswordVisible ? '\0' : '*';
            btnShowConfirmPassword.Image = IconHelper.CreateEyeIcon(16, !_confirmPasswordVisible);
        }
    }
}
