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
            
            // Tạo menu Quản lý động
            CreateManagementMenu();
            
            // Đăng ký event CellFormatting
            dgvDocuments.CellFormatting += dgvDocuments_CellFormatting;
            // Đăng ký event DataError để xử lý lỗi format
            dgvDocuments.DataError += dgvDocuments_DataError;
            // Cho phép click trực tiếp lên cột quan trọng
            dgvDocuments.CellClick += dgvDocuments_CellClick;
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
                    // Tạo menu "Quản lý" (chỉ Admin)
                    ToolStripMenuItem menuManagement = new ToolStripMenuItem("&Quản lý");
                    menuManagement.Name = "menuManagement";

                    // Submenu: Quản lý người dùng
                    ToolStripMenuItem menuManagementUsers = new ToolStripMenuItem("Quản lý người dùng");
                    menuManagementUsers.Name = "menuManagementUsers";
                    menuManagementUsers.ShortcutKeys = Keys.Control | Keys.U;
                    menuManagementUsers.Click += menuManagementUsers_Click;

                    // Thêm submenu vào menu Quản lý
                    menuManagement.DropDownItems.Add(menuManagementUsers);

                    // Thêm menu Quản lý vào MenuStrip
                    int insertIndex = menuStrip.Items.Count;
                    menuStrip.Items.Insert(insertIndex, menuManagement);

                    // Lưu reference để dùng trong ApplyPermissions
                    this.menuManagement = menuManagement;

                    // ===================================
                    // THÊM "ĐĂNG XUẤT" VÀO MENU HIỂN THỊ
                    // ===================================
                    // Tìm menu "Hiển thị" (menuView)
                    ToolStripMenuItem menuView = null;
                    foreach (ToolStripItem item in menuStrip.Items)
                    {
                        if (item.Name == "menuView" || item.Text.Contains("Hiển thị") || item.Text.Contains("View"))
                        {
                            menuView = item as ToolStripMenuItem;
                            break;
                        }
                    }

                    if (menuView != null)
                    {
                        // Thêm menu Kiểm tra file bị thiếu
                        menuView.DropDownItems.Add(new ToolStripSeparator());
                        ToolStripMenuItem menuCheckFiles = new ToolStripMenuItem("Kiểm tra file bị thiếu");
                        menuCheckFiles.Name = "menuCheckFiles";
                        menuCheckFiles.Click += menuCheckFiles_Click;
                        menuView.DropDownItems.Add(menuCheckFiles);

                        // Thêm separator trước Đăng xuất
                        menuView.DropDownItems.Add(new ToolStripSeparator());

                        // Thêm menu Đăng xuất
                        ToolStripMenuItem menuLogout = new ToolStripMenuItem("Đăng xuất");
                        menuLogout.Name = "menuLogout";
                        menuLogout.ShortcutKeys = Keys.Control | Keys.L;
                        menuLogout.Click += menuLogout_Click;
                        menuView.DropDownItems.Add(menuLogout);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error creating menu: " + ex.Message);
            }
        }

        // Field để lưu menu reference
        private ToolStripMenuItem menuManagement;

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeCustomComponents();
            this.Text = $"Study Document Manager - {UserSession.FullName} [{UserSession.Role}]";
            
            // Hiển thị thông tin user
            lblStatus.Text = $"Xin chào, {UserSession.FullName} ({UserSession.Role})";
            
            // ============================================
            // PHÂN QUYỀN
            // ============================================
            ApplyPermissions();
            
            // Áp dụng màu sắc theo README
            this.BackColor = Color.FromArgb(227, 242, 253); // #E3F2FD
            pnlSearch.BackColor = Color.White;
            pnlContent.BackColor = Color.FromArgb(245, 245, 245);
            
            // Load danh sách users cho filter (chỉ Admin/Teacher)
            LoadUsersForFilter();
            
            // Khởi tạo giá trị mặc định cho date pickers
            dtpFromDate.Value = DateTime.Now.AddMonths(-1);
            dtpToDate.Value = DateTime.Now;
            dtpFromDate.Enabled = false;
            dtpToDate.Enabled = false;
            nudMinSize.Enabled = false;
            nudMaxSize.Enabled = false;
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

            // TẤT CẢ USER đều có quyền thêm tài liệu
            // Nhưng Student và Teacher CHỈ sửa/xóa được tài liệu của mình
            // Admin có thể sửa/xóa TẤT CẢ tài liệu

            // Không disable buttons nữa - để tất cả user đều thấy
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
                                // Cột Người tạo (mới) - chỉ hiện cho Teacher/Admin
                if (dgvDocuments.Columns.Contains("creator_name"))
                {
                    dgvDocuments.Columns["creator_name"].HeaderText = "Người tạo";
                    dgvDocuments.Columns["creator_name"].Width = 120;
                    dgvDocuments.Columns["creator_name"].Visible = !UserSession.IsStudent;
                }
                
                if (dgvDocuments.Columns.Contains("creator_username"))
                    dgvDocuments.Columns["creator_username"].Visible = false;
                    
                if (dgvDocuments.Columns.Contains("user_id"))
                    dgvDocuments.Columns["user_id"].Visible = false;
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

                // Sử dụng phân quyền: Student chỉ thấy tài liệu của mình
                DataTable dt = DatabaseHelper.GetDocumentsForCurrentUser();
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                string roleInfo = UserSession.IsStudent ? " (của bạn)" : "";
                lblCount.Text = $"Tổng số: {dt.Rows.Count} tài liệu{roleInfo}";
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
            
            // Kiểm tra quyền sửa tài liệu
            if (!DatabaseHelper.CanUserEditDocument(id, UserSession.UserId, UserSession.Role))
            {
                MessageBox.Show("Bạn không có quyền sửa tài liệu này!\nBạn chỉ có thể sửa tài liệu do bạn tạo.", 
                    "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Vui lòng chọn tài liệu cần xóa!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
            
            // Kiểm tra quyền xóa tài liệu
            if (!DatabaseHelper.CanUserEditDocument(id, UserSession.UserId, UserSession.Role))
            {
                MessageBox.Show("Bạn không có quyền xóa tài liệu này!\nBạn chỉ có thể xóa tài liệu do bạn tạo.", 
                    "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        MessageBox.Show($"File không hỗ trợ: {Path.GetFileName(file)}\n\n" +
                            "Chỉ hỗ trợ: PDF, Word, PowerPoint, Excel, Text",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                
                if (successCount > 0)
                {
                    LoadData();
                    lblStatus.Text = $"Đã thêm {successCount} tài liệu";
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
        /// Menu Quản lý Môn học và Loại
        /// </summary>
        private void menuViewCategories_Click(object sender, EventArgs e)
        {
            // Kiểm tra quyền: Chỉ Teacher và Admin
            if (!UserSession.CanManageCategories)
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng này!\nChỉ Giáo viên và Admin mới được quản lý danh mục.",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Menu Quản lý Người dùng (Admin only)
        /// </summary>
        private void menuManagementUsers_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng này!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Lỗi: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Log activity
                try
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "EXEC sp_LogActivity @user_id, @action, @description",
                        new System.Data.SqlClient.SqlParameter[] {
                            new System.Data.SqlClient.SqlParameter("@user_id", UserSession.UserId),
                            new System.Data.SqlClient.SqlParameter("@action", "Logout"),
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

        private void dgvDocuments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvDocuments.Columns[e.ColumnIndex].Name == "quan_trong")
            {
                dgvDocuments.Rows[e.RowIndex].Selected = true;
                ToggleImportantForSelectedDocument();
            }
        }

        private void ToggleImportantForSelectedDocument()
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài liệu!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int id = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
                var quanTrongCell = dgvDocuments.SelectedRows[0].Cells["quan_trong"].Value;
                bool currentValue = quanTrongCell != null && quanTrongCell != DBNull.Value && Convert.ToBoolean(quanTrongCell);
                bool newValue = !currentValue;

                string query = "UPDATE tai_lieu SET quan_trong = @quan_trong WHERE id = @id";
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@quan_trong", newValue),
                    new System.Data.SqlClient.SqlParameter("@id", id)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    LoadData();
                    lblStatus.Text = newValue ? "Đã đánh dấu quan trọng" : "Đã bỏ đánh dấu quan trọng";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Advanced Filter Methods

        /// <summary>
        /// Load danh sách users để filter (chỉ cho Admin/Teacher)
        /// </summary>
        private void LoadUsersForFilter()
        {
            if (UserSession.IsAdmin || UserSession.IsTeacher)
            {
                try
                {
                    DataTable users = DatabaseHelper.GetUsersForFilter();
                    DataTable dtWithAll = users.Clone();
                    DataRow allRow = dtWithAll.NewRow();
                    allRow["id"] = 0;
                    allRow["username"] = "";
                    allRow["full_name"] = "-- Tất cả --";
                    dtWithAll.Rows.InsertAt(allRow, 0);
                    
                    foreach (DataRow row in users.Rows)
                    {
                        dtWithAll.ImportRow(row);
                    }
                    
                    cboCreatorFilter.DataSource = dtWithAll;
                    cboCreatorFilter.DisplayMember = "full_name";
                    cboCreatorFilter.ValueMember = "id";
                    cboCreatorFilter.SelectedIndex = 0;
                    lblCreatorFilter.Visible = true;
                    cboCreatorFilter.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi load danh sách người dùng: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                lblCreatorFilter.Visible = false;
                cboCreatorFilter.Visible = false;
            }
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
                        MessageBox.Show("Dung lượng tối thiểu không được lớn hơn dung lượng tối đa!",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                bool? isImportant = chkImportantOnly.Checked ? (bool?)true : null;
                int? creatorUserId = null;
                if ((UserSession.IsAdmin || UserSession.IsTeacher) && cboCreatorFilter.Visible)
                {
                    int selectedId = Convert.ToInt32(cboCreatorFilter.SelectedValue);
                    if (selectedId > 0)
                    {
                        creatorUserId = selectedId;
                    }
                }
                
                lblStatus.Text = "Đang lọc dữ liệu...";
                Application.DoEvents();
                
                DataTable dt = DatabaseHelper.SearchDocumentsAdvanced(
                    keyword, monHoc, loai, fromDate, toDate,
                    minSize, maxSize, isImportant, creatorUserId
                );
                
                dgvDocuments.DataSource = dt;
                SetupDataGridView();
                
                string roleInfo = UserSession.IsStudent ? " (của bạn)" : "";
                lblCount.Text = $"Tìm thấy: {dt.Rows.Count} tài liệu{roleInfo}";
                lblStatus.Text = "Đã áp dụng filter";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi khi lọc";
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Vui lòng chọn tài liệu!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var duongDan = dgvDocuments.SelectedRows[0].Cells["duong_dan"].Value;
            if (duongDan != null && duongDan != DBNull.Value)
            {
                Clipboard.SetText(duongDan.ToString());
                lblStatus.Text = "Đã copy đường dẫn vào clipboard";
            }
            else
            {
                MessageBox.Show("Tài liệu không có đường dẫn file!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Context Menu: Mở thư mục chứa file
        /// </summary>
        private void ctxMenuOpenFolder_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài liệu!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show("File không tồn tại!\n" + filePath, 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Tài liệu không có đường dẫn file!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        /// Menu: Kiem tra file bi thieu
        /// </summary>
        private void menuCheckFiles_Click(object sender, EventArgs e)
        {
            FileIntegrityCheckForm form = new FileIntegrityCheckForm();
            form.ShowDialog();
            // Refresh data sau khi dong form
            LoadData();
        }

        #endregion

        private void menuView_Click(object sender, EventArgs e)
        {

        }
    }
}