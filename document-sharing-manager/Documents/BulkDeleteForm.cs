using document_sharing_manager.Core.Data;
using document_sharing_manager.Core;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Infrastructure.Repositories;
using document_sharing_manager.UI;
using document_sharing_manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace document_sharing_manager.Documents
{
    public class BulkDeleteForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        private TextBox txtSearch;
        private ComboBox cboType;
        private DataGridView dgvDocs;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private Button btnDelete;
        private Button btnStar;
        private Label lblStatus;
        private Button btnClose;

        private BindingList<SelectableDocument> _allDocs;
        private BindingList<SelectableDocument> _filteredDocs;
        private readonly DocumentRepository _repo;

        public bool DataChanged { get; private set; }

        public BulkDeleteForm()
        {
            _repo = new DocumentRepository();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            Text = "Quản lý hàng loạt";
            Size = new Size(900, 600);
            MinimumSize = new Size(700, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor = AppTheme.BackgroundMain;
            MaximizeBox = true;
            MinimizeBox = false;
            ShowInTaskbar = false;

            // === Top filter panel ===
            var pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(12, 10, 12, 6),
                BackColor = AppTheme.BackgroundCard
            };

            txtSearch = new TextBox
            {
                Size = new Size(250, 28),
                Location = new Point(12, 12),
                Font = AppTheme.FontSmall
            };
            SendMessage(txtSearch.Handle, EM_SETCUEBANNER, IntPtr.Zero, "Tìm kiếm tên tài liệu...");
            txtSearch.TextChanged += (s, e) => ApplyFilter();

            var lblType = new Label
            {
                Text = "Định dạng:",
                AutoSize = true,
                Location = new Point(280, 15),
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontSmall
            };

            cboType = new ComboBox
            {
                Size = new Size(160, 28),
                Location = new Point(340, 12),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = AppTheme.FontSmall
            };
            cboType.SelectedIndexChanged += (s, e) => ApplyFilter();

            pnlFilter.Controls.AddRange(new Control[] { txtSearch, lblType, cboType });

            // === DataGridView ===
            dgvDocs = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                ReadOnly = false,
                MultiSelect = false
            };

            // Columns
            var colSelect = new DataGridViewCheckBoxColumn
            {
                Name = "Selected",
                HeaderText = "☑",
                DataPropertyName = "Selected",
                Width = 40,
                MinimumWidth = 40,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Resizable = DataGridViewTriState.False
            };

            var colName = new DataGridViewTextBoxColumn
            {
                Name = "Ten",
                HeaderText = "Tên tài liệu",
                DataPropertyName = "Ten",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 35,
                ReadOnly = true
            };

            var colType = new DataGridViewTextBoxColumn
            {
                Name = "DinhDang",
                HeaderText = "Định dạng",
                DataPropertyName = "DinhDang",
                Width = 80,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                ReadOnly = true
            };

            var colDate = new DataGridViewTextBoxColumn
            {
                Name = "NgayThem",
                HeaderText = "Ngày thêm",
                DataPropertyName = "NgayThemFormatted",
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                ReadOnly = true
            };

            var colImportant = new DataGridViewTextBoxColumn
            {
                Name = "QuanTrong",
                HeaderText = "★",
                DataPropertyName = "QuanTrongDisplay",
                Width = 35,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                ReadOnly = true
            };

            dgvDocs.Columns.AddRange(new DataGridViewColumn[] { colSelect, colName, colType, colDate, colImportant });

            // Style
            dgvDocs.BackgroundColor = AppTheme.BackgroundMain;
            dgvDocs.BorderStyle = BorderStyle.None;
            dgvDocs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDocs.GridColor = AppTheme.GridBorder;
            dgvDocs.EnableHeadersVisualStyles = false;
            dgvDocs.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeaderBg;
            dgvDocs.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderFg;
            dgvDocs.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontSmallBold;
            dgvDocs.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            dgvDocs.ColumnHeadersHeight = 40;
            dgvDocs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvDocs.DefaultCellStyle.BackColor = AppTheme.BackgroundCard;
            dgvDocs.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
            dgvDocs.DefaultCellStyle.Font = AppTheme.FontSmall;
            dgvDocs.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            dgvDocs.DefaultCellStyle.SelectionBackColor = AppTheme.GridRowSelected;
            dgvDocs.DefaultCellStyle.SelectionForeColor = AppTheme.TextPrimary;
            dgvDocs.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridRowAlt;
            dgvDocs.RowTemplate.Height = 36;

            colSelect.DefaultCellStyle.Padding = new Padding(0);
            colSelect.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvDocs.CellContentClick += DgvDocs_CellContentClick;

            // === Bottom action panel ===
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                Padding = new Padding(12, 8, 12, 8),
                BackColor = AppTheme.BackgroundCard
            };

            btnSelectAll = new Button
            {
                Text = "Chọn tất cả",
                Size = new Size(100, 35),
                Location = new Point(12, 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyButtonSecondary(btnSelectAll);
            btnSelectAll.Click += BtnSelectAll_Click;

            btnDeselectAll = new Button
            {
                Text = "Bỏ chọn",
                Size = new Size(90, 35),
                Location = new Point(118, 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyButtonSecondary(btnDeselectAll);
            btnDeselectAll.Click += BtnDeselectAll_Click;

            lblStatus = new Label
            {
                Text = "0 tài liệu đã chọn",
                AutoSize = true,
                Location = new Point(220, 18),
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontSmall
            };

            btnDelete = new Button
            {
                Text = "Xóa đã chọn",
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.StatusError,
                ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            btnStar = new Button
            {
                Text = "Đánh dấu",
                Size = new Size(110, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.StatusWarning,
                ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnStar.FlatAppearance.BorderSize = 0;
            btnStar.Click += BtnStar_Click;

            btnClose = new Button
            {
                Text = "Đóng",
                Size = new Size(80, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyButtonSecondary(btnClose);
            btnClose.Click += (s, e) => Close();

            pnlBottom.Resize += (s, e) =>
            {
                int x = pnlBottom.ClientSize.Width - 12;
                btnClose.Location = new Point(x - btnClose.Width, 10);
                x -= btnClose.Width + 8;
                btnStar.Location = new Point(x - btnStar.Width, 10);
                x -= btnStar.Width + 8;
                btnDelete.Location = new Point(x - btnDelete.Width, 10);
            };

            pnlBottom.Controls.AddRange(new Control[] { btnSelectAll, btnDeselectAll, lblStatus, btnDelete, btnStar, btnClose });

            Controls.Add(dgvDocs);
            Controls.Add(pnlFilter);
            Controls.Add(pnlBottom);

            CancelButton = btnClose;
            Load += (s, e) => { if (Owner?.Icon != null) Icon = Owner.Icon; };
        }

        private void LoadData()
        {
            var docs = _repo.GetAll();
            _allDocs = new BindingList<SelectableDocument>(
                docs.Select(d => new SelectableDocument(d)).ToList()
            );

            cboType.Items.Clear();
            cboType.Items.Add("-- Tất cả --");
            try
            {
                var types = DatabaseHelper.GetDistinctTypes();
                foreach (System.Data.DataRow row in types.Rows)
                    cboType.Items.Add(row["dinh_dang"].ToString());
            }
            catch { }
            cboType.SelectedIndex = 0;

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string type = cboType.SelectedIndex > 0 ? cboType.SelectedItem.ToString() : null;

            var filtered = _allDocs.Where(d =>
            {
                if (!string.IsNullOrEmpty(keyword) && !d.Ten.ToLower().Contains(keyword))
                    return false;
                if (type != null && !string.Equals(d.DinhDang, type, StringComparison.OrdinalIgnoreCase))
                    return false;
                return true;
            }).ToList();

            _filteredDocs = new BindingList<SelectableDocument>(filtered);
            dgvDocs.DataSource = _filteredDocs;

            UpdateStatus();
        }

        private void DgvDocs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvDocs.Columns[e.ColumnIndex].Name != "Selected") return;

            dgvDocs.CommitEdit(DataGridViewDataErrorContexts.Commit);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            int count = _allDocs.Count(d => d.Selected);
            lblStatus.Text = $"{count} tài liệu đã chọn (hiển thị {_filteredDocs.Count}/{_allDocs.Count})";
        }

        private List<int> GetSelectedIds()
        {
            return _allDocs.Where(d => d.Selected).Select(d => d.Id).ToList();
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (var doc in _filteredDocs)
                doc.Selected = true;
            dgvDocs.Refresh();
            UpdateStatus();
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (var doc in _filteredDocs)
                doc.Selected = false;
            dgvDocs.Refresh();
            UpdateStatus();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedIds();
            if (ids.Count == 0)
            {
                MessageBox.Show("Chưa chọn tài liệu nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Xóa {ids.Count} tài liệu đã chọn vào thùng rác?", "Xác nhận xóa hàng loạt",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int result = DatabaseHelper.BulkSoftDelete(ids);
                ToastNotification.Success($"Đã xóa {result} tài liệu vào thùng rác.");
                DataChanged = true;
                LoadData();
            }
        }

        private void BtnStar_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedIds();
            if (ids.Count == 0)
            {
                MessageBox.Show("Chưa chọn tài liệu nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int result = DatabaseHelper.BulkToggleImportant(ids, true);
            ToastNotification.Success($"Đã đánh dấu quan trọng {result} tài liệu.");
            DataChanged = true;
            LoadData();
        }

        public class SelectableDocument : INotifyPropertyChanged
        {
            private bool _selected;
            private readonly Document _doc;

            public SelectableDocument(Document doc)
            {
                _doc = doc;
            }

            public bool Selected
            {
                get => _selected;
                set
                {
                    if (_selected != value)
                    {
                        _selected = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
                    }
                }
            }

            public int Id => _doc.Id;
            public string Ten => _doc.Ten ?? "";
            public string DinhDang => _doc.DinhDang ?? "";
            public string NgayThemFormatted => _doc.NgayThem.ToString("dd/MM/yyyy");
            public string QuanTrongDisplay => _doc.QuanTrong ? "★" : "";

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
