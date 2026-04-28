using document_sharing_manager.Core.Data;
using document_sharing_manager.Core;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.Core.Services;

namespace document_sharing_manager.Documents
{
    public class RecentFilesForm : Form
    {
        private DataGridView dgvRecent;
        private Button btnOpen;
        private Button btnRemove;
        private Button btnClearAll;
        private Button btnClose;
        private Panel pnlHeader;
        private Panel pnlActions;

        public RecentFilesForm()
        {
            InitializeUI();
            ApplyTheme();
            LoadRecentFiles();
        }

        private void InitializeUI()
        {
            this.Text = "Tài liệu mở gần đây";
            this.Size = new Size(850, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(16, 8, 16, 8) };
            var lblTitle = new Label
            {
                Text = "Tài liệu mở gần đây",
                Font = new Font(AppTheme.FontFamily, 14f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 12)
            };
            var lblDesc = new Label
            {
                Text = "Double-click để mở file",
                Font = new Font(AppTheme.FontFamily, 9f),
                AutoSize = true,
                Location = new Point(250, 18)
            };
            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblDesc });
            this.Controls.Add(pnlHeader);

            // DataGridView
            dgvRecent = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                ReadOnly = true
            };

            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DocName", HeaderText = "Tên tài liệu", Width = 220
            });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Type", HeaderText = "Định dạng", Width = 100
            });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FilePath", HeaderText = "Đường dẫn", Width = 260
            });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OpenedAt", HeaderText = "Thời gian mở", Width = 140
            });
            dgvRecent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DocId", Visible = false
            });

            dgvRecent.CellDoubleClick += DgvRecent_CellDoubleClick;

            this.Controls.Add(dgvRecent);

            // Actions
            pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12, 8, 12, 8) };

            btnOpen = new Button { Text = "Mở file", Size = new Size(100, 35), Location = new Point(12, 8) };
            btnOpen.Click += BtnOpen_Click;

            btnRemove = new Button { Text = "Xóa mục này", Size = new Size(120, 35), Location = new Point(120, 8) };
            btnRemove.Click += BtnRemove_Click;

            btnClearAll = new Button { Text = "Xóa toàn bộ lịch sử", Size = new Size(160, 35), Location = new Point(248, 8) };
            btnClearAll.Click += BtnClearAll_Click;

            btnClose = new Button { Text = "Đóng", Size = new Size(90, 35), Location = new Point(730, 8) };
            btnClose.Click += (s, e) => this.Close();

            pnlActions.Controls.AddRange(new Control[] { btnOpen, btnRemove, btnClearAll, btnClose });
            this.Controls.Add(pnlActions);

            dgvRecent.BringToFront();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            pnlHeader.BackColor = AppTheme.BackgroundCard;
            pnlActions.BackColor = AppTheme.BackgroundSoft;

            foreach (Control c in pnlHeader.Controls)
                if (c is Label lbl && lbl.Font.Size < 12) lbl.ForeColor = AppTheme.TextSecondary;
                else if (c is Label lbl2) lbl2.ForeColor = AppTheme.TextPrimary;

            AppTheme.ApplyButtonPrimary(btnOpen);
            AppTheme.ApplyButtonSecondary(btnRemove);
            AppTheme.ApplyButtonDanger(btnClearAll);
            AppTheme.ApplyButtonSecondary(btnClose);
            AppTheme.ApplyDataGridViewStyle(dgvRecent);
        }

        private void LoadRecentFiles()
        {
            dgvRecent.Rows.Clear();
            try
            {
                var dt = DatabaseHelper.GetRecentFiles();
                foreach (DataRow row in dt.Rows)
                {
                    dgvRecent.Rows.Add(
                        row["ten"]?.ToString(),
                        row["dinh_dang"]?.ToString(),
                        row["duong_dan"]?.ToString(),
                        row["opened_at"]?.ToString(),
                        row["id"]?.ToString()
                    );
                }
            }
            catch { }
        }

        private void DgvRecent_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OpenSelectedFile();
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        private void OpenSelectedFile()
        {
            if (dgvRecent.SelectedRows.Count == 0) return;

            string rawPath = dgvRecent.SelectedRows[0].Cells["FilePath"].Value?.ToString();
            if (string.IsNullOrEmpty(rawPath))
            {
                MessageBox.Show("Đường dẫn file trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = StorageService.ResolvePath(rawPath);
            if (!File.Exists(path))
            {
                MessageBox.Show("File không tồn tại:\n" + rawPath, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvRecent.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn mục cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int docId;
            if (!int.TryParse(dgvRecent.SelectedRows[0].Cells["DocId"].Value?.ToString(), out docId))
                return;

            DatabaseHelper.RemoveRecentFile(docId);
            LoadRecentFiles();
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            if (dgvRecent.Rows.Count == 0) return;

            var confirm = MessageBox.Show(
                "Xóa toàn bộ lịch sử mở gần đây?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            DatabaseHelper.ClearRecentFiles();
            LoadRecentFiles();
            ToastNotification.Success("Đã xóa lịch sử");
        }
    }
}

