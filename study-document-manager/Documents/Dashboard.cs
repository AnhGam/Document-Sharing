using study_document_manager.Core.Entities;
using study_document_manager.Core.Interfaces;
using study_document_manager.Infrastructure.Repositories;
using study_document_manager.UI;
using study_document_manager.UI.Presenters;
using study_document_manager.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class Dashboard : Form, IDashboardView
    {
        private readonly DashboardPresenter _presenter;
        private readonly IDocumentRepository _repository;

        // Implementation of IDashboardView Properties
        public string SearchKeyword => txtSearch.Text.Trim();
        public string SelectedSubject => cboSubject.SelectedItem?.ToString();
        public string SelectedType => cboType.SelectedItem?.ToString();
        public DateTime? FilterFromDate => chkEnableDateFilter.Checked ? dtpFromDate.Value.Date : (DateTime?)null;
        public DateTime? FilterToDate => chkEnableDateFilter.Checked ? dtpToDate.Value.Date : (DateTime?)null;
        public double? FilterMinSize => chkEnableSizeFilter.Checked ? (double?)nudMinSize.Value : (double?)null;
        public double? FilterMaxSize => chkEnableSizeFilter.Checked ? (double?)nudMaxSize.Value : (double?)null;
        public bool FilterIsImportant => chkImportantOnly.Checked;

        // Events
        public event EventHandler SearchRequested;
        public event EventHandler FilterApplied;
        public event EventHandler RefreshRequested;
        public event EventHandler<int> DeleteRequested;
        public event EventHandler<int> EditRequested;
        public event EventHandler AddRequested;
        public event EventHandler<string> OpenFileRequested;
        public event EventHandler ExportRequested;

        private study_document_manager.UI.Controls.DocumentPreviewPanel previewPanel;
        private SplitContainer splitPreview;
        private DoubleBufferedTreeView treeCategory;
        private SplitContainer splitCategory;
        private TreeNode _hoveredNode;
        private Dictionary<string, Bitmap> _treeIconCache = new Dictionary<string, Bitmap>();

        public Dashboard()
        {
            InitializeComponent();

            // Initialize Repository and Presenter
            // In a real DI container scenario, these would be injected.
            _repository = new DocumentRepository();
            _presenter = new DashboardPresenter(this, _repository);

            // Register internal events
            RegisterEvents();

            // Setup additional UI logic
            CreateManagementMenu();
            SetupDataGridView();
            EnableDragDrop();
            SetupContextMenu();

            // Context menu logic
            AddPersonalNoteContextMenu();

            // Preview panel (hidden by default)
            SetupPreviewPanel();

            // Category tree (left sidebar)
            SetupCategoryTree();
        }

        private void AddPersonalNoteContextMenu()
        {
            contextMenuDocument.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem ctxMenuPersonalNote = new ToolStripMenuItem("Ghi chú cá nhân...");
            ctxMenuPersonalNote.Click += ctxMenuPersonalNote_Click;
            contextMenuDocument.Items.Add(ctxMenuPersonalNote);

            ToolStripMenuItem ctxMenuAddToCollection = new ToolStripMenuItem("Thêm vào bộ sưu tập...");
            ctxMenuAddToCollection.Click += ctxMenuAddToCollection_Click;
            contextMenuDocument.Items.Add(ctxMenuAddToCollection);
        }

        private void RegisterEvents()
        {
            // Forward UI events to Presenter via Interface Events
            btnSearch.Click += (s, e) => SearchRequested?.Invoke(this, EventArgs.Empty);
            txtSearch.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; SearchRequested?.Invoke(this, EventArgs.Empty); } };

            cboSubject.SelectedIndexChanged += (s, e) => FilterApplied?.Invoke(this, EventArgs.Empty);
            cboType.SelectedIndexChanged += (s, e) => FilterApplied?.Invoke(this, EventArgs.Empty);
            btnApplyAdvancedFilter.Click += (s, e) => FilterApplied?.Invoke(this, EventArgs.Empty);

            toolBtnRefresh.Click += (s, e) => TriggerRefresh();
            menuEditRefresh.Click += (s, e) => TriggerRefresh();

            // Clean up old filter
            btnClearAdvancedFilter.Click += (s, e) => {
                ClearUIFilters();
                TriggerRefresh();
            };

            // Raise interface events for Add, Edit, Open, Export (required by IDashboardView contract)
            toolBtnNew.Click += (s, e) => AddRequested?.Invoke(this, EventArgs.Empty);
            toolBtnEdit.Click += (s, e) =>
            {
                var doc = dgvDocuments.SelectedRows.Count > 0
                    ? dgvDocuments.SelectedRows[0].DataBoundItem as Core.Entities.StudyDocument
                    : null;
                EditRequested?.Invoke(this, doc?.Id ?? -1);
            };
            toolBtnOpen.Click += (s, e) =>
            {
                var doc = dgvDocuments.SelectedRows.Count > 0
                    ? dgvDocuments.SelectedRows[0].DataBoundItem as Core.Entities.StudyDocument
                    : null;
                OpenFileRequested?.Invoke(this, doc?.DuongDan ?? string.Empty);
            };
            toolBtnExport.Click += (s, e) => ExportRequested?.Invoke(this, EventArgs.Empty);
        }

        private void TriggerRefresh()
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
            PopulateCategoryTree();
        }

        private void ClearUIFilters()
        {
            txtSearch.Clear();
            if(cboSubject.Items.Count > 0) cboSubject.SelectedIndex = 0;
            if(cboType.Items.Count > 0) cboType.SelectedIndex = 0;
            chkEnableDateFilter.Checked = false;
            chkEnableSizeFilter.Checked = false;
            chkImportantOnly.Checked = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.N:
                    btn_them_Click(null, null);
                    return true;
                case Keys.Control | Keys.F:
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    return true;
                case Keys.Delete:
                    if (dgvDocuments.Focused && dgvDocuments.SelectedRows.Count > 0)
                    {
                        btn_xoa_Click(null, null);
                        return true;
                    }
                    break;
                case Keys.F5:
                    TriggerRefresh();
                    return true;
                case Keys.Control | Keys.E:
                    btn_xuat_Click(null, null);
                    return true;
                case Keys.Control | Keys.O:
                    OpenSelectedFile();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            this.Text = "Study Document Manager - Professional Edition";

            // Load app icon for taskbar & title bar
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "assets", "logo", "icons", "icon-48.png");
                if (System.IO.File.Exists(iconPath))
                {
                    using (var bmp = new System.Drawing.Bitmap(iconPath))
                    {
                        this.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());
                    }
                }
            }
            catch { }

            // Icons
            toolBtnNew.Image = IconHelper.CreateAddIcon(16, AppTheme.StatusSuccess);
            toolBtnNew.ToolTipText = "Thêm tài liệu (Ctrl+N)";
            toolBtnEdit.Image = IconHelper.CreateEditIcon(16, AppTheme.Primary);
            toolBtnEdit.ToolTipText = "Sửa tài liệu";
            toolBtnDelete.Image = IconHelper.CreateDeleteIcon(16, AppTheme.StatusError);
            toolBtnDelete.ToolTipText = "Xóa tài liệu (Del)";
            toolBtnOpen.Image = IconHelper.CreateOpenIcon(16, AppTheme.AccentSky);
            toolBtnOpen.ToolTipText = "Mở file (Ctrl+O)";
            toolBtnExport.Image = IconHelper.CreateExportIcon(16, AppTheme.AccentOrange);
            toolBtnExport.ToolTipText = "Xuất CSV (Ctrl+E)";
            toolBtnRefresh.Image = IconHelper.CreateRefreshIcon(16, AppTheme.Secondary);
            toolBtnRefresh.ToolTipText = "Làm mới (F5)";

            // Add Import & Recycle Bin buttons to toolbar
            toolStrip.Items.Add(new ToolStripSeparator());

            var toolBtnImport = new ToolStripButton
            {
                Text = "Import",
                Image = IconHelper.CreateImportIcon(16, AppTheme.StatusSuccess),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Import tài liệu từ thư mục"
            };
            toolBtnImport.Click += (s, ev) => { new Documents.BatchImportForm().ShowDialog(); TriggerRefresh(); };
            toolStrip.Items.Add(toolBtnImport);

            var toolBtnRecycleBin = new ToolStripButton
            {
                Text = "Thùng rác",
                Image = IconHelper.CreateRecycleBinIcon(16, AppTheme.StatusWarning),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Mở thùng rác"
            };
            toolBtnRecycleBin.Click += (s, ev) => { new Management.RecycleBinForm().ShowDialog(); TriggerRefresh(); };
            toolStrip.Items.Add(toolBtnRecycleBin);

            var toolBtnBulkOps = new ToolStripButton
            {
                Text = "Hàng loạt",
                Image = IconHelper.CreateChecklistIcon(16, AppTheme.StatusInfo),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Quản lý hàng loạt (xóa, đổi môn học, đánh dấu)"
            };
            toolBtnBulkOps.Click += (s, ev) =>
            {
                using (var form = new study_document_manager.Documents.BulkDeleteForm())
                {
                    form.ShowDialog(this);
                    if (form.DataChanged) TriggerRefresh();
                }
            };
            toolStrip.Items.Add(toolBtnBulkOps);

            // Thêm nút Mở gần đây
            var toolBtnRecent = new ToolStripButton
            {
                Text = "Gần đây",
                Image = IconHelper.CreateClockIcon(16, AppTheme.AccentSky),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Xem tài liệu mở gần đây"
            };
            toolBtnRecent.Click += (s, ev) => { new study_document_manager.Documents.RecentFilesForm().ShowDialog(); };
            toolStrip.Items.Add(toolBtnRecent);

            // Thêm nút Sao lưu
            var toolBtnBackup = new ToolStripButton
            {
                Text = "Sao lưu",
                Image = IconHelper.CreateBackupIcon(16, AppTheme.StatusInfo),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Sao lưu nhanh database"
            };
            toolBtnBackup.Click += menuBackup_Click;
            toolStrip.Items.Add(toolBtnBackup);

            // Thêm nút Trùng lặp
            var toolBtnDuplicates = new ToolStripButton
            {
                Text = "Trùng lặp",
                Image = IconHelper.CreateDuplicateIcon(16, AppTheme.StatusWarning),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Phát hiện file trùng lặp"
            };
            toolBtnDuplicates.Click += (s, ev) => { new study_document_manager.Documents.DuplicateDetectionForm().ShowDialog(); TriggerRefresh(); };
            toolStrip.Items.Add(toolBtnDuplicates);

            // Thêm nút Thống kê
            var toolBtnStats = new ToolStripButton
            {
                Text = "Thống kê",
                Image = IconHelper.CreateChartIcon(16, AppTheme.Primary),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Xem thống kê tài liệu"
            };
            toolBtnStats.Click += btn_thong_ke_Click;
            toolStrip.Items.Add(toolBtnStats);

            // Them nut TreeMap
            var toolBtnTreeMap = new ToolStripButton
            {
                Text = "TreeMap",
                Image = IconHelper.CreateTreeMapIcon(16, AppTheme.AccentSky),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Xem phân bố tài liệu dạng TreeMap"
            };
            toolBtnTreeMap.Click += (s, ev) => { new TreeMapForm().ShowDialog(); };
            toolStrip.Items.Add(toolBtnTreeMap);

            HideCreatorFilter();

            // Initialize Presenter
            _presenter.Initialize();

            ToastNotification.Success("Hệ thống đã sẵn sàng!");

            // Auto-check for updates (background, non-blocking)
            CheckForUpdatesAsync();
        }

        private async void CheckForUpdatesAsync()
        {
            try
            {
                var updateInfo = await UpdateChecker.CheckForUpdateAsync();
                if (updateInfo != null && updateInfo.HasUpdate)
                {
                    toolBtnUpdate.Text = $"Cập nhật {updateInfo.NewVersion}";
                    toolBtnUpdate.ToolTipText = $"Phiên bản mới {updateInfo.NewVersion} đã có sẵn. Nhấn để cập nhật.";
                    toolBtnUpdate.Image = IconHelper.CreateUpdateIcon(16, Color.White);
                    toolBtnUpdate.Tag = updateInfo;
                    toolBtnUpdate.Visible = true;
                    toolSeparator3.Visible = true;
                }
            }
            catch { /* Silently ignore update check failures */ }
        }

        private void toolBtnUpdate_Click(object sender, EventArgs e)
        {
            var updateInfo = toolBtnUpdate.Tag as UpdateInfo;
            if (updateInfo == null) return;

            if (!string.IsNullOrEmpty(updateInfo.DownloadUrl))
            {
                UpdateInstaller.DownloadAndInstall(updateInfo.DownloadUrl, updateInfo.NewVersion, this);
            }
            else
            {
                // Fallback: open release page in browser
                if (!string.IsNullOrEmpty(updateInfo.ReleasePageUrl))
                    System.Diagnostics.Process.Start(updateInfo.ReleasePageUrl);
            }
        }

        // --- IDashboardView Implementation ---

        public void SetDocumentList(List<StudyDocument> documents)
        {
            // Convert List<StudyDocument> to DataTable or BindingList for Grid
            // For compatibility with existing Grid setup, we can use a BindingList or map to DataTable
            // Ideally, we should bind directly to List<StudyDocument>, but the grid column names need to match properties.

            // The existing Grid expects columns: "id", "ten", "mon_hoc", etc.
            // Entity has: "Id", "Ten", "MonHoc".
            // We need to map or configure AutoGenerateColumns = true and hide unnecessary ones.
            // OR create a helper to convert List<Entity> to DataTable with legacy column names to minimize Grid changes.

            // Let's try binding directly list and see if we can map columns via DataPropertyName in Designer or Code.
            // To be safe and quick without breaking the Designer-generated columns too much:

            var bindingList = new System.ComponentModel.BindingList<StudyDocument>(documents);
            dgvDocuments.DataSource = bindingList;

            // Re-setup grid to map columns if needed, or rely on AutoGenerate if columns are not manually defined.
            // Since existing grid has columns defined, we need to ensure DataPropertyName matches.
            // Current Grid columns likely have DataPropertyName = "ten", "mon_hoc" (lowercase from DataTable).
            // Entity properties are "Ten", "MonHoc" (PascalCase).
            // WinForms DataBinding is case-insensitive usually, but let's check.

            // Fix column mappings programmatically
            if (dgvDocuments.Columns.Contains("id")) dgvDocuments.Columns["id"].DataPropertyName = "Id";
            if (dgvDocuments.Columns.Contains("ten")) dgvDocuments.Columns["ten"].DataPropertyName = "Ten";
            if (dgvDocuments.Columns.Contains("mon_hoc")) dgvDocuments.Columns["mon_hoc"].DataPropertyName = "MonHoc";
            if (dgvDocuments.Columns.Contains("loai")) dgvDocuments.Columns["loai"].DataPropertyName = "Loai";
            if (dgvDocuments.Columns.Contains("duong_dan")) dgvDocuments.Columns["duong_dan"].DataPropertyName = "DuongDan";
            if (dgvDocuments.Columns.Contains("ghi_chu")) dgvDocuments.Columns["ghi_chu"].DataPropertyName = "GhiChu";
            if (dgvDocuments.Columns.Contains("ngay_them")) dgvDocuments.Columns["ngay_them"].DataPropertyName = "NgayThem";
            if (dgvDocuments.Columns.Contains("kich_thuoc")) dgvDocuments.Columns["kich_thuoc"].DataPropertyName = "KichThuoc";
            if (dgvDocuments.Columns.Contains("tac_gia")) dgvDocuments.Columns["tac_gia"].DataPropertyName = "TacGia";
            if (dgvDocuments.Columns.Contains("quan_trong")) dgvDocuments.Columns["quan_trong"].DataPropertyName = "QuanTrong";
            if (dgvDocuments.Columns.Contains("tags")) dgvDocuments.Columns["tags"].DataPropertyName = "Tags";
            if (dgvDocuments.Columns.Contains("deadline")) dgvDocuments.Columns["deadline"].DataPropertyName = "Deadline";

            SetupDataGridView();
        }

        public void SetSubjects(List<string> subjects)
        {
            cboSubject.DataSource = subjects;
        }

        public void SetTypes(List<string> types)
        {
            cboType.DataSource = types;
        }

        public void UpdateStatusCount(int count)
        {
            lblCount.Text = $"Tổng số: {count} tài liệu";
            lblStatus.Text = "Sẵn sàng";
        }

        public void ShowMessage(string message)
        {
            ToastNotification.Success(message);
            lblStatus.Text = message;
        }

        public void ShowError(string message)
        {
            ToastNotification.Error(message);
            lblStatus.Text = "Lỗi: " + message;
        }

        public bool ConfirmDelete()
        {
            return MessageBox.Show("Bạn có chắc chắn muốn xóa tài liệu này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        // --- Helper Methods ---

        private void SetupDataGridView()
        {
             // Keep existing formatting logic but ensure it works with Objects
             if (dgvDocuments.Columns.Count > 0)
            {
                // Ensure ID is hidden
                if (dgvDocuments.Columns.Contains("id")) dgvDocuments.Columns["id"].Visible = false;
                if (dgvDocuments.Columns.Contains("Id")) dgvDocuments.Columns["Id"].Visible = false;

                // Add Icon column if missing
                if (!dgvDocuments.Columns.Contains("Icon"))
                {
                    DataGridViewImageColumn iconColumn = new DataGridViewImageColumn
                    {
                        Name = "Icon",
                        HeaderText = "",
                        Width = 24,
                        MinimumWidth = 24,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Resizable = DataGridViewTriState.False
                    };
                    dgvDocuments.Columns.Insert(0, iconColumn);
                }

                // Set Vietnamese HeaderText with diacritics
                SetColumnHeader("ten", "Tên tài liệu");
                SetColumnHeader("Ten", "Tên tài liệu");
                SetColumnHeader("mon_hoc", "Danh mục");
                SetColumnHeader("MonHoc", "Danh mục");
                SetColumnHeader("loai", "Loại");
                SetColumnHeader("Loai", "Loại");
                SetColumnHeader("duong_dan", "Đường dẫn");
                SetColumnHeader("DuongDan", "Đường dẫn");
                SetColumnHeader("ghi_chu", "Ghi chú");
                SetColumnHeader("GhiChu", "Ghi chú");
                SetColumnHeader("ngay_them", "Ngày thêm");
                SetColumnHeader("NgayThem", "Ngày thêm");
                SetColumnHeader("kich_thuoc", "Kích thước (MB)");
                SetColumnHeader("KichThuoc", "Kích thước (MB)");
                SetColumnHeader("tac_gia", "Tác giả");
                SetColumnHeader("TacGia", "Tác giả");
                SetColumnHeader("quan_trong", "★");
                SetColumnHeader("QuanTrong", "★");
                SetColumnHeader("tags", "Tags");
                SetColumnHeader("Tags", "Tags");
                SetColumnHeader("deadline", "Hạn chót");
                SetColumnHeader("Deadline", "Hạn chót");

                // Ẩn cột ít quan trọng, giữ cột cần thiết
                HideColumn("duong_dan"); HideColumn("DuongDan");
                HideColumn("ghi_chu"); HideColumn("GhiChu");
                HideColumn("tac_gia"); HideColumn("TacGia");
                HideColumn("tags"); HideColumn("Tags");
                HideColumn("mon_hoc"); HideColumn("MonHoc");

                // Set FillWeight cho các cột hiển thị
                SetColumnFillWeight("ten", 30); SetColumnFillWeight("Ten", 30);
                SetColumnFillWeight("loai", 12); SetColumnFillWeight("Loai", 12);
                SetColumnFillWeight("ngay_them", 15); SetColumnFillWeight("NgayThem", 15);
                SetColumnFillWeight("kich_thuoc", 12); SetColumnFillWeight("KichThuoc", 12);
                SetColumnFillWeight("quan_trong", 5); SetColumnFillWeight("QuanTrong", 5);
                SetColumnFillWeight("deadline", 15); SetColumnFillWeight("Deadline", 15);
            }

            // Styling
            AppTheme.ApplyDataGridViewStyle(dgvDocuments);

            // Allow checkbox interaction for QuanTrong
            dgvDocuments.ReadOnly = false;
            foreach (DataGridViewColumn col in dgvDocuments.Columns)
            {
                col.ReadOnly = (col.Name != "QuanTrong");
            }

            // Fix QuanTrong padding for proper checkbox rendering
            if (dgvDocuments.Columns.Contains("QuanTrong"))
            {
                dgvDocuments.Columns["QuanTrong"].DefaultCellStyle.Padding = new Padding(0);
                dgvDocuments.Columns["QuanTrong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Force Icon to be first column
            if (dgvDocuments.Columns.Contains("Icon"))
            {
                dgvDocuments.Columns["Icon"].DisplayIndex = 0;
            }

            // Register events (unsubscribe first to prevent duplicates)
            dgvDocuments.CellFormatting -= dgvDocuments_CellFormatting;
            dgvDocuments.CellFormatting += dgvDocuments_CellFormatting;
        }

        private void SetColumnHeader(string columnName, string headerText)
        {
            if (dgvDocuments.Columns.Contains(columnName))
            {
                dgvDocuments.Columns[columnName].HeaderText = headerText;
            }
        }

        private void HideColumn(string columnName)
        {
            if (dgvDocuments.Columns.Contains(columnName))
                dgvDocuments.Columns[columnName].Visible = false;
        }

        private void SetColumnFillWeight(string columnName, float weight)
        {
            if (dgvDocuments.Columns.Contains(columnName))
                dgvDocuments.Columns[columnName].FillWeight = weight;
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            this.ForeColor = AppTheme.TextPrimary;
            this.Font = AppTheme.FontBody;

            // Menu and toolbar
            AppTheme.ApplyMenuStripStyle(menuStrip);
            AppTheme.ApplyToolStripStyle(toolStrip);
            AppTheme.ApplyStatusStripStyle(statusStrip);

            // Toolbar bottom border (Win11 style divider)
            toolStrip.Paint += (s, e) =>
            {
                using (var pen = new Pen(AppTheme.BorderLight, 1))
                    e.Graphics.DrawLine(pen, 0, toolStrip.Height - 1, toolStrip.Width, toolStrip.Height - 1);
            };

            // Search panel: white card surface
            pnlSearch.BackColor = AppTheme.BackgroundCard;
            pnlContent.BackColor = AppTheme.BackgroundMain;

            // Buttons
            AppTheme.ApplyButtonPrimary(btnSearch);
            AppTheme.ApplyButtonPrimary(btnApplyAdvancedFilter);
            AppTheme.ApplyButtonSecondary(btnClearAdvancedFilter);

            // Labels
            lblStatus.ForeColor = AppTheme.TextSecondary;
            lblCount.ForeColor = AppTheme.TextSecondary;
            lblStatus.Font = AppTheme.FontCaption;
            lblCount.Font = AppTheme.FontCaption;
        }

        private void HideCreatorFilter()
        {
            if (lblCreatorFilter != null) lblCreatorFilter.Visible = false;
            if (cboCreatorFilter != null) cboCreatorFilter.Visible = false;
        }

        // --- Event Handlers for Buttons (Routing to Logic) ---

        private void btn_them_Click(object sender, EventArgs e)
        {
            AddEditForm form = new AddEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                TriggerRefresh();
                ShowMessage("Đã thêm tài liệu mới");
            }
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0) return;
            // Get object from row bound item
            var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
            if (doc == null) return;

            AddEditForm form = new AddEditForm(doc.Id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                TriggerRefresh();
                ShowMessage("Đã cập nhật tài liệu");
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0) return;
            var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
            if (doc != null)
            {
                DeleteRequested?.Invoke(this, doc.Id);
            }
        }

        private void btn_mo_file_Click(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        private void OpenSelectedFile()
        {
            if (dgvDocuments.SelectedRows.Count == 0) return;
            var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
            if (doc != null && !string.IsNullOrEmpty(doc.DuongDan))
            {
                if (File.Exists(doc.DuongDan))
                {
                    try {
                        System.Diagnostics.Process.Start(doc.DuongDan);
                        lblStatus.Text = "Đã mở file: " + doc.Ten;
                        try { DatabaseHelper.AddRecentFile(doc.Id); } catch { }
                    } catch (Exception ex) { ShowError(ex.Message); }
                }
                else
                {
                    ShowError("File không tồn tại!");
                }
            }
        }

        private void dgv_tai_lieu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) OpenSelectedFile();
        }

        // --- Other legacy handlers (kept for functionality preservation) ---

        private void dgvDocuments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                string colName = dgvDocuments.Columns[e.ColumnIndex].Name;

                var doc = dgvDocuments.Rows[e.RowIndex].DataBoundItem as StudyDocument;
                if (doc == null) return;

                if (colName == "Icon")
                {
                    e.Value = IconHelper.GetDocumentIcon(doc.Loai, 24, doc.DuongDan);
                    e.FormattingApplied = true;
                }
                else if ((colName == "deadline" || colName == "Deadline") && doc.Deadline.HasValue)
                {
                    int daysLeft = (doc.Deadline.Value.Date - DateTime.Now.Date).Days;
                    if (daysLeft < 0) { e.CellStyle.BackColor = AppTheme.ValidationErrorLight; e.CellStyle.ForeColor = AppTheme.StatusError; }
                    else if (daysLeft <= 3) { e.CellStyle.BackColor = AppTheme.ValidationWarningLight; e.CellStyle.ForeColor = AppTheme.StatusWarning; }
                    else if (daysLeft <= 7) { e.CellStyle.BackColor = AppTheme.ValidationWarningLight; }
                }
            }
            catch
            {
                // Ignore formatting errors
            }
        }

        private void dgvDocuments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string colName = dgvDocuments.Columns[e.ColumnIndex].Name;
            if (colName == "quan_trong" || colName == "QuanTrong")
            {
                dgvDocuments.CommitEdit(DataGridViewDataErrorContexts.Commit);
                var doc = dgvDocuments.Rows[e.RowIndex].DataBoundItem as StudyDocument;
                if (doc != null)
                {
                    // doc.QuanTrong is already updated by CommitEdit
                    _repository.Update(doc);
                    dgvDocuments.InvalidateRow(e.RowIndex);
                    // Show message based on the new value
                    ShowMessage(doc.QuanTrong ? "Đã đánh dấu quan trọng" : "Đã bỏ đánh dấu quan trọng");
                }
            }
        }

        private void dgvDocuments_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }

        private void ctxMenuOpen_Click(object sender, EventArgs e) => OpenSelectedFile();
        private void ctxMenuEdit_Click(object sender, EventArgs e) => btn_sua_Click(sender, e);
        private void ctxMenuDelete_Click(object sender, EventArgs e) => btn_xoa_Click(sender, e);

        private void ctxMenuCopyPath_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count > 0)
            {
                var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
                if (doc != null && !string.IsNullOrEmpty(doc.DuongDan)) Clipboard.SetText(doc.DuongDan);
            }
        }

        private void ctxMenuOpenFolder_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count > 0)
            {
                var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
                if (doc != null && File.Exists(doc.DuongDan))
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{doc.DuongDan}\"");
            }
        }

        private void ctxMenuToggleImportant_Click(object sender, EventArgs e)
        {
             if (dgvDocuments.SelectedRows.Count > 0)
            {
                var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
                if (doc != null)
                {
                    doc.QuanTrong = !doc.QuanTrong;
                    _repository.Update(doc);
                    TriggerRefresh();
                }
            }
        }

        private void ctxMenuPersonalNote_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count > 0)
            {
                var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
                if (doc != null)
                {
                    PersonalNoteForm form = new PersonalNoteForm(doc.Id, doc.Ten);
                    form.ShowDialog();
                }
            }
        }

        private void ctxMenuAddToCollection_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài liệu trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var doc = dgvDocuments.SelectedRows[0].DataBoundItem as StudyDocument;
            if (doc == null) return;

            // Get existing collections
            var collections = DatabaseHelper.GetCollections();

            using (var dialog = new Form())
            {
                dialog.Text = "Thêm vào bộ sưu tập";
                dialog.Size = new Size(380, 300);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = AppTheme.BackgroundMain;
                dialog.ShowInTaskbar = false;
                if (this.Icon != null) dialog.Icon = this.Icon;

                var lblInfo = new Label
                {
                    Text = $"Tài liệu: {doc.Ten}",
                    Font = AppTheme.FontSmallBold,
                    ForeColor = AppTheme.TextPrimary,
                    Location = new Point(20, 15),
                    AutoSize = true,
                    MaximumSize = new Size(330, 0)
                };

                var lblSelect = new Label
                {
                    Text = "Chọn bộ sưu tập:",
                    Font = AppTheme.FontSmall,
                    ForeColor = AppTheme.TextSecondary,
                    Location = new Point(20, 50),
                    AutoSize = true
                };

                var lstCollections = new ListBox
                {
                    Location = new Point(20, 72),
                    Size = new Size(325, 100),
                    Font = AppTheme.FontBody,
                    BackColor = AppTheme.InputBackground
                };

                // Populate list
                foreach (DataRow row in collections.Rows)
                {
                    lstCollections.Items.Add(new CollectionItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Name = row["name"].ToString(),
                        Count = Convert.ToInt32(row["item_count"])
                    });
                }

                var lblNew = new Label
                {
                    Text = "Hoặc tạo mới:",
                    Font = AppTheme.FontSmall,
                    ForeColor = AppTheme.TextSecondary,
                    Location = new Point(20, 180),
                    AutoSize = true
                };

                var txtNew = new TextBox
                {
                    Location = new Point(20, 200),
                    Size = new Size(220, 25),
                    Font = AppTheme.FontBody,
                    BackColor = AppTheme.InputBackground,
                    Text = "Tên bộ sưu tập mới...",
                    ForeColor = Color.Gray
                };
                txtNew.GotFocus += (s2, e2) => { if (txtNew.ForeColor == Color.Gray) { txtNew.Text = ""; txtNew.ForeColor = AppTheme.TextPrimary; } };
                txtNew.LostFocus += (s2, e2) => { if (string.IsNullOrWhiteSpace(txtNew.Text)) { txtNew.Text = "Tên bộ sưu tập mới..."; txtNew.ForeColor = Color.Gray; } };

                var btnAdd = new Button
                {
                    Text = "Thêm",
                    Size = new Size(90, 32),
                    Location = new Point(250, 198),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AppTheme.StatusSuccess,
                    ForeColor = Color.White,
                    Font = AppTheme.FontButton,
                    Cursor = Cursors.Hand
                };
                btnAdd.FlatAppearance.BorderSize = 0;

                btnAdd.Click += (s, ev) =>
                {
                    int collectionId = -1;

                    // Create new collection if text entered
                    if (!string.IsNullOrWhiteSpace(txtNew.Text) && txtNew.ForeColor != Color.Gray)
                    {
                        DatabaseHelper.CreateCollection(txtNew.Text.Trim(), "");
                        // Get new collection id
                        var updated = DatabaseHelper.GetCollections();
                        foreach (DataRow row in updated.Rows)
                        {
                            if (row["name"].ToString() == txtNew.Text.Trim())
                            {
                                collectionId = Convert.ToInt32(row["id"]);
                                break;
                            }
                        }
                    }
                    else if (lstCollections.SelectedItem is CollectionItem selected)
                    {
                        collectionId = selected.Id;
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn hoặc tạo bộ sưu tập.", "Thông báo");
                        return;
                    }

                    if (collectionId > 0)
                    {
                        bool added = DatabaseHelper.AddDocumentToCollection(collectionId, doc.Id);
                        if (added)
                        {
                            ToastNotification.Success($"Đã thêm vào bộ sưu tập!");
                            dialog.Close();
                        }
                        else
                        {
                            MessageBox.Show("Tài liệu đã có trong bộ sưu tập này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                };

                dialog.Controls.AddRange(new Control[] { lblInfo, lblSelect, lstCollections, lblNew, txtNew, btnAdd });
                dialog.ShowDialog(this);
            }
        }

        private void menuCheckFiles_Click(object sender, EventArgs e) { new FileIntegrityCheckForm().ShowDialog(); TriggerRefresh(); }
        private void menuUpcomingDeadlines_Click(object sender, EventArgs e)
        {
            var docs = _repository.GetUpcomingDeadlines(7);
            SetDocumentList(docs);
            UpdateStatusCount(docs.Count);
            lblStatus.Text = "Sắp đến hạn (7 ngày)";
        }
        private void menuOverdue_Click(object sender, EventArgs e)
        {
            var docs = _repository.GetOverdueDocuments();
            SetDocumentList(docs);
            UpdateStatusCount(docs.Count);
            lblStatus.Text = "Tài liệu quá hạn";
        }
        private void menuCollections_Click(object sender, EventArgs e) { new CollectionManagementForm().ShowDialog(); }
        private void menuRecycleBin_Click(object sender, EventArgs e) { new Management.RecycleBinForm().ShowDialog(); TriggerRefresh(); }
        private void menuBatchImport_Click(object sender, EventArgs e) { new Documents.BatchImportForm().ShowDialog(); TriggerRefresh(); }
        private void menuFileExit_Click(object sender, EventArgs e) => Application.Exit();
        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new Form())
            {
                aboutForm.Text = "Giới thiệu";
                aboutForm.Size = new Size(420, 360);
                aboutForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                aboutForm.StartPosition = FormStartPosition.CenterParent;
                aboutForm.MaximizeBox = false;
                aboutForm.MinimizeBox = false;
                aboutForm.BackColor = AppTheme.BackgroundMain;
                aboutForm.ShowInTaskbar = false;
                if (this.Icon != null) aboutForm.Icon = this.Icon;

                var lblAppName = new Label
                {
                    Text = "Study Document Manager",
                    Font = new Font(AppTheme.FontFamily, 18F, FontStyle.Bold),
                    ForeColor = AppTheme.Primary,
                    Location = new Point(30, 25),
                    AutoSize = true
                };

                var lblEdition = new Label
                {
                    Text = "Professional Edition",
                    Font = new Font(AppTheme.FontFamily, 10F, FontStyle.Italic),
                    ForeColor = AppTheme.TextSecondary,
                    Location = new Point(32, 60),
                    AutoSize = true
                };

                var lblVersion = new Label
                {
                    Text = $"Phiên bản {AppVersion.Current}",
                    Font = AppTheme.FontBody,
                    ForeColor = AppTheme.TextPrimary,
                    Location = new Point(32, 95),
                    AutoSize = true
                };

                var lblDesc = new Label
                {
                    Text = "Ứng dụng quản lý tài liệu học tập\nKiến trúc MVP • .NET Framework 4.8",
                    Font = AppTheme.FontSmall,
                    ForeColor = AppTheme.TextSecondary,
                    Location = new Point(32, 125),
                    AutoSize = true
                };

                var lblStudent = new Label
                {
                    Text = "Sinh viên thực hiện: Vũ Đức Dũng - TT601-K14\nCán bộ hướng dẫn: Lê Thị Mai",
                    Font = AppTheme.FontSmall,
                    ForeColor = AppTheme.TextPrimary,
                    Location = new Point(32, 170),
                    AutoSize = true
                };

                var lblCopyright = new Label
                {
                    Text = "© 2024-2025 hayato-shino05",
                    Font = AppTheme.FontSmall,
                    ForeColor = AppTheme.TextMuted,
                    Location = new Point(32, 215),
                    AutoSize = true
                };

                var lnkGitHub = new LinkLabel
                {
                    Text = "GitHub: hayato-shino05/study-document-manager",
                    Font = AppTheme.FontSmall,
                    Location = new Point(32, 240),
                    AutoSize = true
                };
                lnkGitHub.LinkClicked += (s, ev) =>
                    System.Diagnostics.Process.Start("https://github.com/hayato-shino05/study-document-manager");

                var btnClose = new Button
                {
                    Text = "Đóng",
                    Size = new Size(100, 36),
                    Location = new Point(290, 275),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AppTheme.Primary,
                    ForeColor = Color.White,
                    Font = AppTheme.FontBody,
                    Cursor = Cursors.Hand,
                    DialogResult = DialogResult.OK
                };
                btnClose.FlatAppearance.BorderSize = 0;

                aboutForm.Controls.AddRange(new Control[] {
                    lblAppName, lblEdition, lblVersion, lblDesc,
                    lblStudent, lblCopyright, lnkGitHub, btnClose
                });
                aboutForm.AcceptButton = btnClose;
                aboutForm.ShowDialog(this);
            }
        }
        private void menuViewCategories_Click(object sender, EventArgs e) { new CategoryManagementForm().ShowDialog(); TriggerRefresh(); }
        private void btn_thong_ke_Click(object sender, EventArgs e) { new Report().ShowDialog(); }
        private void btn_xuat_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Xuất dữ liệu";
                sfd.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt";
                sfd.FileName = $"TaiLieuHocTap_{DateTime.Now:yyyyMMdd}";
                sfd.DefaultExt = "csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                        {
                            // Header
                            writer.WriteLine("Tên tài liệu,Môn học,Loại,Tác giả,Ngày thêm,Kích thước (MB),Quan trọng,Tags,Deadline,Đường dẫn");

                            // Data from current binding
                            var docs = dgvDocuments.DataSource as System.ComponentModel.BindingList<StudyDocument>
                                      ?? (System.Collections.IEnumerable)dgvDocuments.DataSource as System.Collections.Generic.IEnumerable<StudyDocument>;
                            if (docs != null)
                            {
                                foreach (var doc in docs)
                                {
                                    string line = string.Join(",",
                                        EscapeCsv(doc.Ten),
                                        EscapeCsv(doc.MonHoc),
                                        EscapeCsv(doc.Loai),
                                        EscapeCsv(doc.TacGia ?? ""),
                                        doc.NgayThem.ToString("dd/MM/yyyy"),
                                        doc.KichThuoc?.ToString("F2") ?? "",
                                        doc.QuanTrong ? "Có" : "Không",
                                        EscapeCsv(doc.Tags ?? ""),
                                        doc.Deadline?.ToString("dd/MM/yyyy") ?? "",
                                        EscapeCsv(doc.DuongDan ?? "")
                                    );
                                    writer.WriteLine(line);
                                }
                            }
                        }

                        int exportedCount = 0;
                        if (dgvDocuments.DataSource is System.ComponentModel.BindingList<StudyDocument> bl) exportedCount = bl.Count;
                        else if (dgvDocuments.DataSource is List<StudyDocument> ll) exportedCount = ll.Count;

                        var openResult = MessageBox.Show(
                            $"Đã xuất thành công {exportedCount} tài liệu!\n\nBạn có muốn mở file?",
                            "Xuất dữ liệu",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (openResult == DialogResult.Yes)
                            System.Diagnostics.Process.Start(sfd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }

        private void CreateManagementMenu()
        {
            try
            {
                // Tìm MenuStrip trên form
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is MenuStrip menuStrip)
                    {
                        // Tìm Menu Công cụ
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
                            menuView.DropDownItems.Add(new ToolStripSeparator());

                            ToolStripMenuItem menuBulkOps = new ToolStripMenuItem("Quản lý hàng loạt...");
                            menuBulkOps.Click += (s, ev) =>
                            {
                                using (var form = new study_document_manager.Documents.BulkDeleteForm())
                                {
                                    form.ShowDialog(this);
                                    if (form.DataChanged) TriggerRefresh();
                                }
                            };
                            menuView.DropDownItems.Add(menuBulkOps);

                            ToolStripMenuItem menuCheckFiles = new ToolStripMenuItem("Kiểm tra file bị thiếu");
                            menuCheckFiles.Click += menuCheckFiles_Click;
                            menuView.DropDownItems.Add(menuCheckFiles);

                            ToolStripMenuItem menuBatchImport = new ToolStripMenuItem("Import từ thư mục...");
                            menuBatchImport.Click += menuBatchImport_Click;
                            menuView.DropDownItems.Add(menuBatchImport);

                            ToolStripMenuItem menuDuplicates = new ToolStripMenuItem("Phát hiện file trùng lặp");
                            menuDuplicates.Click += (s, ev) => { new study_document_manager.Documents.DuplicateDetectionForm().ShowDialog(); TriggerRefresh(); };
                            menuView.DropDownItems.Add(menuDuplicates);

                            // Backup & Restore
                            menuView.DropDownItems.Add(new ToolStripSeparator());

                            ToolStripMenuItem menuBackup = new ToolStripMenuItem("Sao lưu database...");
                            menuBackup.Click += menuBackup_Click;
                            menuView.DropDownItems.Add(menuBackup);

                            ToolStripMenuItem menuRestore = new ToolStripMenuItem("Khôi phục database...");
                            menuRestore.Click += menuRestore_Click;
                            menuView.DropDownItems.Add(menuRestore);
                        }

                        // Tạo Menu Theo dõi
                        ToolStripMenuItem menuTracking = new ToolStripMenuItem("Theo dõi");

                        ToolStripMenuItem menuUpcoming = new ToolStripMenuItem("Sắp đến hạn (7 ngày)");
                        menuUpcoming.Click += menuUpcomingDeadlines_Click;
                        menuTracking.DropDownItems.Add(menuUpcoming);

                        ToolStripMenuItem menuOverdue = new ToolStripMenuItem("Tài liệu quá hạn");
                        menuOverdue.Click += menuOverdue_Click;
                        menuTracking.DropDownItems.Add(menuOverdue);

                        menuTracking.DropDownItems.Add(new ToolStripSeparator());

                        ToolStripMenuItem menuCollections = new ToolStripMenuItem("Quản lý bộ sưu tập");
                        menuCollections.Click += menuCollections_Click;
                        menuTracking.DropDownItems.Add(menuCollections);

                        menuTracking.DropDownItems.Add(new ToolStripSeparator());

                        ToolStripMenuItem menuRecycleBin = new ToolStripMenuItem("Thùng rác");
                        menuRecycleBin.Click += menuRecycleBin_Click;
                        menuTracking.DropDownItems.Add(menuRecycleBin);

                        // Recent Files submenu
                        menuTracking.DropDownItems.Add(new ToolStripSeparator());
                        ToolStripMenuItem menuRecent = new ToolStripMenuItem("Mở gần đây");
                        menuRecent.DropDownOpening += (s, ev) => PopulateRecentFilesMenu(menuRecent);
                        menuTracking.DropDownItems.Add(menuRecent);

                        // Thêm vào MenuStrip (trước Help hoặc cuối cùng)
                        int helpIndex = -1;
                        for (int i = 0; i < menuStrip.Items.Count; i++)
                        {
                            if (menuStrip.Items[i].Name == "menuHelp" || menuStrip.Items[i].Text.Contains("Trợ giúp"))
                            {
                                helpIndex = i;
                                break;
                            }
                        }

                        if (helpIndex >= 0) menuStrip.Items.Insert(helpIndex, menuTracking);
                        else menuStrip.Items.Add(menuTracking);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error creating menu: " + ex.Message);
            }
        }

        private void PopulateRecentFilesMenu(ToolStripMenuItem menu)
        {
            menu.DropDownItems.Clear();
            try
            {
                var dt = DatabaseHelper.GetRecentFiles();
                if (dt.Rows.Count == 0)
                {
                    var empty = new ToolStripMenuItem("(Không có file nào)") { Enabled = false };
                    menu.DropDownItems.Add(empty);
                    return;
                }
                foreach (DataRow row in dt.Rows)
                {
                    string name = row["ten"].ToString();
                    string path = row["duong_dan"].ToString();
                    var item = new ToolStripMenuItem(name);
                    item.Click += (s, ev) =>
                    {
                        if (File.Exists(path))
                            System.Diagnostics.Process.Start(path);
                        else
                            ShowError("File không tồn tại: " + path);
                    };
                    menu.DropDownItems.Add(item);
                }
                menu.DropDownItems.Add(new ToolStripSeparator());
                var clearItem = new ToolStripMenuItem("Xóa lịch sử");
                clearItem.Click += (s, ev) => { DatabaseHelper.ClearRecentFiles(); };
                menu.DropDownItems.Add(clearItem);
            }
            catch { }
        }

        private void menuBackup_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "SQLite Database|*.db";
                sfd.FileName = $"study_documents_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DatabaseHelper.BackupDatabase(sfd.FileName);
                        ToastNotification.Success("Đã sao lưu thành công!");
                    }
                    catch (Exception ex) { ShowError("Lỗi sao lưu: " + ex.Message); }
                }
            }
        }

        private void menuRestore_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Khôi phục database sẽ thay thế toàn bộ dữ liệu hiện tại.\nBạn có chắc muốn tiếp tục?\n\nỨng dụng sẽ đóng lại sau khi khôi phục.",
                "Xác nhận khôi phục",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQLite Database|*.db";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DatabaseHelper.RestoreDatabase(ofd.FileName);
                        MessageBox.Show("Khôi phục thành công! Ứng dụng sẽ khởi động lại.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Restart();
                    }
                    catch (Exception ex) { ShowError("Lỗi khôi phục: " + ex.Message); }
                }
            }
        }

        #region Category Tree

        private void SetupCategoryTree()
        {
            // TreeView with full custom drawing
            treeCategory = new DoubleBufferedTreeView
            {
                Dock = DockStyle.Fill,
                Font = new Font(AppTheme.FontFamily, 9F),
                BorderStyle = BorderStyle.None,
                DrawMode = TreeViewDrawMode.OwnerDrawAll,
                ShowLines = false,
                ShowRootLines = false,
                ShowPlusMinus = false,
                FullRowSelect = true,
                HideSelection = false,
                Scrollable = true,
                ItemHeight = 36,
                Indent = 18,
                BackColor = AppTheme.BackgroundCard,
                ForeColor = AppTheme.TextPrimary
            };

            // Custom drawing
            treeCategory.DrawNode += TreeCategory_DrawNode;

            // Hover tracking
            treeCategory.MouseMove += (s, ev) =>
            {
                var node = treeCategory.GetNodeAt(ev.Location);
                if (node != _hoveredNode)
                {
                    _hoveredNode = node;
                    treeCategory.Invalidate();
                }
            };
            treeCategory.MouseLeave += (s, ev) =>
            {
                if (_hoveredNode != null)
                {
                    _hoveredNode = null;
                    treeCategory.Invalidate();
                }
            };

            // Double-click to expand/collapse
            treeCategory.NodeMouseDoubleClick += (s, ev) =>
            {
                if (ev.Node.Nodes.Count > 0)
                    ev.Node.Toggle();
            };

            // Click on header nodes to toggle expand/collapse
            treeCategory.NodeMouseClick += (s, ev) =>
            {
                var filter = ev.Node.Tag as TreeFilterInfo;
                if (filter?.FilterType == "header")
                {
                    ev.Node.Toggle();
                    treeCategory.Invalidate();
                }
            };

            // Wrap existing content in SplitContainer
            splitCategory = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 2,
                BorderStyle = BorderStyle.None,
                FixedPanel = FixedPanel.Panel1
            };

            var existingControls = new Control[pnlContent.Controls.Count];
            pnlContent.Controls.CopyTo(existingControls, 0);
            pnlContent.Controls.Clear();
            foreach (var ctrl in existingControls)
                splitCategory.Panel2.Controls.Add(ctrl);

            // Header panel with bottom border
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = AppTheme.BackgroundCard,
                Padding = new Padding(14, 0, 8, 0)
            };
            var lblTreeHeader = new Label
            {
                Text = "Phân loại",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 10F),
                ForeColor = AppTheme.Primary,
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Paint += (s, ev) =>
            {
                using (var pen = new Pen(AppTheme.BorderLight))
                    ev.Graphics.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
            };
            headerPanel.Controls.Add(lblTreeHeader);

            // Tree container panel
            var pnlTree = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.BackgroundCard,
                Padding = new Padding(0, 4, 0, 0)
            };
            pnlTree.Controls.Add(treeCategory);
            pnlTree.Controls.Add(headerPanel);

            // Right border for tree panel
            splitCategory.Panel1.Paint += (s, ev) =>
            {
                using (var pen = new Pen(AppTheme.BorderLight))
                    ev.Graphics.DrawLine(pen,
                        splitCategory.Panel1.Width - 1, 0,
                        splitCategory.Panel1.Width - 1, splitCategory.Panel1.Height);
            };

            splitCategory.Panel1.Controls.Add(pnlTree);
            splitCategory.Panel1.BackColor = AppTheme.BackgroundCard;

            pnlContent.Controls.Add(splitCategory);

            // Set splitter distance after layout
            splitCategory.SplitterDistance = 180;
            splitCategory.Panel1MinSize = 160;

            // Responsive: hide tree when form too narrow
            this.Resize += (s, ev) =>
            {
                if (splitCategory == null) return;
                splitCategory.Panel1Collapsed = this.Width < 700;
            };

            // Node selection
            treeCategory.AfterSelect += TreeCategory_AfterSelect;

            // Populate
            PopulateCategoryTree();
        }

        private void PopulateCategoryTree()
        {
            if (treeCategory == null) return;

            treeCategory.AfterSelect -= TreeCategory_AfterSelect;
            treeCategory.BeginUpdate();
            treeCategory.Nodes.Clear();
            ClearTreeIconCache();

            int totalCount = 0;
            int importantCount = 0;

            try
            {
                var dtTotal = DatabaseHelper.ExecuteQuery(
                    "SELECT COUNT(*) as cnt FROM tai_lieu WHERE (is_deleted IS NULL OR is_deleted = 0)");
                if (dtTotal.Rows.Count > 0) totalCount = Convert.ToInt32(dtTotal.Rows[0]["cnt"]);

                var dtImportant = DatabaseHelper.ExecuteQuery(
                    "SELECT COUNT(*) as cnt FROM tai_lieu WHERE quan_trong = 1 AND (is_deleted IS NULL OR is_deleted = 0)");
                if (dtImportant.Rows.Count > 0) importantCount = Convert.ToInt32(dtImportant.Rows[0]["cnt"]);
            }
            catch { }

            // Root: All Documents
            var nodeAll = treeCategory.Nodes.Add("all", "Tất cả tài liệu");
            nodeAll.Tag = new TreeFilterInfo("all", null) { Count = totalCount };

            // By Subject (header)
            var nodeSubject = treeCategory.Nodes.Add("subjects", "Danh mục");
            nodeSubject.Tag = new TreeFilterInfo("header", null);
            try
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT mon_hoc, COUNT(*) as cnt FROM tai_lieu WHERE mon_hoc IS NOT NULL AND mon_hoc != '' AND (is_deleted IS NULL OR is_deleted = 0) GROUP BY mon_hoc ORDER BY mon_hoc");
                foreach (DataRow row in dt.Rows)
                {
                    string name = row["mon_hoc"].ToString();
                    int count = Convert.ToInt32(row["cnt"]);
                    var child = nodeSubject.Nodes.Add("sub_" + name, name);
                    child.Tag = new TreeFilterInfo("subject", name) { Count = count };
                }
            }
            catch { }

            // By Type (header)
            var nodeType = treeCategory.Nodes.Add("types", "Loại tài liệu");
            nodeType.Tag = new TreeFilterInfo("header", null);
            try
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT loai, COUNT(*) as cnt FROM tai_lieu WHERE loai IS NOT NULL AND loai != '' AND (is_deleted IS NULL OR is_deleted = 0) GROUP BY loai ORDER BY loai");
                foreach (DataRow row in dt.Rows)
                {
                    string name = row["loai"].ToString();
                    int count = Convert.ToInt32(row["cnt"]);
                    var child = nodeType.Nodes.Add("type_" + name, name);
                    child.Tag = new TreeFilterInfo("type", name) { Count = count };
                }
            }
            catch { }

            // Important
            var nodeImportant = treeCategory.Nodes.Add("important", "Quan trọng");
            nodeImportant.Tag = new TreeFilterInfo("important", null) { Count = importantCount };

            // Collections (header)
            var nodeCollections = treeCategory.Nodes.Add("collections", "Bộ sưu tập");
            nodeCollections.Tag = new TreeFilterInfo("header", null);
            try
            {
                var dt = DatabaseHelper.GetCollections();
                foreach (DataRow row in dt.Rows)
                {
                    string name = row["name"].ToString();
                    string id = row["id"].ToString();
                    int count = Convert.ToInt32(row["item_count"]);
                    var child = nodeCollections.Nodes.Add("col_" + id, name);
                    child.Tag = new TreeFilterInfo("collection", id) { Count = count };
                }
            }
            catch { }

            treeCategory.ExpandAll();
            treeCategory.EndUpdate();

            // Select "All" by default
            treeCategory.SelectedNode = nodeAll;
            treeCategory.AfterSelect += TreeCategory_AfterSelect;
        }

        private void TreeCategory_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.Height <= 0) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var node = e.Node;
            var filter = node.Tag as TreeFilterInfo;
            bool isSelected = node == treeCategory.SelectedNode;
            bool isHovered = node == _hoveredNode && !isSelected;
            int treeWidth = treeCategory.ClientSize.Width;

            // Full row background (clear)
            var rowBounds = new Rectangle(0, e.Bounds.Y, treeWidth, e.Bounds.Height);
            using (var bgBrush = new SolidBrush(treeCategory.BackColor))
                g.FillRectangle(bgBrush, rowBounds);

            // Section headers (special rendering)
            if (filter?.FilterType == "header")
            {
                DrawSectionHeader(g, e, node, filter, treeWidth);
                return;
            }

            // Selection background (rounded)
            if (isSelected)
            {
                var selRect = new Rectangle(4, e.Bounds.Y + 2, treeWidth - 8, e.Bounds.Height - 4);
                using (var path = AppTheme.CreateRoundedRectangle(selRect, 6))
                using (var brush = new SolidBrush(AppTheme.PrimaryLighter))
                    g.FillPath(brush, path);

                // Left accent bar
                using (var brush = new SolidBrush(AppTheme.Primary))
                    g.FillRectangle(brush, 0, e.Bounds.Y + 5, 3, e.Bounds.Height - 10);
            }
            else if (isHovered)
            {
                var hoverRect = new Rectangle(4, e.Bounds.Y + 2, treeWidth - 8, e.Bounds.Height - 4);
                using (var path = AppTheme.CreateRoundedRectangle(hoverRect, 6))
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                    g.FillPath(brush, path);
            }

            // Layout
            int baseIndent = 14 + node.Level * 18;
            int iconX = baseIndent;
            int textX = iconX + 20;
            int centerY = e.Bounds.Y + e.Bounds.Height / 2;

            // Draw icon (16x16)
            var icon = GetCachedTreeIcon(filter, 16);
            if (icon != null)
                g.DrawImage(icon, iconX, centerY - 8, 16, 16);

            // Draw text
            bool isBold = filter?.FilterType == "all";
            using (var textFont = isBold ? new Font(treeCategory.Font, FontStyle.Bold) : new Font(treeCategory.Font, FontStyle.Regular))
            {
                var textColor = isSelected ? AppTheme.PrimaryDark : AppTheme.TextPrimary;
                TextRenderer.DrawText(g, node.Text, textFont,
                    new Point(textX, centerY - textFont.Height / 2), textColor);
            }

            // Count badge
            if (filter != null && filter.Count > 0)
            {
                DrawCountBadge(g, filter.Count, treeWidth, centerY, isSelected);
            }
        }

        private void DrawSectionHeader(Graphics g, DrawTreeNodeEventArgs e, TreeNode node, TreeFilterInfo filter, int treeWidth)
        {
            int centerY = e.Bounds.Y + e.Bounds.Height / 2;

            // Separator line above (if not first node) - push down for breathing room
            if (node.PrevNode != null)
            {
                using (var pen = new Pen(AppTheme.BorderLight))
                    g.DrawLine(pen, 12, e.Bounds.Y + 6, treeWidth - 12, e.Bounds.Y + 6);
            }

            // Header text (uppercase, small, muted) - pushed to lower part of cell
            using (var headerFont = new Font(AppTheme.FontFamily, 7.5f, FontStyle.Bold))
            {
                int indent = 14 + node.Level * 18;
                TextRenderer.DrawText(g, node.Text.ToUpper(), headerFont,
                    new Point(indent, centerY - headerFont.Height / 2 + 4), AppTheme.TextMuted);
            }

            // Chevron on the right for expand/collapse
            if (node.Nodes.Count > 0)
            {
                DrawChevron(g, node.IsExpanded, treeWidth - 18, centerY + 2, AppTheme.TextMuted);
            }
        }

        private void DrawCountBadge(Graphics g, int count, int treeWidth, int centerY, bool isSelected)
        {
            string countText = count.ToString();
            using (var countFont = new Font(AppTheme.FontFamily, 7.5f))
            {
                var countSize = TextRenderer.MeasureText(countText, countFont);
                int badgeW = Math.Max(countSize.Width + 2, 22);
                int badgeX = treeWidth - badgeW - 10;
                int badgeY = centerY - 9;

                using (var path = AppTheme.CreateRoundedRectangle(new Rectangle(badgeX, badgeY, badgeW, 18), 9))
                {
                    var badgeBg = isSelected ? AppTheme.Primary : Color.FromArgb(229, 229, 229);
                    var badgeFg = isSelected ? Color.White : AppTheme.TextSecondary;

                    using (var brush = new SolidBrush(badgeBg))
                        g.FillPath(brush, path);

                    TextRenderer.DrawText(g, countText, countFont,
                        new Rectangle(badgeX, badgeY, badgeW, 18), badgeFg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }

        private void DrawChevron(Graphics g, bool expanded, int x, int y, Color color)
        {
            using (var pen = new Pen(color, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                if (expanded)
                {
                    g.DrawLine(pen, x - 3, y - 2, x, y + 1);
                    g.DrawLine(pen, x, y + 1, x + 3, y - 2);
                }
                else
                {
                    g.DrawLine(pen, x - 1, y - 3, x + 2, y);
                    g.DrawLine(pen, x + 2, y, x - 1, y + 3);
                }
            }
        }

        private Bitmap GetCachedTreeIcon(TreeFilterInfo filter, int size)
        {
            if (filter == null) return null;

            string key = $"{filter.FilterType}_{filter.FilterValue}_{size}";
            if (_treeIconCache.TryGetValue(key, out Bitmap cached))
                return cached;

            Bitmap icon = null;
            switch (filter.FilterType)
            {
                case "all":
                    icon = IconHelper.CreateHomeIcon(size, AppTheme.AccentSky);
                    break;
                case "subject":
                    icon = IconHelper.CreateFolderIcon(size, AppTheme.AccentOrange);
                    break;
                case "type":
                    icon = IconHelper.GetDocumentIcon(filter.FilterValue, size);
                    break;
                case "important":
                    icon = IconHelper.CreateStarIcon(size);
                    break;
                case "collection":
                    icon = IconHelper.CreateBookmarkIcon(size, AppTheme.Primary);
                    break;
            }

            if (icon != null)
                _treeIconCache[key] = icon;

            return icon;
        }

        private void ClearTreeIconCache()
        {
            foreach (var icon in _treeIconCache.Values)
                icon?.Dispose();
            _treeIconCache.Clear();
        }

        private void TreeCategory_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag == null) return;
            var filter = e.Node.Tag as TreeFilterInfo;
            if (filter == null) return;

            if (filter.FilterType == "header")
            {
                // Handled by NodeMouseClick
                return;
            }

            // Reset existing UI filters first
            ClearUIFilters();

            switch (filter.FilterType)
            {
                case "all":
                    RefreshRequested?.Invoke(this, EventArgs.Empty);
                    break;

                case "subject":
                    for (int i = 0; i < cboSubject.Items.Count; i++)
                    {
                        if (cboSubject.Items[i].ToString() == filter.FilterValue)
                        {
                            cboSubject.SelectedIndex = i;
                            break;
                        }
                    }
                    break;

                case "type":
                    for (int i = 0; i < cboType.Items.Count; i++)
                    {
                        if (cboType.Items[i].ToString() == filter.FilterValue)
                        {
                            cboType.SelectedIndex = i;
                            break;
                        }
                    }
                    break;

                case "important":
                    chkImportantOnly.Checked = true;
                    FilterApplied?.Invoke(this, EventArgs.Empty);
                    break;

                case "collection":
                    FilterByCollection(int.Parse(filter.FilterValue));
                    break;
            }
        }

        private void FilterByCollection(int collectionId)
        {
            try
            {
                string query = @"SELECT t.* FROM tai_lieu t
                    INNER JOIN collection_items ci ON t.id = ci.document_id
                    WHERE ci.collection_id = @collectionId AND (t.is_deleted IS NULL OR t.is_deleted = 0)
                    ORDER BY t.ngay_them DESC";
                var param = new System.Data.SQLite.SQLiteParameter("@collectionId", collectionId);
                var dt = DatabaseHelper.ExecuteQuery(query, new[] { param });

                var docs = new List<StudyDocument>();
                foreach (DataRow row in dt.Rows)
                {
                    docs.Add(new StudyDocument
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Ten = row["ten"]?.ToString(),
                        MonHoc = row["mon_hoc"]?.ToString(),
                        Loai = row["loai"]?.ToString(),
                        DuongDan = row["duong_dan"]?.ToString(),
                        GhiChu = row["ghi_chu"]?.ToString(),
                        NgayThem = row["ngay_them"] != DBNull.Value ? Convert.ToDateTime(row["ngay_them"]) : DateTime.MinValue,
                        KichThuoc = row["kich_thuoc"] != DBNull.Value ? Convert.ToDouble(row["kich_thuoc"]) : 0,
                        TacGia = row["tac_gia"]?.ToString(),
                        QuanTrong = row["quan_trong"] != DBNull.Value && Convert.ToInt32(row["quan_trong"]) == 1,
                        Tags = row["tags"]?.ToString(),
                        Deadline = row["deadline"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["deadline"]) : null
                    });
                }

                var bindingList = new System.ComponentModel.BindingList<StudyDocument>(docs);
                dgvDocuments.DataSource = bindingList;

                if (dgvDocuments.Columns.Contains("id")) dgvDocuments.Columns["id"].DataPropertyName = "Id";
                if (dgvDocuments.Columns.Contains("ten")) dgvDocuments.Columns["ten"].DataPropertyName = "Ten";
                if (dgvDocuments.Columns.Contains("mon_hoc")) dgvDocuments.Columns["mon_hoc"].DataPropertyName = "MonHoc";
                if (dgvDocuments.Columns.Contains("loai")) dgvDocuments.Columns["loai"].DataPropertyName = "Loai";
                if (dgvDocuments.Columns.Contains("duong_dan")) dgvDocuments.Columns["duong_dan"].DataPropertyName = "DuongDan";
                if (dgvDocuments.Columns.Contains("ghi_chu")) dgvDocuments.Columns["ghi_chu"].DataPropertyName = "GhiChu";
                if (dgvDocuments.Columns.Contains("ngay_them")) dgvDocuments.Columns["ngay_them"].DataPropertyName = "NgayThem";
                if (dgvDocuments.Columns.Contains("kich_thuoc")) dgvDocuments.Columns["kich_thuoc"].DataPropertyName = "KichThuoc";
                if (dgvDocuments.Columns.Contains("tac_gia")) dgvDocuments.Columns["tac_gia"].DataPropertyName = "TacGia";
                if (dgvDocuments.Columns.Contains("quan_trong")) dgvDocuments.Columns["quan_trong"].DataPropertyName = "QuanTrong";
                if (dgvDocuments.Columns.Contains("tags")) dgvDocuments.Columns["tags"].DataPropertyName = "Tags";
                if (dgvDocuments.Columns.Contains("deadline")) dgvDocuments.Columns["deadline"].DataPropertyName = "Deadline";
                SetupDataGridView();

                UpdateStatusCount(docs.Count);
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi khi lọc bộ sưu tập: " + ex.Message);
            }
        }

        private class TreeFilterInfo
        {
            public string FilterType { get; }
            public string FilterValue { get; }
            public int Count { get; set; }

            public TreeFilterInfo(string filterType, string filterValue)
            {
                FilterType = filterType;
                FilterValue = filterValue;
            }
        }

        #endregion

        private void SetupPreviewPanel()
        {
            previewPanel = new study_document_manager.UI.Controls.DocumentPreviewPanel();

            // Find the DataGridView's parent and wrap in SplitContainer
            var dgvParent = dgvDocuments.Parent;
            if (dgvParent == null) return;

            splitPreview = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = (int)(dgvParent.Width * 0.65),
                Panel2Collapsed = true,
                BorderStyle = BorderStyle.None,
                SplitterWidth = 4
            };

            int dgvIndex = dgvParent.Controls.GetChildIndex(dgvDocuments);
            dgvParent.Controls.Remove(dgvDocuments);

            splitPreview.Panel1.Controls.Add(dgvDocuments);
            dgvDocuments.Dock = DockStyle.Fill;

            splitPreview.Panel2.Controls.Add(previewPanel);

            dgvParent.Controls.Add(splitPreview);
            dgvParent.Controls.SetChildIndex(splitPreview, dgvIndex);

            // Auto show/hide preview based on file type
            dgvDocuments.SelectionChanged += (s, ev) =>
            {
                UpdatePreview();
            };
        }

        private static readonly string[] PreviewableExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".ico", ".tiff", ".tif", ".webp", ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".webm", ".flv", ".m4v" };

        private void UpdatePreview()
        {
            if (previewPanel == null || splitPreview == null) return;

            if (dgvDocuments.SelectedRows.Count == 0)
            {
                splitPreview.Panel2Collapsed = true;
                previewPanel.ClearPreview();
                return;
            }

            var doc = dgvDocuments.SelectedRows[0].DataBoundItem as study_document_manager.Core.Entities.StudyDocument;
            if (doc == null || string.IsNullOrEmpty(doc.DuongDan))
            {
                splitPreview.Panel2Collapsed = true;
                previewPanel.ClearPreview();
                return;
            }

            string ext = System.IO.Path.GetExtension(doc.DuongDan).ToLowerInvariant();
            bool isMedia = Array.Exists(PreviewableExtensions, e => e == ext);

            if (isMedia)
            {
                splitPreview.Panel2Collapsed = false;
                previewPanel.LoadPreview(doc.DuongDan, doc.Ten);
            }
            else
            {
                splitPreview.Panel2Collapsed = true;
                previewPanel.ClearPreview();
            }
        }

        private void SetupContextMenu()
        {
            contextMenuDocument.Items.Add(new ToolStripSeparator());
            var menuRelated = new ToolStripMenuItem("Tài liệu liên quan...");
            menuRelated.Click += (s, ev) =>
            {
                if (dgvDocuments.SelectedRows.Count == 0) return;
                var doc = dgvDocuments.SelectedRows[0].DataBoundItem as study_document_manager.Core.Entities.StudyDocument;
                if (doc != null)
                {
                    new study_document_manager.Documents.RelatedDocumentsForm(doc.Id, doc.Ten).ShowDialog();
                }
            };
            contextMenuDocument.Items.Add(menuRelated);
        }

        private void EnableDragDrop()
        {
            dgvDocuments.AllowDrop = true;
            dgvDocuments.DragEnter += (s, e) => { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; };
            dgvDocuments.DragDrop += (s, e) =>
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    // Call logic to add file...
                    // Here we should probably call a Service method directly
                    // For now, open AddEditForm
                    AddEditForm form = new AddEditForm();
                    form.txt_duong_dan.Text = file;
                    form.txt_ten.Text = Path.GetFileNameWithoutExtension(file);
                    form.ShowDialog();
                }
                TriggerRefresh();
            };
        }

        // Unused event handlers required by Designer auto-generated code
        private void chkEnableDateFilter_CheckedChanged(object sender, EventArgs e) { dtpFromDate.Enabled = chkEnableDateFilter.Checked; dtpToDate.Enabled = chkEnableDateFilter.Checked; }
        private void chkEnableSizeFilter_CheckedChanged(object sender, EventArgs e) { nudMinSize.Enabled = chkEnableSizeFilter.Checked; nudMaxSize.Enabled = chkEnableSizeFilter.Checked; }
        private void btnSearch_Click(object sender, EventArgs e) { /* Handled in RegisterEvents */ }
        private void btnApplyAdvancedFilter_Click(object sender, EventArgs e) { /* Handled in RegisterEvents */ }
        private void btnClearAdvancedFilter_Click(object sender, EventArgs e) { /* Handled in RegisterEvents */ }
        private void cbo_mon_hoc_SelectedIndexChanged(object sender, EventArgs e) { /* Handled in RegisterEvents */ }
        private void cbo_loai_SelectedIndexChanged(object sender, EventArgs e) { /* Handled in RegisterEvents */ }
        private void txt_tim_kiem_KeyPress(object sender, KeyPressEventArgs e) { /* Handled in RegisterEvents */ }
        private void btn_lam_moi_Click(object sender, EventArgs e) { /* Handled in RegisterEvents */ }
        private void menuView_Click(object sender, EventArgs e) { }

        private void dgvDocuments_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    /// <summary>
    /// Helper class for Collection ListBox display
    /// </summary>
    internal class CollectionItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

        public override string ToString() => $"{Name} ({Count} tài liệu)";
    }
}
