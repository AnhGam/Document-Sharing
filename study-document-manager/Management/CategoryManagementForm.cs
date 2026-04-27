using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using study_document_manager.UI;
using study_document_manager.UI.Controls;

namespace study_document_manager
{
    public partial class CategoryManagementForm : Form
    {
        private string currentTab = "subject"; // "subject" (danh mục) hoặc "type" (định dạng)

        public CategoryManagementForm()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;

            // Header panel - Teal header
            headerPanel.BackColor = AppTheme.Primary;
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font(AppTheme.FontFamily, 14F, FontStyle.Bold);

            // Close button
            btnClose.BackColor = AppTheme.StatusError;
            btnClose.ForeColor = AppTheme.TextWhite;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;

            // Tabs panel
            pnlTabs.BackColor = AppTheme.BackgroundMain;

            // Tab buttons - Active tab style
            AppTheme.ApplyButtonPrimary(btnSubjects);
            AppTheme.ApplyButtonSecondary(btnTypes);

            // Buttons panel
            pnlButtons.BackColor = AppTheme.BackgroundMain;

            // Action buttons
            AppTheme.ApplyButtonSuccess(btnAdd);
            AppTheme.ApplyButtonPrimary(btnEdit);
            AppTheme.ApplyButtonDanger(btnDelete);

            // Labels
            lblCount.ForeColor = AppTheme.Primary;

            // Status strip
            statusStrip.BackColor = AppTheme.BackgroundSoft;
            lblStatus.ForeColor = AppTheme.TextSecondary;
        }

        private void CategoryManagementFormLoad(object sender, EventArgs e)
        {
            // Chế độ cá nhân: Mọi user đều được quản lý danh mục của mình
            // Không cần kiểm tra quyền nữa

            LoadSubjects();
        }

        /// <summary>
        /// Load danh sách danh mục
        /// </summary>
        private void LoadSubjects()
        {
            currentTab = "subject";
            try
            {
                DataTable dt = DatabaseHelper.GetDistinctSubjects();
                dgvCategories.DataSource = dt;
                SetupDataGridView("Danh mục");
                lblCount.Text = "Tổng số: " + dt.Rows.Count + " danh mục";
                lblStatus.Text = "Đã tải danh sách danh mục";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("L\u1ed7i: " + ex.Message);
            }
        }

        /// <summary>
        /// Load danh sách định dạng tài liệu
        /// </summary>
        private void LoadTypes()
        {
            currentTab = "type";
            try
            {
                DataTable dt = DatabaseHelper.GetDistinctTypes();
                dgvCategories.DataSource = dt;
                SetupDataGridView("Định dạng");
                lblCount.Text = "Tổng số: " + dt.Rows.Count + " định dạng";
                lblStatus.Text = "Đã tải danh sách định dạng";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("L\u1ed7i: " + ex.Message);
            }
        }

        /// <summary>
        /// Setup DataGridView
        /// </summary>
        private void SetupDataGridView(string columnName)
        {
            if (dgvCategories.Columns.Count > 0)
            {
                dgvCategories.Columns[0].HeaderText = columnName;
                dgvCategories.Columns[0].Width = 200;
                dgvCategories.Columns[1].HeaderText = "Số lượng tài liệu";
                dgvCategories.Columns[1].Width = 150;
            }

            AppTheme.ApplyDataGridViewStyle(dgvCategories);
        }

        /// <summary>
        /// Tab danh mục
        /// </summary>
        private void BtnSubjectsClick(object sender, EventArgs e)
        {
            LoadSubjects();
        }

        /// <summary>
        /// Tab định dạng tài liệu
        /// </summary>
        private void BtnTypesClick(object sender, EventArgs e)
        {
            LoadTypes();
        }

        /// <summary>
        /// Button Thêm
        /// </summary>
        private void BtnAddClick(object sender, EventArgs e)
        {
            string title = currentTab == "subject" ? "Thêm danh mục mới" : "Thêm định dạng mới";
            string label = currentTab == "subject" ? "Tên danh mục:" : "Tên định dạng:";

            string newValue = ModernInputBox.Show(title, label, "");

            if (string.IsNullOrWhiteSpace(newValue))
                return;

            // Kiểm tra trùng lặp
            if (CheckDuplicate(newValue))
            {
                ToastNotification.Warning("'" + newValue + "' đã tồn tại!");
                return;
            }

            // Thông báo: Cần tạo ít nhất 1 tài liệu để thêm danh mục
            DialogResult result = MessageBox.Show(
                "Để thêm '" + newValue + "' vào danh sách, bạn có muốn tạo một tài liệu mẫu không?\n\n" +
                "Nếu chọn 'Không', bạn sẽ cần tự tạo tài liệu có danh mục/định dạng này sau.",
                "Xác nhận", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Tạo tài liệu mẫu
                bool success = CreateSampleDocument(newValue);
                if (success)
                {
                    ToastNotification.Success("Đã thêm '" + newValue + "' thành công!");
                    RefreshData();
                }
            }
            else if (result == DialogResult.No)
            {
                ToastNotification.Info("Vui lòng tạo tài liệu có " + label.ToLower() + " '" + newValue + "' để xuất hiện trong danh sách.");
            }
        }

        /// <summary>
        /// Button Sửa
        /// </summary>
        private void BtnEditClick(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn mục cần sửa!");
                return;
            }

            string oldValue = dgvCategories.SelectedRows[0].Cells[0].Value.ToString();
            int count = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells[1].Value);

            string title = currentTab == "subject" ? "Sửa danh mục" : "Sửa định dạng";
            string label = currentTab == "subject" ? "Tên danh mục mới:" : "Tên định dạng mới:";

            string newValue = ModernInputBox.Show(title, label, oldValue);

            if (string.IsNullOrWhiteSpace(newValue) || newValue == oldValue)
                return;

            // Kiểm tra trùng lặp
            if (CheckDuplicate(newValue))
            {
                ToastNotification.Warning("'" + newValue + "' đã tồn tại!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Đổi '" + oldValue + "' thành '" + newValue + "'?\n\n" +
                "Thao tác này sẽ cập nhật " + count + " tài liệu!",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool success = UpdateCategoryValue(oldValue, newValue);
                if (success)
                {
                    ToastNotification.Success("Đã cập nhật thành công!");
                    RefreshData();
                }
            }
        }

        /// <summary>
        /// Button Xóa
        /// </summary>
        private void BtnDeleteClick(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                ToastNotification.Warning("Vui lòng chọn mục cần xóa!");
                return;
            }

            string value = dgvCategories.SelectedRows[0].Cells[0].Value.ToString();
            int count = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells[1].Value);

            string message = currentTab == "subject" 
                ? "Xóa danh mục '" + value + "'?\n\nThao tác này sẽ:\n" +
                  "- Xóa toàn bộ " + count + " tài liệu có danh mục này\n" +
                  "- KHÔNG THỂ HOÀN TÁC!\n\n" +
                  "Bạn có chắc chắn muốn tiếp tục?"
                : "Xóa định dạng '" + value + "'?\n\nThao tác này sẽ:\n" +
                  "- Xóa toàn bộ " + count + " tài liệu có định dạng này\n" +
                  "- KHÔNG THỂ HOÀN TÁC!\n\n" +
                  "Bạn có chắc chắn muốn tiếp tục?";

            DialogResult result = MessageBox.Show(message, 
                "Cảnh báo - Xóa vĩnh viễn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Xác nhận lần 2
                DialogResult confirm = MessageBox.Show(
                    "XÁC NHẬN LẦN CUỐI:\n\nXóa " + count + " tài liệu có " + (currentTab == "subject" ? "danh mục" : "định dạng") + " '" + value + "'?", 
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (confirm == DialogResult.Yes)
                {
                    bool success = DeleteCategoryDocuments(value);
                    if (success)
                    {
                        ToastNotification.Success("\u0110\u00e3 x\u00f3a th\u00e0nh c\u00f4ng! " + count + " t\u00e0i li\u1ec7u \u0111\u00e3 b\u1ecb x\u00f3a.");
                        RefreshData();
                    }
                }
            }
        }

        /// <summary>
        /// Button Đóng
        /// </summary>
        private void BtnCloseClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Làm mới dữ liệu
        /// </summary>
        private void RefreshData()
        {
            if (currentTab == "subject")
                LoadSubjects();
            else
                LoadTypes();
        }

        /// <summary>
        /// Kiểm tra trùng lặp
        /// </summary>
        private bool CheckDuplicate(string value)
        {
            DataTable dt = (DataTable)dgvCategories.DataSource;
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Tạo tài liệu mẫu
        /// </summary>
        private bool CreateSampleDocument(string categoryValue)
        {
            string ten = currentTab == "subject" 
                ? "Tài liệu mẫu - " + categoryValue
                : "Tài liệu mẫu - " + categoryValue;
            
            string danhMuc = currentTab == "subject" ? categoryValue : "";
            string dinhDang = currentTab == "type" ? categoryValue : "tài liệu khác";
            string duongDan = ""; // Để trống, người dùng sẽ sửa sau
            
            return DatabaseHelper.InsertDocument(ten, danhMuc, dinhDang, duongDan, 
                "Đây là tài liệu mẫu. Vui lòng cập nhật thông tin.", null, false, "");
        }

        /// <summary>
        /// Cập nhật giá trị danh mục
        /// </summary>
        private bool UpdateCategoryValue(string oldValue, string newValue)
        {
            if (currentTab == "subject")
                return DatabaseHelper.UpdateSubjectName(oldValue, newValue);
            else
                return DatabaseHelper.UpdateTypeName(oldValue, newValue);
        }

        /// <summary>
        /// Xóa tất cả tài liệu có danh mục này
        /// </summary>
        private bool DeleteCategoryDocuments(string categoryValue)
        {
            if (currentTab == "subject")
                return DatabaseHelper.DeleteDocumentsBySubject(categoryValue);
            else
                return DatabaseHelper.DeleteDocumentsByType(categoryValue);
        }
    }
}
