using System.Windows.Forms;

namespace study_document_manager.Documents
{
    public partial class AddEditForm : global::System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditForm));
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTen = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboDanhMuc = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboDinhDang = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.filePanel = new System.Windows.Forms.Panel();
            this.txtDuongDan = new System.Windows.Forms.TextBox();
            this.btnChonFile = new study_document_manager.UI.Controls.ModernButton();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGhiChu = new System.Windows.Forms.TextBox();
            this.sizePanel = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.txtKichThuoc = new System.Windows.Forms.TextBox();
            this.lblTags = new System.Windows.Forms.Label();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.chkQuanTrong = new System.Windows.Forms.CheckBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnLuu = new study_document_manager.UI.Controls.ModernButton();
            this.btnHuy = new study_document_manager.UI.Controls.ModernButton();
            this.label8 = new System.Windows.Forms.Label();
            this.mainTableLayout.SuspendLayout();
            this.filePanel.SuspendLayout();
            this.sizePanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.AutoScroll = true;
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.label1, 0, 0);
            this.mainTableLayout.Controls.Add(this.txtTen, 0, 1);
            this.mainTableLayout.Controls.Add(this.label2, 0, 2);
            this.mainTableLayout.Controls.Add(this.cboDanhMuc, 0, 3);
            this.mainTableLayout.Controls.Add(this.label3, 0, 4);
            this.mainTableLayout.Controls.Add(this.cboDinhDang, 0, 5);
            this.mainTableLayout.Controls.Add(this.label4, 0, 6);
            this.mainTableLayout.Controls.Add(this.filePanel, 0, 7);
            this.mainTableLayout.Controls.Add(this.label5, 0, 8);
            this.mainTableLayout.Controls.Add(this.txtGhiChu, 0, 9);
            this.mainTableLayout.Controls.Add(this.sizePanel, 0, 10);
            this.mainTableLayout.Controls.Add(this.lblTags, 0, 11);
            this.mainTableLayout.Controls.Add(this.txtTags, 0, 12);
            this.mainTableLayout.Controls.Add(this.chkQuanTrong, 0, 14);
            this.mainTableLayout.Controls.Add(this.buttonPanel, 0, 15);
            this.mainTableLayout.Controls.Add(this.label8, 0, 16);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.Padding = new System.Windows.Forms.Padding(30, 24, 30, 24);
            this.mainTableLayout.RowCount = 17;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.Size = new System.Drawing.Size(675, 975);
            this.mainTableLayout.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label1.Location = new System.Drawing.Point(34, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tên tài liệu: *";
            // 
            // txt_ten
            // 
            this.txtTen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTen.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTen.Location = new System.Drawing.Point(34, 65);
            this.txtTen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.txtTen.Name = "txtTen";
            this.txtTen.Size = new System.Drawing.Size(607, 34);
            this.txtTen.TabIndex = 1;
            this.txtTen.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtTenKeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label2.Location = new System.Drawing.Point(34, 117);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Danh mục:";
            // 
            // cboDanhMuc
            // 
            this.cboDanhMuc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboDanhMuc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDanhMuc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboDanhMuc.FormattingEnabled = true;
            this.cboDanhMuc.Location = new System.Drawing.Point(34, 152);
            this.cboDanhMuc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.cboDanhMuc.Name = "cboDanhMuc";
            this.cboDanhMuc.Size = new System.Drawing.Size(607, 36);
            this.cboDanhMuc.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label3.Location = new System.Drawing.Point(34, 206);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Định dạng:";
            // 
            // cboDinhDang
            // 
            this.cboDinhDang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboDinhDang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDinhDang.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboDinhDang.FormattingEnabled = true;
            this.cboDinhDang.Location = new System.Drawing.Point(34, 241);
            this.cboDinhDang.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.cboDinhDang.Name = "cboDinhDang";
            this.cboDinhDang.Size = new System.Drawing.Size(607, 36);
            this.cboDinhDang.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label4.Location = new System.Drawing.Point(34, 295);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 25);
            this.label4.TabIndex = 6;
            this.label4.Text = "Nội dung file *";
            // 
            // filePanel
            // 
            this.filePanel.Controls.Add(this.txtDuongDan);
            this.filePanel.Controls.Add(this.btnChonFile);
            this.filePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filePanel.Location = new System.Drawing.Point(30, 326);
            this.filePanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.filePanel.Name = "filePanel";
            this.filePanel.Size = new System.Drawing.Size(615, 48);
            this.filePanel.TabIndex = 7;
            // 
            // txt_duong_dan
            // 
            this.txtDuongDan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDuongDan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDuongDan.Location = new System.Drawing.Point(0, 4);
            this.txtDuongDan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDuongDan.Name = "txtDuongDan";
            this.txtDuongDan.ReadOnly = true;
            this.txtDuongDan.Size = new System.Drawing.Size(442, 34);
            this.txtDuongDan.TabIndex = 0;
            // 
            // btn_chon_file
            // 
            this.btnChonFile.Location = new System.Drawing.Point(453, 0);
            this.btnChonFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnChonFile.Name = "btnChonFile";
            this.btnChonFile.Size = new System.Drawing.Size(162, 48);
            this.btnChonFile.TabIndex = 1;
            this.btnChonFile.Text = "Chọn file...";
            this.btnChonFile.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnChonFile.UseVisualStyleBackColor = false;
            this.btnChonFile.Variant = study_document_manager.UI.Controls.ModernButton.ButtonVariant.Primary;
            this.btnChonFile.Click += new System.EventHandler(this.BtnChonFileClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label5.Location = new System.Drawing.Point(34, 392);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 25);
            this.label5.TabIndex = 8;
            this.label5.Text = "Ghi chú:";
            // 
            // txt_ghi_chu
            // 
            this.txtGhiChu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGhiChu.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtGhiChu.Location = new System.Drawing.Point(34, 427);
            this.txtGhiChu.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.txtGhiChu.Multiline = true;
            this.txtGhiChu.Name = "txtGhiChu";
            this.txtGhiChu.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGhiChu.Size = new System.Drawing.Size(607, 134);
            this.txtGhiChu.TabIndex = 9;
            // 
            // sizePanel
            // 
            this.sizePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.sizePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.sizePanel.Controls.Add(this.label7, 0, 0);
            this.sizePanel.Controls.Add(this.txtKichThuoc, 1, 0);
            this.sizePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sizePanel.Location = new System.Drawing.Point(30, 573);
            this.sizePanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.sizePanel.Name = "sizePanel";
            this.sizePanel.RowCount = 1;
            this.sizePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.sizePanel.Size = new System.Drawing.Size(615, 45);
            this.sizePanel.TabIndex = 10;

            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label7.Location = new System.Drawing.Point(339, 10);
            this.label7.Margin = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(147, 25);
            this.label7.TabIndex = 2;
            this.label7.Text = "Kích thước (MB):";
            // 
            // txt_kich_thuoc
            // 
            this.txtKichThuoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKichThuoc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtKichThuoc.Location = new System.Drawing.Point(499, 4);
            this.txtKichThuoc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtKichThuoc.Name = "txtKichThuoc";
            this.txtKichThuoc.ReadOnly = true;
            this.txtKichThuoc.Size = new System.Drawing.Size(112, 34);
            this.txtKichThuoc.TabIndex = 3;
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblTags.Location = new System.Drawing.Point(34, 636);
            this.lblTags.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(82, 25);
            this.lblTags.TabIndex = 11;
            this.lblTags.Text = "Thẻ gắn:";
            // 
            // txtTags
            // 
            this.txtTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTags.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTags.Location = new System.Drawing.Point(34, 671);
            this.txtTags.Margin = new System.Windows.Forms.Padding(4, 4, 4, 12);
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(607, 34);
            this.txtTags.TabIndex = 12;
            // 

            // 
            // chk_quan_trong
            // 
            this.chkQuanTrong.AutoSize = true;
            this.chkQuanTrong.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.chkQuanTrong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.chkQuanTrong.Location = new System.Drawing.Point(34, 780);
            this.chkQuanTrong.Margin = new System.Windows.Forms.Padding(4, 6, 4, 12);
            this.chkQuanTrong.Name = "chkQuanTrong";
            this.chkQuanTrong.Size = new System.Drawing.Size(158, 29);
            this.chkQuanTrong.TabIndex = 14;
            this.chkQuanTrong.Text = "★ Quan trọng";
            this.chkQuanTrong.UseVisualStyleBackColor = true;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.btnLuu);
            this.buttonPanel.Controls.Add(this.btnHuy);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Location = new System.Drawing.Point(30, 821);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(615, 66);
            this.buttonPanel.TabIndex = 15;
            // 
            // btn_luu
            // 
            this.btnLuu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnLuu.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnLuu.BorderColor = System.Drawing.Color.Transparent;
            this.btnLuu.BorderRadius = 8;
            this.btnLuu.BorderSize = 0;
            this.btnLuu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLuu.FlatAppearance.BorderSize = 0;
            this.btnLuu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLuu.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.Location = new System.Drawing.Point(443, 4);
            this.btnLuu.Margin = new System.Windows.Forms.Padding(12, 4, 4, 4);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(168, 57);
            this.btnLuu.TabIndex = 0;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnLuu.UseVisualStyleBackColor = false;
            this.btnLuu.Variant = study_document_manager.UI.Controls.ModernButton.ButtonVariant.Primary;
            this.btnLuu.Click += new System.EventHandler(this.BtnLuuClick);
            // 
            // btnHuy
            // 
            this.btnHuy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.btnHuy.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnHuy.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnHuy.BorderRadius = 8;
            this.btnHuy.BorderSize = 1;
            this.btnHuy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHuy.FlatAppearance.BorderSize = 0;
            this.btnHuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHuy.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnHuy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnHuy.Location = new System.Drawing.Point(277, 4);
            this.btnHuy.Margin = new System.Windows.Forms.Padding(12, 4, 4, 4);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(150, 57);
            this.btnHuy.TabIndex = 1;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnHuy.UseVisualStyleBackColor = false;
            this.btnHuy.Variant = study_document_manager.UI.Controls.ModernButton.ButtonVariant.Secondary;
            this.btnHuy.Click += new System.EventHandler(this.BtnHuyClick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label8.Location = new System.Drawing.Point(34, 899);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(176, 21);
            this.label8.TabIndex = 16;
            this.label8.Text = "* Trường bắt buộc nhập";
            // 
            // AddEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(675, 975);
            this.Controls.Add(this.mainTableLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(589, 797);
            this.Name = "AddEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm/Sửa tài liệu";
            this.Load += new System.EventHandler(this.AddEditFormLoad);
            this.mainTableLayout.ResumeLayout(false);
            this.mainTableLayout.PerformLayout();
            this.filePanel.ResumeLayout(false);
            this.filePanel.PerformLayout();
            this.sizePanel.ResumeLayout(false);
            this.sizePanel.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txtTen;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ComboBox cboDanhMuc;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.ComboBox cboDinhDang;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel filePanel;
        internal System.Windows.Forms.TextBox txtDuongDan;
        private study_document_manager.UI.Controls.ModernButton btnChonFile;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txtGhiChu;
        private System.Windows.Forms.TableLayoutPanel sizePanel;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.TextBox txtKichThuoc;
        private System.Windows.Forms.Label lblTags;
        internal System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.CheckBox chkQuanTrong;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private study_document_manager.UI.Controls.ModernButton btnLuu;
        private study_document_manager.UI.Controls.ModernButton btnHuy;
        private System.Windows.Forms.Label label8;
    }
}

