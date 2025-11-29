namespace study_document_manager
{
    partial class Report
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.lblChartType = new System.Windows.Forms.Label();
            this.cboChartType = new System.Windows.Forms.ComboBox();
            this.lblStatType = new System.Windows.Forms.Label();
            this.btnBySubject = new System.Windows.Forms.Button();
            this.btnByType = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pnlChart = new System.Windows.Forms.Panel();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlTop.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.pnlChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.btnClose);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1000, 60);
            this.pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(437, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Thống kê tài liệu học tập";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(900, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnlOptions
            // 
            this.pnlOptions.BackColor = System.Drawing.Color.White;
            this.pnlOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOptions.Controls.Add(this.lblChartType);
            this.pnlOptions.Controls.Add(this.cboChartType);
            this.pnlOptions.Controls.Add(this.lblStatType);
            this.pnlOptions.Controls.Add(this.btnBySubject);
            this.pnlOptions.Controls.Add(this.btnByType);
            this.pnlOptions.Controls.Add(this.lblTotal);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOptions.Location = new System.Drawing.Point(0, 60);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(1000, 80);
            this.pnlOptions.TabIndex = 1;
            // 
            // lblChartType
            // 
            this.lblChartType.AutoSize = true;
            this.lblChartType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblChartType.Location = new System.Drawing.Point(600, 20);
            this.lblChartType.Name = "lblChartType";
            this.lblChartType.Size = new System.Drawing.Size(115, 25);
            this.lblChartType.TabIndex = 5;
            this.lblChartType.Text = "Kiểu biểu đồ:";
            // 
            // cboChartType
            // 
            this.cboChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChartType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboChartType.FormattingEnabled = true;
            this.cboChartType.Location = new System.Drawing.Point(600, 40);
            this.cboChartType.Name = "cboChartType";
            this.cboChartType.Size = new System.Drawing.Size(180, 33);
            this.cboChartType.TabIndex = 4;
            this.cboChartType.SelectedIndexChanged += new System.EventHandler(this.cboChartType_SelectedIndexChanged);
            // 
            // lblStatType
            // 
            this.lblStatType.AutoSize = true;
            this.lblStatType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatType.Location = new System.Drawing.Point(20, 20);
            this.lblStatType.Name = "lblStatType";
            this.lblStatType.Size = new System.Drawing.Size(131, 25);
            this.lblStatType.TabIndex = 3;
            this.lblStatType.Text = "Thống kê theo:";
            // 
            // btnBySubject
            // 
            this.btnBySubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnBySubject.FlatAppearance.BorderSize = 0;
            this.btnBySubject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBySubject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBySubject.ForeColor = System.Drawing.Color.White;
            this.btnBySubject.Location = new System.Drawing.Point(20, 40);
            this.btnBySubject.Name = "btnBySubject";
            this.btnBySubject.Size = new System.Drawing.Size(120, 30);
            this.btnBySubject.TabIndex = 0;
            this.btnBySubject.Text = "Môn học";
            this.btnBySubject.UseVisualStyleBackColor = false;
            this.btnBySubject.Click += new System.EventHandler(this.btnBySubject_Click);
            // 
            // btnByType
            // 
            this.btnByType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnByType.FlatAppearance.BorderSize = 0;
            this.btnByType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnByType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnByType.ForeColor = System.Drawing.Color.White;
            this.btnByType.Location = new System.Drawing.Point(150, 40);
            this.btnByType.Name = "btnByType";
            this.btnByType.Size = new System.Drawing.Size(120, 30);
            this.btnByType.TabIndex = 1;
            this.btnByType.Text = "Loại tài liệu";
            this.btnByType.UseVisualStyleBackColor = false;
            this.btnByType.Click += new System.EventHandler(this.btnByType_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblTotal.Location = new System.Drawing.Point(800, 25);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(180, 40);
            this.lblTotal.TabIndex = 2;
            this.lblTotal.Text = "Tổng: 0 tài liệu";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlChart
            // 
            this.pnlChart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlChart.Controls.Add(this.chart);
            this.pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChart.Location = new System.Drawing.Point(0, 140);
            this.pnlChart.Name = "pnlChart";
            this.pnlChart.Padding = new System.Windows.Forms.Padding(20);
            this.pnlChart.Size = new System.Drawing.Size(1000, 450);
            this.pnlChart.TabIndex = 2;
            // 
            // chart
            // 
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(20, 20);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(960, 410);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart";
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.White;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 590);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1000, 32);
            this.statusStrip.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(251, 25);
            this.lblStatus.Text = "Đã tải thống kê theo môn học";
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 622);
            this.Controls.Add(this.pnlChart);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "Report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thống kê tài liệu";
            this.Load += new System.EventHandler(this.Report_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.pnlChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Label lblStatType;
        private System.Windows.Forms.Button btnBySubject;
        private System.Windows.Forms.Button btnByType;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Panel pnlChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblChartType;
        private System.Windows.Forms.ComboBox cboChartType;
    }
}