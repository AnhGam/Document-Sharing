using System;
using System.IO;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class AddEditForm : Form
    {
        private int? document_id = null; // null = thêm mới, có giá trị = sửa
        
        /// <summary>
        /// Constructor cho chế độ thêm mới
        /// </summary>
        public AddEditForm()
        {
            InitializeComponent();
            this.Text = "Thêm tài liệu mới";
            LoadComboBoxData();
            
            // Event cho checkbox deadline
            chkHasDeadline.CheckedChanged += ChkHasDeadline_CheckedChanged;
        }

        private void ChkHasDeadline_CheckedChanged(object sender, EventArgs e)
        {
            dtpDeadline.Enabled = chkHasDeadline.Checked;
            if (chkHasDeadline.Checked && dtpDeadline.Value < DateTime.Now.Date)
            {
                dtpDeadline.Value = DateTime.Now.AddDays(7); // Mặc định 1 tuần
            }
        }

        /// <summary>
        /// Constructor cho chế độ sửa
        /// </summary>
        /// <param name="id">ID của tài liệu cần sửa</param>
        public AddEditForm(int id) : this()
        {
            document_id = id;
            this.Text = "Sửa tài liệu";
            LoadDocumentData();
        }

        /// <summary>
        /// Load dữ liệu vào ComboBox
        /// </summary>
        private void LoadComboBoxData()
        {
            // Danh mục - Phù hợp cho mọi đối tượng người dùng
            cbo_mon_hoc.Items.Clear();
            cbo_mon_hoc.Items.Add("Công việc");
            cbo_mon_hoc.Items.Add("Cá nhân");
            cbo_mon_hoc.Items.Add("Học tập");
            cbo_mon_hoc.Items.Add("Dự án");
            cbo_mon_hoc.Items.Add("Tài chính");
            cbo_mon_hoc.Items.Add("Hợp đồng");
            cbo_mon_hoc.Items.Add("Tham khảo");
            cbo_mon_hoc.Items.Add("Khác");

            // Loại tài liệu
            cbo_loai.Items.Clear();
            cbo_loai.Items.Add("Tài liệu");
            cbo_loai.Items.Add("Báo cáo");
            cbo_loai.Items.Add("Hướng dẫn");
            cbo_loai.Items.Add("Biểu mẫu");
            cbo_loai.Items.Add("Hình ảnh");
            cbo_loai.Items.Add("Video");
            cbo_loai.Items.Add("Khác");
        }

        /// <summary>
        /// Load dữ liệu tài liệu cần sửa
        /// </summary>
        private void LoadDocumentData()
        {
            if (!document_id.HasValue) return;

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM tai_lieu WHERE id = @id",
                    new System.Data.SqlClient.SqlParameter[]
                    {
                        new System.Data.SqlClient.SqlParameter("@id", document_id.Value)
                    });

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txt_ten.Text = row["ten"].ToString();
                    cbo_mon_hoc.Text = row["mon_hoc"].ToString();
                    cbo_loai.Text = row["loai"].ToString();
                    txt_duong_dan.Text = row["duong_dan"].ToString();
                    txt_ghi_chu.Text = row["ghi_chu"].ToString();
                    txt_tac_gia.Text = row["tac_gia"].ToString();
                    
                    if (row["kich_thuoc"] != DBNull.Value)
                    {
                        double size = Convert.ToDouble(row["kich_thuoc"]);
                        txt_kich_thuoc.Text = size.ToString("F2");
                    }
                    
                    chk_quan_trong.Checked = Convert.ToBoolean(row["quan_trong"]);

                    // Load Tags (Phase 2)
                    if (row["tags"] != DBNull.Value)
                    {
                        txtTags.Text = row["tags"].ToString();
                    }

                    // Load Deadline (Phase 2)
                    if (row["deadline"] != DBNull.Value)
                    {
                        chkHasDeadline.Checked = true;
                        dtpDeadline.Value = Convert.ToDateTime(row["deadline"]);
                    }
                    else
                    {
                        chkHasDeadline.Checked = false;
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
        private void btn_chon_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "All Files|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.txt;*.xlsx;*.xls|" +
                                      "PDF Files (*.pdf)|*.pdf|" +
                                      "Word Files (*.doc;*.docx)|*.doc;*.docx|" +
                                      "PowerPoint Files (*.ppt;*.pptx)|*.ppt;*.pptx|" +
                                      "Text Files (*.txt)|*.txt|" +
                                      "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls";
            open_file_dialog.Title = "Chọn tài liệu";

            if (open_file_dialog.ShowDialog() == DialogResult.OK)
            {
                string duong_dan = open_file_dialog.FileName;
                txt_duong_dan.Text = duong_dan;

                // Tự động điền tên file nếu chưa có tên
                if (string.IsNullOrWhiteSpace(txt_ten.Text))
                {
                    txt_ten.Text = Path.GetFileNameWithoutExtension(duong_dan);
                }

                // Tính kích thước file
                try
                {
                    FileInfo file_info = new FileInfo(duong_dan);
                    double kich_thuoc = file_info.Length / (1024.0 * 1024.0); // Convert to MB
                    txt_kich_thuoc.Text = kich_thuoc.ToString("F2");
                }
                catch
                {
                    txt_kich_thuoc.Text = "0.00";
                }
            }
        }

        /// <summary>
        /// Button lưu
        /// </summary>
        private void btn_luu_Click(object sender, EventArgs e)
        {
            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(txt_ten.Text))
            {
                ToastNotification.Warning("Vui lòng nhập tên tài liệu!");
                txt_ten.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_duong_dan.Text))
            {
                ToastNotification.Warning("Vui lòng chọn file tài liệu!");
                btn_chon_file.Focus();
                return;
            }

            // Kiểm tra file có tồn tại không
            if (!File.Exists(txt_duong_dan.Text))
            {
                ToastNotification.Error("File không tồn tại! Vui lòng chọn file khác.");
                return;
            }

            try
            {
                double? kich_thuoc = null;
                if (!string.IsNullOrWhiteSpace(txt_kich_thuoc.Text))
                {
                    kich_thuoc = Convert.ToDouble(txt_kich_thuoc.Text);
                }

                bool success = false;

                // Lấy giá trị tags và deadline
                string tags = txtTags.Text.Trim();
                DateTime? deadline = chkHasDeadline.Checked ? (DateTime?)dtpDeadline.Value.Date : null;

                if (document_id.HasValue)
                {
                    // Sửa tài liệu
                    success = DatabaseHelper.UpdateDocument(
                        document_id.Value,
                        txt_ten.Text.Trim(),
                        cbo_mon_hoc.Text.Trim(),
                        cbo_loai.Text.Trim(),
                        txt_duong_dan.Text.Trim(),
                        txt_ghi_chu.Text.Trim(),
                        kich_thuoc,
                        txt_tac_gia.Text.Trim(),
                        chk_quan_trong.Checked,
                        tags,
                        deadline
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
                        txt_ten.Text.Trim(),
                        cbo_mon_hoc.Text.Trim(),
                        cbo_loai.Text.Trim(),
                        txt_duong_dan.Text.Trim(),
                        txt_ghi_chu.Text.Trim(),
                        kich_thuoc,
                        txt_tac_gia.Text.Trim(),
                        chk_quan_trong.Checked,
                        tags,
                        deadline
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
        private void btn_huy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Nhấn Enter ở TextBox tên -> chuyển focus
        /// </summary>
        private void txt_ten_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                cbo_mon_hoc.Focus();
            }
        }

        private void AddEditForm_Load(object sender, EventArgs e)
        {

        }
    }
}
