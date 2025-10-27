using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Đăng ký event CellFormatting
            dgvDocuments.CellFormatting += dgvDocuments_CellFormatting;
            // Đăng ký event DataError để xử lý lỗi format
            dgvDocuments.DataError += dgvDocuments_DataError;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeCustomComponents();
            this.Text = "Study Document Manager - Quản lý tài liệu học tập";
            
            // Áp dụng màu sắc theo README
            this.BackColor = Color.FromArgb(227, 242, 253); // #E3F2FD
            pnlSearch.BackColor = Color.White;
            pnlContent.BackColor = Color.FromArgb(245, 245, 245);
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
            // ComboBox môn học
            cboSubject.Items.Clear();
            cboSubject.Items.Add("Tất cả");
            cboSubject.Items.Add("Lập trình");
            cboSubject.Items.Add("Toán");
            cboSubject.Items.Add("Anh văn");
            cboSubject.Items.Add("Vật lý");
            cboSubject.Items.Add("Hóa học");
            cboSubject.Items.Add("Văn học");
            cboSubject.Items.Add("Lịch sử");
            cboSubject.Items.Add("Địa lý");
            cboSubject.SelectedIndex = 0;

            // ComboBox loại
            cboType.Items.Clear();
            cboType.Items.Add("Tất cả");
            cboType.Items.Add("slide");
            cboType.Items.Add("bài tập");
            cboType.Items.Add("đề thi");
            cboType.Items.Add("tài liệu khác");
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
                    dgvDocuments.Columns["mon_hoc"].HeaderText = "Môn học";
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
                if (dgvDocuments.Columns.Contains("quan_trong"))
                {
                    dgvDocuments.Columns["quan_trong"].HeaderText = "★";
                    dgvDocuments.Columns["quan_trong"].Width = 30;
                }
            }

            // Style
            dgvDocuments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvDocuments.RowsDefaultCellStyle.BackColor = Color.White;
            dgvDocuments.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);
            dgvDocuments.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            dgvDocuments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvDocuments.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDocuments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvDocuments.EnableHeadersVisualStyles = false;
            dgvDocuments.RowTemplate.Height = 32;
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

                DataTable dt = DatabaseHelper.GetAllDocuments();
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                lblCount.Text = $"Tổng số: {dt.Rows.Count} tài liệu";
                lblStatus.Text = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi load dữ liệu";
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Vui lòng chọn tài liệu cần sửa!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
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
                MessageBox.Show("Vui lòng chọn tài liệu cần xóa!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ten = dgvDocuments.SelectedRows[0].Cells["ten"].Value.ToString();
            if (MessageBox.Show($"Xóa tài liệu:\n\"{ten}\"?", 
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
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
                MessageBox.Show("Vui lòng chọn tài liệu!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string duong_dan = dgvDocuments.SelectedRows[0].Cells["duong_dan"].Value.ToString();
            
            if (!File.Exists(duong_dan))
            {
                MessageBox.Show("File không tồn tại!\n" + duong_dan, 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(duong_dan);
                lblStatus.Text = "Đã mở file";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở file: " + ex.Message, 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lọc theo môn học
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
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        writer.WriteLine("ID,Tên tài liệu,Môn học,Loại,Đường dẫn,Ghi chú,Ngày thêm,Kích thước (MB),Tác giả,Quan trọng");
                        
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
                    MessageBox.Show($"Xuất thành công!\n\nFile: {save.FileName}", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + save.FileName + "\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".pdf" || ext == ".doc" || ext == ".docx" || 
                        ext == ".ppt" || ext == ".pptx" || ext == ".txt" || 
                        ext == ".xlsx" || ext == ".xls")
                    {
                        AddEditForm form = new AddEditForm();
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
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
        /// Format cells để hiển thị icon và sao vàng
        /// </summary>
        private void dgvDocuments_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                // Kiểm tra cột "Icon" phải là ImageColumn
                if (dgvDocuments.Columns[e.ColumnIndex].Name == "Icon" && e.RowIndex >= 0)
                {
                    // Chỉ format nếu là DataGridViewImageColumn
                    if (dgvDocuments.Columns[e.ColumnIndex] is DataGridViewImageColumn)
                    {
                        var loaiCell = dgvDocuments.Rows[e.RowIndex].Cells["loai"];
                        if (loaiCell.Value != null && loaiCell.Value != DBNull.Value)
                        {
                            string loai = loaiCell.Value.ToString();
                            e.Value = IconHelper.GetDocumentIcon(loai, 24);
                            e.FormattingApplied = true;
                        }
                    }
                }
                else if (dgvDocuments.Columns[e.ColumnIndex].Name == "quan_trong" && e.RowIndex >= 0)
                {
                    // Chỉ format nếu là TextBoxColumn hoặc DataGridViewColumn thông thường
                    if (!(dgvDocuments.Columns[e.ColumnIndex] is DataGridViewImageColumn))
                    {
                        if (e.Value != null && e.Value != DBNull.Value)
                        {
                            bool isImportant = Convert.ToBoolean(e.Value);
                            if (isImportant)
                            {
                                e.Value = "★";
                                e.CellStyle.ForeColor = Color.FromArgb(255, 202, 40);
                                e.CellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }
                            else
                            {
                                e.Value = "";
                            }
                            e.FormattingApplied = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết để debug
                System.Diagnostics.Debug.WriteLine($"CellFormatting error at column {dgvDocuments.Columns[e.ColumnIndex].Name}: {ex.Message}");
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
    }
}