namespace study_document_manager.Documents
{
    public partial class PersonalNoteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonalNoteForm));
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDocumentName = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new study_document_manager.UI.Controls.ModernButton();
            this.btnDelete = new study_document_manager.UI.Controls.ModernButton();
            this.btnCancel = new study_document_manager.UI.Controls.ModernButton();
            this.mainTableLayout.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // mainTableLayout
            //
            this.mainTableLayout.AutoScroll = true;
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.lblTitle, 0, 0);
            this.mainTableLayout.Controls.Add(this.lblDocumentName, 0, 1);
            this.mainTableLayout.Controls.Add(this.lblStatus, 0, 2);
            this.mainTableLayout.Controls.Add(this.cboStatus, 0, 3);
            this.mainTableLayout.Controls.Add(this.lblNote, 0, 4);
            this.mainTableLayout.Controls.Add(this.txtNote, 0, 5);
            this.mainTableLayout.Controls.Add(this.buttonPanel, 0, 6);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.Padding = new System.Windows.Forms.Padding(20, 16, 20, 16);
            this.mainTableLayout.RowCount = 7;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Title
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Document name
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Status label
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Status combo
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Note label
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F)); // Note textbox - fills remaining space
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Buttons
            this.mainTableLayout.Size = new System.Drawing.Size(450, 500);
            this.mainTableLayout.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblTitle.Location = new System.Drawing.Point(23, 16);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(404, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Ghi chu ca nhan";
            //
            // lblDocumentName
            //
            this.lblDocumentName.AutoSize = true;
            this.lblDocumentName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDocumentName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.lblDocumentName.ForeColor = System.Drawing.Color.Gray;
            this.lblDocumentName.Location = new System.Drawing.Point(23, 56);
            this.lblDocumentName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 16);
            this.lblDocumentName.Name = "lblDocumentName";
            this.lblDocumentName.Size = new System.Drawing.Size(404, 23);
            this.lblDocumentName.TabIndex = 1;
            this.lblDocumentName.Text = "[Ten tai lieu]";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(23, 95);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(404, 20);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Trang thai:";
            //
            // cboStatus
            //
            this.cboStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Location = new System.Drawing.Point(23, 122);
            this.cboStatus.Margin = new System.Windows.Forms.Padding(3, 3, 3, 12);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(404, 31);
            this.cboStatus.TabIndex = 3;
            //
            // lblNote
            //
            this.lblNote.AutoSize = true;
            this.lblNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNote.Location = new System.Drawing.Point(23, 165);
            this.lblNote.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(404, 20);
            this.lblNote.TabIndex = 4;
            this.lblNote.Text = "Ghi chu:";
            //
            // txtNote
            //
            this.txtNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNote.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNote.Location = new System.Drawing.Point(23, 192);
            this.txtNote.Margin = new System.Windows.Forms.Padding(3, 3, 3, 12);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNote.Size = new System.Drawing.Size(404, 224);
            this.txtNote.TabIndex = 5;
            //
            // buttonPanel
            //
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(this.btnSave);
            this.buttonPanel.Controls.Add(this.btnDelete);
            this.buttonPanel.Controls.Add(this.btnCancel);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.buttonPanel.Location = new System.Drawing.Point(23, 431);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(404, 53);
            this.buttonPanel.TabIndex = 6;
            this.buttonPanel.WrapContents = false;
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnSave.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnSave.BorderRadius = 8;
            this.btnSave.BorderSize = 0;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.btnSave.MinimumSize = new System.Drawing.Size(100, 44);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 44);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Luu";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnDelete
            //
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnDelete.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnDelete.BorderRadius = 8;
            this.btnDelete.BorderSize = 0;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(134, 3);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.btnDelete.MinimumSize = new System.Drawing.Size(100, 44);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 44);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Xoa";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnCancel
            //
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btnCancel.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnCancel.BorderRadius = 8;
            this.btnCancel.BorderSize = 0;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(265, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnCancel.MinimumSize = new System.Drawing.Size(100, 44);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 44);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Dong";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // PersonalNoteForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(450, 500);
            this.Controls.Add(this.mainTableLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 450);
            this.Name = "PersonalNoteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ghi chu ca nhan";
            this.Load += new System.EventHandler(this.PersonalNoteForm_Load);
            this.mainTableLayout.ResumeLayout(false);
            this.mainTableLayout.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDocumentName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private study_document_manager.UI.Controls.ModernButton btnSave;
        private study_document_manager.UI.Controls.ModernButton btnCancel;
        private study_document_manager.UI.Controls.ModernButton btnDelete;
    }
}

