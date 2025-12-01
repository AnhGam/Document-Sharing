using study_document_manager.UI;

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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
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
            
            this.pnlHeader.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabProfile.SuspendLayout();
            this.tabPassword.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(253)))), ((int)(((byte)(250)))));
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(480, 80);
            this.pnlHeader.TabIndex = 0;
            
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTitle.Location = new System.Drawing.Point(24, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Cài đặt tài khoản";
            
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblSubtitle.Location = new System.Drawing.Point(26, 53);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(260, 20);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Quản lý thông tin cá nhân và mật khẩu";
            
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProfile);
            this.tabControl.Controls.Add(this.tabPassword);
            this.tabControl.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.tabControl.Location = new System.Drawing.Point(24, 96);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(432, 320);
            this.tabControl.TabIndex = 1;
            
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
            this.tabProfile.Location = new System.Drawing.Point(4, 31);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Padding = new System.Windows.Forms.Padding(24);
            this.tabProfile.Size = new System.Drawing.Size(424, 285);
            this.tabProfile.TabIndex = 0;
            this.tabProfile.Text = "Thông tin cá nhân";
            
            // 
            // lblUsernameLabel
            // 
            this.lblUsernameLabel.AutoSize = true;
            this.lblUsernameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblUsernameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblUsernameLabel.Location = new System.Drawing.Point(24, 24);
            this.lblUsernameLabel.Name = "lblUsernameLabel";
            this.lblUsernameLabel.Size = new System.Drawing.Size(100, 20);
            this.lblUsernameLabel.TabIndex = 0;
            this.lblUsernameLabel.Text = "Tên đăng nhập";
            
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.txtUsername.Location = new System.Drawing.Point(24, 47);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.ReadOnly = true;
            this.txtUsername.Size = new System.Drawing.Size(376, 30);
            this.txtUsername.TabIndex = 1;
            
            // 
            // lblFullNameLabel
            // 
            this.lblFullNameLabel.AutoSize = true;
            this.lblFullNameLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblFullNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblFullNameLabel.Location = new System.Drawing.Point(24, 88);
            this.lblFullNameLabel.Name = "lblFullNameLabel";
            this.lblFullNameLabel.Size = new System.Drawing.Size(55, 20);
            this.lblFullNameLabel.TabIndex = 2;
            this.lblFullNameLabel.Text = "Họ tên";
            
            // 
            // txtFullName
            // 
            this.txtFullName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtFullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFullName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtFullName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtFullName.Location = new System.Drawing.Point(24, 111);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(376, 30);
            this.txtFullName.TabIndex = 3;
            
            // 
            // lblEmailLabel
            // 
            this.lblEmailLabel.AutoSize = true;
            this.lblEmailLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblEmailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblEmailLabel.Location = new System.Drawing.Point(24, 152);
            this.lblEmailLabel.Name = "lblEmailLabel";
            this.lblEmailLabel.Size = new System.Drawing.Size(42, 20);
            this.lblEmailLabel.TabIndex = 4;
            this.lblEmailLabel.Text = "Email";
            
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtEmail.Location = new System.Drawing.Point(24, 175);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(376, 30);
            this.txtEmail.TabIndex = 5;
            
            // 
            // lblRoleLabel
            // 
            this.lblRoleLabel.AutoSize = true;
            this.lblRoleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRoleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblRoleLabel.Location = new System.Drawing.Point(24, 218);
            this.lblRoleLabel.Name = "lblRoleLabel";
            this.lblRoleLabel.Size = new System.Drawing.Size(51, 20);
            this.lblRoleLabel.TabIndex = 6;
            this.lblRoleLabel.Text = "Vai trò:";
            
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.lblRole.Location = new System.Drawing.Point(80, 218);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(37, 20);
            this.lblRole.TabIndex = 7;
            this.lblRole.Text = "User";
            
            // 
            // lblLoginTimeLabel
            // 
            this.lblLoginTimeLabel.AutoSize = true;
            this.lblLoginTimeLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLoginTimeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblLoginTimeLabel.Location = new System.Drawing.Point(200, 218);
            this.lblLoginTimeLabel.Name = "lblLoginTimeLabel";
            this.lblLoginTimeLabel.Size = new System.Drawing.Size(101, 20);
            this.lblLoginTimeLabel.TabIndex = 8;
            this.lblLoginTimeLabel.Text = "Đăng nhập lúc:";
            
            // 
            // lblLoginTime
            // 
            this.lblLoginTime.AutoSize = true;
            this.lblLoginTime.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLoginTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblLoginTime.Location = new System.Drawing.Point(307, 218);
            this.lblLoginTime.Name = "lblLoginTime";
            this.lblLoginTime.Size = new System.Drawing.Size(25, 20);
            this.lblLoginTime.TabIndex = 9;
            this.lblLoginTime.Text = "---";
            
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.btnSaveProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveProfile.FlatAppearance.BorderSize = 0;
            this.btnSaveProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveProfile.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveProfile.ForeColor = System.Drawing.Color.White;
            this.btnSaveProfile.Location = new System.Drawing.Point(24, 248);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(140, 40);
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
            this.tabPassword.Location = new System.Drawing.Point(4, 31);
            this.tabPassword.Name = "tabPassword";
            this.tabPassword.Padding = new System.Windows.Forms.Padding(24);
            this.tabPassword.Size = new System.Drawing.Size(424, 285);
            this.tabPassword.TabIndex = 1;
            this.tabPassword.Text = "Đổi mật khẩu";
            
            // 
            // lblCurrentPasswordLabel
            // 
            this.lblCurrentPasswordLabel.AutoSize = true;
            this.lblCurrentPasswordLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblCurrentPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblCurrentPasswordLabel.Location = new System.Drawing.Point(24, 24);
            this.lblCurrentPasswordLabel.Name = "lblCurrentPasswordLabel";
            this.lblCurrentPasswordLabel.Size = new System.Drawing.Size(124, 20);
            this.lblCurrentPasswordLabel.TabIndex = 0;
            this.lblCurrentPasswordLabel.Text = "Mật khẩu hiện tại";
            
            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtCurrentPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCurrentPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCurrentPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtCurrentPassword.Location = new System.Drawing.Point(24, 47);
            this.txtCurrentPassword.Name = "txtCurrentPassword";
            this.txtCurrentPassword.PasswordChar = '*';
            this.txtCurrentPassword.Size = new System.Drawing.Size(376, 30);
            this.txtCurrentPassword.TabIndex = 1;
            
            // 
            // lblNewPasswordLabel
            // 
            this.lblNewPasswordLabel.AutoSize = true;
            this.lblNewPasswordLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblNewPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNewPasswordLabel.Location = new System.Drawing.Point(24, 92);
            this.lblNewPasswordLabel.Name = "lblNewPasswordLabel";
            this.lblNewPasswordLabel.Size = new System.Drawing.Size(100, 20);
            this.lblNewPasswordLabel.TabIndex = 2;
            this.lblNewPasswordLabel.Text = "Mật khẩu mới";
            
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtNewPassword.Location = new System.Drawing.Point(24, 115);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.Size = new System.Drawing.Size(376, 30);
            this.txtNewPassword.TabIndex = 3;
            
            // 
            // lblConfirmPasswordLabel
            // 
            this.lblConfirmPasswordLabel.AutoSize = true;
            this.lblConfirmPasswordLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblConfirmPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblConfirmPasswordLabel.Location = new System.Drawing.Point(24, 160);
            this.lblConfirmPasswordLabel.Name = "lblConfirmPasswordLabel";
            this.lblConfirmPasswordLabel.Size = new System.Drawing.Size(134, 20);
            this.lblConfirmPasswordLabel.TabIndex = 4;
            this.lblConfirmPasswordLabel.Text = "Xác nhận mật khẩu";
            
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtConfirmPassword.Location = new System.Drawing.Point(24, 183);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(376, 30);
            this.txtConfirmPassword.TabIndex = 5;
            
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.btnChangePassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangePassword.FlatAppearance.BorderSize = 0;
            this.btnChangePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangePassword.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnChangePassword.ForeColor = System.Drawing.Color.White;
            this.btnChangePassword.Location = new System.Drawing.Point(24, 230);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(140, 40);
            this.btnChangePassword.TabIndex = 6;
            this.btnChangePassword.Text = "Đổi mật khẩu";
            this.btnChangePassword.UseVisualStyleBackColor = false;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnCancel.Location = new System.Drawing.Point(356, 428);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Đóng";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // 
            // AccountSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(480, 488);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cài đặt tài khoản - Study Document Manager";
            this.Load += new System.EventHandler(this.AccountSettingsForm_Load);
            
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabProfile.ResumeLayout(false);
            this.tabProfile.PerformLayout();
            this.tabPassword.ResumeLayout(false);
            this.tabPassword.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
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
