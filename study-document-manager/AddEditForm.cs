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
            // Môn học
            cbo_mon_hoc.Items.Clear();
            cbo_mon_hoc.Items.Add("Lập trình");
            cbo_mon_hoc.Items.Add("Toán");
            cbo_mon_hoc.Items.Add("Anh văn");
            cbo_mon_hoc.Items.Add("Vật lý");
            cbo_mon_hoc.Items.Add("Hóa học");
            cbo_mon_hoc.Items.Add("Văn học");
            cbo_mon_hoc.Items.Add("Lịch sử");
            cbo_mon_hoc.Items.Add("Địa lý");

            // Loại tài liệu
            cbo_loai.Items.Clear();
            cbo_loai.Items.Add("slide");
            cbo_loai.Items.Add("bài tập");
            cbo_loai.Items.Add("đề thi");
            cbo_loai.Items.Add("tài liệu khác");
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
                MessageBox.Show("Lỗi khi load dữ liệu: " + ex.Message, 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Vui lòng nhập tên tài liệu!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_ten.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_duong_dan.Text))
            {
                MessageBox.Show("Vui lòng chọn file tài liệu!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btn_chon_file.Focus();
                return;
            }

            // Kiểm tra file có tồn tại không
            if (!File.Exists(txt_duong_dan.Text))
            {
                MessageBox.Show("File không tồn tại! Vui lòng chọn file khác.", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show("Cập nhật tài liệu thành công!", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        MessageBox.Show("Thêm tài liệu thành công!", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
