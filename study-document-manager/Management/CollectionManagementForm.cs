using study_document_manager.Core.Data;
using study_document_manager.Core;
using study_document_manager.Core.Entities;
using study_document_manager.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace study_document_manager.Management
{
    public partial class CollectionManagementForm : Form
    {
        public CollectionManagementForm()
        {
            InitializeComponent();
        }

        private void CollectionManagementFormLoad(object sender, EventArgs e)
        {
            LoadCollections();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            pnlCollectionHeader.BackColor = AppTheme.Primary;
            pnlDocHeader.BackColor = AppTheme.Primary;
            
            AppTheme.ApplyButtonSuccess(btnNewCollection);
            AppTheme.ApplyButtonDanger(btnDeleteCollection);
            AppTheme.ApplyButtonPrimary(btnOpenAll);
            AppTheme.ApplyButtonWarning(btnRemoveFromCollection);
            AppTheme.ApplyButtonDanger(btnClose);
            
            AppTheme.ApplyDataGridViewStyle(dgvDocuments);
            
            lblStatus.ForeColor = AppTheme.TextSecondary;
            lblDocCount.ForeColor = AppTheme.Primary;
        }

        private void LoadCollections()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetCollections();
                lstCollections.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    var item = new ListViewItem(row["name"].ToString());
                    item.SubItems.Add(row["item_count"].ToString());
                    item.Tag = Convert.ToInt32(row["id"]);
                    lstCollections.Items.Add(item);
                }
                
                dgvDocuments.DataSource = null;
                btnDeleteCollection.Enabled = false;
                btnRemoveFromCollection.Enabled = false;
                btnOpenAll.Enabled = false;
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi tải bộ sưu tập: " + ex.Message);
            }
        }

        private void LstCollectionsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCollections.SelectedItems.Count > 0)
            {
                int collectionId = (int)lstCollections.SelectedItems[0].Tag;
                LoadDocumentsInCollection(collectionId);
                btnDeleteCollection.Enabled = true;
            }
            else
            {
                dgvDocuments.DataSource = null;
                btnDeleteCollection.Enabled = false;
                btnRemoveFromCollection.Enabled = false;
                btnOpenAll.Enabled = false;
            }
        }

        private void LoadDocumentsInCollection(int collectionId)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetDocumentsInCollection(collectionId);
                dgvDocuments.DataSource = dt;
                
                // Format grid
                if (dgvDocuments.Columns.Contains("id")) dgvDocuments.Columns["id"].Visible = false;
                if (dgvDocuments.Columns.Contains("ten")) dgvDocuments.Columns["ten"].HeaderText = "Tên tài liệu";
                if (dgvDocuments.Columns.Contains("dinh_dang")) dgvDocuments.Columns["dinh_dang"].HeaderText = "Định dạng";
                if (dgvDocuments.Columns.Contains("added_at")) dgvDocuments.Columns["added_at"].HeaderText = "Ngày thêm vào bộ";

                lblDocCount.Text = $"{dt.Rows.Count} tài liệu trong bộ sưu tập này";
                btnOpenAll.Enabled = dt.Rows.Count > 0;
                btnRemoveFromCollection.Enabled = dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi tải tài liệu: " + ex.Message);
            }
        }

        private void BtnNewCollectionClick(object sender, EventArgs e)
        {
            string name = "Bộ sưu tập mới"; // Placeholder until custom dialog is implemented
            // In a real app, you'd use a small form for this.
            if (!string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    DatabaseHelper.CreateCollection(name.Trim(), "");
                    LoadCollections();
                    ToastNotification.Success("Đã tạo bộ sưu tập mới.");
                }
                catch (Exception ex)
                {
                    ToastNotification.Error("Lỗi: " + ex.Message);
                }
            }
        }

        private void BtnDeleteCollectionClick(object sender, EventArgs e)
        {
            if (lstCollections.SelectedItems.Count > 0)
            {
                string name = lstCollections.SelectedItems[0].Text;
                if (MessageBox.Show($"Xóa bộ sưu tập '{name}'?\n\n(Tài liệu sẽ KHÔNG bị xóa, chỉ xóa bộ sưu tập)", 
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        int id = (int)lstCollections.SelectedItems[0].Tag;
                        DatabaseHelper.DeleteCollection(id);
                        LoadCollections();
                        ToastNotification.Success("Đã xóa bộ sưu tập.");
                    }
                    catch (Exception ex)
                    {
                        ToastNotification.Error("Lỗi: " + ex.Message);
                    }
                }
            }
        }

        private void BtnRemoveFromCollectionClick(object sender, EventArgs e)
        {
            if (lstCollections.SelectedItems.Count > 0 && dgvDocuments.SelectedRows.Count > 0)
            {
                string docName = dgvDocuments.SelectedRows[0].Cells["ten"].Value.ToString();
                if (MessageBox.Show($"Xóa '{docName}' khỏi bộ sưu tập?", 
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        int colId = (int)lstCollections.SelectedItems[0].Tag;
                        int docId = Convert.ToInt32(dgvDocuments.SelectedRows[0].Cells["id"].Value);
                        DatabaseHelper.RemoveDocumentFromCollection(colId, docId);
                        LoadDocumentsInCollection(colId);
                        
                        // Update count in list view
                        DataTable dt = DatabaseHelper.GetCollections();
                        foreach(DataRow row in dt.Rows)
                        {
                            if (Convert.ToInt32(row["id"]) == colId)
                            {
                                lstCollections.SelectedItems[0].SubItems[1].Text = row["item_count"].ToString();
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ToastNotification.Error("Lỗi: " + ex.Message);
                    }
                }
            }
        }

        private void BtnOpenAllClick(object sender, EventArgs e)
        {
            if (dgvDocuments.Rows.Count > 0)
            {
                int opened = 0;
                foreach (DataGridViewRow row in dgvDocuments.Rows)
                {
                    string path = row.Cells["duong_dan"].Value?.ToString();
                    if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                    {
                        try { System.Diagnostics.Process.Start(path); opened++; } catch { }
                    }
                }
                ToastNotification.Info($"Đang mở {opened} tài liệu...");
            }
        }

        private void DgvDocumentsCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string path = dgvDocuments.Rows[e.RowIndex].Cells["duong_dan"].Value?.ToString();
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    try { System.Diagnostics.Process.Start(path); }
                    catch (Exception ex) { ToastNotification.Error(ex.Message); }
                }
            }
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}


