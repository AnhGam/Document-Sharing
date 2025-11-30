using System;
using System.Drawing;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Thiết lập icon cho button show password
            btnShowPassword.Image = IconHelper.CreateEyeIcon(16, true);
            btnShowPassword.Text = "";
            
            // Focus vào username
            txtUsername.Focus();
        }

        /// <summary>
        /// Button Đăng nhập
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Validation
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

            // Xác thực
            var user = DatabaseHelper.AuthenticateUser(username, password);
            
            if (user != null)
            {
                // Kiểm tra tài khoản có active không
                if (!Convert.ToBoolean(user["is_active"]))
                {
                    ToastNotification.Error("Tài khoản đã bị khóa!");
                    return;
                }

                // Lưu thông tin session
                UserSession.UserId = Convert.ToInt32(user["id"]);
                UserSession.Username = user["username"].ToString();
                UserSession.FullName = user["full_name"].ToString();
                UserSession.Email = user["email"] != DBNull.Value ? user["email"].ToString() : "";
                UserSession.Role = user["role"].ToString();
                UserSession.LoginTime = DateTime.Now;

                // Cập nhật last_login
                DatabaseHelper.UpdateLastLogin(UserSession.UserId);

                // Lưu Remember Me
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

        /// <summary>
        /// Link Đăng ký
        /// </summary>
        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                // Sau khi đăng ký thành công, điền username
                txtUsername.Text = registerForm.RegisteredUsername;
                txtPassword.Focus();
            }
        }

        /// <summary>
        /// Enter key để login
        /// </summary>
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnLogin_Click(sender, e);
            }
        }

        /// <summary>
        /// Button Thoát
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
    }
}
