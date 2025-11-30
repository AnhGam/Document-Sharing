namespace study_document_manager
{
    partial class AccountSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProfile = new System.Windows.Forms.TabPage();
            this.tabPassword = new System.Windows.Forms.TabPage();
            
            // Profile Tab Controls
            this.lblUsernameLabel = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblFullNameLabel = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblEmailLabel = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblRoleLabel = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblLoginTimeLabel = new System.Windows.Forms.Label();
            this.lblLoginTime = new System.Windows.Forms.Label();
            this.btnSaveProfile = new System.Windows.Forms.Button();
            
            // Password Tab Controls
            this.lblCurrentPasswordLabel = new System.Windows.Forms.Label();
            this.txtCurrentPassword = new System.Windows.Forms.TextBox();
            this.lblNewPasswordLabel = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmPasswordLabel = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.btnChangePassword = new System.Windows.Forms.Button();
            
            this.btnCancel = new System.Windows.Forms.Button();
            
            this.tabControl.SuspendLayout();
            this.tabProfile.SuspendLayout();
            this.tabPassword.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProfile);
            this.tabControl.Controls.Add(this.tabPassword);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(410, 300);
            this.tabControl.TabIndex = 0;
            
            // 
            // tabProfile
            // 
            this.tabProfile.BackColor = System.Drawing.Color.White;
            this.tabProfile.Controls.Add(this.lblUsernameLabel);
            this.tabProfile.Controls.Add(this.txtUsername);
            this.tabProfile.Controls.Add(this.lblFullNameLabel);
            this.tabProfile.Controls.Add(this.txtFullName);
            this.tabProfile.Controls.Add(this.lblEmailLabel);
            this.tabProfile.Controls.Add(this.txtEmail);
            this.tabProfile.Controls.Add(this.lblRoleLabel);
            this.tabProfile.Controls.Add(this.lblRole);
            this.tabProfile.Controls.Add(this.lblLoginTimeLabel);
            this.tabProfile.Controls.Add(this.lblLoginTime);
            this.tabProfile.Controls.Add(this.btnSaveProfile);
            this.tabProfile.Location = new System.Drawing.Point(4, 29);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Padding = new System.Windows.Forms.Padding(20);
            this.tabProfile.Size = new System.Drawing.Size(402, 267);
            this.tabProfile.TabIndex = 0;
            this.tabProfile.Text = "Thông tin cá nhân";
            
            // 
            // lblUsernameLabel
            // 
            this.lblUsernameLabel.AutoSize = true;
            this.lblUsernameLabel.Location = new System.Drawing.Point(20, 25);
            this.lblUsernameLabel.Name = "lblUsernameLabel";
            this.lblUsernameLabel.Size = new System.Drawing.Size(100, 20);
            this.lblUsernameLabel.TabIndex = 0;
            this.lblUsernameLabel.Text = "Tên đăng nhập:";
            
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(150, 22);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.ReadOnly = true;
            this.txtUsername.Size = new System.Drawing.Size(220, 27);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            
            // 
            // lblFullNameLabel
            // 
            this.lblFullNameLabel.AutoSize = true;
            this.lblFullNameLabel.Location = new System.Drawing.Point(20, 65);
            this.lblFullNameLabel.Name = "lblFullNameLabel";
            this.lblFullNameLabel.Size = new System.Drawing.Size(60, 20);
            this.lblFullNameLabel.TabIndex = 2;
            this.lblFullNameLabel.Text = "Họ tên:";
            
            // 
            // txtFullName
            // 
            this.txtFullName.Location = new System.Drawing.Point(150, 62);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(220, 27);
            this.txtFullName.TabIndex = 3;
            
            // 
            // lblEmailLabel
            // 
            this.lblEmailLabel.AutoSize = true;
            this.lblEmailLabel.Location = new System.Drawing.Point(20, 105);
            this.lblEmailLabel.Name = "lblEmailLabel";
            this.lblEmailLabel.Size = new System.Drawing.Size(46, 20);
            this.lblEmailLabel.TabIndex = 4;
            this.lblEmailLabel.Text = "Email:";
            
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(150, 102);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(220, 27);
            this.txtEmail.TabIndex = 5;
            
            // 
            // lblRoleLabel
            // 
            this.lblRoleLabel.AutoSize = true;
            this.lblRoleLabel.Location = new System.Drawing.Point(20, 145);
            this.lblRoleLabel.Name = "lblRoleLabel";
            this.lblRoleLabel.Size = new System.Drawing.Size(55, 20);
            this.lblRoleLabel.TabIndex = 6;
            this.lblRoleLabel.Text = "Vai trò:";
            
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.lblRole.Location = new System.Drawing.Point(150, 145);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(40, 20);
            this.lblRole.TabIndex = 7;
            this.lblRole.Text = "User";
            
            // 
            // lblLoginTimeLabel
            // 
            this.lblLoginTimeLabel.AutoSize = true;
            this.lblLoginTimeLabel.Location = new System.Drawing.Point(20, 175);
            this.lblLoginTimeLabel.Name = "lblLoginTimeLabel";
            this.lblLoginTimeLabel.Size = new System.Drawing.Size(120, 20);
            this.lblLoginTimeLabel.TabIndex = 8;
            this.lblLoginTimeLabel.Text = "Đăng nhập lúc:";
            
            // 
            // lblLoginTime
            // 
            this.lblLoginTime.AutoSize = true;
            this.lblLoginTime.ForeColor = System.Drawing.Color.Gray;
            this.lblLoginTime.Location = new System.Drawing.Point(150, 175);
            this.lblLoginTime.Name = "lblLoginTime";
            this.lblLoginTime.Size = new System.Drawing.Size(100, 20);
            this.lblLoginTime.TabIndex = 9;
            this.lblLoginTime.Text = "---";
            
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this.btnSaveProfile.FlatAppearance.BorderSize = 0;
            this.btnSaveProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveProfile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveProfile.ForeColor = System.Drawing.Color.White;
            this.btnSaveProfile.Location = new System.Drawing.Point(150, 215);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(120, 35);
            this.btnSaveProfile.TabIndex = 10;
            this.btnSaveProfile.Text = "Lưu thay đổi";
            this.btnSaveProfile.UseVisualStyleBackColor = false;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            
            // 
            // tabPassword
            // 
            this.tabPassword.BackColor = System.Drawing.Color.White;
            this.tabPassword.Controls.Add(this.lblCurrentPasswordLabel);
            this.tabPassword.Controls.Add(this.txtCurrentPassword);
            this.tabPassword.Controls.Add(this.lblNewPasswordLabel);
            this.tabPassword.Controls.Add(this.txtNewPassword);
            this.tabPassword.Controls.Add(this.lblConfirmPasswordLabel);
            this.tabPassword.Controls.Add(this.txtConfirmPassword);
            this.tabPassword.Controls.Add(this.btnChangePassword);
            this.tabPassword.Location = new System.Drawing.Point(4, 29);
            this.tabPassword.Name = "tabPassword";
            this.tabPassword.Padding = new System.Windows.Forms.Padding(20);
            this.tabPassword.Size = new System.Drawing.Size(402, 267);
            this.tabPassword.TabIndex = 1;
            this.tabPassword.Text = "Đổi mật khẩu";
            
            // 
            // lblCurrentPasswordLabel
            // 
            this.lblCurrentPasswordLabel.AutoSize = true;
            this.lblCurrentPasswordLabel.Location = new System.Drawing.Point(20, 35);
            this.lblCurrentPasswordLabel.Name = "lblCurrentPasswordLabel";
            this.lblCurrentPasswordLabel.Size = new System.Drawing.Size(120, 20);
            this.lblCurrentPasswordLabel.TabIndex = 0;
            this.lblCurrentPasswordLabel.Text = "Mật khẩu hiện tại:";
            
            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.Location = new System.Drawing.Point(150, 32);
            this.txtCurrentPassword.Name = "txtCurrentPassword";
            this.txtCurrentPassword.PasswordChar = '*';
            this.txtCurrentPassword.Size = new System.Drawing.Size(220, 27);
            this.txtCurrentPassword.TabIndex = 1;
            
            // 
            // lblNewPasswordLabel
            // 
            this.lblNewPasswordLabel.AutoSize = true;
            this.lblNewPasswordLabel.Location = new System.Drawing.Point(20, 80);
            this.lblNewPasswordLabel.Name = "lblNewPasswordLabel";
            this.lblNewPasswordLabel.Size = new System.Drawing.Size(100, 20);
            this.lblNewPasswordLabel.TabIndex = 2;
            this.lblNewPasswordLabel.Text = "Mật khẩu mới:";
            
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Location = new System.Drawing.Point(150, 77);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.Size = new System.Drawing.Size(220, 27);
            this.txtNewPassword.TabIndex = 3;
            
            // 
            // lblConfirmPasswordLabel
            // 
            this.lblConfirmPasswordLabel.AutoSize = true;
            this.lblConfirmPasswordLabel.Location = new System.Drawing.Point(20, 125);
            this.lblConfirmPasswordLabel.Name = "lblConfirmPasswordLabel";
            this.lblConfirmPasswordLabel.Size = new System.Drawing.Size(120, 20);
            this.lblConfirmPasswordLabel.TabIndex = 4;
            this.lblConfirmPasswordLabel.Text = "Xác nhận mật khẩu:";
            
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Location = new System.Drawing.Point(150, 122);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(220, 27);
            this.txtConfirmPassword.TabIndex = 5;
            
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.BackColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.btnChangePassword.FlatAppearance.BorderSize = 0;
            this.btnChangePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangePassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnChangePassword.ForeColor = System.Drawing.Color.White;
            this.btnChangePassword.Location = new System.Drawing.Point(150, 175);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(120, 35);
            this.btnChangePassword.TabIndex = 6;
            this.btnChangePassword.Text = "Đổi mật khẩu";
            this.btnChangePassword.UseVisualStyleBackColor = false;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(327, 320);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Đóng";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // 
            // AccountSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.ClientSize = new System.Drawing.Size(434, 367);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cài đặt tài khoản";
            this.Load += new System.EventHandler(this.AccountSettingsForm_Load);
            
            this.tabControl.ResumeLayout(false);
            this.tabProfile.ResumeLayout(false);
            this.tabProfile.PerformLayout();
            this.tabPassword.ResumeLayout(false);
            this.tabPassword.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabProfile;
        private System.Windows.Forms.TabPage tabPassword;
        
        private System.Windows.Forms.Label lblUsernameLabel;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblFullNameLabel;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblEmailLabel;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblRoleLabel;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblLoginTimeLabel;
        private System.Windows.Forms.Label lblLoginTime;
        private System.Windows.Forms.Button btnSaveProfile;
        
        private System.Windows.Forms.Label lblCurrentPasswordLabel;
        private System.Windows.Forms.TextBox txtCurrentPassword;
        private System.Windows.Forms.Label lblNewPasswordLabel;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblConfirmPasswordLabel;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Button btnChangePassword;
        
        private System.Windows.Forms.Button btnCancel;
    }
}
