using study_document_manager.Core.Data;
using study_document_manager.Core;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using study_document_manager.UI;
using study_document_manager.UI.Controls;

namespace study_document_manager.Management
{
    public class RecycleBinForm : Form
    {
        private DataGridView dgvDeleted;
        private Button btnRestore;
        private Button btnPermanentDelete;
        private Button btnEmptyBin;
        private Button btnClose;
        private Label lblStatus;
        private Panel pnlHeader;
        private Panel pnlActions;
        private Label lblTitle;

        public RecycleBinForm()
        {
            InitializeUI();
            ApplyTheme();
            LoadDeletedDocuments();
        }

        private void InitializeUI()
        {
            this.Text = "Thùng rác";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header Panel
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(16, 12, 16, 8)
            };

            lblTitle = new Label
            {
                Text = "Thùng rác",
                Font = new Font(AppTheme.FontFamily, 14f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 12)
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // DataGridView
            dgvDeleted = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None
            };
            SetupColumns();
            this.Controls.Add(dgvDeleted);

            // Actions Panel
            pnlActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                Padding = new Padding(12, 8, 12, 8)
            };

            btnRestore = new Button { Text = "Khôi phục", Size = new Size(120, 35), Location = new Point(12, 10) };
            btnPermanentDelete = new Button { Text = "Xóa vĩnh viễn", Size = new Size(140, 35), Location = new Point(140, 10) };
            btnEmptyBin = new Button { Text = "Dọn sạch", Size = new Size(120, 35), Location = new Point(288, 10) };

            lblStatus = new Label
            {
                AutoSize = true,
                Location = new Point(420, 18),
                Font = new Font(AppTheme.FontFamily, 9f)
            };

            btnClose = new Button { Text = "Đóng", Size = new Size(90, 35) };
            btnClose.Location = new Point(pnlActions.Width - btnClose.Width - 20, 10);
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            pnlActions.Controls.AddRange(new Control[] { btnRestore, btnPermanentDelete, btnEmptyBin, lblStatus, btnClose });
            this.Controls.Add(pnlActions);

            // Events
            btnRestore.Click += BtnRestoreClick;
            btnPermanentDelete.Click += BtnPermanentDeleteClick;
            btnEmptyBin.Click += BtnEmptyBinClick;
            btnClose.Click += (s, e) => this.Close();

            // Ensure correct z-order (DGV fills remaining space)
            dgvDeleted.BringToFront();
        }

        private void SetupColumns()
        {
            dgvDeleted.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id", DataPropertyName = "id", Visible = false
            });

            dgvDeleted.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ten", DataPropertyName = "ten",
                HeaderText = "Tên tài liệu", Width = 280
            });


            dgvDeleted.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "dinh_dang", DataPropertyName = "dinh_dang",
                HeaderText = "Định dạng", Width = 90
            });

            dgvDeleted.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "deleted_at", DataPropertyName = "deleted_at",
                HeaderText = "Ngày xóa", Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            pnlHeader.BackColor = AppTheme.BackgroundCard;
            lblTitle.ForeColor = AppTheme.TextPrimary;
            pnlActions.BackColor = AppTheme.BackgroundSoft;
            lblStatus.ForeColor = AppTheme.TextSecondary;

            AppTheme.ApplyButtonPrimary(btnRestore);
            AppTheme.ApplyButtonDanger(btnPermanentDelete);
            AppTheme.ApplyButtonWarning(btnEmptyBin);
            AppTheme.ApplyButtonDanger(btnClose);
            AppTheme.ApplyDataGridViewStyle(dgvDeleted);
        }

        private void LoadDeletedDocuments()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetDeletedDocuments();
                dgvDeleted.DataSource = dt;
                lblStatus.Text = $"Có {dt.Rows.Count} tài liệu trong thùng rác";

                btnRestore.Enabled = dt.Rows.Count > 0;
                btnPermanentDelete.Enabled = dt.Rows.Count > 0;
                btnEmptyBin.Enabled = dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        private int? GetSelectedId()
        {
            if (dgvDeleted.SelectedRows.Count > 0)
                return Convert.ToInt32(dgvDeleted.SelectedRows[0].Cells["id"].Value);
            return null;
        }

        private void BtnRestoreClick(object sender, EventArgs e)
        {
            int? id = GetSelectedId();
            if (!id.HasValue)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu cần khôi phục.");
                return;
            }

            if (DatabaseHelper.RestoreDocument(id.Value))
            {
                ToastNotification.Success("Đã khôi phục tài liệu.");
                LoadDeletedDocuments();
            }
            else
            {
                ToastNotification.Error("Không thể khôi phục tài liệu.");
            }
        }

        private void BtnPermanentDeleteClick(object sender, EventArgs e)
        {
            int? id = GetSelectedId();
            if (!id.HasValue)
            {
                ToastNotification.Warning("Vui lòng chọn tài liệu cần xóa.");
                return;
            }

            string docName = dgvDeleted.SelectedRows[0].Cells["ten"].Value?.ToString();
            if (MessageBox.Show($"Xóa vĩnh viễn '{docName}'?\n\nHành động này KHÔNG thể hoàn tác!",
                "Xác nhận xóa vĩnh viễn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (DatabaseHelper.PermanentDeleteDocument(id.Value))
                {
                    ToastNotification.Success("Đã xóa vĩnh viễn.");
                    LoadDeletedDocuments();
                }
            }
        }

        private void BtnEmptyBinClick(object sender, EventArgs e)
        {
            int count = DatabaseHelper.GetDeletedDocumentCount();
            if (count == 0)
            {
                ToastNotification.Info("Thùng rác đã trống.");
                return;
            }

            if (MessageBox.Show($"Xóa vĩnh viễn TẤT CẢ {count} tài liệu trong thùng rác?\n\nHành động này KHÔNG thể hoàn tác!",
                "Dọn sạch thùng rác", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int deleted = DatabaseHelper.EmptyRecycleBin();
                ToastNotification.Success($"Đã xóa vĩnh viễn {deleted} tài liệu.");
                LoadDeletedDocuments();
            }
        }
    }
}

