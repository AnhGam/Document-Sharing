namespace study_document_manager.Management
{
    public partial class CollectionManagementForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionManagementForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.lstCollections = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlCollectionHeader = new System.Windows.Forms.Panel();
            this.lblCollectionTitle = new System.Windows.Forms.Label();
            this.pnlCollectionButtons = new System.Windows.Forms.Panel();
            this.btnNewCollection = new System.Windows.Forms.Button();
            this.btnDeleteCollection = new System.Windows.Forms.Button();
            this.dgvDocuments = new System.Windows.Forms.DataGridView();
            this.pnlDocHeader = new System.Windows.Forms.Panel();
            this.lblDocTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlDocButtons = new System.Windows.Forms.Panel();
            this.btnOpenAll = new System.Windows.Forms.Button();
            this.btnRemoveFromCollection = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDocCount = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlCollectionHeader.SuspendLayout();
            this.pnlCollectionButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).BeginInit();
            this.pnlDocHeader.SuspendLayout();
            this.pnlDocButtons.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer
            //
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            //
            // splitContainer.Panel1 - Left panel (Collections list)
            //
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel1.Controls.Add(this.lstCollections);
            this.splitContainer.Panel1.Controls.Add(this.pnlCollectionButtons);
            this.splitContainer.Panel1.Controls.Add(this.pnlCollectionHeader);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(12);
            this.splitContainer.Panel1MinSize = 200;
            //
            // splitContainer.Panel2 - Right panel (Documents grid)
            //
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.splitContainer.Panel2.Controls.Add(this.dgvDocuments);
            this.splitContainer.Panel2.Controls.Add(this.pnlDocButtons);
            this.splitContainer.Panel2.Controls.Add(this.pnlDocHeader);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(12);
            this.splitContainer.Panel2MinSize = 350;
            this.splitContainer.Size = new System.Drawing.Size(900, 500);
            this.splitContainer.SplitterDistance = 280;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 0;
            //
            // pnlCollectionHeader
            //
            this.pnlCollectionHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.pnlCollectionHeader.Controls.Add(this.lblCollectionTitle);
            this.pnlCollectionHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCollectionHeader.Location = new System.Drawing.Point(12, 12);
            this.pnlCollectionHeader.Name = "pnlCollectionHeader";
            this.pnlCollectionHeader.Size = new System.Drawing.Size(256, 40);
            this.pnlCollectionHeader.TabIndex = 0;
            //
            // lblCollectionTitle
            //
            this.lblCollectionTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCollectionTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblCollectionTitle.ForeColor = System.Drawing.Color.White;
            this.lblCollectionTitle.Location = new System.Drawing.Point(0, 0);
            this.lblCollectionTitle.Name = "lblCollectionTitle";
            this.lblCollectionTitle.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblCollectionTitle.Size = new System.Drawing.Size(256, 40);
            this.lblCollectionTitle.TabIndex = 0;
            this.lblCollectionTitle.Text = "Bộ sưu tập";
            this.lblCollectionTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lstCollections
            //
            this.lstCollections.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colCount});
            this.lstCollections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCollections.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstCollections.FullRowSelect = true;
            this.lstCollections.HideSelection = false;
            this.lstCollections.Location = new System.Drawing.Point(12, 52);
            this.lstCollections.MultiSelect = false;
            this.lstCollections.Name = "lstCollections";
            this.lstCollections.Size = new System.Drawing.Size(256, 384);
            this.lstCollections.TabIndex = 1;
            this.lstCollections.UseCompatibleStateImageBehavior = false;
            this.lstCollections.View = System.Windows.Forms.View.Details;
            this.lstCollections.SelectedIndexChanged += new System.EventHandler(this.LstCollectionsSelectedIndexChanged);
            //
            // colName
            //
            this.colName.Text = "Tên";
            this.colName.Width = 150;
            //
            // colCount
            //
            this.colCount.Text = "Số lượng";
            this.colCount.Width = 80;
            //
            // pnlCollectionButtons
            //
            this.pnlCollectionButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlCollectionButtons.Controls.Add(this.btnNewCollection);
            this.pnlCollectionButtons.Controls.Add(this.btnDeleteCollection);
            this.pnlCollectionButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCollectionButtons.Location = new System.Drawing.Point(12, 436);
            this.pnlCollectionButtons.Name = "pnlCollectionButtons";
            this.pnlCollectionButtons.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.pnlCollectionButtons.Size = new System.Drawing.Size(256, 52);
            this.pnlCollectionButtons.TabIndex = 2;
            //
            // btnNewCollection
            //
            this.btnNewCollection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnNewCollection.FlatAppearance.BorderSize = 0;
            this.btnNewCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewCollection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNewCollection.ForeColor = System.Drawing.Color.White;
            this.btnNewCollection.Location = new System.Drawing.Point(0, 10);
            this.btnNewCollection.Name = "btnNewCollection";
            this.btnNewCollection.Size = new System.Drawing.Size(110, 38);
            this.btnNewCollection.TabIndex = 0;
            this.btnNewCollection.Text = "+ Tạo mới";
            this.btnNewCollection.UseVisualStyleBackColor = false;
            this.btnNewCollection.Click += new System.EventHandler(this.BtnNewCollectionClick);
            //
            // btnDeleteCollection
            //
            this.btnDeleteCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteCollection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDeleteCollection.Enabled = false;
            this.btnDeleteCollection.FlatAppearance.BorderSize = 0;
            this.btnDeleteCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteCollection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeleteCollection.ForeColor = System.Drawing.Color.White;
            this.btnDeleteCollection.Location = new System.Drawing.Point(146, 10);
            this.btnDeleteCollection.Name = "btnDeleteCollection";
            this.btnDeleteCollection.Size = new System.Drawing.Size(110, 38);
            this.btnDeleteCollection.TabIndex = 1;
            this.btnDeleteCollection.Text = "Xóa";
            this.btnDeleteCollection.UseVisualStyleBackColor = false;
            this.btnDeleteCollection.Click += new System.EventHandler(this.BtnDeleteCollectionClick);
            //
            // pnlDocHeader
            //
            this.pnlDocHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.pnlDocHeader.Controls.Add(this.lblDocTitle);
            this.pnlDocHeader.Controls.Add(this.btnClose);
            this.pnlDocHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDocHeader.Location = new System.Drawing.Point(12, 12);
            this.pnlDocHeader.Name = "pnlDocHeader";
            this.pnlDocHeader.Size = new System.Drawing.Size(590, 40);
            this.pnlDocHeader.TabIndex = 0;
            //
            // lblDocTitle
            //
            this.lblDocTitle.AutoSize = true;
            this.lblDocTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDocTitle.ForeColor = System.Drawing.Color.White;
            this.lblDocTitle.Location = new System.Drawing.Point(8, 10);
            this.lblDocTitle.Name = "lblDocTitle";
            this.lblDocTitle.Size = new System.Drawing.Size(186, 20);
            this.lblDocTitle.TabIndex = 0;
            this.lblDocTitle.Text = "Tài liệu trong bộ sưu tập";
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(500, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
            //
            // dgvDocuments
            //
            this.dgvDocuments.AllowUserToAddRows = false;
            this.dgvDocuments.AllowUserToDeleteRows = false;
            this.dgvDocuments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDocuments.BackgroundColor = System.Drawing.Color.White;
            this.dgvDocuments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDocuments.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvDocuments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDocuments.Location = new System.Drawing.Point(12, 52);
            this.dgvDocuments.MultiSelect = false;
            this.dgvDocuments.Name = "dgvDocuments";
            this.dgvDocuments.ReadOnly = true;
            this.dgvDocuments.RowHeadersVisible = false;
            this.dgvDocuments.RowHeadersWidth = 30;
            this.dgvDocuments.RowTemplate.Height = 36;
            this.dgvDocuments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDocuments.Size = new System.Drawing.Size(590, 384);
            this.dgvDocuments.TabIndex = 1;
            this.dgvDocuments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvDocumentsCellDoubleClick);
            //
            // pnlDocButtons
            //
            this.pnlDocButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlDocButtons.Controls.Add(this.btnOpenAll);
            this.pnlDocButtons.Controls.Add(this.btnRemoveFromCollection);
            this.pnlDocButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDocButtons.Location = new System.Drawing.Point(12, 436);
            this.pnlDocButtons.Name = "pnlDocButtons";
            this.pnlDocButtons.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.pnlDocButtons.Size = new System.Drawing.Size(590, 52);
            this.pnlDocButtons.TabIndex = 2;
            //
            // btnOpenAll
            //
            this.btnOpenAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnOpenAll.Enabled = false;
            this.btnOpenAll.FlatAppearance.BorderSize = 0;
            this.btnOpenAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOpenAll.ForeColor = System.Drawing.Color.White;
            this.btnOpenAll.Location = new System.Drawing.Point(0, 10);
            this.btnOpenAll.Name = "btnOpenAll";
            this.btnOpenAll.Size = new System.Drawing.Size(120, 38);
            this.btnOpenAll.TabIndex = 0;
            this.btnOpenAll.Text = "Mở tất cả";
            this.btnOpenAll.UseVisualStyleBackColor = false;
            this.btnOpenAll.Click += new System.EventHandler(this.BtnOpenAllClick);
            //
            // btnRemoveFromCollection
            //
            this.btnRemoveFromCollection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.btnRemoveFromCollection.FlatAppearance.BorderSize = 0;
            this.btnRemoveFromCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveFromCollection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRemoveFromCollection.ForeColor = System.Drawing.Color.White;
            this.btnRemoveFromCollection.Location = new System.Drawing.Point(130, 10);
            this.btnRemoveFromCollection.Name = "btnRemoveFromCollection";
            this.btnRemoveFromCollection.Size = new System.Drawing.Size(160, 38);
            this.btnRemoveFromCollection.TabIndex = 1;
            this.btnRemoveFromCollection.Text = "Xóa khỏi bộ sưu tập";
            this.btnRemoveFromCollection.UseVisualStyleBackColor = false;
            this.btnRemoveFromCollection.Click += new System.EventHandler(this.BtnRemoveFromCollectionClick);
            //
            // statusStrip
            //
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblDocCount});
            this.statusStrip.Location = new System.Drawing.Point(0, 500);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(900, 30);
            this.statusStrip.TabIndex = 1;
            //
            // lblStatus
            //
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(68, 24);
            this.lblStatus.Text = "Sẵn sàng";
            //
            // lblDocCount
            //
            this.lblDocCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.lblDocCount.Name = "lblDocCount";
            this.lblDocCount.Size = new System.Drawing.Size(817, 24);
            this.lblDocCount.Spring = true;
            this.lblDocCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // CollectionManagementForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(900, 530);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Name = "CollectionManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý Bộ sưu tập";
            this.Load += new System.EventHandler(this.CollectionManagementFormLoad);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlCollectionHeader.ResumeLayout(false);
            this.pnlCollectionButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).EndInit();
            this.pnlDocHeader.ResumeLayout(false);
            this.pnlDocHeader.PerformLayout();
            this.pnlDocButtons.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlCollectionHeader;
        private System.Windows.Forms.Label lblCollectionTitle;
        private System.Windows.Forms.ListView lstCollections;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colCount;
        private System.Windows.Forms.Panel pnlCollectionButtons;
        private System.Windows.Forms.Button btnNewCollection;
        private System.Windows.Forms.Button btnDeleteCollection;
        private System.Windows.Forms.Panel pnlDocHeader;
        private System.Windows.Forms.Label lblDocTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvDocuments;
        private System.Windows.Forms.Panel pnlDocButtons;
        private System.Windows.Forms.Button btnOpenAll;
        private System.Windows.Forms.Button btnRemoveFromCollection;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblDocCount;
    }
}


