using study_document_manager.Core.Data;
using study_document_manager.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager.Documents
{
    public class DuplicateDetectionForm : Form
    {
        private DataGridView dgvDuplicates;
        private Button btnScan;
        private Button btnDeleteSelected;
        private Button btnClose;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Panel pnlHeader;
        private Panel pnlActions;

        private List<DuplicateGroup> duplicateGroups = new List<DuplicateGroup>();

        public DuplicateDetectionForm()
        {
            InitializeUI();
            ApplyTheme();
        }

        private void InitializeUI()
        {
            this.Text = "Phát hiện file trùng lặp";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(16, 8, 16, 8) };

            var lblTitle = new Label
            {
                Text = "Phát hiện file trùng lặp",
                Font = new Font(AppTheme.FontFamily, 14f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 8)
            };

            btnScan = new Button { Text = "Quét", Size = new Size(120, 35), Location = new Point(16, 38) };
            btnScan.Click += BtnScan_Click;

            progressBar = new ProgressBar { Location = new Point(150, 42), Size = new Size(400, 25), Visible = false };
            lblStatus = new Label { AutoSize = true, Location = new Point(560, 46), Font = new Font(AppTheme.FontFamily, 9f) };

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, btnScan, progressBar, lblStatus });
            this.Controls.Add(pnlHeader);

            // DataGridView
            dgvDuplicates = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                ReadOnly = false
            };

            dgvDuplicates.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Selected", HeaderText = "", Width = 30
            });
            dgvDuplicates.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Group", HeaderText = "Nhóm", Width = 60, ReadOnly = true
            });
            dgvDuplicates.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DocName", HeaderText = "Tên tài liệu", Width = 250, ReadOnly = true
            });
            dgvDuplicates.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileType", HeaderText = "Định dạng", Width = 60, ReadOnly = true
            });
            dgvDuplicates.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileSize", HeaderText = "Kích thước", Width = 90, ReadOnly = true
            });
            dgvDuplicates.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FilePath", HeaderText = "Đường dẫn", Width = 350, ReadOnly = true
            });
            dgvDuplicates.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DocId", HeaderText = "ID", Visible = false, ReadOnly = true
            });

            this.Controls.Add(dgvDuplicates);

            // Actions
            pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12, 8, 12, 8) };

            btnDeleteSelected = new Button { Text = "Xóa bản trùng đã chọn", Size = new Size(180, 35), Location = new Point(12, 8), Enabled = false };
            btnDeleteSelected.Click += BtnDeleteSelected_Click;

            btnClose = new Button { Text = "Đóng", Size = new Size(90, 35), Location = new Point(830, 8) };
            btnClose.Click += (s, e) => this.Close();

            pnlActions.Controls.AddRange(new Control[] { btnDeleteSelected, btnClose });
            this.Controls.Add(pnlActions);

            dgvDuplicates.BringToFront();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            pnlHeader.BackColor = AppTheme.BackgroundCard;
            pnlActions.BackColor = AppTheme.BackgroundSoft;
            lblStatus.ForeColor = AppTheme.TextSecondary;

            AppTheme.ApplyButtonPrimary(btnScan);
            AppTheme.ApplyButtonDanger(btnDeleteSelected);
            AppTheme.ApplyButtonSecondary(btnClose);
            AppTheme.ApplyDataGridViewStyle(dgvDuplicates);

            dgvDuplicates.ReadOnly = false;
            foreach (DataGridViewColumn col in dgvDuplicates.Columns)
                col.ReadOnly = col.Name != "Selected";
        }

        private async void BtnScan_Click(object sender, EventArgs e)
        {
            btnScan.Enabled = false;
            progressBar.Visible = true;
            dgvDuplicates.Rows.Clear();
            duplicateGroups.Clear();
            lblStatus.Text = "Đang quét...";

            try
            {
                var docs = DatabaseHelper.GetAllDocuments();
                var hashMap = new Dictionary<string, List<DataRow>>();

                progressBar.Maximum = docs.Rows.Count;
                progressBar.Value = 0;

                await System.Threading.Tasks.Task.Run(() =>
                {
                    foreach (DataRow row in docs.Rows)
                    {
                        string path = row["duong_dan"]?.ToString();
                        if (string.IsNullOrEmpty(path) || !File.Exists(path)) continue;

                        try
                        {
                            string hash = ComputeMD5(path);
                            lock (hashMap)
                            {
                                if (!hashMap.ContainsKey(hash))
                                    hashMap[hash] = new List<DataRow>();
                                hashMap[hash].Add(row);
                            }
                        }
                        catch { }

                        this.BeginInvoke((Action)(() =>
                        {
                            if (progressBar.Value < progressBar.Maximum)
                                progressBar.Value++;
                        }));
                    }
                });

                int groupNum = 0;
                foreach (var kvp in hashMap.Where(h => h.Value.Count > 1))
                {
                    groupNum++;
                    var group = new DuplicateGroup { GroupId = groupNum, Hash = kvp.Key };
                    foreach (var row in kvp.Value)
                    {
                        double sizeKb = 0;
                        double.TryParse(row["kich_thuoc"]?.ToString(), out sizeKb);

                        dgvDuplicates.Rows.Add(
                            false,
                            $"#{groupNum}",
                            row["ten"]?.ToString(),
                            row["dinh_dang"]?.ToString(),
                            FormatSize(sizeKb),
                            row["duong_dan"]?.ToString(),
                            row["id"]?.ToString()
                        );
                        group.DocumentIds.Add(Convert.ToInt32(row["id"]));
                    }
                    duplicateGroups.Add(group);
                }

                lblStatus.Text = $"Tìm thấy {duplicateGroups.Count} nhóm trùng lặp ({dgvDuplicates.Rows.Count} file)";
                btnDeleteSelected.Enabled = duplicateGroups.Count > 0;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi: " + ex.Message;
            }
            finally
            {
                btnScan.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            var idsToDelete = new List<int>();
            foreach (DataGridViewRow row in dgvDuplicates.Rows)
            {
                if (row.Cells["Selected"].Value != null && (bool)row.Cells["Selected"].Value)
                {
                    int id;
                    if (int.TryParse(row.Cells["DocId"].Value?.ToString(), out id))
                        idsToDelete.Add(id);
                }
            }

            if (idsToDelete.Count == 0)
            {
                MessageBox.Show("Chưa chọn file nào để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Xóa {idsToDelete.Count} bản ghi khỏi database?\n(File trên máy tính không bị xóa)",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            int deleted = DatabaseHelper.BulkSoftDelete(idsToDelete);
            ToastNotification.Success($"Đã xóa {deleted} bản ghi trùng lặp");

            // Remove deleted rows from grid
            for (int i = dgvDuplicates.Rows.Count - 1; i >= 0; i--)
            {
                int rowId;
                if (int.TryParse(dgvDuplicates.Rows[i].Cells["DocId"].Value?.ToString(), out rowId)
                    && idsToDelete.Contains(rowId))
                {
                    dgvDuplicates.Rows.RemoveAt(i);
                }
            }
        }

        private static string ComputeMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static string FormatSize(double sizeInMb)
        {
            if (sizeInMb < 1) return $"{sizeInMb * 1024:F0} KB";
            return $"{sizeInMb:F1} MB";
        }

        private class DuplicateGroup
        {
            public int GroupId { get; set; }
            public string Hash { get; set; }
            public List<int> DocumentIds { get; set; } = new List<int>();
        }
    }
}

