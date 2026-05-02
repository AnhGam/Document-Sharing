using document_sharing_manager.Core.Data;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Core.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.UI.Controls;

namespace document_sharing_manager.Documents
{
    public partial class AddEditForm : global::System.Windows.Forms.Form
    {
        private readonly int? _documentId = null;
        private string _originalPath = null;

        public AddEditForm()
        {
            InitializeComponent();
            this.Text = "Thêm tài liệu mới";
            LoadComboBoxData();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            this.ForeColor = AppTheme.TextPrimary;
            this.Font = AppTheme.FontBody;
            this.Padding = new Padding(AppTheme.Space4);

            // Style all controls recursively (including those inside containers)
            ApplyThemeToControls(this.Controls);

            // Buttons
            AppTheme.ApplyButtonPrimary(btnChonFile);
            AppTheme.ApplyButtonSuccess(btnLuu);
            AppTheme.ApplyButtonSecondary(btnHuy);

            // Important checkbox
            chkQuanTrong.ForeColor = AppTheme.AccentAmber;
            chkQuanTrong.Font = new Font(AppTheme.FontFamily, 9F, FontStyle.Bold);
        }

        /// <summary>
        /// Constructor cho chế độ sửa
        /// </summary>
        /// <param name="id">ID của tài liệu cần sửa</param>
        public AddEditForm(int id) : this()
        {
            _documentId = id;
            this.Text = "Sửa tài liệu";
            LoadDocumentData();
        }

        /// <summary>
        /// Load dữ liệu vào ComboBox
        /// </summary>
        private void LoadComboBoxData()
        {
            // cboDanhMuc logic removed

            // Định dạng tài liệu
            cboDinhDang.Items.Clear();
            cboDinhDang.Items.Add("Tài liệu");
            cboDinhDang.Items.Add("Báo cáo");
            cboDinhDang.Items.Add("Hướng dẫn");
            cboDinhDang.Items.Add("Biểu mẫu");
            cboDinhDang.Items.Add("Hình ảnh");
            cboDinhDang.Items.Add("Video");
            cboDinhDang.Items.Add("Khác");
        }

        /// <summary>
        /// Load dữ liệu tài liệu cần sửa
        /// </summary>
        private void LoadDocumentData()
        {
            if (!_documentId.HasValue) return;

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM tai_lieu WHERE id = @id",
                    new System.Data.SQLite.SQLiteParameter[]
                    {
                        new System.Data.SQLite.SQLiteParameter("@id", _documentId.Value)
                    });

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txtTen.Text = row["ten"].ToString();
                    cboDinhDang.Text = row["dinh_dang"].ToString();
                    txtDuongDan.Text = row["duong_dan"].ToString();
                    _originalPath = txtDuongDan.Text;
                    txtGhiChu.Text = row["ghi_chu"].ToString();
                    
                    if (row["kich_thuoc"] != DBNull.Value)
                    {
                        double size = Convert.ToDouble(row["kich_thuoc"]);
                        txtKichThuoc.Text = FormatFileSize(size);
                    }
                    
                    chkQuanTrong.Checked = Convert.ToBoolean(row["quan_trong"]);

                    // Load Tags (Phase 2)
                    if (row["tags"] != DBNull.Value)
                    {
                        txtTags.Text = row["tags"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi khi load dữ liệu: " + ex.Message);
            }
        }

        /// <summary>
        /// Button chọn file
        /// </summary>
        private void BtnChonFileClick(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Tất cả file hỗ trợ|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt;*.xlsx;*.xls;*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.ico;*.tiff;*.webp;*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.webm;*.flv;*.m4v|" +
                          "Tài liệu|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt;*.xlsx;*.xls|" +
                          "Hình ảnh|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.ico;*.tiff;*.webp|" +
                          "Video|*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.webm;*.flv;*.m4v|" +
                          "PDF (*.pdf)|*.pdf|" +
                          "Word (*.doc;*.docx)|*.doc;*.docx|" +
                          "Excel (*.xlsx;*.xls)|*.xlsx;*.xls|" +
                          "PowerPoint (*.ppt;*.pptx)|*.ppt;*.pptx",
                Title = "Chọn file"
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string duongDan = openFileDialog.FileName;
                    txtDuongDan.Text = duongDan;

                    // Tự động điền tên file nếu chưa có tên
                    if (string.IsNullOrWhiteSpace(txtTen.Text))
                    {
                        txtTen.Text = Path.GetFileNameWithoutExtension(duongDan);
                    }

                    // Tự động nhận diện loại file
                    string ext = Path.GetExtension(duongDan).ToLowerInvariant();
                    string detectedType = DetectFileType(ext);
                    if (!string.IsNullOrEmpty(detectedType))
                    {
                        cboDinhDang.SelectedItem = detectedType;
                    }

                    // Tính kích thước file
                    try
                    {
                        FileInfo fileInfo = new FileInfo(duongDan);
                        double kichThuoc = fileInfo.Length / (1024.0 * 1024.0); // Convert to MB
                        txtKichThuoc.Text = FormatFileSize(kichThuoc);
                    }
                    catch
                    {
                        txtKichThuoc.Text = "0.00";
                    }
                }
            }
        }


        /// <summary>
        /// Button lưu
        /// </summary>
        private void BtnLuuClick(object sender, EventArgs e)
        {
            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                ToastNotification.Warning("Vui lòng nhập tên tài liệu!");
                txtTen.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDuongDan.Text))
            {
                ToastNotification.Warning("Vui lòng chọn file tài liệu!");
                btnChonFile.Focus();
                return;
            }

            // Kiểm tra file có tồn tại không
            if (!File.Exists(txtDuongDan.Text))
            {
                ToastNotification.Error("File không tồn tại! Vui lòng chọn file khác.");
                return;
            }

            try
            {
                decimal? kichThuoc = null;
                if (!string.IsNullOrWhiteSpace(txtKichThuoc.Text))
                {
                    kichThuoc = Convert.ToDecimal(txtKichThuoc.Text);
                }

                bool success = false;

                // Lấy giá trị tags
                string tags = txtTags.Text.Trim();

                if (_documentId.HasValue)
                {
                    // Sửa tài liệu
                    string finalPath = txtDuongDan.Text.Trim();
                    if (finalPath != _originalPath)
                    {
                        finalPath = FileStorageService.ImportFile(finalPath);
                    }

                    success = DatabaseHelper.UpdateDocument(
                        _documentId.Value,
                        txtTen.Text.Trim(),
                        cboDinhDang.Text.Trim(),
                        finalPath,
                        txtGhiChu.Text.Trim(),
                        kichThuoc,
                        chkQuanTrong.Checked,
                        tags
                    );

                    if (success)
                    {
                        ToastNotification.Success("Cập nhật tài liệu thành công!");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    // Thêm tài liệu mới
                    success = DatabaseHelper.InsertDocument(
                        txtTen.Text.Trim(),
                        cboDinhDang.Text.Trim(),
                        FileStorageService.ImportFile(txtDuongDan.Text.Trim()),
                        txtGhiChu.Text.Trim(),
                        kichThuoc,
                        chkQuanTrong.Checked,
                        tags
                    );

                    if (success)
                    {
                        ToastNotification.Success("Thêm tài liệu thành công!");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi khi lưu: " + ex.Message);
            }
        }

        /// <summary>
        /// Button hủy
        /// </summary>
        private void BtnHuyClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Nhấn Enter ở TextBox tên -> chuyển focus
        /// </summary>
        private void TxtTenKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private string FormatFileSize(double mb)
        {
            double bytes = mb * 1024 * 1024;
            if (bytes < 1024) return $"{bytes:F0} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F2} KB";
            if (bytes < 1024L * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F2} MB";
            return $"{bytes / (1024.0 * 1024 * 1024.0):F2} GB";
        }

        private void AddEditFormLoad(object sender, EventArgs e)
        {
            // Inherit app icon from parent form
            if (this.Owner != null && this.Owner.Icon != null)
                this.Icon = this.Owner.Icon;
        }

        private static string DetectFileType(string ext)
        {
            switch (ext)
            {
                case ".jpg": case ".jpeg": case ".png": case ".gif":
                case ".bmp": case ".ico": case ".tiff": case ".webp":
                    return "Hình ảnh";
                case ".mp4": case ".avi": case ".mkv": case ".mov":
                case ".wmv": case ".webm": case ".flv": case ".m4v":
                    return "Video";
                case ".pdf":
                case ".doc": case ".docx":
                case ".ppt": case ".pptx":
                case ".xls": case ".xlsx":
                case ".txt":
                    return "Tài liệu";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Recursively apply theme to all controls including those inside containers
        /// </summary>
        private void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.ForeColor = AppTheme.TextPrimary;
                    lbl.Font = AppTheme.FontSmallBold;
                }
                else if (ctrl is TextBox txt)
                {
                    txt.BackColor = AppTheme.InputBackground;
                    txt.ForeColor = AppTheme.TextPrimary;
                    txt.Font = AppTheme.FontInput;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (ctrl is ComboBox cbo)
                {
                    AppTheme.ApplyComboBoxStyle(cbo);
                }
                else if (ctrl is CheckBox chk)
                {
                    chk.ForeColor = AppTheme.TextPrimary;
                    chk.Font = AppTheme.FontSmall;
                }

                // Recurse into containers
                if (ctrl.HasChildren)
                {
                    ApplyThemeToControls(ctrl.Controls);
                }
            }
        }
    }
}
