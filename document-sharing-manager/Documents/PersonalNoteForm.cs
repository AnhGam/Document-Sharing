using document_sharing_manager.Core.Data;
using document_sharing_manager.Core;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.UI.Controls;

namespace document_sharing_manager.Documents
{
    public partial class PersonalNoteForm : Form
    {
        private int documentId;
        private string documentName;
        private int? noteId = null;

        public PersonalNoteForm(int docId, string docName)
        {
            InitializeComponent();
            documentId = docId;
            documentName = docName;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;

            // Title styling
            lblTitle.ForeColor = AppTheme.Primary;
            lblTitle.Font = new Font(AppTheme.FontFamily, 14F, FontStyle.Bold);

            // Document name styling
            lblDocumentName.ForeColor = AppTheme.TextSecondary;
            lblDocumentName.Font = new Font(AppTheme.FontFamily, 10F, FontStyle.Italic);

            // Labels
            lblStatus.ForeColor = AppTheme.TextPrimary;
            lblStatus.Font = AppTheme.FontSmallBold;
            lblNote.ForeColor = AppTheme.TextPrimary;
            lblNote.Font = AppTheme.FontSmallBold;

            // ComboBox styling
            AppTheme.ApplyComboBoxStyle(cboStatus);

            // TextBox styling - larger for note
            txtNote.BackColor = Color.White;
            txtNote.ForeColor = AppTheme.TextPrimary;
            txtNote.Font = AppTheme.FontBody;
            txtNote.BorderStyle = BorderStyle.FixedSingle;

            // Buttons - use ModernButton variants
            AppTheme.ApplyButtonSuccess(btnSave);
            AppTheme.ApplyButtonDanger(btnDelete);
            AppTheme.ApplyButtonSecondary(btnCancel);
        }

        private void PersonalNoteForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Ghi chú - {documentName}";
            lblDocumentName.Text = documentName;

            // Load status options
            cboStatus.Items.Add("Chưa đọc");
            cboStatus.Items.Add("Đang học");
            cboStatus.Items.Add("Đã ôn xong");
            cboStatus.SelectedIndex = 0;

            // Load existing note
            LoadNote();
        }

        private void LoadNote()
        {
            try
            {
                // Chế độ cá nhân: không cần user_id, mỗi document chỉ có 1 note
                string query = @"SELECT id, note_content, status FROM personal_notes
                                WHERE document_id = @docId";

                System.Data.SQLite.SQLiteParameter[] parameters = new System.Data.SQLite.SQLiteParameter[]
                {
                    new System.Data.SQLite.SQLiteParameter("@docId", documentId)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    noteId = Convert.ToInt32(row["id"]);
                    txtNote.Text = row["note_content"]?.ToString() ?? "";

                    string status = row["status"]?.ToString() ?? "Chưa đọc";
                    int statusIndex = cboStatus.Items.IndexOf(status);
                    if (statusIndex >= 0)
                        cboStatus.SelectedIndex = statusIndex;
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi khi load ghi chú: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string noteContent = txtNote.Text.Trim();
                string status = cboStatus.SelectedItem?.ToString() ?? "Chưa đọc";

                if (noteId.HasValue)
                {
                    // Update existing note
                    string query = @"UPDATE personal_notes
                                    SET note_content = @content, status = @status, updated_at = datetime('now', 'localtime')
                                    WHERE id = @id";

                    System.Data.SQLite.SQLiteParameter[] parameters = new System.Data.SQLite.SQLiteParameter[]
                    {
                        new System.Data.SQLite.SQLiteParameter("@content", string.IsNullOrEmpty(noteContent) ? DBNull.Value : (object)noteContent),
                        new System.Data.SQLite.SQLiteParameter("@status", status),
                        new System.Data.SQLite.SQLiteParameter("@id", noteId.Value)
                    };

                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                }
                else
                {
                    // Insert new note - không cần user_id nữa
                    string query = @"INSERT INTO personal_notes (document_id, note_content, status)
                                    VALUES (@docId, @content, @status)";

                    System.Data.SQLite.SQLiteParameter[] parameters = new System.Data.SQLite.SQLiteParameter[]
                    {
                        new System.Data.SQLite.SQLiteParameter("@docId", documentId),
                        new System.Data.SQLite.SQLiteParameter("@content", string.IsNullOrEmpty(noteContent) ? DBNull.Value : (object)noteContent),
                        new System.Data.SQLite.SQLiteParameter("@status", status)
                    };

                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                }

                ToastNotification.Success("Đã lưu ghi chú!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi khi lưu ghi chú: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!noteId.HasValue)
            {
                ToastNotification.Info("Chưa có ghi chú để xóa!");
                return;
            }

            if (MessageBox.Show("Xóa ghi chú này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM personal_notes WHERE id = @id";
                    System.Data.SQLite.SQLiteParameter[] parameters = new System.Data.SQLite.SQLiteParameter[]
                    {
                        new System.Data.SQLite.SQLiteParameter("@id", noteId.Value)
                    };

                    DatabaseHelper.ExecuteNonQuery(query, parameters);

                    ToastNotification.Success("Đã xóa ghi chú!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    ToastNotification.Error("Lỗi khi xóa ghi chú: " + ex.Message);
                }
            }
        }
    }
}


