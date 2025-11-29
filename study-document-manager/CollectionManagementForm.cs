using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace study_document_manager
{
    public partial class CollectionManagementForm : Form
    {
        private int? selectedCollectionId = null;

        public CollectionManagementForm()
        {
            InitializeComponent();
        }

        private void CollectionManagementForm_Load(object sender, EventArgs e)
        {
            LoadCollections();
            SetupDocumentsGrid();
        }

        private void LoadCollections()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetCollections();
                lstCollections.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    string name = row["name"].ToString();
                    int itemCount = Convert.ToInt32(row["item_count"]);

                    ListViewItem item = new ListViewItem(name);
                    item.SubItems.Add($"{itemCount} tài liệu");
                    item.Tag = id;
                    lstCollections.Items.Add(item);
                }

                lblStatus.Text = $"Có {dt.Rows.Count} bộ sưu tập";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load bộ sưu tập: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDocumentsGrid()
        {
            dgvDocuments.AutoGenerateColumns = false;
            dgvDocuments.Columns.Clear();

            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                Visible = false
            });

            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ten",
                DataPropertyName = "ten",
                HeaderText = "Tên tài liệu",
                Width = 250
            });

            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "mon_hoc",
                DataPropertyName = "mon_hoc",
                HeaderText = "Môn học",
                Width = 100
            });

            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "loai",
                DataPropertyName = "loai",
                HeaderText = "Loại",
                Width = 80
            });

            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "duong_dan",
                DataPropertyName = "duong_dan",
                Visible = false
            });
        }

        private void LoadDocumentsInCollection(int collectionId)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetDocumentsInCollection(collectionId);
                dgvDocuments.DataSource = dt;
                lblDocCount.Text = $"Có {dt.Rows.Count} tài liệu trong bộ sưu tập";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstCollections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCollections.SelectedItems.Count > 0)
            {
                selectedCollectionId = (int)lstCollections.SelectedItems[0].Tag;
                LoadDocumentsInCollection(selectedCollectionId.Value);
                btnDeleteCollection.Enabled = true;
                btnOpenAll.Enabled = true;
            }
            else
            {
                selectedCollectionId = null;
                dgvDocuments.DataSource = null;
                btnDeleteCollection.Enabled = false;
                btnOpenAll.Enabled = false;
                lblDocCount.Text = "";
            }
        }

        private void btnNewCollection_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập tên bộ sưu tập:", "Tạo bộ sưu tập mới", "");

            if (!string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    int newId = DatabaseHelper.CreateCollection(name.Trim());
                    if (newId > 0)
                    {
                        LoadCollections();
                        lblStatus.Text = "Đã tạo bộ sưu tập: " + name;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDeleteCollection_Click(object sender, EventArgs e)
        {
            if (!selectedCollectionId.HasValue) return;

            string collectionName = lstCollections.SelectedItems[0].Text;

            if (MessageBox.Show($"Xóa bộ sưu tập '{collectionName}'?\n\n(Tài liệu sẽ KHÔNG bị xóa, chỉ xóa bộ sưu tập)",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (DatabaseHelper.DeleteCollection(selectedCollectionId.Value))
                    {
                        LoadCollections();
                        dgvDocuments.DataSource = null;
                        selectedCollectionId = null;
                        lblStatus.Text = "Đã xóa bộ sưu tập";
                        lblDocCount.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRemoveFromCollection_Click(object sender, EventArgs e)
        {
            if (!selectedCollectionId.HasValue || dgvDocuments.SelectedRows.Count == 0) return;

            int docId = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
            string docName = dgvDocuments.SelectedRows[0].Cells["ten"].Value.ToString();

            if (MessageBox.Show($"Xóa '{docName}' khỏi bộ sưu tập?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (DatabaseHelper.RemoveDocumentFromCollection(selectedCollectionId.Value, docId))
                    {
                        LoadDocumentsInCollection(selectedCollectionId.Value);
                        LoadCollections(); // Refresh count
                        lblStatus.Text = "Đã xóa tài liệu khỏi bộ sưu tập";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnOpenAll_Click(object sender, EventArgs e)
        {
            if (!selectedCollectionId.HasValue) return;

            try
            {
                DataTable dt = DatabaseHelper.GetDocumentsInCollection(selectedCollectionId.Value);
                int openedCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string path = row["duong_dan"]?.ToString();
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        openedCount++;
                    }
                }

                lblStatus.Text = $"Đã mở {openedCount}/{dt.Rows.Count} tài liệu";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDocuments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string path = dgvDocuments.Rows[e.RowIndex].Cells["duong_dan"].Value?.ToString();
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("File không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
