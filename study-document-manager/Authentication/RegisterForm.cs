using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class RegisterForm : Form
    {
        public string RegisteredUsername { get; private set; }

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            // Thiết lập icon cho button show password
            btnShowPassword.Image = IconHelper.CreateEyeIcon(16, true);
            btnShowPassword.Text = "";
            btnShowConfirmPassword.Image = IconHelper.CreateEyeIcon(16, true);
            btnShowConfirmPassword.Text = "";

            txtUsername.Focus();
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
        /// Hiện mật khẩu khi giữ chuột
        /// </summary>
        private void btnShowPassword_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.PasswordChar = '\0';
        }

        /// <summary>
        /// Ẩn mật khẩu khi nhả chuột
        /// </summary>
        private void btnShowPassword_MouseUp(object sender, MouseEventArgs e)
        {
            txtPassword.PasswordChar = '*';
        }

        /// <summary>
        /// Hiện mật khẩu xác nhận khi giữ chuột
        /// </summary>
        private void btnShowConfirmPassword_MouseDown(object sender, MouseEventArgs e)
        {
            txtConfirmPassword.PasswordChar = '\0';
        }

        /// <summary>
        /// Ẩn mật khẩu xác nhận khi nhả chuột
        /// </summary>
        private void btnShowConfirmPassword_MouseUp(object sender, MouseEventArgs e)
        {
            txtConfirmPassword.PasswordChar = '*';
        }
    }
}
