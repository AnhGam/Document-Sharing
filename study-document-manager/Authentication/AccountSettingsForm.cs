using System;
using System.Drawing;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class AccountSettingsForm : Form
    {
        public AccountSettingsForm()
        {
            InitializeComponent();
        }

        private void AccountSettingsForm_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            txtUsername.Text = UserSession.Username;
            txtFullName.Text = UserSession.FullName;
            txtEmail.Text = UserSession.Email;
            lblRole.Text = UserSession.Role;
            lblLoginTime.Text = UserSession.LoginTime.ToString("dd/MM/yyyy HH:mm");
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(fullName))
            {
                ToastNotification.Warning("Vui lòng nhập họ tên!");
                txtFullName.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                ToastNotification.Warning("Email không hợp lệ!");
                txtEmail.Focus();
                return;
            }

            try
            {
                bool success = DatabaseHelper.UpdateUserProfile(UserSession.UserId, fullName, email);
                if (success)
                {
                    UserSession.FullName = fullName;
                    UserSession.Email = email;

                    ToastNotification.Success("Cập nhật thông tin thành công!");
                }
                else
                {
                    ToastNotification.Error("Không thể cập nhật thông tin!");
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error($"Lỗi: {ex.Message}");
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(currentPassword))
            {
                ToastNotification.Warning("Vui lòng nhập mật khẩu hiện tại!");
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                ToastNotification.Warning("Vui lòng nhập mật khẩu mới!");
                txtNewPassword.Focus();
                return;
            }

            if (newPassword.Length < 6)
            {
                ToastNotification.Warning("Mật khẩu mới phải có ít nhất 6 ký tự!");
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                ToastNotification.Warning("Xác nhận mật khẩu không khớp!");
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                var result = DatabaseHelper.ChangePasswordSelf(UserSession.UserId, currentPassword, newPassword);
                
                if (result.Success)
                {
                    ToastNotification.Success("Đổi mật khẩu thành công!");
                    
                    txtCurrentPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                }
                else
                {
                    ToastNotification.Error(result.Message);
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error($"Lỗi: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
