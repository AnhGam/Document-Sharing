namespace study_document_manager
{
    partial class Form1
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuEditRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolBtnNew = new System.Windows.Forms.ToolStripButton();
            this.toolBtnEdit = new System.Windows.Forms.ToolStripButton();
            this.toolBtnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnOpen = new System.Windows.Forms.ToolStripButton();
            this.toolBtnExport = new System.Windows.Forms.ToolStripButton();
            this.toolSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnRefresh = new System.Windows.Forms.ToolStripButton();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSubject = new System.Windows.Forms.Label();
            this.cboSubject = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.dgvDocuments = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.White;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuView,
            this.menuHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileNew,
            this.menuFileOpen,
            this.menuFileSeparator1,
            this.menuFileExport,
            this.menuFileSeparator2,
            this.menuFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(55, 20);
            this.menuFile.Text = "Tệp tin";
            // 
            // menuFileNew
            // 
            this.menuFileNew.Name = "menuFileNew";
            this.menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuFileNew.Size = new System.Drawing.Size(209, 22);
            this.menuFileNew.Text = "Thêm tài liệu mới";
            this.menuFileNew.Click += new System.EventHandler(this.btn_them_Click);
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFileOpen.Size = new System.Drawing.Size(209, 22);
            this.menuFileOpen.Text = "Mở tài liệu";
            this.menuFileOpen.Click += new System.EventHandler(this.btn_mo_file_Click);
            // 
            // menuFileSeparator1
            // 
            this.menuFileSeparator1.Name = "menuFileSeparator1";
            this.menuFileSeparator1.Size = new System.Drawing.Size(206, 6);
            // 
            // menuFileExport
            // 
            this.menuFileExport.Name = "menuFileExport";
            this.menuFileExport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.menuFileExport.Size = new System.Drawing.Size(209, 22);
            this.menuFileExport.Text = "Xuất dữ liệu";
            this.menuFileExport.Click += new System.EventHandler(this.btn_xuat_Click);
            // 
            // menuFileSeparator2
            // 
            this.menuFileSeparator2.Name = "menuFileSeparator2";
            this.menuFileSeparator2.Size = new System.Drawing.Size(206, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.menuFileExit.Size = new System.Drawing.Size(209, 22);
            this.menuFileExit.Text = "Thoát";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditUpdate,
            this.menuEditDelete,
            this.menuEditSeparator,
            this.menuEditRefresh});
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(72, 20);
            this.menuEdit.Text = "Chỉnh sửa";
            // 
            // menuEditUpdate
            // 
            this.menuEditUpdate.Name = "menuEditUpdate";
            this.menuEditUpdate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.menuEditUpdate.Size = new System.Drawing.Size(197, 22);
            this.menuEditUpdate.Text = "Sửa tài liệu";
            this.menuEditUpdate.Click += new System.EventHandler(this.btn_sua_Click);
            // 
            // menuEditDelete
            // 
            this.menuEditDelete.Name = "menuEditDelete";
            this.menuEditDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.menuEditDelete.Size = new System.Drawing.Size(197, 22);
            this.menuEditDelete.Text = "Xóa tài liệu";
            this.menuEditDelete.Click += new System.EventHandler(this.btn_xoa_Click);
            // 
            // menuEditSeparator
            // 
            this.menuEditSeparator.Name = "menuEditSeparator";
            this.menuEditSeparator.Size = new System.Drawing.Size(194, 6);
            // 
            // menuEditRefresh
            // 
            this.menuEditRefresh.Name = "menuEditRefresh";
            this.menuEditRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.menuEditRefresh.Size = new System.Drawing.Size(197, 22);
            this.menuEditRefresh.Text = "Làm mới danh sách";
            this.menuEditRefresh.Click += new System.EventHandler(this.btn_lam_moi_Click);
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewStatistics});
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(61, 20);
            this.menuView.Text = "Hiển thị";
            // 
            // menuViewStatistics
            // 
            this.menuViewStatistics.Name = "menuViewStatistics";
            this.menuViewStatistics.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuViewStatistics.Size = new System.Drawing.Size(180, 22);
            this.menuViewStatistics.Text = "Thống kê";
            this.menuViewStatistics.Click += new System.EventHandler(this.btn_thong_ke_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(62, 20);
            this.menuHelp.Text = "Trợ giúp";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(180, 22);
            this.menuHelpAbout.Text = "Giới thiệu";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBtnNew,
            this.toolBtnEdit,
            this.toolBtnDelete,
            this.toolSeparator1,
            this.toolBtnOpen,
            this.toolBtnExport,
            this.toolSeparator2,
            this.toolBtnRefresh});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(8, 4, 1, 4);
            this.toolStrip.Size = new System.Drawing.Size(1200, 31);
            this.toolStrip.TabIndex = 1;
            // 
            // toolBtnNew
            // 
            this.toolBtnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolBtnNew.Name = "toolBtnNew";
            this.toolBtnNew.Size = new System.Drawing.Size(43, 20);
            this.toolBtnNew.Text = "Thêm";
            this.toolBtnNew.Click += new System.EventHandler(this.btn_them_Click);
            // 
            // toolBtnEdit
            // 
            this.toolBtnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnEdit.Name = "toolBtnEdit";
            this.toolBtnEdit.Size = new System.Drawing.Size(30, 20);
            this.toolBtnEdit.Text = "Sửa";
            this.toolBtnEdit.Click += new System.EventHandler(this.btn_sua_Click);
            // 
            // toolBtnDelete
            // 
            this.toolBtnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnDelete.Name = "toolBtnDelete";
            this.toolBtnDelete.Size = new System.Drawing.Size(31, 20);
            this.toolBtnDelete.Text = "Xóa";
            this.toolBtnDelete.Click += new System.EventHandler(this.btn_xoa_Click);
            // 
            // toolSeparator1
            // 
            this.toolSeparator1.Name = "toolSeparator1";
            this.toolSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // toolBtnOpen
            // 
            this.toolBtnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnOpen.Name = "toolBtnOpen";
            this.toolBtnOpen.Size = new System.Drawing.Size(48, 20);
            this.toolBtnOpen.Text = "Mở file";
            this.toolBtnOpen.Click += new System.EventHandler(this.btn_mo_file_Click);
            // 
            // toolBtnExport
            // 
            this.toolBtnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnExport.Name = "toolBtnExport";
            this.toolBtnExport.Size = new System.Drawing.Size(35, 20);
            this.toolBtnExport.Text = "Xuất";
            this.toolBtnExport.Click += new System.EventHandler(this.btn_xuat_Click);
            // 
            // toolSeparator2
            // 
            this.toolSeparator2.Name = "toolSeparator2";
            this.toolSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // toolBtnRefresh
            // 
            this.toolBtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnRefresh.Name = "toolBtnRefresh";
            this.toolBtnRefresh.Size = new System.Drawing.Size(58, 20);
            this.toolBtnRefresh.Text = "Làm mới";
            this.toolBtnRefresh.Click += new System.EventHandler(this.btn_lam_moi_Click);
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblSubject);
            this.pnlSearch.Controls.Add(this.cboSubject);
            this.pnlSearch.Controls.Add(this.lblType);
            this.pnlSearch.Controls.Add(this.cboType);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 55);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlSearch.Size = new System.Drawing.Size(1200, 56);
            this.pnlSearch.TabIndex = 2;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSearch.Location = new System.Drawing.Point(12, 19);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(59, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm kiếm:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtSearch.Location = new System.Drawing.Point(80, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(500, 25);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_tim_kiem_KeyPress);
            // 
            // lblSubject
            // 
            this.lblSubject.AutoSize = true;
            this.lblSubject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubject.Location = new System.Drawing.Point(600, 19);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(58, 15);
            this.lblSubject.TabIndex = 2;
            this.lblSubject.Text = "Môn học:";
            // 
            // cboSubject
            // 
            this.cboSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboSubject.FormattingEnabled = true;
            this.cboSubject.Location = new System.Drawing.Point(664, 16);
            this.cboSubject.Name = "cboSubject";
            this.cboSubject.Size = new System.Drawing.Size(200, 23);
            this.cboSubject.TabIndex = 3;
            this.cboSubject.SelectedIndexChanged += new System.EventHandler(this.cbo_mon_hoc_SelectedIndexChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblType.Location = new System.Drawing.Point(884, 19);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(32, 15);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "Loại:";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(923, 16);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(200, 23);
            this.cboType.TabIndex = 5;
            this.cboType.SelectedIndexChanged += new System.EventHandler(this.cbo_loai_SelectedIndexChanged);
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlContent.Controls.Add(this.dgvDocuments);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 111);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlContent.Size = new System.Drawing.Size(1200, 539);
            this.pnlContent.TabIndex = 3;
            // 
            // dgvDocuments
            // 
            this.dgvDocuments.AllowUserToAddRows = false;
            this.dgvDocuments.AllowUserToDeleteRows = false;
            this.dgvDocuments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDocuments.BackgroundColor = System.Drawing.Color.White;
            this.dgvDocuments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDocuments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDocuments.Location = new System.Drawing.Point(12, 8);
            this.dgvDocuments.MultiSelect = false;
            this.dgvDocuments.Name = "dgvDocuments";
            this.dgvDocuments.ReadOnly = true;
            this.dgvDocuments.RowHeadersWidth = 25;
            this.dgvDocuments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDocuments.Size = new System.Drawing.Size(1176, 523);
            this.dgvDocuments.TabIndex = 0;
            this.dgvDocuments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_tai_lieu_CellDoubleClick);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.White;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblCount});
            this.statusStrip.Location = new System.Drawing.Point(0, 650);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 22);
            this.statusStrip.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(54, 17);
            this.lblStatus.Text = "Sẵn sàng";
            // 
            // lblCount
            // 
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(1131, 17);
            this.lblCount.Spring = true;
            this.lblCount.Text = "Tổng số: 0 tài liệu";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 672);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Study Document Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileNew;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripSeparator menuFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuFileExport;
        private System.Windows.Forms.ToolStripSeparator menuFileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuEditUpdate;
        private System.Windows.Forms.ToolStripMenuItem menuEditDelete;
        private System.Windows.Forms.ToolStripSeparator menuEditSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuEditRefresh;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuViewStatistics;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolBtnNew;
        private System.Windows.Forms.ToolStripButton toolBtnEdit;
        private System.Windows.Forms.ToolStripButton toolBtnDelete;
        private System.Windows.Forms.ToolStripSeparator toolSeparator1;
        private System.Windows.Forms.ToolStripButton toolBtnOpen;
        private System.Windows.Forms.ToolStripButton toolBtnExport;
        private System.Windows.Forms.ToolStripSeparator toolSeparator2;
        private System.Windows.Forms.ToolStripButton toolBtnRefresh;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.ComboBox cboSubject;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.DataGridView dgvDocuments;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblCount;
    }
}

