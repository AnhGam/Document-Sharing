using study_document_manager.Core.Data;
using study_document_manager.Core;
using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager.Documents
{
    public class RelatedDocumentsForm : Form
    {
        private readonly int currentDocId;
        private readonly string currentDocName;
        private DataGridView dgvRelated;
        private ComboBox cmbDocuments;
        private ComboBox cmbRelationType;
        private Button btnAdd;
        private Button btnRemove;
        private Button btnClose;
        private Panel pnlHeader;
        private Panel pnlAddRelation;
        private Panel pnlActions;
        private FlowLayoutPanel pnlSuggestions;
        private Label lblSuggestTitle;
        private readonly ToolTip toolTip;

        public RelatedDocumentsForm(int docId, string docName)
        {
            currentDocId = docId;
            currentDocName = docName;
            toolTip = new ToolTip();
            InitializeUI();
            ApplyTheme();
            LoadRelatedDocuments();
            LoadAllDocuments();
            LoadSuggestions();
        }

        private void InitializeUI()
        {
            this.Text = "Tài liệu liên quan - " + currentDocName;
            this.Size = new Size(750, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(16, 8, 16, 8) };
            var lblTitle = new Label
            {
                Text = "Tài liệu liên quan với: " + currentDocName,
                Font = new Font(AppTheme.FontFamily, 12f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 12)
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // Add relation panel
            pnlAddRelation = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(16, 8, 16, 8) };

            var lblDoc = new Label { Text = "Chọn tài liệu:", AutoSize = true, Location = new Point(16, 16), Font = new Font(AppTheme.FontFamily, 9f) };
            cmbDocuments = new ComboBox
            {
                Location = new Point(120, 12),
                Size = new Size(280, 25),
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };

            cmbRelationType = new ComboBox
            {
                Location = new Point(410, 12),
                Size = new Size(130, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRelationType.Items.AddRange(new object[] { "Liên quan", "Tài liệu đính kèm", "Ghi chú", "Tham khảo", "Phụ lục" });
            cmbRelationType.SelectedIndex = 0;

            btnAdd = new Button { Text = "Thêm", Size = new Size(80, 28), Location = new Point(550, 11) };
            btnAdd.Click += BtnAddClick;

            pnlAddRelation.Controls.AddRange(new Control[] { lblDoc, cmbDocuments, cmbRelationType, btnAdd });
            this.Controls.Add(pnlAddRelation);

            // DataGridView
            dgvRelated = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                ReadOnly = true
            };

            dgvRelated.Columns.Add(new DataGridViewTextBoxColumn { Name = "DocName", HeaderText = "Tên tài liệu", Width = 250 });
            dgvRelated.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subject", HeaderText = "Danh mục", Width = 120 });
            dgvRelated.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Định dạng", Width = 80 });
            dgvRelated.Columns.Add(new DataGridViewTextBoxColumn { Name = "RelationType", HeaderText = "Quan hệ", Width = 100 });
            dgvRelated.Columns.Add(new DataGridViewTextBoxColumn { Name = "RelationId", Visible = false });
            dgvRelated.Columns.Add(new DataGridViewTextBoxColumn { Name = "DocId", Visible = false });

            this.Controls.Add(dgvRelated);

            // Bottom actions
            // Suggestions Panel
            pnlSuggestions = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                Padding = new Padding(16, 4, 16, 4),
                AutoScroll = true
            };
            lblSuggestTitle = new Label
            {
                Text = "Gợi ý tài liệu cùng danh mục/tag:",
                Dock = DockStyle.Bottom,
                Height = 25,
                Font = new Font(AppTheme.FontFamily, 9f, FontStyle.Italic),
                ForeColor = AppTheme.TextSecondary,
                Padding = new Padding(16, 0, 0, 0)
            };
            this.Controls.Add(lblSuggestTitle);
            this.Controls.Add(pnlSuggestions);

            pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12, 8, 12, 8) };

            btnRemove = new Button { Text = "Xóa liên kết", Size = new Size(120, 35), Location = new Point(12, 8) };
            btnRemove.Click += BtnRemoveClick;

            btnClose = new Button { Text = "Đóng", Size = new Size(90, 35), Location = new Point(630, 8) };
            btnClose.Click += (s, e) => this.Close();

            pnlActions.Controls.AddRange(new Control[] { btnRemove, btnClose });
            this.Controls.Add(pnlActions);

            dgvRelated.CellDoubleClick += DgvRelatedCellDoubleClick;
            dgvRelated.BringToFront();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            pnlHeader.BackColor = AppTheme.BackgroundCard;
            pnlAddRelation.BackColor = AppTheme.BackgroundSoft;
            pnlActions.BackColor = AppTheme.BackgroundSoft;

            AppTheme.ApplyButtonPrimary(btnAdd);
            AppTheme.ApplyButtonDanger(btnRemove);
            AppTheme.ApplyButtonSecondary(btnClose);
            AppTheme.ApplyDataGridViewStyle(dgvRelated);
        }

        private void LoadRelatedDocuments()
        {
            dgvRelated.Rows.Clear();
            try
            {
                var dt = DatabaseHelper.GetRelatedDocuments(currentDocId);
                foreach (DataRow row in dt.Rows)
                {
                    dgvRelated.Rows.Add(
                        row["ten"]?.ToString(),
                        row["danh_muc"]?.ToString(),
                        row["dinh_dang"]?.ToString(),
                        row["relation_type"]?.ToString(),
                        row["relation_id"]?.ToString(),
                        row["id"]?.ToString()
                    );
                }
            }
            catch { }
        }

        private void LoadAllDocuments()
        {
            cmbDocuments.Items.Clear();
            try
            {
                var dt = DatabaseHelper.GetAllDocuments();
                foreach (DataRow row in dt.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    if (id == currentDocId) continue;
                    cmbDocuments.Items.Add(new DocItem { Id = id, Name = row["ten"]?.ToString() });
                }
                cmbDocuments.DisplayMember = "Name";
            }
            catch { }
        }

        private void BtnAddClick(object sender, EventArgs e)
        {
            if (!(cmbDocuments.SelectedItem is DocItem selected))
            {
                MessageBox.Show("Vui lòng chọn tài liệu từ danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string relType = cmbRelationType.SelectedItem?.ToString() ?? "Liên quan";
            try
            {
                DatabaseHelper.AddDocumentRelation(currentDocId, selected.Id, relType);
                LoadRelatedDocuments();
                ToastNotification.Success("Đã thêm liên kết");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveClick(object sender, EventArgs e)
        {
            if (dgvRelated.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn liên kết cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(dgvRelated.SelectedRows[0].Cells["RelationId"].Value?.ToString(), out int relationId))
                return;

            var confirm = MessageBox.Show("Xóa liên kết này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            DatabaseHelper.RemoveDocumentRelation(relationId);
            LoadRelatedDocuments();
            LoadSuggestions(); // Reload suggestions as some might have been unlinked
        }

        private void LoadSuggestions()
        {
            pnlSuggestions.Controls.Clear();
            try
            {
                var dt = DatabaseHelper.GetSuggestedRelatedDocuments(currentDocId);
                if (dt.Rows.Count == 0)
                {
                    lblSuggestTitle.Visible = false;
                    pnlSuggestions.Visible = false;
                    return;
                }

                lblSuggestTitle.Visible = true;
                pnlSuggestions.Visible = true;

                foreach (DataRow row in dt.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    string name = row["ten"]?.ToString();

                    Button btnSuggest = new Button
                    {
                        Text = "+ " + (name.Length > 20 ? name.Substring(0, 17) + "..." : name),
                        Height = 30,
                        AutoSize = true,
                        Margin = new Padding(0, 0, 8, 0),
                        Tag = id,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    toolTip.SetToolTip(btnSuggest, name);
                    btnSuggest.Click += (s, e) =>
                    {
                        DatabaseHelper.AddDocumentRelation(currentDocId, (int)btnSuggest.Tag, "Liên quan");
                        LoadRelatedDocuments();
                        LoadSuggestions();
                        ToastNotification.Success("Đã liên kết!");
                    };
                    pnlSuggestions.Controls.Add(btnSuggest);
                }
            }
            catch { }
        }

        private void DgvRelatedCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string docIdStr = dgvRelated.Rows[e.RowIndex].Cells["DocId"].Value?.ToString();
            if (string.IsNullOrEmpty(docIdStr)) return;

            int docId = int.Parse(docIdStr);
            OpenDocument(docId);
        }

        private void OpenDocument(int docId)
        {
            try
            {
                // Lấy đường dẫn file từ DB
                string query = "SELECT duong_dan FROM tai_lieu WHERE id = @id";
                var dt = DatabaseHelper.ExecuteQuery(query, new System.Data.SQLite.SQLiteParameter[] { new System.Data.SQLite.SQLiteParameter("@id", docId) });

                if (dt.Rows.Count > 0)
                {
                    string path = dt.Rows[0]["duong_dan"]?.ToString();
                    if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy file tại đường dẫn: " + path, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở tài liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class DocItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() { return Name; }
        }
    }
}

