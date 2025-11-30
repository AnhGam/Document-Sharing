namespace study_document_manager
{
    partial class AddEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ten = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbo_mon_hoc = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbo_loai = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_duong_dan = new System.Windows.Forms.TextBox();
            this.btn_chon_file = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_ghi_chu = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_tac_gia = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_kich_thuoc = new System.Windows.Forms.TextBox();
            this.chk_quan_trong = new System.Windows.Forms.CheckBox();
            this.btn_luu = new System.Windows.Forms.Button();
            this.btn_huy = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTags = new System.Windows.Forms.Label();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.lblDeadline = new System.Windows.Forms.Label();
            this.dtpDeadline = new System.Windows.Forms.DateTimePicker();
            this.chkHasDeadline = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tên tài liệu: *";
            // 
            // txt_ten
            // 
            this.txt_ten.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txt_ten.Location = new System.Drawing.Point(20, 40);
            this.txt_ten.Name = "txt_ten";
            this.txt_ten.Size = new System.Drawing.Size(360, 23);
            this.txt_ten.TabIndex = 1;
            this.txt_ten.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_ten_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(20, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Danh mục:";
            // 
            // cbo_mon_hoc
            // 
            this.cbo_mon_hoc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbo_mon_hoc.FormattingEnabled = true;
            this.cbo_mon_hoc.Location = new System.Drawing.Point(20, 95);
            this.cbo_mon_hoc.Name = "cbo_mon_hoc";
            this.cbo_mon_hoc.Size = new System.Drawing.Size(360, 23);
            this.cbo_mon_hoc.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(20, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Loại tài liệu:";
            // 
            // cbo_loai
            // 
            this.cbo_loai.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbo_loai.FormattingEnabled = true;
            this.cbo_loai.Location = new System.Drawing.Point(20, 150);
            this.cbo_loai.Name = "cbo_loai";
            this.cbo_loai.Size = new System.Drawing.Size(360, 23);
            this.cbo_loai.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.Location = new System.Drawing.Point(20, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Nội dung file *";
            // 
            // txt_duong_dan
            // 
            this.txt_duong_dan.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txt_duong_dan.Location = new System.Drawing.Point(20, 205);
            this.txt_duong_dan.Name = "txt_duong_dan";
            this.txt_duong_dan.ReadOnly = true;
            this.txt_duong_dan.Size = new System.Drawing.Size(250, 23);
            this.txt_duong_dan.TabIndex = 7;
            // 
            // btn_chon_file
            // 
            this.btn_chon_file.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btn_chon_file.FlatAppearance.BorderSize = 0;
            this.btn_chon_file.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_chon_file.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_chon_file.ForeColor = System.Drawing.Color.White;
            this.btn_chon_file.Location = new System.Drawing.Point(280, 205);
            this.btn_chon_file.Name = "btn_chon_file";
            this.btn_chon_file.Size = new System.Drawing.Size(100, 23);
            this.btn_chon_file.TabIndex = 8;
            this.btn_chon_file.Text = "Chọn file...";
            this.btn_chon_file.UseVisualStyleBackColor = false;
            this.btn_chon_file.Click += new System.EventHandler(this.btn_chon_file_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.Location = new System.Drawing.Point(20, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Ghi chú:";
            // 
            // txt_ghi_chu
            // 
            this.txt_ghi_chu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txt_ghi_chu.Location = new System.Drawing.Point(20, 260);
            this.txt_ghi_chu.Multiline = true;
            this.txt_ghi_chu.Name = "txt_ghi_chu";
            this.txt_ghi_chu.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_ghi_chu.Size = new System.Drawing.Size(360, 80);
            this.txt_ghi_chu.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.Location = new System.Drawing.Point(20, 355);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Tác giả:";
            // 
            // txt_tac_gia
            // 
            this.txt_tac_gia.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txt_tac_gia.Location = new System.Drawing.Point(20, 375);
            this.txt_tac_gia.Name = "txt_tac_gia";
            this.txt_tac_gia.Size = new System.Drawing.Size(250, 23);
            this.txt_tac_gia.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.Location = new System.Drawing.Point(200, 465);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Kích thước (MB):";
            // 
            // txt_kich_thuoc
            // 
            this.txt_kich_thuoc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txt_kich_thuoc.Location = new System.Drawing.Point(200, 485);
            this.txt_kich_thuoc.Name = "txt_kich_thuoc";
            this.txt_kich_thuoc.ReadOnly = true;
            this.txt_kich_thuoc.Size = new System.Drawing.Size(80, 23);
            this.txt_kich_thuoc.TabIndex = 14;
            // 
            // chk_quan_trong
            // 
            this.chk_quan_trong.AutoSize = true;
            this.chk_quan_trong.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chk_quan_trong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(202)))), ((int)(((byte)(40)))));
            this.chk_quan_trong.Location = new System.Drawing.Point(290, 487);
            this.chk_quan_trong.Name = "chk_quan_trong";
            this.chk_quan_trong.Size = new System.Drawing.Size(103, 19);
            this.chk_quan_trong.TabIndex = 15;
            this.chk_quan_trong.Text = "★ Quan trọng";
            this.chk_quan_trong.UseVisualStyleBackColor = true;
            // 
            // btn_luu
            // 
            this.btn_luu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btn_luu.FlatAppearance.BorderSize = 0;
            this.btn_luu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_luu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_luu.ForeColor = System.Drawing.Color.White;
            this.btn_luu.Location = new System.Drawing.Point(180, 530);
            this.btn_luu.Name = "btn_luu";
            this.btn_luu.Size = new System.Drawing.Size(100, 35);
            this.btn_luu.TabIndex = 24;
            this.btn_luu.Text = "Lưu";
            this.btn_luu.UseVisualStyleBackColor = false;
            this.btn_luu.Click += new System.EventHandler(this.btn_luu_Click);
            // 
            // btn_huy
            // 
            this.btn_huy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btn_huy.FlatAppearance.BorderSize = 0;
            this.btn_huy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_huy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_huy.ForeColor = System.Drawing.Color.White;
            this.btn_huy.Location = new System.Drawing.Point(290, 530);
            this.btn_huy.Name = "btn_huy";
            this.btn_huy.Size = new System.Drawing.Size(90, 35);
            this.btn_huy.TabIndex = 25;
            this.btn_huy.Text = "Hủy";
            this.btn_huy.UseVisualStyleBackColor = false;
            this.btn_huy.Click += new System.EventHandler(this.btn_huy_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Italic);
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(20, 575);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 12);
            this.label8.TabIndex = 26;
            this.label8.Text = "* Trường bắt buộc nhập";
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTags.Location = new System.Drawing.Point(20, 410);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(34, 15);
            this.lblTags.TabIndex = 19;
            this.lblTags.Text = "Tags:";
            // 
            // txtTags
            // 
            this.txtTags.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTags.Location = new System.Drawing.Point(20, 430);
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(360, 23);
            this.txtTags.TabIndex = 20;
            // 
            // lblDeadline
            // 
            this.lblDeadline.AutoSize = true;
            this.lblDeadline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDeadline.Location = new System.Drawing.Point(20, 465);
            this.lblDeadline.Name = "lblDeadline";
            this.lblDeadline.Size = new System.Drawing.Size(59, 15);
            this.lblDeadline.TabIndex = 21;
            this.lblDeadline.Text = "Hạn chót:";
            // 
            // dtpDeadline
            // 
            this.dtpDeadline.Enabled = false;
            this.dtpDeadline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpDeadline.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDeadline.Location = new System.Drawing.Point(20, 485);
            this.dtpDeadline.Name = "dtpDeadline";
            this.dtpDeadline.Size = new System.Drawing.Size(150, 23);
            this.dtpDeadline.TabIndex = 23;
            // 
            // chkHasDeadline
            // 
            this.chkHasDeadline.AutoSize = true;
            this.chkHasDeadline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkHasDeadline.Location = new System.Drawing.Point(90, 464);
            this.chkHasDeadline.Name = "chkHasDeadline";
            this.chkHasDeadline.Size = new System.Drawing.Size(64, 19);
            this.chkHasDeadline.TabIndex = 22;
            this.chkHasDeadline.Text = "Có hạn";
            this.chkHasDeadline.UseVisualStyleBackColor = true;
            // 
            // AddEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 600);
            this.Controls.Add(this.dtpDeadline);
            this.Controls.Add(this.chkHasDeadline);
            this.Controls.Add(this.lblDeadline);
            this.Controls.Add(this.txtTags);
            this.Controls.Add(this.lblTags);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btn_huy);
            this.Controls.Add(this.btn_luu);
            this.Controls.Add(this.chk_quan_trong);
            this.Controls.Add(this.txt_kich_thuoc);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txt_tac_gia);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt_ghi_chu);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btn_chon_file);
            this.Controls.Add(this.txt_duong_dan);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbo_loai);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbo_mon_hoc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_ten);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm/Sửa tài liệu";
            this.Load += new System.EventHandler(this.AddEditForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Đổi từ private -> internal để Form1 có thể truy cập khi drag-drop
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txt_ten;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ComboBox cbo_mon_hoc;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.ComboBox cbo_loai;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.TextBox txt_duong_dan;
        private System.Windows.Forms.Button btn_chon_file;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txt_ghi_chu;
        private System.Windows.Forms.Label label6;
        internal System.Windows.Forms.TextBox txt_tac_gia;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.TextBox txt_kich_thuoc;
        private System.Windows.Forms.CheckBox chk_quan_trong;
        private System.Windows.Forms.Button btn_luu;
        private System.Windows.Forms.Button btn_huy;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblTags;
        internal System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.Label lblDeadline;
        internal System.Windows.Forms.DateTimePicker dtpDeadline;
        internal System.Windows.Forms.CheckBox chkHasDeadline;
    }
}
