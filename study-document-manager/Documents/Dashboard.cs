using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            
            // Tạo menu Quản lý động
            CreateManagementMenu();
            
            // Đăng ký event CellFormatting
            dgvDocuments.CellFormatting += dgvDocuments_CellFormatting;
            // Đăng ký event DataError để xử lý lỗi format
            dgvDocuments.DataError += dgvDocuments_DataError;
            // Cho phép click trực tiếp lên cột quan trọng
            dgvDocuments.CellContentClick += dgvDocuments_CellContentClick;

            // Phase 2: Thêm menu Ghi chú cá nhân vào context menu
            AddPersonalNoteContextMenu();
        }

        /// <summary>
        /// Thêm menu Ghi chú cá nhân vào context menu (Phase 2)
        /// </summary>
        private void AddPersonalNoteContextMenu()
        {
            contextMenuDocument.Items.Add(new ToolStripSeparator());
            
            ToolStripMenuItem ctxMenuPersonalNote = new ToolStripMenuItem("Ghi chú cá nhân...");
            ctxMenuPersonalNote.Name = "ctxMenuPersonalNote";
            ctxMenuPersonalNote.Click += ctxMenuPersonalNote_Click;
            contextMenuDocument.Items.Add(ctxMenuPersonalNote);

            // Thêm menu "Thêm vào bộ sưu tập"
            ToolStripMenuItem ctxMenuAddToCollection = new ToolStripMenuItem("Thêm vào bộ sưu tập...");
            ctxMenuAddToCollection.Name = "ctxMenuAddToCollection";
            ctxMenuAddToCollection.Click += ctxMenuAddToCollection_Click;
            contextMenuDocument.Items.Add(ctxMenuAddToCollection);
        }

        /// <summary>
        /// Context Menu: Thêm vào bộ sưu tập (Phase 2)
        /// </summary>
        private void ctxMenuAddToCollection_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu!");
                return;
            }

            int docId = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
            string docName = dgvDocuments.SelectedRows[0].Cells["ten"].Value.ToString();

            // Lấy danh sách collections
            DataTable collections = DatabaseHelper.GetCollections();
            
            if (collections.Rows.Count == 0)
            {
                if (MessageBox.Show("Bạn chưa có bộ sưu tập nào.\nTạo bộ sưu tập mới?",
                    "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string name = Microsoft.VisualBasic.Interaction.InputBox(
                        "Nhập tên bộ sưu tập:", "Tạo bộ sưu tập mới", "");
                    
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        int newId = DatabaseHelper.CreateCollection(name.Trim());
                        if (newId > 0)
                        {
                            DatabaseHelper.AddDocumentToCollection(newId, docId);
                            lblStatus.Text = $"Đã thêm '{docName}' vào bộ sưu tập '{name}'";
                        }
                    }
                }
                return;
            }

            // Hiển thị dialog chọn collection
            using (Form selectForm = new Form())
            {
                selectForm.Text = "Chọn bộ sưu tập";
                selectForm.Size = new Size(300, 200);
                selectForm.StartPosition = FormStartPosition.CenterParent;
                selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                selectForm.MaximizeBox = false;
                selectForm.MinimizeBox = false;

                ComboBox cboCollections = new ComboBox
                {
                    Location = new Point(20, 30),
                    Size = new Size(240, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                foreach (DataRow row in collections.Rows)
                {
                    cboCollections.Items.Add(new { Id = row["id"], Name = row["name"].ToString() });
                }
                cboCollections.DisplayMember = "Name";
                cboCollections.SelectedIndex = 0;

                Button btnOK = new Button
                {
                    Text = "Thêm",
                    Location = new Point(80, 100),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.OK
                };

                Button btnCancel = new Button
                {
                    Text = "Hủy",
                    Location = new Point(170, 100),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.Cancel
                };

                selectForm.Controls.AddRange(new Control[] { cboCollections, btnOK, btnCancel });
                selectForm.AcceptButton = btnOK;
                selectForm.CancelButton = btnCancel;

                if (selectForm.ShowDialog() == DialogResult.OK && cboCollections.SelectedItem != null)
                {
                    dynamic selected = cboCollections.SelectedItem;
                    int collectionId = Convert.ToInt32(selected.Id);
                    string collectionName = selected.Name;

                    if (DatabaseHelper.AddDocumentToCollection(collectionId, docId))
                    {
                        ToastNotification.Success($"Đã thêm '{docName}' vào '{collectionName}'");
                    }
                    else
                    {
                        ToastNotification.Info("Tài liệu đã có trong bộ sưu tập này!");
                    }
                }
            }
        }

        /// <summary>
        /// Context Menu: Ghi chú cá nhân (Phase 2)
        /// </summary>
        private void ctxMenuPersonalNote_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu!");
                return;
            }

            int docId = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
            string docName = dgvDocuments.SelectedRows[0].Cells["ten"].Value.ToString();

            PersonalNoteForm form = new PersonalNoteForm(docId, docName);
            form.ShowDialog();
        }

        /// <summary>
        /// Tạo menu Quản lý (Admin only)
        /// </summary>
        private void CreateManagementMenu()
        {
            try
            {
                // Tìm MenuStrip trên form
                MenuStrip menuStrip = null;
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is MenuStrip)
                    {
                        menuStrip = (MenuStrip)ctrl;
                        break;
                    }
                }

                if (menuStrip != null)
                {
                    // ===================================
                    // TÌM MENU "CÔNG CỤ" (menuView)
                    // ===================================
                    ToolStripMenuItem menuView = null;
                    foreach (ToolStripItem item in menuStrip.Items)
                    {
                        if (item.Name == "menuView" || item.Text.Contains("Công cụ"))
                        {
                            menuView = item as ToolStripMenuItem;
                            break;
                        }
                    }

                    if (menuView != null)
                    {
                        // Thêm Kiểm tra file bị thiếu vào Công cụ
                        menuView.DropDownItems.Add(new ToolStripSeparator());
                        ToolStripMenuItem menuCheckFiles = new ToolStripMenuItem("Kiểm tra file bị thiếu");
                        menuCheckFiles.Name = "menuCheckFiles";
                        menuCheckFiles.Click += menuCheckFiles_Click;
                        menuView.DropDownItems.Add(menuCheckFiles);
                    }

                    // ===================================
                    // TẠO MENU "THEO DÕI" (MỚI)
                    // ===================================
                    ToolStripMenuItem menuTracking = new ToolStripMenuItem("Theo dõi");
                    menuTracking.Name = "menuTracking";

                    ToolStripMenuItem menuUpcomingDeadlines = new ToolStripMenuItem("Sắp đến hạn (7 ngày)");
                    menuUpcomingDeadlines.Name = "menuUpcomingDeadlines";
                    menuUpcomingDeadlines.Click += menuUpcomingDeadlines_Click;
                    menuTracking.DropDownItems.Add(menuUpcomingDeadlines);

                    ToolStripMenuItem menuOverdue = new ToolStripMenuItem("Tài liệu quá hạn");
                    menuOverdue.Name = "menuOverdue";
                    menuOverdue.Click += menuOverdue_Click;
                    menuTracking.DropDownItems.Add(menuOverdue);

                    menuTracking.DropDownItems.Add(new ToolStripSeparator());

                    ToolStripMenuItem menuCollections = new ToolStripMenuItem("Quản lý bộ sưu tập");
                    menuCollections.Name = "menuCollections";
                    menuCollections.Click += menuCollections_Click;
                    menuTracking.DropDownItems.Add(menuCollections);

                    // ===================================
                    // TẠO MENU "TÀI KHOẢN"
                    // ===================================
                    ToolStripMenuItem menuAccount = new ToolStripMenuItem("Tài khoản");
                    menuAccount.Name = "menuAccount";

                    ToolStripMenuItem menuAccountSettings = new ToolStripMenuItem("Cài đặt tài khoản");
                    menuAccountSettings.Name = "menuAccountSettings";
                    menuAccountSettings.Click += menuAccountSettings_Click;
                    menuAccount.DropDownItems.Add(menuAccountSettings);

                    menuAccount.DropDownItems.Add(new ToolStripSeparator());

                    ToolStripMenuItem menuAccountLogout = new ToolStripMenuItem("Đăng xuất");
                    menuAccountLogout.Name = "menuAccountLogout";
                    menuAccountLogout.ShortcutKeys = Keys.Control | Keys.L;
                    menuAccountLogout.Click += menuLogout_Click;
                    menuAccount.DropDownItems.Add(menuAccountLogout);

                    // ===================================
                    // TẠO MENU "QUẢN LÝ" (Admin only)
                    // ===================================
                    ToolStripMenuItem menuManagement = new ToolStripMenuItem("&Quản lý");
                    menuManagement.Name = "menuManagement";

                    ToolStripMenuItem menuManagementUsers = new ToolStripMenuItem("Quản lý người dùng");
                    menuManagementUsers.Name = "menuManagementUsers";
                    menuManagementUsers.ShortcutKeys = Keys.Control | Keys.U;
                    menuManagementUsers.Click += menuManagementUsers_Click;
                    menuManagement.DropDownItems.Add(menuManagementUsers);

                    // Lưu reference để dùng trong ApplyPermissions
                    this.menuManagement = menuManagement;

                    // ===================================
                    // TẠO NÚT "ĐĂNG XUẤT" (góc phải)
                    // ===================================
                    ToolStripMenuItem menuLogout = new ToolStripMenuItem("Đăng xuất");
                    menuLogout.Name = "menuLogout";
                    menuLogout.ShortcutKeys = Keys.Control | Keys.L;
                    menuLogout.Alignment = ToolStripItemAlignment.Right;
                    menuLogout.Click += menuLogout_Click;

                    // ===================================
                    // THÊM CÁC MENU VÀO MENUSTRIP
                    // ===================================
                    // Tìm vị trí sau "Trợ giúp"
                    int helpIndex = -1;
                    for (int i = 0; i < menuStrip.Items.Count; i++)
                    {
                        if (menuStrip.Items[i].Name == "menuHelp" || menuStrip.Items[i].Text.Contains("Trợ giúp"))
                        {
                            helpIndex = i;
                            break;
                        }
                    }

                    if (helpIndex >= 0)
                    {
                        // Chèn trước Trợ giúp: Theo dõi, Tài khoản, Quản lý
                        menuStrip.Items.Insert(helpIndex, menuTracking);
                        menuStrip.Items.Insert(helpIndex + 1, menuAccount);
                        menuStrip.Items.Insert(helpIndex + 2, menuManagement);
                    }
                    else
                    {
                        menuStrip.Items.Add(menuTracking);
                        menuStrip.Items.Add(menuAccount);
                        menuStrip.Items.Add(menuManagement);
                    }

                    // Thêm Đăng xuất ở cuối (sẽ hiển thị bên phải do Alignment)
                    menuStrip.Items.Add(menuLogout);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error creating menu: " + ex.Message);
            }
        }

        /// <summary>
        /// Menu: Cài đặt tài khoản
        /// </summary>
        private void menuAccountSettings_Click(object sender, EventArgs e)
        {
            AccountSettingsForm form = new AccountSettingsForm();
            form.ShowDialog();
            
            // Cập nhật title nếu user đổi tên
            this.Text = $"Study Document Manager - {UserSession.FullName} [{UserSession.Role}]";
            lblStatus.Text = $"Xin chào, {UserSession.FullName} ({UserSession.Role})";
        }

        // Field để lưu menu reference
        private ToolStripMenuItem menuManagement;

        private void Form1_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            InitializeCustomComponents();
            this.Text = $"Study Document Manager - {UserSession.FullName} [{UserSession.Role}]";
            
            // Hiển thị thông tin user
            lblStatus.Text = $"Xin chào, {UserSession.FullName} ({UserSession.Role})";
            
            // ============================================
            // PHÂN QUYỀN
            // ============================================
            ApplyPermissions();
            
            // Setup icons cho ToolStrip buttons
            toolBtnNew.Image = IconHelper.CreateAddIcon(16, AppTheme.StatusSuccess);
            toolBtnEdit.Image = IconHelper.CreateEditIcon(16, AppTheme.Primary);
            toolBtnDelete.Image = IconHelper.CreateDeleteIcon(16, AppTheme.StatusError);
            toolBtnOpen.Image = IconHelper.CreateOpenIcon(16, AppTheme.AccentSky);
            toolBtnExport.Image = IconHelper.CreateExportIcon(16, AppTheme.AccentOrange);
            toolBtnRefresh.Image = IconHelper.CreateRefreshIcon(16, AppTheme.Secondary);
            
            // Chế độ cá nhân: Không cần filter theo người tạo
            LoadUsersForFilter();
            
            // Khởi tạo giá trị mặc định cho date pickers
            dtpFromDate.Value = DateTime.Now.AddMonths(-1);
            dtpToDate.Value = DateTime.Now;
            dtpFromDate.Enabled = false;
            dtpToDate.Enabled = false;
            nudMinSize.Enabled = false;
            nudMaxSize.Enabled = false;
            
            // Hiển thị Toast chào mừng
            ToastNotification.Success($"Xin chào, {UserSession.FullName}!");
        }

        /// <summary>
        /// Áp dụng theme cho Dashboard
        /// </summary>
        private void ApplyTheme()
        {
            // Form background
            this.BackColor = AppTheme.BackgroundSoft;
            
            // MenuStrip
            AppTheme.ApplyMenuStripStyle(menuStrip);
            
            // ToolStrip
            AppTheme.ApplyToolStripStyle(toolStrip);
            
            // Search Panel
            pnlSearch.BackColor = AppTheme.BackgroundMain;
            lblSearch.ForeColor = AppTheme.TextSecondary;
            AppTheme.ApplyTextBoxStyle(txtSearch);
            AppTheme.ApplyComboBoxStyle(cboSubject);
            AppTheme.ApplyComboBoxStyle(cboType);
            
            // Search button - Primary style
            btnSearch.BackColor = AppTheme.Primary;
            btnSearch.ForeColor = AppTheme.TextWhite;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Font = AppTheme.FontButton;
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.MouseEnter += (s, e) => btnSearch.BackColor = AppTheme.PrimaryLight;
            btnSearch.MouseLeave += (s, e) => btnSearch.BackColor = AppTheme.Primary;
            
            // Content Panel
            pnlContent.BackColor = AppTheme.BackgroundSoft;
            
            // DataGridView
            AppTheme.ApplyDataGridViewStyle(dgvDocuments);
            
            // Status Strip
            AppTheme.ApplyStatusStripStyle(statusStrip);
            
            // Advanced Filter GroupBox
            AppTheme.ApplyGroupBoxStyle(grpAdvancedFilter);
            grpAdvancedFilter.BackColor = AppTheme.BackgroundMain;
            
            // Filter Buttons
            btnApplyAdvancedFilter.BackColor = AppTheme.Primary;
            btnApplyAdvancedFilter.ForeColor = AppTheme.TextWhite;
            btnApplyAdvancedFilter.FlatStyle = FlatStyle.Flat;
            btnApplyAdvancedFilter.FlatAppearance.BorderSize = 0;
            btnApplyAdvancedFilter.Cursor = Cursors.Hand;
            btnApplyAdvancedFilter.MouseEnter += (s, e) => btnApplyAdvancedFilter.BackColor = AppTheme.PrimaryLight;
            btnApplyAdvancedFilter.MouseLeave += (s, e) => btnApplyAdvancedFilter.BackColor = AppTheme.Primary;
            
            btnClearAdvancedFilter.BackColor = AppTheme.BackgroundSoft;
            btnClearAdvancedFilter.ForeColor = AppTheme.TextSecondary;
            btnClearAdvancedFilter.FlatStyle = FlatStyle.Flat;
            btnClearAdvancedFilter.FlatAppearance.BorderSize = 1;
            btnClearAdvancedFilter.FlatAppearance.BorderColor = AppTheme.BorderMedium;
            btnClearAdvancedFilter.Cursor = Cursors.Hand;
            btnClearAdvancedFilter.MouseEnter += (s, e) => { btnClearAdvancedFilter.BackColor = AppTheme.BorderLight; };
            btnClearAdvancedFilter.MouseLeave += (s, e) => { btnClearAdvancedFilter.BackColor = AppTheme.BackgroundSoft; };
        }

        /// <summary>
        /// Áp dụng phân quyền theo role
        /// </summary>
        private void ApplyPermissions()
        {
            // Chỉ Admin mới thấy menu Quản lý
            if (menuManagement != null)
            {
                menuManagement.Visible = UserSession.IsAdmin;
            }

            // Chế độ cá nhân: Mọi user chỉ sửa/xóa được tài liệu của mình

            // Không disable buttons - để tất cả user đều thấy
            // Logic kiểm tra quyền sẽ được áp dụng khi click button Sửa/Xóa
        }

        /// <summary>
        /// Khởi tạo components
        /// </summary>
        private void InitializeCustomComponents()
        {
            LoadComboBoxData();
            LoadData();
            SetupDataGridView();
            EnableDragDrop();
        }

        /// <summary>
        /// Load dữ liệu ComboBox
        /// </summary>
        private void LoadComboBoxData()
        {
            // ComboBox danh mục
            cboSubject.Items.Clear();
            cboSubject.Items.Add("Tất cả");
            cboSubject.Items.Add("Công việc");
            cboSubject.Items.Add("Cá nhân");
            cboSubject.Items.Add("Học tập");
            cboSubject.Items.Add("Dự án");
            cboSubject.Items.Add("Tài chính");
            cboSubject.Items.Add("Hợp đồng");
            cboSubject.Items.Add("Tham khảo");
            cboSubject.Items.Add("Khác");
            cboSubject.SelectedIndex = 0;

            // ComboBox loại
            cboType.Items.Clear();
            cboType.Items.Add("Tất cả");
            cboType.Items.Add("Tài liệu");
            cboType.Items.Add("Báo cáo");
            cboType.Items.Add("Hướng dẫn");
            cboType.Items.Add("Biểu mẫu");
            cboType.Items.Add("Hình ảnh");
            cboType.Items.Add("Video");
            cboType.Items.Add("Khác");
            cboType.SelectedIndex = 0;
        }

        /// <summary>
        /// Setup DataGridView
        /// </summary>
        private void SetupDataGridView()
        {
            if (dgvDocuments.Columns.Count > 0)
            {
                // Ẩn cột không cần thiết
                if (dgvDocuments.Columns.Contains("id"))
                    dgvDocuments.Columns["id"].Visible = false;
                if (dgvDocuments.Columns.Contains("duong_dan"))
                    dgvDocuments.Columns["duong_dan"].Visible = false;

                // Xóa cột Icon cũ nếu có và không phải ImageColumn
                if (dgvDocuments.Columns.Contains("Icon"))
                {
                    if (!(dgvDocuments.Columns["Icon"] is DataGridViewImageColumn))
                    {
                        dgvDocuments.Columns.Remove("Icon");
                    }
                }

                // Thêm cột Icon nếu chưa có
                if (!dgvDocuments.Columns.Contains("Icon"))
                {
                    DataGridViewImageColumn iconColumn = new DataGridViewImageColumn
                    {
                        Name = "Icon",
                        HeaderText = "",
                        Width = 30,
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        DisplayIndex = 0,
                        // Quan trọng: Đặt null value
                        DefaultCellStyle = { NullValue = null }
                    };
                    dgvDocuments.Columns.Insert(0, iconColumn);
                }

                // Đặt header text
                if (dgvDocuments.Columns.Contains("ten"))
                    dgvDocuments.Columns["ten"].HeaderText = "Tên tài liệu";
                if (dgvDocuments.Columns.Contains("mon_hoc"))
                    dgvDocuments.Columns["mon_hoc"].HeaderText = "Danh mục";
                if (dgvDocuments.Columns.Contains("loai"))
                    dgvDocuments.Columns["loai"].HeaderText = "Loại";
                if (dgvDocuments.Columns.Contains("ngay_them"))
                {
                    dgvDocuments.Columns["ngay_them"].HeaderText = "Ngày thêm";
                    dgvDocuments.Columns["ngay_them"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvDocuments.Columns.Contains("ghi_chu"))
                    dgvDocuments.Columns["ghi_chu"].HeaderText = "Ghi chú";
                if (dgvDocuments.Columns.Contains("kich_thuoc"))
                {
                    dgvDocuments.Columns["kich_thuoc"].HeaderText = "Kích thước (MB)";
                    dgvDocuments.Columns["kich_thuoc"].DefaultCellStyle.Format = "F2";
                }
                if (dgvDocuments.Columns.Contains("tac_gia"))
                    dgvDocuments.Columns["tac_gia"].HeaderText = "Tác giả";
                
                // Cột Tags (Phase 2)
                if (dgvDocuments.Columns.Contains("tags"))
                {
                    dgvDocuments.Columns["tags"].HeaderText = "Tags";
                    dgvDocuments.Columns["tags"].Width = 150;
                }

                // Cột Deadline (Phase 2)
                if (dgvDocuments.Columns.Contains("deadline"))
                {
                    dgvDocuments.Columns["deadline"].HeaderText = "Hạn chót";
                    dgvDocuments.Columns["deadline"].Width = 100;
                    dgvDocuments.Columns["deadline"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                // Cột quan_trong - cho phép click checkbox (đặt gần cuối)
                if (dgvDocuments.Columns.Contains("quan_trong"))
                {
                    dgvDocuments.Columns["quan_trong"].HeaderText = "★";
                    dgvDocuments.Columns["quan_trong"].Width = 40;
                    dgvDocuments.Columns["quan_trong"].ReadOnly = false;
                    dgvDocuments.Columns["quan_trong"].DisplayIndex = dgvDocuments.Columns.Count - 2; // Gần cuối, trước Người tạo
                }

                // Cột Người tạo - ẨN vì chế độ cá nhân (mọi tài liệu đều của user hiện tại)
                if (dgvDocuments.Columns.Contains("creator_name"))
                {
                    dgvDocuments.Columns["creator_name"].Visible = false;
                }
                
                if (dgvDocuments.Columns.Contains("creator_username"))
                    dgvDocuments.Columns["creator_username"].Visible = false;
                    
                if (dgvDocuments.Columns.Contains("user_id"))
                    dgvDocuments.Columns["user_id"].Visible = false;
                
                // Set ReadOnly = false cho grid để checkbox hoạt động
                dgvDocuments.ReadOnly = false;
                
                // Set ReadOnly = true cho các cột khác (không cho phép edit)
                foreach (DataGridViewColumn col in dgvDocuments.Columns)
                {
                    if (col.Name != "quan_trong")
                    {
                        col.ReadOnly = true;
                    }
                }

                // Enable sorting
                dgvDocuments.AllowUserToOrderColumns = true;
                foreach (DataGridViewColumn column in dgvDocuments.Columns)
                {
                    if (column is DataGridViewImageColumn)
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    else
                        column.SortMode = DataGridViewColumnSortMode.Automatic;
                }
            } // End if (dgvDocuments.Columns.Count > 0)

            // Style - Dùng AppTheme
            dgvDocuments.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridRowAlt;
            dgvDocuments.RowsDefaultCellStyle.BackColor = AppTheme.BackgroundMain;
            dgvDocuments.RowsDefaultCellStyle.SelectionBackColor = AppTheme.GridRowSelected;
            dgvDocuments.RowsDefaultCellStyle.SelectionForeColor = AppTheme.TextPrimary;
            dgvDocuments.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeaderBg;
            dgvDocuments.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderFg;
            dgvDocuments.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontSmallBold;
            dgvDocuments.EnableHeadersVisualStyles = false;
            dgvDocuments.RowTemplate.Height = 40;
        }

        /// <summary>
        /// Load dữ liệu
        /// </summary>
        private void LoadData()
        {
            try
            {
                if (!DatabaseHelper.TestConnection())
                {
                    lblStatus.Text = "Không thể kết nối database";
                    MessageBox.Show("Không thể kết nối database!\n\nVui lòng kiểm tra:\n" +
                        "1. SQL Server đang chạy\n" +
                        "2. Database 'quan_ly_tai_lieu' đã tạo\n" +
                        "3. Connection string trong App.config đúng",
                        "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Sử dụng phân quyền: Mọi user chỉ thấy tài liệu của mình
                DataTable dt = DatabaseHelper.GetDocumentsForCurrentUser();
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                lblCount.Text = $"Tổng số: {dt.Rows.Count} tài liệu";
                lblStatus.Text = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi load dữ liệu";
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Button Thêm
        /// </summary>
        private void btn_them_Click(object sender, EventArgs e)
        {
            AddEditForm form = new AddEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
                lblStatus.Text = "Đã thêm tài liệu mới";
            }
        }

        /// <summary>
        /// Button Sửa
        /// </summary>
        private void btn_sua_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu cần sửa!");
                return;
            }

            int id = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
            
            // Kiểm tra quyền sửa tài liệu
            if (!DatabaseHelper.CanUserEditDocument(id, UserSession.UserId, UserSession.Role))
            {
                ToastNotification.Warning("Bạn không có quyền sửa tài liệu này!");
                return;
            }

            AddEditForm form = new AddEditForm(id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
                lblStatus.Text = "Đã cập nhật tài liệu";
            }
        }

        /// <summary>
        /// Button Xóa
        /// </summary>
        private void btn_xoa_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu cần xóa!");
                return;
            }

            int id = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
            
            // Kiểm tra quyền xóa tài liệu
            if (!DatabaseHelper.CanUserEditDocument(id, UserSession.UserId, UserSession.Role))
            {
                ToastNotification.Warning("Bạn không có quyền xóa tài liệu này!");
                return;
            }

            string ten = dgvDocuments.SelectedRows[0].Cells["ten"].Value.ToString();
            if (MessageBox.Show($"Xóa tài liệu:\n\"{ten}\"?", 
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (DatabaseHelper.DeleteDocument(id))
                {
                    LoadData();
                    lblStatus.Text = "Đã xóa tài liệu";
                }
            }
        }

        /// <summary>
        /// Button Mở file
        /// </summary>
        private void btn_mo_file_Click(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        /// <summary>
        /// Mở file được chọn
        /// </summary>
        private void OpenSelectedFile()
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu!");
                return;
            }

            string duong_dan = dgvDocuments.SelectedRows[0].Cells["duong_dan"].Value.ToString();
            
            if (!File.Exists(duong_dan))
            {
                ToastNotification.Error("File không tồn tại!");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(duong_dan);
                lblStatus.Text = "Đã mở file";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Không thể mở file: " + ex.Message);
            }
        }

        /// <summary>
        /// Double-click mở file
        /// </summary>
        private void dgv_tai_lieu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                OpenSelectedFile();
            }
        }

        /// <summary>
        /// Tìm kiếm (Enter key)
        /// </summary>
        private void txt_tim_kiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                PerformSearch();
            }
        }

        /// <summary>
        /// Button Tìm kiếm - click event
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        /// <summary>
        /// Thực hiện tìm kiếm
        /// </summary>
        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            
            if (string.IsNullOrEmpty(keyword))
            {
                LoadData();
                return;
            }

            try
            {
                DataTable dt = DatabaseHelper.SearchDocuments(keyword);
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                lblCount.Text = $"Tìm thấy: {dt.Rows.Count} tài liệu";
                lblStatus.Text = $"Tìm kiếm: {keyword}";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Lọc theo danh mục
        /// </summary>
        private void cbo_mon_hoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// Lọc theo loại
        /// </summary>
        private void cbo_loai_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// Áp dụng filter
        /// </summary>
        private void ApplyFilter()
        {
            string mon_hoc = cboSubject.SelectedItem?.ToString();
            string loai = cboType.SelectedItem?.ToString();

            if (mon_hoc == "Tất cả" && loai == "Tất cả")
            {
                LoadData();
                return;
            }

            try
            {
                DataTable dt = DatabaseHelper.FilterDocuments(
                    mon_hoc == "Tất cả" ? "" : mon_hoc,
                    loai == "Tất cả" ? "" : loai
                );
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                lblCount.Text = $"Lọc được: {dt.Rows.Count} tài liệu";
                lblStatus.Text = "Đã lọc dữ liệu";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Làm mới
        /// </summary>
        private void btn_lam_moi_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboSubject.SelectedIndex = 0;
            cboType.SelectedIndex = 0;
            LoadData();
        }

        /// <summary>
        /// Xuất dữ liệu
        /// </summary>
        private void btn_xuat_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";
            save.FileName = $"DanhSachTaiLieu_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (save.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable dt = (DataTable)dgvDocuments.DataSource;
                    
                    using (StreamWriter writer = new StreamWriter(save.FileName, false, System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine("ID,Tên tài liệu,Danh mục,Loại,Đường dẫn,Ghi chú,Ngày thêm,Kích thước (MB),Tác giả,Quan trọng");
                        
                        foreach (DataRow row in dt.Rows)
                        {
                            writer.WriteLine($"{row["id"]}," +
                                $"\"{row["ten"]}\"," +
                                $"\"{row["mon_hoc"]}\"," +
                                $"\"{row["loai"]}\"," +
                                $"\"{row["duong_dan"]}\"," +
                                $"\"{row["ghi_chu"]}\"," +
                                $"{row["ngay_them"]}," +
                                $"{row["kich_thuoc"]}," +
                                $"\"{row["tac_gia"]}\"," +
                                $"{row["quan_trong"]}");
                        }
                    }

                    lblStatus.Text = "Đã xuất file thành công";
                    ToastNotification.Success($"Xuất thành công: {System.IO.Path.GetFileName(save.FileName)}");
                    
                    System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + save.FileName + "\"");
                }
                catch (Exception ex)
                {
                    ToastNotification.Error("Lỗi: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Thống kê
        /// </summary>
        private void btn_thong_ke_Click(object sender, EventArgs e)
        {
            try
            {
                Report reportForm = new Report();
                reportForm.ShowDialog();
                lblStatus.Text = "Đã hiển thị thống kê";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Drag-drop support
        /// </summary>
        private void EnableDragDrop()
        {
            dgvDocuments.AllowDrop = true;
            
            dgvDocuments.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            };

            dgvDocuments.DragDrop += (s, e) =>
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                int successCount = 0;
                
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".pdf" || ext == ".doc" || ext == ".docx" || 
                        ext == ".ppt" || ext == ".pptx" || ext == ".txt" || 
                        ext == ".xlsx" || ext == ".xls")
                    {
                        // Tạo AddEditForm và tự động điền thông tin file
                        AddEditForm form = new AddEditForm();
                        
                        // Điền đường dẫn file
                        form.txt_duong_dan.Text = file;
                        
                        // Tự động điền tên file
                        form.txt_ten.Text = Path.GetFileNameWithoutExtension(file);
                        
                        // Tính kích thước file
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            double size = fileInfo.Length / (1024.0 * 1024.0); // MB
                            form.txt_kich_thuoc.Text = size.ToString("F2");
                        }
                        catch
                        {
                            form.txt_kich_thuoc.Text = "0.00";
                        }
                        
                        // Tự động chọn loại tài liệu dựa vào extension
                        if (ext == ".ppt" || ext == ".pptx")
                            form.cbo_loai.Text = "slide";
                        else if (ext == ".doc" || ext == ".docx")
                            form.cbo_loai.Text = "bài tập";
                        else if (ext == ".pdf")
                            form.cbo_loai.Text = "đề thi";
                        else if (ext == ".xlsx" || ext == ".xls")
                            form.cbo_loai.Text = "tài liệu khác";
                        
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            successCount++;
                        }
                    }
                    else
                    {
                        ToastNotification.Warning($"File không hỗ trợ: {Path.GetFileName(file)}");
                    }
                }
                
                if (successCount > 0)
                {
                    LoadData();
                    ToastNotification.Success($"Đã thêm {successCount} tài liệu");
                }
            };
        }

        /// <summary>
        /// Menu File Exit
        /// </summary>
        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Menu Help About
        /// </summary>
        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Study Document Manager v1.0\n\n" +
                "Ứng dụng quản lý tài liệu học tập\n\n" +
                "Phát triển bởi: Vũ Đức Dũng - TT601K14\n" +
                "Công nghệ: C# Windows Forms, SQL Server",
                "Giới thiệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Menu Quản lý Danh mục và Loại
        /// </summary>
        private void menuViewCategories_Click(object sender, EventArgs e)
        {
            // Chế độ cá nhân: Mọi user đều được quản lý danh mục của mình
            try
            {
                CategoryManagementForm categoryForm = new CategoryManagementForm();
                categoryForm.ShowDialog();
                
                // Refresh lại dữ liệu sau khi đóng form quản lý danh mục
                LoadData();
                lblStatus.Text = "Đã làm mới dữ liệu";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Menu Quản lý Người dùng (Admin only)
        /// </summary>
        private void menuManagementUsers_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsAdmin)
            {
                ToastNotification.Warning("Bạn không có quyền truy cập chức năng này!");
                return;
            }

            try
            {
                UserManagementForm form = new UserManagementForm();
                form.ShowDialog();
                lblStatus.Text = "Đã đóng form quản lý người dùng";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Menu Đăng xuất
        /// </summary>
        private void menuLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Log activity (optional - skip if table doesn't exist)
                try
                {
                    DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO activity_logs (user_id, action, entity_type, description, created_date) 
                          VALUES (@user_id, @action, @entity_type, @description, GETDATE())",
                        new System.Data.SqlClient.SqlParameter[] {
                            new System.Data.SqlClient.SqlParameter("@user_id", UserSession.UserId),
                            new System.Data.SqlClient.SqlParameter("@action", "Logout"),
                            new System.Data.SqlClient.SqlParameter("@entity_type", "Session"),
                            new System.Data.SqlClient.SqlParameter("@description", "Đăng xuất khỏi hệ thống")
                        });
                }
                catch { }

                // Clear session
                UserSession.Logout();

                // Ẩn form hiện tại
                this.Hide();

                // Hiển thị LoginForm lại
                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Đăng nhập thành công, restart app để load lại với user mới
                    Application.Restart();
                }
                else
                {
                    // User đóng LoginForm hoặc thoát
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// Format cells để hiển thị icon và deadline
        /// </summary>
        private void dgvDocuments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                string colName = dgvDocuments.Columns[e.ColumnIndex].Name;

                // Format Icon column
                if (colName == "Icon" && dgvDocuments.Columns[e.ColumnIndex] is DataGridViewImageColumn)
                {
                    var loaiCell = dgvDocuments.Rows[e.RowIndex].Cells["loai"];
                    var tenCell = dgvDocuments.Rows[e.RowIndex].Cells["ten"];
                    
                    string loai = loaiCell?.Value?.ToString() ?? "";
                    string fileName = tenCell?.Value?.ToString() ?? "";
                    
                    e.Value = IconHelper.GetDocumentIcon(loai, 24, fileName);
                    e.FormattingApplied = true;
                }
                // Format deadline column - highlight màu theo ngày còn lại
                else if (colName == "deadline")
                {
                    if (e.Value != null && e.Value != DBNull.Value)
                    {
                        DateTime deadline = Convert.ToDateTime(e.Value);
                        int daysLeft = (deadline.Date - DateTime.Now.Date).Days;

                        if (daysLeft < 0)
                        {
                            e.CellStyle.ForeColor = Color.White;
                            e.CellStyle.BackColor = Color.FromArgb(244, 67, 54);
                            e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        }
                        else if (daysLeft <= 3)
                        {
                            e.CellStyle.ForeColor = Color.White;
                            e.CellStyle.BackColor = Color.FromArgb(255, 152, 0);
                            e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        }
                        else if (daysLeft <= 7)
                        {
                            e.CellStyle.ForeColor = Color.Black;
                            e.CellStyle.BackColor = Color.FromArgb(255, 235, 59);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CellFormatting error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý lỗi DataGridView
        /// </summary>
        private void dgvDocuments_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Log lỗi để debug
            System.Diagnostics.Debug.WriteLine($"DataGridView error at row {e.RowIndex}, column {e.ColumnIndex}: {e.Exception.Message}");
            
            // Ngăn hiển thị dialog lỗi mặc định
            e.ThrowException = false;
            
            // Có thể hiển thị thông báo lỗi tùy chỉnh nếu cần
            if (e.Context == DataGridViewDataErrorContexts.Formatting)
            {
                lblStatus.Text = "Lỗi hiển thị dữ liệu";
            }
        }

        private void dgvDocuments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Xử lý checkbox quan_trong
            if (dgvDocuments.Columns[e.ColumnIndex].Name == "quan_trong")
            {
                // Commit edit để lấy giá trị mới của checkbox
                dgvDocuments.CommitEdit(DataGridViewDataErrorContexts.Commit);
                
                // Lấy giá trị mới (đã được thay đổi bởi click)
                var cell = dgvDocuments.Rows[e.RowIndex].Cells["quan_trong"];
                bool newValue = cell.Value != null && cell.Value != DBNull.Value && Convert.ToBoolean(cell.Value);
                int id = Convert.ToInt32(dgvDocuments.Rows[e.RowIndex].Cells["id"].Value);

                SaveImportantValue(id, newValue);
            }
        }

        private void SaveImportantValue(int documentId, bool isImportant)
        {
            try
            {
                string query = "UPDATE tai_lieu SET quan_trong = @quan_trong WHERE id = @id";
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@quan_trong", isImportant),
                    new System.Data.SqlClient.SqlParameter("@id", documentId)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    lblStatus.Text = isImportant ? "Đã đánh dấu quan trọng" : "Đã bỏ đánh dấu quan trọng";
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
                LoadData();
            }
        }

        /// <summary>
        /// Toggle quan trọng cho tài liệu được chọn (dùng cho Context Menu)
        /// </summary>
        private void ToggleImportantForSelectedDocument()
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu!");
                return;
            }

            try
            {
                int id = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
                var quanTrongCell = dgvDocuments.SelectedRows[0].Cells["quan_trong"].Value;
                bool currentValue = quanTrongCell != null && quanTrongCell != DBNull.Value && Convert.ToBoolean(quanTrongCell);
                bool newValue = !currentValue;

                SaveImportantValue(id, newValue);
                LoadData();
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        #region Advanced Filter Methods

        /// <summary>
        /// Load danh sách users để filter - ĐÃ XÓA vì chế độ cá nhân
        /// </summary>
        private void LoadUsersForFilter()
        {
            // Chế độ cá nhân: Không cần filter theo người tạo
            // Mọi user chỉ thấy tài liệu của mình
            if (lblCreatorFilter != null) lblCreatorFilter.Visible = false;
            if (cboCreatorFilter != null) cboCreatorFilter.Visible = false;
        }

        /// <summary>
        /// Áp dụng filter nâng cao
        /// </summary>
        private void ApplyAdvancedFilter()
        {
            try
            {
                string keyword = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim();
                string monHoc = cboSubject.SelectedItem?.ToString();
                if (monHoc == "Tất cả") monHoc = null;
                string loai = cboType.SelectedItem?.ToString();
                if (loai == "Tất cả") loai = null;
                
                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (chkEnableDateFilter.Checked)
                {
                    fromDate = dtpFromDate.Value.Date;
                    toDate = dtpToDate.Value.Date;
                }
                
                double? minSize = null;
                double? maxSize = null;
                if (chkEnableSizeFilter.Checked)
                {
                    minSize = (double)nudMinSize.Value;
                    maxSize = (double)nudMaxSize.Value;
                    if (minSize > maxSize)
                    {
                        ToastNotification.Warning("Dung lượng tối thiểu không được lớn hơn dung lượng tối đa!");
                        return;
                    }
                }
                
                bool? isImportant = chkImportantOnly.Checked ? (bool?)true : null;
                
                // Chế độ cá nhân: Không cần filter theo người tạo
                int? creatorUserId = null;
                
                lblStatus.Text = "Đang lọc dữ liệu...";
                Application.DoEvents();
                
                DataTable dt = DatabaseHelper.SearchDocumentsAdvanced(
                    keyword, monHoc, loai, fromDate, toDate,
                    minSize, maxSize, isImportant, creatorUserId
                );
                
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                
                lblCount.Text = $"Tìm thấy: {dt.Rows.Count} tài liệu";
                lblStatus.Text = "Đã áp dụng filter";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi khi lọc";
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa tất cả filter về mặc định
        /// </summary>
        private void ClearAdvancedFilter()
        {
            txtSearch.Clear();
            cboSubject.SelectedIndex = 0;
            cboType.SelectedIndex = 0;
            chkEnableDateFilter.Checked = false;
            dtpFromDate.Value = DateTime.Now.AddMonths(-1);
            dtpToDate.Value = DateTime.Now;
            chkEnableSizeFilter.Checked = false;
            nudMinSize.Value = 0;
            nudMaxSize.Value = 100;
            chkImportantOnly.Checked = false;
            
            if (cboCreatorFilter.Visible && cboCreatorFilter.Items.Count > 0)
            {
                cboCreatorFilter.SelectedIndex = 0;
            }
            
            LoadData();
            lblStatus.Text = "Đã xóa filter";
        }

        /// <summary>
        /// Enable/Disable date pickers
        /// </summary>
        private void chkEnableDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkEnableDateFilter.Checked;
            dtpFromDate.Enabled = enabled;
            dtpToDate.Enabled = enabled;
        }

        /// <summary>
        /// Enable/Disable numeric updowns
        /// </summary>
        private void chkEnableSizeFilter_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkEnableSizeFilter.Checked;
            nudMinSize.Enabled = enabled;
            nudMaxSize.Enabled = enabled;
        }

        /// <summary>
        /// Button Áp dụng Filter - Click
        /// </summary>
        private void btnApplyAdvancedFilter_Click(object sender, EventArgs e)
        {
            ApplyAdvancedFilter();
        }

        /// <summary>
        /// Button Xóa Filter - Click
        /// </summary>
        private void btnClearAdvancedFilter_Click(object sender, EventArgs e)
        {
            ClearAdvancedFilter();
        }

        #endregion

        #region Context Menu Event Handlers

        /// <summary>
        /// Context Menu: Mo file
        /// </summary>
        private void ctxMenuOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        /// <summary>
        /// Context Menu: Sua
        /// </summary>
        private void ctxMenuEdit_Click(object sender, EventArgs e)
        {
            btn_sua_Click(sender, e);
        }

        /// <summary>
        /// Context Menu: Xoa
        /// </summary>
        private void ctxMenuDelete_Click(object sender, EventArgs e)
        {
            btn_xoa_Click(sender, e);
        }

        /// <summary>
        /// Context Menu: Copy đường dẫn file
        /// </summary>
        private void ctxMenuCopyPath_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu!");
                return;
            }

            var duongDan = dgvDocuments.SelectedRows[0].Cells["duong_dan"].Value;
            if (duongDan != null && duongDan != DBNull.Value)
            {
                Clipboard.SetText(duongDan.ToString());
                ToastNotification.Success("Đã copy đường dẫn vào clipboard");
            }
            else
            {
                ToastNotification.Warning("Tài liệu không có đường dẫn file!");
            }
        }

        /// <summary>
        /// Context Menu: Mở thư mục chứa file
        /// </summary>
        private void ctxMenuOpenFolder_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu!");
                return;
            }

            var duongDan = dgvDocuments.SelectedRows[0].Cells["duong_dan"].Value;
            if (duongDan != null && duongDan != DBNull.Value)
            {
                string filePath = duongDan.ToString();
                if (File.Exists(filePath))
                {
                    string folderPath = Path.GetDirectoryName(filePath);
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                    lblStatus.Text = "Đã mở thư mục chứa file";
                }
                else
                {
                    ToastNotification.Error("File không tồn tại!");
                }
            }
            else
            {
                ToastNotification.Warning("Tài liệu không có đường dẫn file!");
            }
        }

        /// <summary>
        /// Context Menu: Đánh dấu/ Bỏ đánh dấu quan trọng
        /// </summary>
        private void ctxMenuToggleImportant_Click(object sender, EventArgs e)
        {
            ToggleImportantForSelectedDocument();
        }

        #endregion

        #region Menu Event Handlers

        /// <summary>
        /// Menu: Kiểm tra file bị thiếu
        /// </summary>
        private void menuCheckFiles_Click(object sender, EventArgs e)
        {
            FileIntegrityCheckForm form = new FileIntegrityCheckForm();
            form.ShowDialog();
            // Refresh data sau khi dong form
            LoadData();
        }

        /// <summary>
        /// Menu: Sắp đến hạn (Phase 2)
        /// </summary>
        private void menuUpcomingDeadlines_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetUpcomingDeadlines(7);
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                
                lblCount.Text = $"Sắp đến hạn: {dt.Rows.Count} tài liệu";
                lblStatus.Text = "Đang xem tài liệu sắp đến hạn (7 ngày tới)";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Menu: Tài liệu quá hạn (Phase 2)
        /// </summary>
        private void menuOverdue_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetOverdueDocuments();
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                
                lblCount.Text = $"Quá hạn: {dt.Rows.Count} tài liệu";
                lblStatus.Text = "Đang xem tài liệu đã quá hạn";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Menu: Quản lý bộ sưu tập (Phase 2)
        /// </summary>
        private void menuCollections_Click(object sender, EventArgs e)
        {
            CollectionManagementForm form = new CollectionManagementForm();
            form.ShowDialog();
        }

        #endregion

        private void menuView_Click(object sender, EventArgs e)
        {

        }
    }
}