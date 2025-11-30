namespace study_document_manager
{
    partial class CollectionManagementForm
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
            this.pnlCollectionButtons = new System.Windows.Forms.Panel();
            this.btnDeleteCollection = new System.Windows.Forms.Button();
            this.btnNewCollection = new System.Windows.Forms.Button();
            this.lblCollectionTitle = new System.Windows.Forms.Label();
            this.dgvDocuments = new System.Windows.Forms.DataGridView();
            this.pnlDocButtons = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpenAll = new System.Windows.Forms.Button();
            this.btnRemoveFromCollection = new System.Windows.Forms.Button();
            this.lblDocTitle = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDocCount = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlCollectionButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).BeginInit();
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
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel1.Controls.Add(this.lstCollections);
            this.splitContainer.Panel1.Controls.Add(this.pnlCollectionButtons);
            this.splitContainer.Panel1.Controls.Add(this.lblCollectionTitle);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.splitContainer.Panel2.Controls.Add(this.dgvDocuments);
            this.splitContainer.Panel2.Controls.Add(this.pnlDocButtons);
            this.splitContainer.Panel2.Controls.Add(this.lblDocTitle);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainer.Size = new System.Drawing.Size(900, 500);
            this.splitContainer.SplitterDistance = 280;
            this.splitContainer.TabIndex = 0;
            // 
            // lstCollections
            // 
            this.lstCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colCount});
            this.lstCollections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCollections.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstCollections.FullRowSelect = true;
            this.lstCollections.HideSelection = false;
            this.lstCollections.Location = new System.Drawing.Point(10, 41);
            this.lstCollections.MultiSelect = false;
            this.lstCollections.Name = "lstCollections";
            this.lstCollections.Size = new System.Drawing.Size(260, 399);
            this.lstCollections.TabIndex = 1;
            this.lstCollections.UseCompatibleStateImageBehavior = false;
            this.lstCollections.View = System.Windows.Forms.View.Details;
            this.lstCollections.SelectedIndexChanged += new System.EventHandler(this.lstCollections_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Text = "Tên";
            this.colName.Width = 160;
            // 
            // colCount
            // 
            this.colCount.Text = "Số lượng";
            this.colCount.Width = 80;
            // 
            // pnlCollectionButtons
            // 
            this.pnlCollectionButtons.Controls.Add(this.btnDeleteCollection);
            this.pnlCollectionButtons.Controls.Add(this.btnNewCollection);
            this.pnlCollectionButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCollectionButtons.Location = new System.Drawing.Point(10, 440);
            this.pnlCollectionButtons.Name = "pnlCollectionButtons";
            this.pnlCollectionButtons.Size = new System.Drawing.Size(260, 50);
            this.pnlCollectionButtons.TabIndex = 2;
            // 
            // btnDeleteCollection
            // 
            this.btnDeleteCollection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnDeleteCollection.Enabled = false;
            this.btnDeleteCollection.FlatAppearance.BorderSize = 0;
            this.btnDeleteCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteCollection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeleteCollection.ForeColor = System.Drawing.Color.White;
            this.btnDeleteCollection.Location = new System.Drawing.Point(135, 10);
            this.btnDeleteCollection.Name = "btnDeleteCollection";
            this.btnDeleteCollection.Size = new System.Drawing.Size(120, 32);
            this.btnDeleteCollection.TabIndex = 1;
            this.btnDeleteCollection.Text = "Xóa";
            this.btnDeleteCollection.UseVisualStyleBackColor = false;
            this.btnDeleteCollection.Click += new System.EventHandler(this.btnDeleteCollection_Click);
            // 
            // btnNewCollection
            // 
            this.btnNewCollection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnNewCollection.FlatAppearance.BorderSize = 0;
            this.btnNewCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewCollection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNewCollection.ForeColor = System.Drawing.Color.White;
            this.btnNewCollection.Location = new System.Drawing.Point(5, 10);
            this.btnNewCollection.Name = "btnNewCollection";
            this.btnNewCollection.Size = new System.Drawing.Size(120, 32);
            this.btnNewCollection.TabIndex = 0;
            this.btnNewCollection.Text = "+ Tạo mới";
            this.btnNewCollection.UseVisualStyleBackColor = false;
            this.btnNewCollection.Click += new System.EventHandler(this.btnNewCollection_Click);
            // 
            // lblCollectionTitle
            // 
            this.lblCollectionTitle.AutoSize = true;
            this.lblCollectionTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCollectionTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCollectionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblCollectionTitle.Location = new System.Drawing.Point(10, 10);
            this.lblCollectionTitle.Name = "lblCollectionTitle";
            this.lblCollectionTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblCollectionTitle.Size = new System.Drawing.Size(91, 31);
            this.lblCollectionTitle.TabIndex = 0;
            this.lblCollectionTitle.Text = "Bộ sưu tập";
            // 
            // dgvDocuments
            // 
            this.dgvDocuments.AllowUserToAddRows = false;
            this.dgvDocuments.AllowUserToDeleteRows = false;
            this.dgvDocuments.BackgroundColor = System.Drawing.Color.White;
            this.dgvDocuments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDocuments.Location = new System.Drawing.Point(10, 41);
            this.dgvDocuments.MultiSelect = false;
            this.dgvDocuments.Name = "dgvDocuments";
            this.dgvDocuments.ReadOnly = true;
            this.dgvDocuments.RowHeadersVisible = false;
            this.dgvDocuments.RowHeadersWidth = 62;
            this.dgvDocuments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDocuments.Size = new System.Drawing.Size(596, 399);
            this.dgvDocuments.TabIndex = 1;
            this.dgvDocuments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDocuments_CellDoubleClick);
            // 
            // pnlDocButtons
            // 
            this.pnlDocButtons.Controls.Add(this.btnClose);
            this.pnlDocButtons.Controls.Add(this.btnOpenAll);
            this.pnlDocButtons.Controls.Add(this.btnRemoveFromCollection);
            this.pnlDocButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDocButtons.Location = new System.Drawing.Point(10, 440);
            this.pnlDocButtons.Name = "pnlDocButtons";
            this.pnlDocButtons.Size = new System.Drawing.Size(596, 50);
            this.pnlDocButtons.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(486, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpenAll
            // 
            this.btnOpenAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnOpenAll.Enabled = false;
            this.btnOpenAll.FlatAppearance.BorderSize = 0;
            this.btnOpenAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOpenAll.ForeColor = System.Drawing.Color.White;
            this.btnOpenAll.Location = new System.Drawing.Point(5, 10);
            this.btnOpenAll.Name = "btnOpenAll";
            this.btnOpenAll.Size = new System.Drawing.Size(130, 32);
            this.btnOpenAll.TabIndex = 0;
            this.btnOpenAll.Text = "Mở tất cả";
            this.btnOpenAll.UseVisualStyleBackColor = false;
            this.btnOpenAll.Click += new System.EventHandler(this.btnOpenAll_Click);
            // 
            // btnRemoveFromCollection
            // 
            this.btnRemoveFromCollection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnRemoveFromCollection.FlatAppearance.BorderSize = 0;
            this.btnRemoveFromCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveFromCollection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRemoveFromCollection.ForeColor = System.Drawing.Color.White;
            this.btnRemoveFromCollection.Location = new System.Drawing.Point(145, 10);
            this.btnRemoveFromCollection.Name = "btnRemoveFromCollection";
            this.btnRemoveFromCollection.Size = new System.Drawing.Size(180, 32);
            this.btnRemoveFromCollection.TabIndex = 1;
            this.btnRemoveFromCollection.Text = "Xóa khỏi bộ sưu tập";
            this.btnRemoveFromCollection.UseVisualStyleBackColor = false;
            this.btnRemoveFromCollection.Click += new System.EventHandler(this.btnRemoveFromCollection_Click);
            // 
            // lblDocTitle
            // 
            this.lblDocTitle.AutoSize = true;
            this.lblDocTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDocTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblDocTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblDocTitle.Location = new System.Drawing.Point(10, 10);
            this.lblDocTitle.Name = "lblDocTitle";
            this.lblDocTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblDocTitle.Size = new System.Drawing.Size(196, 31);
            this.lblDocTitle.TabIndex = 0;
            this.lblDocTitle.Text = "Tài liệu trong bộ sưu tập";
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblDocCount});
            this.statusStrip.Location = new System.Drawing.Point(0, 500);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(900, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(54, 17);
            this.lblStatus.Text = "Sẵn sàng";
            // 
            // lblDocCount
            // 
            this.lblDocCount.Name = "lblDocCount";
            this.lblDocCount.Size = new System.Drawing.Size(831, 17);
            this.lblDocCount.Spring = true;
            this.lblDocCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CollectionManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 522);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(698, 394);
            this.Name = "CollectionManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý Bộ sưu tập";
            this.Load += new System.EventHandler(this.CollectionManagementForm_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlCollectionButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDocuments)).EndInit();
            this.pnlDocButtons.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label lblCollectionTitle;
        private System.Windows.Forms.ListView lstCollections;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colCount;
        private System.Windows.Forms.Panel pnlCollectionButtons;
        private System.Windows.Forms.Button btnNewCollection;
        private System.Windows.Forms.Button btnDeleteCollection;
        private System.Windows.Forms.Label lblDocTitle;
        private System.Windows.Forms.DataGridView dgvDocuments;
        private System.Windows.Forms.Panel pnlDocButtons;
        private System.Windows.Forms.Button btnOpenAll;
        private System.Windows.Forms.Button btnRemoveFromCollection;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblDocCount;
    }
}
