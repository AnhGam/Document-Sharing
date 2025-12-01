using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager
{
    public partial class UserManagementForm : Form
    {
        public UserManagementForm()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            
            // Header panel
            pnlHeader.BackColor = AppTheme.Primary;
            lblTitle.ForeColor = Color.White;
            
            // Buttons
            AppTheme.ApplyButtonPrimary(btnRefresh);
            AppTheme.ApplyButtonSuccess(btnAdd);
            AppTheme.ApplyButtonPrimary(btnChangePassword);
            AppTheme.ApplyButtonSecondary(btnEditRole);
            AppTheme.ApplyButtonWarning(btnToggleActive);
            AppTheme.ApplyButtonDanger(btnDelete);
            AppTheme.ApplyButtonDanger(btnClose);
            
            // Status strip
            statusStrip1.BackColor = AppTheme.BackgroundSoft;
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            // Kiểm tra quyền Admin
            if (!UserSession.IsAdmin)
            {
                ToastNotification.Warning("Bạn không có quyền truy cập chức năng này!");
                this.Close();
                return;
            }

            // Setup icons cho buttons
            btnEditRole.Image = IconHelper.CreateRoleIcon(16, Color.White);

            LoadUsers();
            SetupDataGridView();
        }

        /// <summary>
        /// Load danh sách users
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetAllUsers();
                dgvUsers.DataSource = dt;
                lblCount.Text = $"Tổng số: {dt.Rows.Count} users";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi load dữ liệu: " + ex.Message);
            }
        }

        /// <summary>
        /// Setup DataGridView
        /// </summary>
        private void SetupDataGridView()
        {
            if (dgvUsers.Columns.Count > 0)
            {
                // Ẩn cột id và password_hash
                if (dgvUsers.Columns.Contains("id"))
                    dgvUsers.Columns["id"].Visible = false;
                if (dgvUsers.Columns.Contains("password_hash"))
                    dgvUsers.Columns["password_hash"].Visible = false;

                // Set header text
                if (dgvUsers.Columns.Contains("username"))
                    dgvUsers.Columns["username"].HeaderText = "Tên đăng nhập";
                if (dgvUsers.Columns.Contains("full_name"))
                    dgvUsers.Columns["full_name"].HeaderText = "Họ tên";
                if (dgvUsers.Columns.Contains("email"))
                    dgvUsers.Columns["email"].HeaderText = "Email";
                if (dgvUsers.Columns.Contains("role"))
                    dgvUsers.Columns["role"].HeaderText = "Vai trò";
                if (dgvUsers.Columns.Contains("is_active"))
                {
                    dgvUsers.Columns["is_active"].HeaderText = "Trạng thái";
                    dgvUsers.Columns["is_active"].Width = 100;
                }
                if (dgvUsers.Columns.Contains("created_date"))
                {
                    dgvUsers.Columns["created_date"].HeaderText = "Ngày tạo";
                    dgvUsers.Columns["created_date"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvUsers.Columns["created_date"].Width = 130;
                }
                if (dgvUsers.Columns.Contains("last_login"))
                {
                    dgvUsers.Columns["last_login"].HeaderText = "Đăng nhập cuối";
                    dgvUsers.Columns["last_login"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvUsers.Columns["last_login"].Width = 130;
                }
                
                // Ẩn các cột không cần thiết
                if (dgvUsers.Columns.Contains("failed_login_attempts"))
                    dgvUsers.Columns["failed_login_attempts"].Visible = false;
                if (dgvUsers.Columns.Contains("locked_until"))
                    dgvUsers.Columns["locked_until"].Visible = false;
                if (dgvUsers.Columns.Contains("created_by"))
                    dgvUsers.Columns["created_by"].Visible = false;
                if (dgvUsers.Columns.Contains("updated_date"))
                    dgvUsers.Columns["updated_date"].Visible = false;
                if (dgvUsers.Columns.Contains("updated_by"))
                    dgvUsers.Columns["updated_by"].Visible = false;
            }

            // Style using AppTheme
            AppTheme.ApplyDataGridViewStyle(dgvUsers);
            
            // QUAN TRỌNG: Xử lý lỗi DataError
            dgvUsers.DataError += DgvUsers_DataError;
        }

        /// <summary>
        /// Xử lý lỗi DataGridView
        /// </summary>
        private void DgvUsers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Ngăn hiện thị dialog lỗi
            e.ThrowException = false;
            e.Cancel = true;
            
            // Log để debug
            System.Diagnostics.Debug.WriteLine($"DataGridView error: {e.Exception.Message}");
        }

        /// <summary>
        /// Button Làm mới
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        /// <summary>
        /// Button Thêm user
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            RegisterForm form = new RegisterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
                ToastNotification.Success("Đã thêm user mới thành công!");
            }
        }

        /// <summary>
        /// Button Đổi mật khẩu
        /// </summary>
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn user cần đổi mật khẩu!");
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["id"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["username"].Value.ToString();

            // Form nhập mật khẩu mới
            using (Form promptForm = new Form())
            {
                promptForm.Text = "Đổi mật khẩu";
                promptForm.Width = 400;
                promptForm.Height = 180;
                promptForm.StartPosition = FormStartPosition.CenterParent;
                promptForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                promptForm.MaximizeBox = false;
                promptForm.MinimizeBox = false;

                Label lblInfo = new Label()
                {
                    Text = $"Đổi mật khẩu cho: {username}",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                Label lblPassword = new Label()
                {
                    Text = "Mật khẩu mới:",
                    Location = new Point(20, 50),
                    AutoSize = true
                };

                TextBox txtPassword = new TextBox()
                {
                    Location = new Point(20, 75),
                    Width = 340,
                    PasswordChar = '*'
                };

                Button btnOK = new Button()
                {
                    Text = "Đổi mật khẩu",
                    Location = new Point(120, 110),
                    Width = 120,
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(76, 175, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnOK.FlatAppearance.BorderSize = 0;

                Button btnCancel = new Button()
                {
                    Text = "Hủy",
                    Location = new Point(250, 110),
                    Width = 80,
                    DialogResult = DialogResult.Cancel
                };

                promptForm.Controls.AddRange(new Control[] { lblInfo, lblPassword, txtPassword, btnOK, btnCancel });
                promptForm.AcceptButton = btnOK;

                if (promptForm.ShowDialog() == DialogResult.OK)
                {
                    string newPassword = txtPassword.Text.Trim();

                    if (string.IsNullOrEmpty(newPassword))
                    {
                        ToastNotification.Error("Mật khẩu không được để trống!");
                        return;
                    }

                    if (newPassword.Length < 6)
                    {
                        ToastNotification.Error("Mật khẩu phải có ít nhất 6 ký tự!");
                        return;
                    }

                    // Reset password (admin không cần old password)
                    if (DatabaseHelper.AdminResetPassword(userId, newPassword))
                    {
                        ToastNotification.Success("Đã đổi mật khẩu thành công!");
                    }
                    else
                    {
                        ToastNotification.Error("Đổi mật khẩu thất bại!");
                    }
                }
            }
        }

        /// <summary>
        /// Button Edit Role
        /// </summary>
        private void btnEditRole_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn user cần đổi vai trò!");
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["id"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["username"].Value.ToString();
            string currentRole = dgvUsers.SelectedRows[0].Cells["role"].Value.ToString();

            // Không cho đổi role của chính mình
            if (userId == UserSession.UserId)
            {
                ToastNotification.Warning("Bạn không thể đổi vai trò của chính mình!");
                return;
            }

            // Form chọn role mới
            using (Form promptForm = new Form())
            {
                promptForm.Text = "Đổi vai trò";
                promptForm.Width = 400;
                promptForm.Height = 200;
                promptForm.StartPosition = FormStartPosition.CenterParent;
                promptForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                promptForm.MaximizeBox = false;
                promptForm.MinimizeBox = false;

                Label lblInfo = new Label()
                {
                    Text = $"Đổi vai trò cho: {username}",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };

                Label lblCurrentRole = new Label()
                {
                    Text = $"Vai trò hiện tại: {currentRole}",
                    Location = new Point(20, 45),
                    AutoSize = true
                };

                Label lblNewRole = new Label()
                {
                    Text = "Vai trò mới:",
                    Location = new Point(20, 75),
                    AutoSize = true
                };

                ComboBox cboRole = new ComboBox()
                {
                    Location = new Point(20, 100),
                    Width = 340,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10F)
                };
                // Chế độ cá nhân: chỉ có Admin và User
                cboRole.Items.AddRange(new string[] { "Admin", "User" });
                cboRole.SelectedItem = currentRole;

                Button btnOK = new Button()
                {
                    Text = "Lưu",
                    Location = new Point(160, 140),
                    Width = 100,
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnOK.FlatAppearance.BorderSize = 0;

                Button btnCancel = new Button()
                {
                    Text = "Hủy",
                    Location = new Point(270, 140),
                    Width = 80,
                    DialogResult = DialogResult.Cancel
                };

                promptForm.Controls.AddRange(new Control[] { lblInfo, lblCurrentRole, lblNewRole, cboRole, btnOK, btnCancel });
                promptForm.AcceptButton = btnOK;

                if (promptForm.ShowDialog() == DialogResult.OK)
                {
                    string newRole = cboRole.SelectedItem.ToString();

                    if (newRole == currentRole)
                    {
                        ToastNotification.Info("Vai trò mới giống vai trò hiện tại!");
                        return;
                    }

                    DialogResult confirm = MessageBox.Show(
                        $"Bạn có chắc chắn muốn đổi vai trò của {username}\n" +
                        $"từ {currentRole} sang {newRole}?",
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        if (DatabaseHelper.UpdateUserRole(userId, newRole))
                        {
                            LoadUsers();
                            ToastNotification.Success("Đã đổi vai trò thành công!");
                        }
                        else
                        {
                            ToastNotification.Error("Đổi vai trò thất bại!");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Button Khóa/Mở khóa
        /// </summary>
        private void btnToggleActive_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn user!");
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["id"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["username"].Value.ToString();
            bool isActive = Convert.ToBoolean(dgvUsers.SelectedRows[0].Cells["is_active"].Value);

            // Không cho khóa chính mình
            if (userId == UserSession.UserId)
            {
                ToastNotification.Warning("Bạn không thể khóa tài khoản của chính mình!");
                return;
            }

            string action = isActive ? "khóa" : "mở khóa";
            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn {action} user:\n\n{username}?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (DatabaseHelper.ToggleUserActive(userId, !isActive))
                {
                    LoadUsers();
                    ToastNotification.Success($"Đã {action} user thành công!");
                }
                else
                {
                    ToastNotification.Error($"Lỗi khi {action} user!");
                }
            }
        }

        /// <summary>
        /// Button Xóa user
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn user cần xóa!");
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["id"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["username"].Value.ToString();

            // Không cho xóa chính mình
            if (userId == UserSession.UserId)
            {
                ToastNotification.Warning("Bạn không thể xóa tài khoản của chính mình!");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn XÓA VĨNH VIỄN user:\n\n{username}?\n\n" +
                "Thao tác này KHÔNG THỂ HOÀN TÁC!",
                "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (DatabaseHelper.DeleteUser(userId))
                {
                    LoadUsers();
                    ToastNotification.Success("Đã xóa user thành công!");
                }
                else
                {
                    ToastNotification.Error("Lỗi khi xóa user!");
                }
            }
        }

        /// <summary>
        /// Button Đóng
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Double-click để xem chi tiết
        /// </summary>
        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ShowUserDetails();
            }
        }

        /// <summary>
        /// Hiện thị chi tiết user
        /// </summary>
        private void ShowUserDetails()
        {
            if (dgvUsers.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = dgvUsers.SelectedRows[0];

            string info = "THÔNG TIN USER\n" +
                         "═══════════════════════════════════\n\n" +
                         $"ID: {row.Cells["id"].Value}\n" +
                         $"Username: {row.Cells["username"].Value}\n" +
                         $"Họ tên: {row.Cells["full_name"].Value}\n" +
                         $"Email: {row.Cells["email"].Value}\n" +
                         $"Vai trò: {row.Cells["role"].Value}\n" +
                         $"Trạng thái: {(Convert.ToBoolean(row.Cells["is_active"].Value) ? "Hoạt động" : "Bị khóa")}\n" +
                         $"Ngày tạo: {Convert.ToDateTime(row.Cells["created_date"].Value):dd/MM/yyyy HH:mm:ss}\n" +
                         $"Đăng nhập cuối: {(row.Cells["last_login"].Value != DBNull.Value ? Convert.ToDateTime(row.Cells["last_login"].Value).ToString("dd/MM/yyyy HH:mm:ss") : "Chưa đăng nhập")}";

            MessageBox.Show(info, "Chi tiết User",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Format cell để hiện thị màu sắc
        /// </summary>
        private void dgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                string columnName = dgvUsers.Columns[e.ColumnIndex].Name;

                // Format cột is_active
                if (columnName == "is_active")
                {
                    if (e.Value != null && e.Value != DBNull.Value)
                    {
                        bool isActive = Convert.ToBoolean(e.Value);
                        
                        // Đổi giá trị thành text
                        e.Value = isActive ? "Hoạt động" : "Bị khóa";
                        
                        // Đổi màu
                        e.CellStyle.ForeColor = isActive ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        e.FormattingApplied = true;
                    }
                }
                // Format cột role
                else if (columnName == "role")
                {
                    if (e.Value != null && e.Value != DBNull.Value)
                    {
                        string role = e.Value.ToString();
                        
                        // Chế độ cá nhân: Admin (hồng) và User (xanh)
                        if (role == "Admin")
                            e.CellStyle.ForeColor = Color.FromArgb(233, 30, 99); // Pink
                        else
                            e.CellStyle.ForeColor = Color.FromArgb(33, 150, 243); // Blue for User

                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                }
                // Format cột last_login (xử lý NULL)
                else if (columnName == "last_login")
                {
                    if (e.Value == null || e.Value == DBNull.Value)
                    {
                        e.Value = "Chưa đăng nhập";
                        e.CellStyle.ForeColor = Color.Gray;
                        e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
                        e.FormattingApplied = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw exception
                System.Diagnostics.Debug.WriteLine($"CellFormatting error: {ex.Message}");
                e.FormattingApplied = false;
            }
        }
    }
}
