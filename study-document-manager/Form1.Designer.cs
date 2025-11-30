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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.menuViewSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuViewCategories = new System.Windows.Forms.ToolStripMenuItem();
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblSubject = new System.Windows.Forms.Label();
            this.cboSubject = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.dgvDocuments = new System.Windows.Forms.DataGridView();
            this.contextMenuDocument = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxMenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxMenuCopyPath = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxMenuToggleImportant = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpAdvancedFilter = new System.Windows.Forms.GroupBox();
            this.btnClearAdvancedFilter = new System.Windows.Forms.Button();
            this.btnApplyAdvancedFilter = new System.Windows.Forms.Button();
            this.cboCreatorFilter = new System.Windows.Forms.ComboBox();
            this.lblCreatorFilter = new System.Windows.Forms.Label();
            this.chkImportantOnly = new System.Windows.Forms.CheckBox();
            this.lblSizeMB = new System.Windows.Forms.Label();
            this.nudMaxSize = new System.Windows.Forms.NumericUpDown();
            this.lblSizeTo = new System.Windows.Forms.Label();
            this.nudMinSize = new System.Windows.Forms.NumericUpDown();
            this.lblSizeFilter = new System.Windows.Forms.Label();
            this.chkEnableSizeFilter = new System.Windows.Forms.CheckBox();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblDateTo = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblDateFilter = new System.Windows.Forms.Label();
            this.chkEnableDateFilter = new System.Windows.Forms.CheckBox();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).BeginInit();
            this.contextMenuDocument.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.grpAdvancedFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSize)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.White;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuView,
            this.menuHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 33);
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
            this.menuFile.Size = new System.Drawing.Size(82, 32);
            this.menuFile.Text = "Tệp tin";
            // 
            // menuFileNew
            // 
            this.menuFileNew.Name = "menuFileNew";
            this.menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuFileNew.Size = new System.Drawing.Size(314, 34);
            this.menuFileNew.Text = "Thêm tài liệu mới";
            this.menuFileNew.Click += new System.EventHandler(this.btn_them_Click);
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFileOpen.Size = new System.Drawing.Size(314, 34);
            this.menuFileOpen.Text = "Mở tài liệu";
            this.menuFileOpen.Click += new System.EventHandler(this.btn_mo_file_Click);
            // 
            // menuFileSeparator1
            // 
            this.menuFileSeparator1.Name = "menuFileSeparator1";
            this.menuFileSeparator1.Size = new System.Drawing.Size(311, 6);
            // 
            // menuFileExport
            // 
            this.menuFileExport.Name = "menuFileExport";
            this.menuFileExport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.menuFileExport.Size = new System.Drawing.Size(314, 34);
            this.menuFileExport.Text = "Xuất dữ liệu";
            this.menuFileExport.Click += new System.EventHandler(this.btn_xuat_Click);
            // 
            // menuFileSeparator2
            // 
            this.menuFileSeparator2.Name = "menuFileSeparator2";
            this.menuFileSeparator2.Size = new System.Drawing.Size(311, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.menuFileExit.Size = new System.Drawing.Size(314, 34);
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
            this.menuEdit.Size = new System.Drawing.Size(106, 32);
            this.menuEdit.Text = "Chỉnh sửa";
            // 
            // menuEditUpdate
            // 
            this.menuEditUpdate.Name = "menuEditUpdate";
            this.menuEditUpdate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.menuEditUpdate.Size = new System.Drawing.Size(299, 34);
            this.menuEditUpdate.Text = "Sửa tài liệu";
            this.menuEditUpdate.Click += new System.EventHandler(this.btn_sua_Click);
            // 
            // menuEditDelete
            // 
            this.menuEditDelete.Name = "menuEditDelete";
            this.menuEditDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.menuEditDelete.Size = new System.Drawing.Size(299, 34);
            this.menuEditDelete.Text = "Xóa tài liệu";
            this.menuEditDelete.Click += new System.EventHandler(this.btn_xoa_Click);
            // 
            // menuEditSeparator
            // 
            this.menuEditSeparator.Name = "menuEditSeparator";
            this.menuEditSeparator.Size = new System.Drawing.Size(296, 6);
            // 
            // menuEditRefresh
            // 
            this.menuEditRefresh.Name = "menuEditRefresh";
            this.menuEditRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.menuEditRefresh.Size = new System.Drawing.Size(299, 34);
            this.menuEditRefresh.Text = "Làm mới danh sách";
            this.menuEditRefresh.Click += new System.EventHandler(this.btn_lam_moi_Click);
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewStatistics,
            this.menuViewSeparator,
            this.menuViewCategories});
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(94, 32);
            this.menuView.Text = "Công cụ";
            this.menuView.Click += new System.EventHandler(this.menuView_Click);
            // 
            // menuViewStatistics
            // 
            this.menuViewStatistics.Name = "menuViewStatistics";
            this.menuViewStatistics.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuViewStatistics.Size = new System.Drawing.Size(378, 34);
            this.menuViewStatistics.Text = "Thống kê";
            this.menuViewStatistics.Click += new System.EventHandler(this.btn_thong_ke_Click);
            // 
            // menuViewSeparator
            // 
            this.menuViewSeparator.Name = "menuViewSeparator";
            this.menuViewSeparator.Size = new System.Drawing.Size(375, 6);
            // 
            // menuViewCategories
            // 
            this.menuViewCategories.Name = "menuViewCategories";
            this.menuViewCategories.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.menuViewCategories.Size = new System.Drawing.Size(378, 34);
            this.menuViewCategories.Text = "Quản lý Danh mục";
            this.menuViewCategories.Click += new System.EventHandler(this.menuViewCategories_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(93, 32);
            this.menuHelp.Text = "Trợ giúp";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(189, 34);
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
            this.toolStrip.Location = new System.Drawing.Point(0, 33);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(8, 4, 1, 4);
            this.toolStrip.Size = new System.Drawing.Size(1200, 42);
            this.toolStrip.TabIndex = 1;
            // 
            // toolBtnNew
            // 
            this.toolBtnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolBtnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolBtnNew.Name = "toolBtnNew";
            this.toolBtnNew.Size = new System.Drawing.Size(64, 29);
            this.toolBtnNew.Text = "Thêm";
            this.toolBtnNew.Click += new System.EventHandler(this.btn_them_Click);
            // 
            // toolBtnEdit
            // 
            this.toolBtnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolBtnEdit.Name = "toolBtnEdit";
            this.toolBtnEdit.Size = new System.Drawing.Size(46, 29);
            this.toolBtnEdit.Text = "Sửa";
            this.toolBtnEdit.Click += new System.EventHandler(this.btn_sua_Click);
            // 
            // toolBtnDelete
            // 
            this.toolBtnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolBtnDelete.Name = "toolBtnDelete";
            this.toolBtnDelete.Size = new System.Drawing.Size(47, 29);
            this.toolBtnDelete.Text = "Xóa";
            this.toolBtnDelete.Click += new System.EventHandler(this.btn_xoa_Click);
            // 
            // toolSeparator1
            // 
            this.toolSeparator1.Name = "toolSeparator1";
            this.toolSeparator1.Size = new System.Drawing.Size(6, 34);
            // 
            // toolBtnOpen
            // 
            this.toolBtnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolBtnOpen.Name = "toolBtnOpen";
            this.toolBtnOpen.Size = new System.Drawing.Size(71, 29);
            this.toolBtnOpen.Text = "Mở file";
            this.toolBtnOpen.Click += new System.EventHandler(this.btn_mo_file_Click);
            // 
            // toolBtnExport
            // 
            this.toolBtnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolBtnExport.Name = "toolBtnExport";
            this.toolBtnExport.Size = new System.Drawing.Size(52, 29);
            this.toolBtnExport.Text = "Xuất";
            this.toolBtnExport.Click += new System.EventHandler(this.btn_xuat_Click);
            // 
            // toolSeparator2
            // 
            this.toolSeparator2.Name = "toolSeparator2";
            this.toolSeparator2.Size = new System.Drawing.Size(6, 34);
            // 
            // toolBtnRefresh
            // 
            this.toolBtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolBtnRefresh.Name = "toolBtnRefresh";
            this.toolBtnRefresh.Size = new System.Drawing.Size(85, 29);
            this.toolBtnRefresh.Text = "Làm mới";
            this.toolBtnRefresh.Click += new System.EventHandler(this.btn_lam_moi_Click);
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.lblSubject);
            this.pnlSearch.Controls.Add(this.cboSubject);
            this.pnlSearch.Controls.Add(this.lblType);
            this.pnlSearch.Controls.Add(this.cboType);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 75);
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
            this.lblSearch.Size = new System.Drawing.Size(88, 25);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm kiếm:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtSearch.Location = new System.Drawing.Point(80, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(380, 33);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_tim_kiem_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(470, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 27);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblSubject
            // 
            this.lblSubject.AutoSize = true;
            this.lblSubject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubject.Location = new System.Drawing.Point(590, 19);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(87, 25);
            this.lblSubject.TabIndex = 3;
            this.lblSubject.Text = "Danh mục:";
            // 
            // cboSubject
            // 
            this.cboSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboSubject.FormattingEnabled = true;
            this.cboSubject.Location = new System.Drawing.Point(654, 16);
            this.cboSubject.Name = "cboSubject";
            this.cboSubject.Size = new System.Drawing.Size(200, 33);
            this.cboSubject.TabIndex = 4;
            this.cboSubject.SelectedIndexChanged += new System.EventHandler(this.cbo_mon_hoc_SelectedIndexChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblType.Location = new System.Drawing.Point(874, 19);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(48, 25);
            this.lblType.TabIndex = 5;
            this.lblType.Text = "Loại:";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(913, 16);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(200, 33);
            this.cboType.TabIndex = 6;
            this.cboType.SelectedIndexChanged += new System.EventHandler(this.cbo_loai_SelectedIndexChanged);
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlContent.Controls.Add(this.dgvDocuments);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 296);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlContent.Size = new System.Drawing.Size(1200, 376);
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
            this.dgvDocuments.ContextMenuStrip = this.contextMenuDocument;
            this.dgvDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDocuments.Location = new System.Drawing.Point(12, 8);
            this.dgvDocuments.MultiSelect = false;
            this.dgvDocuments.Name = "dgvDocuments";
            this.dgvDocuments.ReadOnly = true;
            this.dgvDocuments.RowHeadersWidth = 25;
            this.dgvDocuments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDocuments.Size = new System.Drawing.Size(1176, 360);
            this.dgvDocuments.TabIndex = 0;
            this.dgvDocuments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_tai_lieu_CellDoubleClick);
            // 
            // contextMenuDocument
            // 
            this.contextMenuDocument.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuDocument.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuOpen,
            this.ctxMenuEdit,
            this.ctxMenuDelete,
            this.ctxMenuSeparator1,
            this.ctxMenuCopyPath,
            this.ctxMenuOpenFolder,
            this.ctxMenuSeparator2,
            this.ctxMenuToggleImportant});
            this.contextMenuDocument.Name = "contextMenuDocument";
            this.contextMenuDocument.Size = new System.Drawing.Size(256, 208);
            // 
            // ctxMenuOpen
            // 
            this.ctxMenuOpen.Name = "ctxMenuOpen";
            this.ctxMenuOpen.Size = new System.Drawing.Size(255, 32);
            this.ctxMenuOpen.Text = "Mở file";
            this.ctxMenuOpen.Click += new System.EventHandler(this.ctxMenuOpen_Click);
            // 
            // ctxMenuEdit
            // 
            this.ctxMenuEdit.Name = "ctxMenuEdit";
            this.ctxMenuEdit.Size = new System.Drawing.Size(255, 32);
            this.ctxMenuEdit.Text = "Sửa";
            this.ctxMenuEdit.Click += new System.EventHandler(this.ctxMenuEdit_Click);
            // 
            // ctxMenuDelete
            // 
            this.ctxMenuDelete.Name = "ctxMenuDelete";
            this.ctxMenuDelete.Size = new System.Drawing.Size(255, 32);
            this.ctxMenuDelete.Text = "Xóa";
            this.ctxMenuDelete.Click += new System.EventHandler(this.ctxMenuDelete_Click);
            // 
            // ctxMenuSeparator1
            // 
            this.ctxMenuSeparator1.Name = "ctxMenuSeparator1";
            this.ctxMenuSeparator1.Size = new System.Drawing.Size(252, 6);
            // 
            // ctxMenuCopyPath
            // 
            this.ctxMenuCopyPath.Name = "ctxMenuCopyPath";
            this.ctxMenuCopyPath.Size = new System.Drawing.Size(255, 32);
            this.ctxMenuCopyPath.Text = "Copy đường dẫn file";
            this.ctxMenuCopyPath.Click += new System.EventHandler(this.ctxMenuCopyPath_Click);
            // 
            // ctxMenuOpenFolder
            // 
            this.ctxMenuOpenFolder.Name = "ctxMenuOpenFolder";
            this.ctxMenuOpenFolder.Size = new System.Drawing.Size(255, 32);
            this.ctxMenuOpenFolder.Text = "Mở thư mục chứa file";
            this.ctxMenuOpenFolder.Click += new System.EventHandler(this.ctxMenuOpenFolder_Click);
            // 
            // ctxMenuSeparator2
            // 
            this.ctxMenuSeparator2.Name = "ctxMenuSeparator2";
            this.ctxMenuSeparator2.Size = new System.Drawing.Size(252, 6);
            // 
            // ctxMenuToggleImportant
            // 
            this.ctxMenuToggleImportant.Name = "ctxMenuToggleImportant";
            this.ctxMenuToggleImportant.Size = new System.Drawing.Size(255, 32);
            this.ctxMenuToggleImportant.Text = "Đánh dấu quan trọng";
            this.ctxMenuToggleImportant.Click += new System.EventHandler(this.ctxMenuToggleImportant_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.White;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblCount});
            this.statusStrip.Location = new System.Drawing.Point(0, 640);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 32);
            this.statusStrip.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(84, 25);
            this.lblStatus.Text = "Sẵn sàng";
            // 
            // lblCount
            // 
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(1101, 25);
            this.lblCount.Spring = true;
            this.lblCount.Text = "Tổng số: 0 tài liệu";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpAdvancedFilter
            // 
            this.grpAdvancedFilter.BackColor = System.Drawing.Color.White;
            this.grpAdvancedFilter.Controls.Add(this.btnClearAdvancedFilter);
            this.grpAdvancedFilter.Controls.Add(this.btnApplyAdvancedFilter);
            this.grpAdvancedFilter.Controls.Add(this.cboCreatorFilter);
            this.grpAdvancedFilter.Controls.Add(this.lblCreatorFilter);
            this.grpAdvancedFilter.Controls.Add(this.chkImportantOnly);
            this.grpAdvancedFilter.Controls.Add(this.lblSizeMB);
            this.grpAdvancedFilter.Controls.Add(this.nudMaxSize);
            this.grpAdvancedFilter.Controls.Add(this.lblSizeTo);
            this.grpAdvancedFilter.Controls.Add(this.nudMinSize);
            this.grpAdvancedFilter.Controls.Add(this.lblSizeFilter);
            this.grpAdvancedFilter.Controls.Add(this.chkEnableSizeFilter);
            this.grpAdvancedFilter.Controls.Add(this.dtpToDate);
            this.grpAdvancedFilter.Controls.Add(this.lblDateTo);
            this.grpAdvancedFilter.Controls.Add(this.dtpFromDate);
            this.grpAdvancedFilter.Controls.Add(this.lblDateFilter);
            this.grpAdvancedFilter.Controls.Add(this.chkEnableDateFilter);
            this.grpAdvancedFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpAdvancedFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpAdvancedFilter.Location = new System.Drawing.Point(0, 131);
            this.grpAdvancedFilter.Name = "grpAdvancedFilter";
            this.grpAdvancedFilter.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.grpAdvancedFilter.Size = new System.Drawing.Size(1200, 165);
            this.grpAdvancedFilter.TabIndex = 100;
            this.grpAdvancedFilter.TabStop = false;
            this.grpAdvancedFilter.Text = "Filter Nâng Cao";
            // 
            // btnClearAdvancedFilter
            // 
            this.btnClearAdvancedFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btnClearAdvancedFilter.FlatAppearance.BorderSize = 0;
            this.btnClearAdvancedFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearAdvancedFilter.ForeColor = System.Drawing.Color.White;
            this.btnClearAdvancedFilter.Location = new System.Drawing.Point(580, 70);
            this.btnClearAdvancedFilter.Name = "btnClearAdvancedFilter";
            this.btnClearAdvancedFilter.Size = new System.Drawing.Size(180, 35);
            this.btnClearAdvancedFilter.TabIndex = 15;
            this.btnClearAdvancedFilter.Text = "    ✕  Xóa Filter";
            this.btnClearAdvancedFilter.UseVisualStyleBackColor = false;
            this.btnClearAdvancedFilter.Click += new System.EventHandler(this.btnClearAdvancedFilter_Click);
            // 
            // btnApplyAdvancedFilter
            // 
            this.btnApplyAdvancedFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnApplyAdvancedFilter.FlatAppearance.BorderSize = 0;
            this.btnApplyAdvancedFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyAdvancedFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnApplyAdvancedFilter.ForeColor = System.Drawing.Color.White;
            this.btnApplyAdvancedFilter.Location = new System.Drawing.Point(580, 25);
            this.btnApplyAdvancedFilter.Name = "btnApplyAdvancedFilter";
            this.btnApplyAdvancedFilter.Size = new System.Drawing.Size(180, 35);
            this.btnApplyAdvancedFilter.TabIndex = 14;
            this.btnApplyAdvancedFilter.Text = "Áp dụng Filter";
            this.btnApplyAdvancedFilter.UseVisualStyleBackColor = false;
            this.btnApplyAdvancedFilter.Click += new System.EventHandler(this.btnApplyAdvancedFilter_Click);
            // 
            // cboCreatorFilter
            // 
            this.cboCreatorFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCreatorFilter.FormattingEnabled = true;
            this.cboCreatorFilter.Location = new System.Drawing.Point(360, 95);
            this.cboCreatorFilter.Name = "cboCreatorFilter";
            this.cboCreatorFilter.Size = new System.Drawing.Size(200, 33);
            this.cboCreatorFilter.TabIndex = 13;
            this.cboCreatorFilter.Visible = false;
            // 
            // lblCreatorFilter
            // 
            this.lblCreatorFilter.AutoSize = true;
            this.lblCreatorFilter.Location = new System.Drawing.Point(280, 98);
            this.lblCreatorFilter.Name = "lblCreatorFilter";
            this.lblCreatorFilter.Size = new System.Drawing.Size(97, 25);
            this.lblCreatorFilter.TabIndex = 12;
            this.lblCreatorFilter.Text = "Người tạo:";
            this.lblCreatorFilter.Visible = false;
            // 
            // chkImportantOnly
            // 
            this.chkImportantOnly.AutoSize = true;
            this.chkImportantOnly.Location = new System.Drawing.Point(15, 95);
            this.chkImportantOnly.Name = "chkImportantOnly";
            this.chkImportantOnly.Size = new System.Drawing.Size(306, 29);
            this.chkImportantOnly.TabIndex = 11;
            this.chkImportantOnly.Text = "Chỉ hiển thị tài liệu quan trọng (★)";
            this.chkImportantOnly.UseVisualStyleBackColor = true;
            // 
            // lblSizeMB
            // 
            this.lblSizeMB.AutoSize = true;
            this.lblSizeMB.Location = new System.Drawing.Point(335, 63);
            this.lblSizeMB.Name = "lblSizeMB";
            this.lblSizeMB.Size = new System.Drawing.Size(38, 25);
            this.lblSizeMB.TabIndex = 10;
            this.lblSizeMB.Text = "MB";
            // 
            // nudMaxSize
            // 
            this.nudMaxSize.DecimalPlaces = 2;
            this.nudMaxSize.Enabled = false;
            this.nudMaxSize.Location = new System.Drawing.Point(250, 60);
            this.nudMaxSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaxSize.Name = "nudMaxSize";
            this.nudMaxSize.Size = new System.Drawing.Size(80, 31);
            this.nudMaxSize.TabIndex = 9;
            this.nudMaxSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblSizeTo
            // 
            this.lblSizeTo.AutoSize = true;
            this.lblSizeTo.Location = new System.Drawing.Point(205, 63);
            this.lblSizeTo.Name = "lblSizeTo";
            this.lblSizeTo.Size = new System.Drawing.Size(59, 25);
            this.lblSizeTo.TabIndex = 8;
            this.lblSizeTo.Text = "MB →";
            // 
            // nudMinSize
            // 
            this.nudMinSize.DecimalPlaces = 2;
            this.nudMinSize.Enabled = false;
            this.nudMinSize.Location = new System.Drawing.Point(120, 60);
            this.nudMinSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMinSize.Name = "nudMinSize";
            this.nudMinSize.Size = new System.Drawing.Size(80, 31);
            this.nudMinSize.TabIndex = 7;
            // 
            // lblSizeFilter
            // 
            this.lblSizeFilter.AutoSize = true;
            this.lblSizeFilter.Location = new System.Drawing.Point(40, 63);
            this.lblSizeFilter.Name = "lblSizeFilter";
            this.lblSizeFilter.Size = new System.Drawing.Size(112, 25);
            this.lblSizeFilter.TabIndex = 6;
            this.lblSizeFilter.Text = "Dung lượng:";
            // 
            // chkEnableSizeFilter
            // 
            this.chkEnableSizeFilter.AutoSize = true;
            this.chkEnableSizeFilter.Location = new System.Drawing.Point(15, 60);
            this.chkEnableSizeFilter.Name = "chkEnableSizeFilter";
            this.chkEnableSizeFilter.Size = new System.Drawing.Size(22, 21);
            this.chkEnableSizeFilter.TabIndex = 5;
            this.chkEnableSizeFilter.UseVisualStyleBackColor = true;
            this.chkEnableSizeFilter.CheckedChanged += new System.EventHandler(this.chkEnableSizeFilter_CheckedChanged);
            // 
            // dtpToDate
            // 
            this.dtpToDate.Enabled = false;
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(280, 25);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(120, 31);
            this.dtpToDate.TabIndex = 4;
            // 
            // lblDateTo
            // 
            this.lblDateTo.AutoSize = true;
            this.lblDateTo.Location = new System.Drawing.Point(230, 28);
            this.lblDateTo.Name = "lblDateTo";
            this.lblDateTo.Size = new System.Drawing.Size(69, 25);
            this.lblDateTo.TabIndex = 3;
            this.lblDateTo.Text = "→ Đến:";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Enabled = false;
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(100, 25);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(120, 31);
            this.dtpFromDate.TabIndex = 2;
            // 
            // lblDateFilter
            // 
            this.lblDateFilter.AutoSize = true;
            this.lblDateFilter.Location = new System.Drawing.Point(40, 28);
            this.lblDateFilter.Name = "lblDateFilter";
            this.lblDateFilter.Size = new System.Drawing.Size(80, 25);
            this.lblDateFilter.TabIndex = 1;
            this.lblDateFilter.Text = "Từ ngày:";
            // 
            // chkEnableDateFilter
            // 
            this.chkEnableDateFilter.AutoSize = true;
            this.chkEnableDateFilter.Location = new System.Drawing.Point(15, 25);
            this.chkEnableDateFilter.Name = "chkEnableDateFilter";
            this.chkEnableDateFilter.Size = new System.Drawing.Size(22, 21);
            this.chkEnableDateFilter.TabIndex = 0;
            this.chkEnableDateFilter.UseVisualStyleBackColor = true;
            this.chkEnableDateFilter.CheckedChanged += new System.EventHandler(this.chkEnableDateFilter_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 672);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.grpAdvancedFilter);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.contextMenuDocument.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.grpAdvancedFilter.ResumeLayout(false);
            this.grpAdvancedFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSize)).EndInit();
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
        private System.Windows.Forms.ToolStripSeparator menuViewSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuViewCategories;
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
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.ComboBox cboSubject;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.DataGridView dgvDocuments;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblCount;
        private System.Windows.Forms.GroupBox grpAdvancedFilter;
        private System.Windows.Forms.CheckBox chkEnableDateFilter;
        private System.Windows.Forms.Label lblDateFilter;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblDateTo;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.CheckBox chkEnableSizeFilter;
        private System.Windows.Forms.Label lblSizeFilter;
        private System.Windows.Forms.NumericUpDown nudMinSize;
        private System.Windows.Forms.Label lblSizeTo;
        private System.Windows.Forms.NumericUpDown nudMaxSize;
        private System.Windows.Forms.Label lblSizeMB;
        private System.Windows.Forms.CheckBox chkImportantOnly;
        private System.Windows.Forms.Label lblCreatorFilter;
        private System.Windows.Forms.ComboBox cboCreatorFilter;
        private System.Windows.Forms.Button btnApplyAdvancedFilter;
        private System.Windows.Forms.Button btnClearAdvancedFilter;
        private System.Windows.Forms.ContextMenuStrip contextMenuDocument;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuOpen;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuEdit;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuDelete;
        private System.Windows.Forms.ToolStripSeparator ctxMenuSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuCopyPath;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuOpenFolder;
        private System.Windows.Forms.ToolStripSeparator ctxMenuSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuToggleImportant;
    }
}

