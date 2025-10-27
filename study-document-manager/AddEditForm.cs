using System;
using System.IO;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class AddEditForm : Form
    {
        private int? document_id = null; // null = thêm m?i, có giá tr? = s?a
        
        /// <summary>
        /// Constructor cho ch? ?? thêm m?i
        /// </summary>
        public AddEditForm()
        {
            InitializeComponent();
            this.Text = "Thêm tài li?u m?i";
            LoadComboBoxData();
        }

        /// <summary>
        /// Constructor cho ch? ?? s?a
        /// </summary>
        /// <param name="id">ID c?a tài li?u c?n s?a</param>
        public AddEditForm(int id) : this()
        {
            document_id = id;
            this.Text = "S?a tài li?u";
            LoadDocumentData();
        }

        /// <summary>
        /// Load d? li?u vào ComboBox
        /// </summary>
        private void LoadComboBoxData()
        {
            // Môn h?c
            cbo_mon_hoc.Items.Clear();
            cbo_mon_hoc.Items.Add("L?p trình");
            cbo_mon_hoc.Items.Add("Toán");
            cbo_mon_hoc.Items.Add("Anh v?n");
            cbo_mon_hoc.Items.Add("V?t lý");
            cbo_mon_hoc.Items.Add("Hóa h?c");
            cbo_mon_hoc.Items.Add("V?n h?c");
            cbo_mon_hoc.Items.Add("L?ch s?");
            cbo_mon_hoc.Items.Add("??a lý");

            // Lo?i tài li?u
            cbo_loai.Items.Clear();
            cbo_loai.Items.Add("slide");
            cbo_loai.Items.Add("bài t?p");
            cbo_loai.Items.Add("?? thi");
            cbo_loai.Items.Add("tài li?u khác");
        }

        /// <summary>
        /// Load d? li?u tài li?u c?n s?a
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i khi load d? li?u: " + ex.Message, 
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Button ch?n file
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
            open_file_dialog.Title = "Ch?n tài li?u";

            if (open_file_dialog.ShowDialog() == DialogResult.OK)
            {
                string duong_dan = open_file_dialog.FileName;
                txt_duong_dan.Text = duong_dan;

                // T? ??ng ?i?n tên file n?u ch?a có tên
                if (string.IsNullOrWhiteSpace(txt_ten.Text))
                {
                    txt_ten.Text = Path.GetFileNameWithoutExtension(duong_dan);
                }

                // Tính kích th??c file
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
        /// Button l?u
        /// </summary>
        private void btn_luu_Click(object sender, EventArgs e)
        {
            // Validate d? li?u
            if (string.IsNullOrWhiteSpace(txt_ten.Text))
            {
                MessageBox.Show("Vui lòng nh?p tên tài li?u!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_ten.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_duong_dan.Text))
            {
                MessageBox.Show("Vui lòng ch?n file tài li?u!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btn_chon_file.Focus();
                return;
            }

            // Ki?m tra file có t?n t?i không
            if (!File.Exists(txt_duong_dan.Text))
            {
                MessageBox.Show("File không t?n t?i! Vui lòng ch?n file khác.", 
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (document_id.HasValue)
                {
                    // S?a tài li?u
                    success = DatabaseHelper.UpdateDocument(
                        document_id.Value,
                        txt_ten.Text.Trim(),
                        cbo_mon_hoc.Text.Trim(),
                        cbo_loai.Text.Trim(),
                        txt_duong_dan.Text.Trim(),
                        txt_ghi_chu.Text.Trim(),
                        kich_thuoc,
                        txt_tac_gia.Text.Trim(),
                        chk_quan_trong.Checked
                    );

                    if (success)
                    {
                        MessageBox.Show("C?p nh?t tài li?u thành công!", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    // Thêm tài li?u m?i
                    success = DatabaseHelper.InsertDocument(
                        txt_ten.Text.Trim(),
                        cbo_mon_hoc.Text.Trim(),
                        cbo_loai.Text.Trim(),
                        txt_duong_dan.Text.Trim(),
                        txt_ghi_chu.Text.Trim(),
                        kich_thuoc,
                        txt_tac_gia.Text.Trim(),
                        chk_quan_trong.Checked
                    );

                    if (success)
                    {
                        MessageBox.Show("Thêm tài li?u thành công!", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i khi l?u: " + ex.Message, 
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Button h?y
        /// </summary>
        private void btn_huy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Nh?n Enter ? TextBox tên -> chuy?n focus
        /// </summary>
        private void txt_ten_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                cbo_mon_hoc.Focus();
            }
        }
    }
}
