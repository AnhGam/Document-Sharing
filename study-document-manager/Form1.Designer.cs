namespace study_document_manager
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tongsotailieu = new System.Windows.Forms.Panel();
            this.tongtailieu = new System.Windows.Forms.Label();
            this.daduyet = new System.Windows.Forms.Panel();
            this.saphethan = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.tongsotailieu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("VNI-Cooper", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.label1.Location = new System.Drawing.Point(251, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(478, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "Study Document Manager";
            // 
            // tongsotailieu
            // 
            this.tongsotailieu.BackColor = System.Drawing.Color.Silver;
            this.tongsotailieu.Controls.Add(this.tongtailieu);
            this.tongsotailieu.Location = new System.Drawing.Point(54, 95);
            this.tongsotailieu.Name = "tongsotailieu";
            this.tongsotailieu.Size = new System.Drawing.Size(202, 104);
            this.tongsotailieu.TabIndex = 2;
            this.tongsotailieu.Paint += new System.Windows.Forms.PaintEventHandler(this.tongsotailieu_Paint);
            // 
            // tongtailieu
            // 
            this.tongtailieu.AutoSize = true;
            this.tongtailieu.Location = new System.Drawing.Point(86, 44);
            this.tongtailieu.Name = "tongtailieu";
            this.tongtailieu.Size = new System.Drawing.Size(35, 13);
            this.tongtailieu.TabIndex = 0;
            this.tongtailieu.Text = "label2";
            this.tongtailieu.Click += new System.EventHandler(this.tongtailieu_Click);
            // 
            // daduyet
            // 
            this.daduyet.BackColor = System.Drawing.Color.Silver;
            this.daduyet.Location = new System.Drawing.Point(376, 95);
            this.daduyet.Name = "daduyet";
            this.daduyet.Size = new System.Drawing.Size(202, 104);
            this.daduyet.TabIndex = 3;
            this.daduyet.Paint += new System.Windows.Forms.PaintEventHandler(this.daduyet_Paint);
            // 
            // saphethan
            // 
            this.saphethan.BackColor = System.Drawing.Color.Silver;
            this.saphethan.Location = new System.Drawing.Point(694, 95);
            this.saphethan.Name = "saphethan";
            this.saphethan.Size = new System.Drawing.Size(202, 104);
            this.saphethan.TabIndex = 3;
            this.saphethan.Paint += new System.Windows.Forms.PaintEventHandler(this.saphethan_Paint);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(54, 243);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(842, 71);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // dataGridView2
            // 
            this.dataGridView2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(54, 356);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(842, 71);
            this.dataGridView2.TabIndex = 5;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(376, 462);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(202, 20);
            this.dateTimePicker1.TabIndex = 6;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(964, 505);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.saphethan);
            this.Controls.Add(this.daduyet);
            this.Controls.Add(this.tongsotailieu);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Study Document Manager";
            this.tongsotailieu.ResumeLayout(false);
            this.tongsotailieu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel tongsotailieu;
        private System.Windows.Forms.Panel daduyet;
        private System.Windows.Forms.Panel saphethan;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label tongtailieu;
    }
}

