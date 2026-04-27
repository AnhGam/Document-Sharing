using study_document_manager.Core.Data;
using study_document_manager.Core;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager.Management
{
    public partial class FileIntegrityCheckForm : Form
    {
        private DataTable missingFilesData;
        private int totalFiles = 0;
        private int missingCount = 0;

        public FileIntegrityCheckForm()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            
            // Title
            lblTitle.ForeColor = AppTheme.Primary;
            lblTitle.Font = new Font(AppTheme.FontFamily, 14F, FontStyle.Bold);
            
            // Buttons
            AppTheme.ApplyButtonPrimary(btnScan);
            AppTheme.ApplyButtonDanger(btnDeleteAll);
            
            // Close button
            btnClose.BackColor = AppTheme.BackgroundSoft;
            btnClose.ForeColor = AppTheme.TextSecondary;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 1;
            btnClose.FlatAppearance.BorderColor = AppTheme.BorderMedium;
            btnClose.Cursor = Cursors.Hand;
            
            // Labels
            lblProgress.ForeColor = AppTheme.TextSecondary;
            lblProgress.Font = AppTheme.FontSmall;
            lblSummary.ForeColor = AppTheme.Primary;
            lblSummary.Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold);
            
            // Progress bar
            progressBar.ForeColor = AppTheme.Primary;
            
            // DataGridView
            AppTheme.ApplyDataGridViewStyle(dgvMissingFiles);
        }

        /// <summary>
        /// Nut Quet - Bat dau quet file
        /// </summary>
        private void btnScan_Click(object sender, EventArgs e)
        {
            ScanForMissingFiles();
        }

        /// <summary>
        /// Quet tat ca file trong database
        /// </summary>
        private void ScanForMissingFiles()
        {
            try
            {
                btnScan.Enabled = false;
                btnDeleteAll.Enabled = false;
                dgvMissingFiles.Rows.Clear();
                missingCount = 0;

                // Lấy danh sách tất cả tài liệu có đường dẫn
                string query = "SELECT id, ten, duong_dan FROM tai_lieu WHERE duong_dan IS NOT NULL AND duong_dan <> ''";
                DataTable allFiles = DatabaseHelper.ExecuteQuery(query);
                totalFiles = allFiles.Rows.Count;

                if (totalFiles == 0)
                {
                    lblProgress.Text = "Không có tài liệu nào có đường dẫn file.";
                    btnScan.Enabled = true;
                    return;
                }

                progressBar.Maximum = totalFiles;
                progressBar.Value = 0;

                missingFilesData = new DataTable();
                missingFilesData.Columns.Add("id", typeof(int));
                missingFilesData.Columns.Add("ten", typeof(string));
                missingFilesData.Columns.Add("duong_dan", typeof(string));

                int scannedCount = 0;
                foreach (DataRow row in allFiles.Rows)
                {
                    scannedCount++;
                    progressBar.Value = scannedCount;
                    lblProgress.Text = $"Đang quét: {scannedCount}/{totalFiles}...";
                    Application.DoEvents();

                    string duongDan = row["duong_dan"].ToString();
                    if (!File.Exists(duongDan))
                    {
                        missingCount++;
                        missingFilesData.Rows.Add(
                            Convert.ToInt32(row["id"]),
                            row["ten"].ToString(),
                            duongDan
                        );

                        dgvMissingFiles.Rows.Add(
                            row["id"].ToString(),
                            row["ten"].ToString(),
                            duongDan,
                            "Xử lý"
                        );
                    }
                }

                lblProgress.Text = $"Hoàn thành! Tìm thấy {missingCount}/{totalFiles} file bị thiếu.";
                lblSummary.Text = $"File thiếu: {missingCount}";
                btnDeleteAll.Enabled = missingCount > 0;
                btnScan.Enabled = true;
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi khi quét: " + ex.Message);
                btnScan.Enabled = true;
            }
        }

        /// <summary>
        /// Xu ly khi click vao cell trong DataGridView
        /// </summary>
        private void dgvMissingFiles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Chi xu ly khi click vao cot Action
            if (e.ColumnIndex == dgvMissingFiles.Columns["colAction"].Index)
            {
                int id = Convert.ToInt32(dgvMissingFiles.Rows[e.RowIndex].Cells["colId"].Value);
                string ten = dgvMissingFiles.Rows[e.RowIndex].Cells["colTen"].Value.ToString();
                string duongDan = dgvMissingFiles.Rows[e.RowIndex].Cells["colDuongDan"].Value.ToString();

                ShowActionMenu(e.RowIndex, id, ten, duongDan);
            }
        }

        /// <summary>
        /// Hien thi menu hanh dong cho file bi thieu
        /// </summary>
        private void ShowActionMenu(int rowIndex, int id, string ten, string duongDan)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem itemSelectNew = new ToolStripMenuItem("Chọn file mới...");
            itemSelectNew.Click += (s, e) => SelectNewFile(rowIndex, id);

            ToolStripMenuItem itemClearPath = new ToolStripMenuItem("Xóa đường dẫn (giữ metadata)");
            itemClearPath.Click += (s, e) => ClearFilePath(rowIndex, id);

            ToolStripMenuItem itemDelete = new ToolStripMenuItem("Xóa tài liệu");
            itemDelete.Click += (s, e) => DeleteDocument(rowIndex, id, ten);

            menu.Items.Add(itemSelectNew);
            menu.Items.Add(itemClearPath);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemDelete);

            // Hiển thị menu tại vị trí chuột
            menu.Show(Cursor.Position);
        }

        /// <summary>
        /// Chon file moi de cap nhat duong dan
        /// </summary>
        private void SelectNewFile(int rowIndex, int id)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file mới";
                ofd.Filter = "Tất cả file (*.*)|*.*|PDF (*.pdf)|*.pdf|Word (*.docx;*.doc)|*.docx;*.doc|PowerPoint (*.pptx;*.ppt)|*.pptx;*.ppt|Excel (*.xlsx;*.xls)|*.xlsx;*.xls";
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string newPath = ofd.FileName;
                        string query = "UPDATE tai_lieu SET duong_dan = @duong_dan WHERE id = @id";
                        System.Data.SQLite.SQLiteParameter[] parameters = new System.Data.SQLite.SQLiteParameter[]
                        {
                            new System.Data.SQLite.SQLiteParameter("@duong_dan", newPath),
                            new System.Data.SQLite.SQLiteParameter("@id", id)
                        };

                        int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                        if (result > 0)
                        {
                            dgvMissingFiles.Rows.RemoveAt(rowIndex);
                            missingCount--;
                            lblSummary.Text = $"File thiếu: {missingCount}";
                            ToastNotification.Success("Đã cập nhật đường dẫn file!");
                        }
                    }
                    catch (Exception ex)
                    {
                        ToastNotification.Error("Lỗi: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Xoa duong dan file nhung giu lai metadata
        /// </summary>
        private void ClearFilePath(int rowIndex, int id)
        {
            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa đường dẫn file?\nMetadata tài liệu sẽ được giữ lại (đường dẫn sẽ rỗng).",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE tai_lieu SET duong_dan = '' WHERE id = @id";
                    System.Data.SQLite.SQLiteParameter[] parameters = new System.Data.SQLite.SQLiteParameter[]
                    {
                        new System.Data.SQLite.SQLiteParameter("@id", id)
                    };

                    int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                    if (result > 0)
                    {
                        dgvMissingFiles.Rows.RemoveAt(rowIndex);
                        missingCount--;
                        lblSummary.Text = $"File thiếu: {missingCount}";
                        ToastNotification.Success("Đã xóa đường dẫn file!");
                    }
                }
                catch (Exception ex)
                {
                    ToastNotification.Error("Lỗi: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Xoa tai lieu khoi database
        /// </summary>
        private void DeleteDocument(int rowIndex, int id, string ten)
        {
            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn XOÁ VĨNH VIỄN tài liệu:\n\"{ten}\"?\n\nHành động này không thể hoàn tác!",
                "Xác nhận xoá",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    bool deleted = DatabaseHelper.DeleteDocument(id);
                    if (deleted)
                    {
                        dgvMissingFiles.Rows.RemoveAt(rowIndex);
                        missingCount--;
                        lblSummary.Text = $"File thiếu: {missingCount}";
                        ToastNotification.Success("Đã xóa tài liệu!");
                    }
                }
                catch (Exception ex)
                {
                    ToastNotification.Error("Lỗi: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Xoa tat ca tai lieu bi thieu file
        /// </summary>
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (dgvMissingFiles.Rows.Count == 0)
            {
                ToastNotification.Info("Không có tài liệu nào để xóa!");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn XÓA TẤT CẢ {dgvMissingFiles.Rows.Count} tài liệu bị thiếu file?\n\nHÀNH ĐỘNG NÀY KHÔNG THỂ HOÀN TÁC!",
                "Xác nhận xóa tất cả",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    int deletedCount = 0;
                    for (int i = dgvMissingFiles.Rows.Count - 1; i >= 0; i--)
                    {
                        int id = Convert.ToInt32(dgvMissingFiles.Rows[i].Cells["colId"].Value);
                        if (DatabaseHelper.DeleteDocument(id))
                        {
                            dgvMissingFiles.Rows.RemoveAt(i);
                            deletedCount++;
                        }
                    }

                    missingCount = 0;
                    lblSummary.Text = $"File thiếu: 0";
                    btnDeleteAll.Enabled = false;
                    ToastNotification.Success($"Đã xóa {deletedCount} tài liệu!");
                }
                catch (Exception ex)
                {
                    ToastNotification.Error("Lỗi: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Dong form
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


