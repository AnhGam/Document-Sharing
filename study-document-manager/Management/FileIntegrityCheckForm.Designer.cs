namespace study_document_manager
{
    partial class FileIntegrityCheckForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileIntegrityCheckForm));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.dgvMissingFiles = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuongDan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAction = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMissingFiles)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.pnlHeader.Size = new System.Drawing.Size(700, 56);
            this.pnlHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(210, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Kiểm tra file bị thiếu";
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(600, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // pnlProgress
            //
            this.pnlProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlProgress.Controls.Add(this.progressBar);
            this.pnlProgress.Controls.Add(this.lblProgress);
            this.pnlProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProgress.Location = new System.Drawing.Point(0, 56);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
            this.pnlProgress.Size = new System.Drawing.Size(700, 70);
            this.pnlProgress.TabIndex = 1;
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(20, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(660, 22);
            this.progressBar.TabIndex = 0;
            //
            // lblProgress
            //
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblProgress.Location = new System.Drawing.Point(20, 42);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(200, 20);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "Nhấn \"Quét\" để bắt đầu kiểm tra...";
            //
            // dgvMissingFiles
            //
            this.dgvMissingFiles.AllowUserToAddRows = false;
            this.dgvMissingFiles.AllowUserToDeleteRows = false;
            this.dgvMissingFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMissingFiles.BackgroundColor = System.Drawing.Color.White;
            this.dgvMissingFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMissingFiles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvMissingFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMissingFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colTen,
            this.colDuongDan,
            this.colAction});
            this.dgvMissingFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMissingFiles.Location = new System.Drawing.Point(0, 126);
            this.dgvMissingFiles.Margin = new System.Windows.Forms.Padding(20);
            this.dgvMissingFiles.MultiSelect = false;
            this.dgvMissingFiles.Name = "dgvMissingFiles";
            this.dgvMissingFiles.ReadOnly = true;
            this.dgvMissingFiles.RowHeadersVisible = false;
            this.dgvMissingFiles.RowHeadersWidth = 25;
            this.dgvMissingFiles.RowTemplate.Height = 36;
            this.dgvMissingFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMissingFiles.Size = new System.Drawing.Size(700, 278);
            this.dgvMissingFiles.TabIndex = 2;
            this.dgvMissingFiles.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMissingFiles_CellClick);
            //
            // colId
            //
            this.colId.FillWeight = 10F;
            this.colId.HeaderText = "ID";
            this.colId.MinimumWidth = 50;
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colTen
            //
            this.colTen.FillWeight = 30F;
            this.colTen.HeaderText = "Tên tài liệu";
            this.colTen.MinimumWidth = 100;
            this.colTen.Name = "colTen";
            this.colTen.ReadOnly = true;
            //
            // colDuongDan
            //
            this.colDuongDan.FillWeight = 50F;
            this.colDuongDan.HeaderText = "Đường dẫn file";
            this.colDuongDan.MinimumWidth = 150;
            this.colDuongDan.Name = "colDuongDan";
            this.colDuongDan.ReadOnly = true;
            //
            // colAction
            //
            this.colAction.FillWeight = 15F;
            this.colAction.HeaderText = "Hành động";
            this.colAction.MinimumWidth = 80;
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            this.colAction.Text = "Xử lý";
            this.colAction.UseColumnTextForButtonValue = true;
            //
            // pnlFooter
            //
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlFooter.Controls.Add(this.btnScan);
            this.pnlFooter.Controls.Add(this.btnDeleteAll);
            this.pnlFooter.Controls.Add(this.lblSummary);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 404);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
            this.pnlFooter.Size = new System.Drawing.Size(700, 66);
            this.pnlFooter.TabIndex = 3;
            //
            // lblSummary
            //
            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.lblSummary.Location = new System.Drawing.Point(480, 18);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(200, 30);
            this.lblSummary.TabIndex = 2;
            this.lblSummary.Text = "";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnDeleteAll
            //
            this.btnDeleteAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDeleteAll.Enabled = false;
            this.btnDeleteAll.FlatAppearance.BorderSize = 0;
            this.btnDeleteAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteAll.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDeleteAll.ForeColor = System.Drawing.Color.White;
            this.btnDeleteAll.Location = new System.Drawing.Point(150, 12);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(120, 40);
            this.btnDeleteAll.TabIndex = 1;
            this.btnDeleteAll.Text = "Xóa tất cả";
            this.btnDeleteAll.UseVisualStyleBackColor = false;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            //
            // btnScan
            //
            this.btnScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.btnScan.FlatAppearance.BorderSize = 0;
            this.btnScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnScan.ForeColor = System.Drawing.Color.White;
            this.btnScan.Location = new System.Drawing.Point(20, 12);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(120, 40);
            this.btnScan.TabIndex = 0;
            this.btnScan.Text = "Quét";
            this.btnScan.UseVisualStyleBackColor = false;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            //
            // FileIntegrityCheckForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(700, 470);
            this.Controls.Add(this.dgvMissingFiles);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlProgress);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 450);
            this.Name = "FileIntegrityCheckForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kiểm tra file bị thiếu";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlProgress.ResumeLayout(false);
            this.pnlProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMissingFiles)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.DataGridView dgvMissingFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuongDan;
        private System.Windows.Forms.DataGridViewButtonColumn colAction;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Label lblSummary;
    }
}
