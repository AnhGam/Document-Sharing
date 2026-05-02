using document_sharing_manager.Core.Data;
using document_sharing_manager.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using document_sharing_manager.UI;
using document_sharing_manager.UI.Controls;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Core.Services;

namespace document_sharing_manager.Documents
{
    public class BatchImportForm : Form
    {
        public event EventHandler ImportCompleted;
        private DataGridView dgvFiles;
        private Button btnSelect;
        private ContextMenuStrip menuSelect;
        private Button btnImport;
        private Button btnClose;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private Label lblFolder;
        private Label lblStatus;
        private ProgressBar progressBar;
        private Panel pnlHeader;
        private Panel pnlActions;
        private CheckBox chkRecursive;

        private readonly List<FileEntry> fileEntries = [];

        public BatchImportForm()
        {
            InitializeUI();
            ApplyTheme();
        }

        private void InitializeUI()
        {
            this.Text = "Import tài liệu từ thư mục";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header Panel
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(16, 8, 16, 8) };

            var lblTitle = new Label
            {
                Text = "Import tài liệu",
                Font = new Font(AppTheme.FontFamily, 14f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 8)
            };

            btnSelect = new Button
            {
                Text = "Thêm tài liệu...",
                Size = new Size(180, 32),
                Location = new Point(16, 48)
            };
            btnSelect.Click += BtnSelectClick;

            menuSelect = new ContextMenuStrip();
            menuSelect.Items.Add("📄 Chọn file(s)...", null, BtnSelectFilesClick);
            menuSelect.Items.Add("📁 Chọn thư mục...", null, BtnSelectFolderClick);

            lblFolder = new Label
            {
                Text = "Chưa chọn tài liệu",
                AutoSize = true,
                Location = new Point(315, 56),
                Font = new Font(AppTheme.FontFamily, 9f, FontStyle.Italic)
            };

            chkRecursive = new CheckBox
            {
                Text = "Bao gồm thư mục con",
                AutoSize = true,
                Location = new Point(650, 56),
                Checked = true
            };

            pnlHeader.Controls.AddRange([lblTitle, btnSelect, lblFolder, chkRecursive]);
            this.Controls.Add(pnlHeader);

            // DataGridView
            dgvFiles = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                ReadOnly = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None
            };
            SetupColumns();
            this.Controls.Add(dgvFiles);

            // Actions Panel
            pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 85, Padding = new Padding(12, 8, 12, 8) };

            btnSelectAll = new Button { Text = "Chọn tất cả", Size = new Size(100, 30), Location = new Point(16, 10) };
            btnSelectAll.Click += (s, e) => ToggleAll(true);

            btnDeselectAll = new Button { Text = "Bỏ chọn", Size = new Size(90, 30), Location = new Point(124, 10) };
            btnDeselectAll.Click += (s, e) => ToggleAll(false);

            // Row 2: Progress + Import + Close
            progressBar = new ProgressBar { Location = new Point(16, 45), Size = new Size(450, 22), Visible = false };
            lblStatus = new Label { AutoSize = true, Location = new Point(16, 70), Font = new Font(AppTheme.FontFamily, 9f) };

            btnImport = new Button { Text = "Import", Size = new Size(120, 35), Location = new Point(650, 8) };
            btnImport.Click += BtnImportClick;
            btnImport.Enabled = false;

            btnClose = new Button { Text = "Đóng", Size = new Size(90, 35), Location = new Point(778, 8) };
            btnClose.Click += (s, e) => this.Close();

            pnlActions.Controls.AddRange([
                btnSelectAll, btnDeselectAll,
                progressBar, lblStatus, btnImport, btnClose
            ]);
            this.Controls.Add(pnlActions);

            dgvFiles.BringToFront();
        }

        private void SetupColumns()
        {
            dgvFiles.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Selected",
                HeaderText = "",
                Width = 30
            });

            dgvFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileName", HeaderText = "Tên file", Width = 280, ReadOnly = true
            });

            dgvFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileType", HeaderText = "Định dạng", Width = 100, ReadOnly = true,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileSize", HeaderText = "Kích thước", Width = 100, ReadOnly = true
            });

            dgvFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FilePath", HeaderText = "Đường dẫn", Width = 250, ReadOnly = true
            });

            dgvFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Important", HeaderText = "★", Width = 40, ReadOnly = false,
                DefaultCellStyle = { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter, 
                    Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                    ForeColor = Color.Gold 
                }
            });
            dgvFiles.Columns["Important"].HeaderCell.Style.ForeColor = Color.Gold;

            dgvFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Note", HeaderText = "Ghi chú (Note)", Width = 200, ReadOnly = false
            });

            dgvFiles.CellPainting += DgvFilesCellPainting;
            dgvFiles.CellClick += DgvFilesCellClick;
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            pnlHeader.BackColor = AppTheme.BackgroundCard;
            pnlActions.BackColor = AppTheme.BackgroundSoft;
            lblStatus.ForeColor = AppTheme.TextSecondary;

            AppTheme.ApplyButtonPrimary(btnSelect);
            AppTheme.ApplyButtonSuccess(btnImport);
            AppTheme.ApplyButtonSecondary(btnSelectAll);
            AppTheme.ApplyButtonSecondary(btnDeselectAll);
            AppTheme.ApplyButtonDanger(btnClose);
            AppTheme.ApplyDataGridViewStyle(dgvFiles);

            // Fix: allow interaction after theme sets ReadOnly=true
            dgvFiles.ReadOnly = false;
            foreach (DataGridViewColumn col in dgvFiles.Columns)
                col.ReadOnly = (col.Name != "Selected" && col.Name != "Important" && col.Name != "Note");
        }

        private void BtnSelectClick(object sender, EventArgs e)
        {
            menuSelect.Show(btnSelect, new Point(0, btnSelect.Height));
        }

        private void BtnSelectFolderClick(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.Description = "Chọn thư mục chứa tài liệu cần import";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ScanFolder(dialog.SelectedPath);
            }
        }

        private void BtnSelectFilesClick(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = "Chọn các file cần import";
            dialog.Filter = "Tất cả các file|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddFilesToGrid(dialog.FileNames);
            }
        }

        private void AddFilesToGrid(string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (fileEntries.Any(e => e.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var info = new FileInfo(filePath);
                var entry = new FileEntry
                {
                    FileName = Path.GetFileNameWithoutExtension(filePath),
                    FileType = DetectFileType(info.Extension),
                    FileSize = info.Length,
                    FilePath = filePath,
                    Extension = info.Extension
                };
                fileEntries.Add(entry);

                int rowIndex = dgvFiles.Rows.Add();
                var row = dgvFiles.Rows[rowIndex];
                row.Cells["Selected"].Value = true;
                row.Cells["FileName"].Value = entry.FileName;
                row.Cells["FileType"].Value = entry.FileType;
                row.Cells["FileSize"].Value = Document.FormatFileSize(entry.FileSize);
                row.Cells["FilePath"].Value = entry.FilePath;
                row.Cells["Important"].Value = false;
                row.Cells["Note"].Value = "";
            }

            lblFolder.Text = $"Đang chờ import {fileEntries.Count} tài liệu";
            lblStatus.Text = $"Đã chọn {fileEntries.Count} file";
            btnImport.Enabled = fileEntries.Count > 0;
        }

        private void ScanFolder(string folderPath)
        {
            lblFolder.Text = folderPath;

            var searchOption = chkRecursive.Checked
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            try
            {
                var files = Directory.GetFiles(folderPath, "*.*", searchOption);

                foreach (var filePath in files)
                {
                    if (fileEntries.Any(e => e.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    var info = new FileInfo(filePath);
                    var entry = new FileEntry
                    {
                        FileName = Path.GetFileNameWithoutExtension(filePath),
                        FileType = DetectFileType(info.Extension),
                        FileSize = info.Length,
                        FilePath = filePath,
                        Extension = info.Extension
                    };
                    fileEntries.Add(entry);

                    int rowIndex = dgvFiles.Rows.Add();
                    var row = dgvFiles.Rows[rowIndex];
                    row.Cells["Selected"].Value = true;
                    row.Cells["FileName"].Value = entry.FileName;
                    row.Cells["FileType"].Value = entry.FileType;
                    row.Cells["FileSize"].Value = Document.FormatFileSize(entry.FileSize);
                    row.Cells["FilePath"].Value = entry.FilePath;
                    row.Cells["Important"].Value = false;
                    row.Cells["Note"].Value = "";
                }

                lblFolder.Text = $"Đang chờ import {fileEntries.Count} tài liệu";
                lblStatus.Text = $"Đã có {fileEntries.Count} file trong danh sách";
                btnImport.Enabled = fileEntries.Count > 0;
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi quét thư mục: " + ex.Message);
            }
        }

        private void ToggleAll(bool selected)
        {
            foreach (DataGridViewRow row in dgvFiles.Rows)
            {
                row.Cells["Selected"].Value = selected;
            }
        }

        private void BtnImportClick(object sender, EventArgs e)
        {
            var selectedIndices = new List<int>();
            for (int i = 0; i < dgvFiles.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvFiles.Rows[i].Cells["Selected"].Value ?? false))
                    selectedIndices.Add(i);
            }

            if (selectedIndices.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn ít nhất 1 file để import.");
                return;
            }

            progressBar.Visible = true;
            progressBar.Minimum = 0;
            progressBar.Maximum = selectedIndices.Count;
            progressBar.Value = 0;

            btnImport.Enabled = false;
            btnSelect.Enabled = false;

            int success = 0;
            int failed = 0;
            int skipped = 0;

            foreach (int idx in selectedIndices)
            {
                var entry = fileEntries[idx];
                
                // Kiểm tra trùng lặp dựa trên đường dẫn
                if (DatabaseHelper.CheckDocumentExists(entry.FilePath))
                {
                    skipped++;
                    progressBar.Value++;
                    continue;
                }

                try
                {
                    string note = dgvFiles.Rows[idx].Cells["Note"].Value?.ToString() ?? "";
                    bool isImportant = Convert.ToBoolean(dgvFiles.Rows[idx].Cells["Important"].Value ?? false);
                    string managedPath = FileStorageService.ImportFile(entry.FilePath);
                    bool result = DatabaseHelper.InsertDocument(
                        entry.FileName,
                        entry.FileType,
                        managedPath,
                        note,
                        (decimal)entry.FileSize / (1024.0m * 1024.0m),
                        isImportant,
                        UserSession.CurrentUserId,
                        Guid.NewGuid(),
                        1, // Version
                        ""
                    );

                    if (result) success++;
                    else failed++;
                }
                catch
                {
                    failed++;
                }

                progressBar.Value++;
                Application.DoEvents();
            }

            progressBar.Visible = false;
            btnImport.Enabled = true;
            btnSelect.Enabled = true;

            string statusText = $"Hoàn tất: {success} thành công";
            if (skipped > 0) statusText += $", {skipped} đã tồn tại";
            if (failed > 0) statusText += $", {failed} lỗi";
            lblStatus.Text = statusText;

            if (failed > 0)
                ToastNotification.Warning($"Import xong: {success} thành công, {skipped} bỏ qua, {failed} thất bại.");
            else if (success > 0)
                ToastNotification.Success($"Đã import thành công {success} tài liệu!");
            else if (skipped > 0)
                ToastNotification.Info($"Tài liệu đã tồn tại trong hệ thống ({skipped} file).");

            if (success > 0)
            {
                // Clear list after successful import as requested
                fileEntries.Clear();
                dgvFiles.Rows.Clear();
                lblFolder.Text = "Sẵn sàng import";
                btnImport.Enabled = false;
                
                ImportCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private static string DetectFileType(string extension) => extension.ToLowerInvariant() switch
        {
            ".pdf" => "PDF",
            ".doc" or ".docx" => "Word",
            ".xls" or ".xlsx" => "Excel",
            ".ppt" or ".pptx" => "PowerPoint",
            ".txt" => "Text",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "Hình ảnh",
            ".mp4" or ".avi" or ".mkv" or ".mov" => "Video",
            ".mp3" or ".wav" or ".flac" => "Audio",
            ".zip" or ".rar" or ".7z" => "Nén",
            ".html" or ".htm" => "HTML",
            ".cs" or ".java" or ".py" or ".js" or ".ts" => "Code",
            _ => extension.TrimStart('.').ToUpper()
        };


        private void DgvFilesCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvFiles.Columns[e.ColumnIndex].Name == "Important")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                bool isImportant = Convert.ToBoolean(e.Value ?? false);
                string star = "★";
                
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                using GraphicsPath path = new();
                float emSize = e.Graphics.DpiY * 16 / 72; 
                path.AddString(star, e.CellStyle.Font.FontFamily, (int)FontStyle.Bold, emSize, 
                    new Point(0, 0), StringFormat.GenericDefault);

                RectangleF bounds = path.GetBounds();
                float x = e.CellBounds.Left + (e.CellBounds.Width - bounds.Width) / 2 - bounds.Left;
                float y = e.CellBounds.Top + (e.CellBounds.Height - bounds.Height) / 2 - bounds.Top;

                using Matrix m = new();
                m.Translate(x, y);
                path.Transform(m);

                if (isImportant)
                {
                    using Brush goldBrush = new SolidBrush(Color.Gold);
                    using Pen borderPen = new(Color.FromArgb(160, 160, 160), 1f);
                    e.Graphics.FillPath(goldBrush, path);
                    e.Graphics.DrawPath(borderPen, path);
                }
                else
                {
                    using Pen grayPen = new(Color.LightGray, 1.5f);
                    e.Graphics.DrawPath(grayPen, path);
                }
                e.Handled = true;
            }
        }

        private void DgvFilesCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvFiles.Columns[e.ColumnIndex].Name == "Important")
            {
                var cell = dgvFiles.Rows[e.RowIndex].Cells[e.ColumnIndex];
                bool currentValue = Convert.ToBoolean(cell.Value ?? false);
                cell.Value = !currentValue;
                dgvFiles.InvalidateCell(e.ColumnIndex, e.RowIndex);
            }
        }

        private class FileEntry
        {
            public string FileName { get; set; }
            public string FileType { get; set; }
            public long FileSize { get; set; }
            public string FilePath { get; set; }
            public string Extension { get; set; }
        }
    }
}
