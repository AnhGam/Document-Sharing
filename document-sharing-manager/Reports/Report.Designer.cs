namespace document_sharing_manager.Reports
{
    public partial class Report
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlStats = new System.Windows.Forms.Panel();
            this.pnlStatCard1 = new System.Windows.Forms.Panel();
            this.lblStatValue1 = new System.Windows.Forms.Label();
            this.lblStatLabel1 = new System.Windows.Forms.Label();
            this.pnlStatCard2 = new System.Windows.Forms.Panel();
            this.lblStatValue2 = new System.Windows.Forms.Label();
            this.lblStatLabel2 = new System.Windows.Forms.Label();
            this.pnlStatCard3 = new System.Windows.Forms.Panel();
            this.lblStatValue3 = new System.Windows.Forms.Label();
            this.lblStatLabel3 = new System.Windows.Forms.Label();
            this.pnlStatCard4 = new System.Windows.Forms.Panel();
            this.lblStatValue4 = new System.Windows.Forms.Label();
            this.lblStatLabel4 = new System.Windows.Forms.Label();
            this.pnlStatCard5 = new System.Windows.Forms.Panel();
            this.lblStatValue5 = new System.Windows.Forms.Label();
            this.lblStatLabel5 = new System.Windows.Forms.Label();
            this.pnlStatCard6 = new System.Windows.Forms.Panel();
            this.lblStatValue6 = new System.Windows.Forms.Label();
            this.lblStatLabel6 = new System.Windows.Forms.Label();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.lblChartType = new System.Windows.Forms.Label();
            this.cboChartType = new System.Windows.Forms.ComboBox();
            this.lblStatType = new System.Windows.Forms.Label();
            this.btnBySubject = new System.Windows.Forms.Button();
            this.btnByType = new System.Windows.Forms.Button();
            this.btnByTimeline = new System.Windows.Forms.Button();
            this.btnByMonth = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pnlCharts = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlChart = new System.Windows.Forms.Panel();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlTimelineChart = new System.Windows.Forms.Panel();
            this.lblTimelineTitle = new System.Windows.Forms.Label();
            this.chartTimeline = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlTop.SuspendLayout();
            this.pnlStats.SuspendLayout();
            this.pnlStatCard1.SuspendLayout();
            this.pnlStatCard2.SuspendLayout();
            this.pnlStatCard3.SuspendLayout();
            this.pnlStatCard4.SuspendLayout();
            this.pnlStatCard5.SuspendLayout();
            this.pnlStatCard6.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.pnlCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.pnlTimelineChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTimeline)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.btnClose);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1200, 60);
            this.pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(280, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard Thống kê";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1100, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnCloseClick);
            // 
            // pnlStats
            // 
            this.pnlStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlStats.Controls.Add(this.pnlStatCard1);
            this.pnlStats.Controls.Add(this.pnlStatCard2);
            this.pnlStats.Controls.Add(this.pnlStatCard3);
            this.pnlStats.Controls.Add(this.pnlStatCard4);
            this.pnlStats.Controls.Add(this.pnlStatCard5);
            this.pnlStats.Controls.Add(this.pnlStatCard6);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStats.Location = new System.Drawing.Point(0, 60);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.pnlStats.Size = new System.Drawing.Size(1200, 100);
            this.pnlStats.TabIndex = 4;
            // 
            // pnlStatCard1
            // 
            this.pnlStatCard1.BackColor = System.Drawing.Color.White;
            this.pnlStatCard1.Controls.Add(this.lblStatValue1);
            this.pnlStatCard1.Controls.Add(this.lblStatLabel1);
            this.pnlStatCard1.Location = new System.Drawing.Point(20, 12);
            this.pnlStatCard1.Name = "pnlStatCard1";
            this.pnlStatCard1.Size = new System.Drawing.Size(180, 76);
            this.pnlStatCard1.TabIndex = 0;
            // 
            // lblStatValue1
            // 
            this.lblStatValue1.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblStatValue1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.lblStatValue1.Location = new System.Drawing.Point(12, 8);
            this.lblStatValue1.Name = "lblStatValue1";
            this.lblStatValue1.Size = new System.Drawing.Size(156, 40);
            this.lblStatValue1.TabIndex = 0;
            this.lblStatValue1.Text = "0";
            // 
            // lblStatLabel1
            // 
            this.lblStatLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatLabel1.Location = new System.Drawing.Point(12, 50);
            this.lblStatLabel1.Name = "lblStatLabel1";
            this.lblStatLabel1.Size = new System.Drawing.Size(156, 20);
            this.lblStatLabel1.TabIndex = 1;
            this.lblStatLabel1.Text = "Tổng tài liệu";
            // 
            // pnlStatCard2
            // 
            this.pnlStatCard2.BackColor = System.Drawing.Color.White;
            this.pnlStatCard2.Controls.Add(this.lblStatValue2);
            this.pnlStatCard2.Controls.Add(this.lblStatLabel2);
            this.pnlStatCard2.Location = new System.Drawing.Point(210, 12);
            this.pnlStatCard2.Name = "pnlStatCard2";
            this.pnlStatCard2.Size = new System.Drawing.Size(180, 76);
            this.pnlStatCard2.TabIndex = 1;
            // 
            // lblStatValue2
            // 
            this.lblStatValue2.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblStatValue2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.lblStatValue2.Location = new System.Drawing.Point(12, 8);
            this.lblStatValue2.Name = "lblStatValue2";
            this.lblStatValue2.Size = new System.Drawing.Size(156, 40);
            this.lblStatValue2.TabIndex = 0;
            this.lblStatValue2.Text = "0";
            // 
            // lblStatLabel2
            // 
            this.lblStatLabel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatLabel2.Location = new System.Drawing.Point(12, 50);
            this.lblStatLabel2.Name = "lblStatLabel2";
            this.lblStatLabel2.Size = new System.Drawing.Size(156, 20);
            this.lblStatLabel2.TabIndex = 1;
            this.lblStatLabel2.Text = "Quan trọng";
            // 
            // pnlStatCard3
            // 
            this.pnlStatCard3.BackColor = System.Drawing.Color.White;
            this.pnlStatCard3.Controls.Add(this.lblStatValue3);
            this.pnlStatCard3.Controls.Add(this.lblStatLabel3);
            this.pnlStatCard3.Location = new System.Drawing.Point(400, 12);
            this.pnlStatCard3.Name = "pnlStatCard3";
            this.pnlStatCard3.Size = new System.Drawing.Size(180, 76);
            this.pnlStatCard3.TabIndex = 2;
            // 
            // lblStatValue3
            // 
            this.lblStatValue3.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblStatValue3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.lblStatValue3.Location = new System.Drawing.Point(12, 8);
            this.lblStatValue3.Name = "lblStatValue3";
            this.lblStatValue3.Size = new System.Drawing.Size(156, 40);
            this.lblStatValue3.TabIndex = 0;
            this.lblStatValue3.Text = "0";
            // 
            // lblStatLabel3
            // 
            this.lblStatLabel3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatLabel3.Location = new System.Drawing.Point(12, 50);
            this.lblStatLabel3.Name = "lblStatLabel3";
            this.lblStatLabel3.Size = new System.Drawing.Size(156, 20);
            this.lblStatLabel3.TabIndex = 1;
            this.lblStatLabel3.Text = "Đã xóa";
            // 
            // pnlStatCard4
            // 
            this.pnlStatCard4.BackColor = System.Drawing.Color.White;
            this.pnlStatCard4.Controls.Add(this.lblStatValue4);
            this.pnlStatCard4.Controls.Add(this.lblStatLabel4);
            this.pnlStatCard4.Location = new System.Drawing.Point(590, 12);
            this.pnlStatCard4.Name = "pnlStatCard4";
            this.pnlStatCard4.Size = new System.Drawing.Size(180, 76);
            this.pnlStatCard4.TabIndex = 3;
            // 
            // lblStatValue4
            // 
            this.lblStatValue4.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblStatValue4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblStatValue4.Location = new System.Drawing.Point(12, 8);
            this.lblStatValue4.Name = "lblStatValue4";
            this.lblStatValue4.Size = new System.Drawing.Size(156, 40);
            this.lblStatValue4.TabIndex = 0;
            this.lblStatValue4.Text = "0";
            // 
            // lblStatLabel4
            // 
            this.lblStatLabel4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatLabel4.Location = new System.Drawing.Point(12, 50);
            this.lblStatLabel4.Name = "lblStatLabel4";
            this.lblStatLabel4.Size = new System.Drawing.Size(156, 20);
            this.lblStatLabel4.TabIndex = 1;
            this.lblStatLabel4.Text = "Bộ sưu tập";
            // 
            // pnlStatCard5
            // 
            this.pnlStatCard5.BackColor = System.Drawing.Color.White;
            this.pnlStatCard5.Controls.Add(this.lblStatValue5);
            this.pnlStatCard5.Controls.Add(this.lblStatLabel5);
            this.pnlStatCard5.Location = new System.Drawing.Point(780, 12);
            this.pnlStatCard5.Name = "pnlStatCard5";
            this.pnlStatCard5.Size = new System.Drawing.Size(180, 76);
            this.pnlStatCard5.TabIndex = 4;
            // 
            // lblStatValue5
            // 
            this.lblStatValue5.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblStatValue5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblStatValue5.Location = new System.Drawing.Point(12, 8);
            this.lblStatValue5.Name = "lblStatValue5";
            this.lblStatValue5.Size = new System.Drawing.Size(156, 40);
            this.lblStatValue5.TabIndex = 0;
            this.lblStatValue5.Text = "0";
            // 
            // lblStatLabel5
            // 
            this.lblStatLabel5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatLabel5.Location = new System.Drawing.Point(12, 50);
            this.lblStatLabel5.Name = "lblStatLabel5";
            this.lblStatLabel5.Size = new System.Drawing.Size(156, 20);
            this.lblStatLabel5.TabIndex = 1;
            this.lblStatLabel5.Text = "Chưa có file";
            // 
            // pnlStatCard6
            // 
            this.pnlStatCard6.BackColor = System.Drawing.Color.White;
            this.pnlStatCard6.Controls.Add(this.lblStatValue6);
            this.pnlStatCard6.Controls.Add(this.lblStatLabel6);
            this.pnlStatCard6.Location = new System.Drawing.Point(970, 12);
            this.pnlStatCard6.Name = "pnlStatCard6";
            this.pnlStatCard6.Size = new System.Drawing.Size(180, 76);
            this.pnlStatCard6.TabIndex = 5;
            // 
            // lblStatValue6
            // 
            this.lblStatValue6.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblStatValue6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblStatValue6.Location = new System.Drawing.Point(12, 8);
            this.lblStatValue6.Name = "lblStatValue6";
            this.lblStatValue6.Size = new System.Drawing.Size(156, 40);
            this.lblStatValue6.TabIndex = 0;
            this.lblStatValue6.Text = "0";
            // 
            // lblStatLabel6
            // 
            this.lblStatLabel6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatLabel6.Location = new System.Drawing.Point(12, 50);
            this.lblStatLabel6.Name = "lblStatLabel6";
            this.lblStatLabel6.Size = new System.Drawing.Size(156, 20);
            this.lblStatLabel6.TabIndex = 1;
            this.lblStatLabel6.Text = "Bộ sưu tập";
            // 
            // pnlOptions
            // 
            this.pnlOptions.BackColor = System.Drawing.Color.White;
            this.pnlOptions.Controls.Add(this.lblChartType);
            this.pnlOptions.Controls.Add(this.cboChartType);
            this.pnlOptions.Controls.Add(this.lblStatType);
            this.pnlOptions.Controls.Add(this.btnByType);
            this.pnlOptions.Controls.Add(this.btnByTimeline);
            this.pnlOptions.Controls.Add(this.btnByMonth);
            this.pnlOptions.Controls.Add(this.lblTotal);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOptions.Location = new System.Drawing.Point(0, 160);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(1200, 56);
            this.pnlOptions.TabIndex = 1;
            // 
            // lblChartType
            // 
            this.lblChartType.AutoSize = true;
            this.lblChartType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblChartType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblChartType.Location = new System.Drawing.Point(580, 18);
            this.lblChartType.Name = "lblChartType";
            this.lblChartType.Size = new System.Drawing.Size(85, 20);
            this.lblChartType.TabIndex = 5;
            this.lblChartType.Text = "Kiểu biểu đồ:";
            // 
            // cboChartType
            // 
            this.cboChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChartType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboChartType.FormattingEnabled = true;
            this.cboChartType.Location = new System.Drawing.Point(670, 14);
            this.cboChartType.Name = "cboChartType";
            this.cboChartType.Size = new System.Drawing.Size(160, 28);
            this.cboChartType.TabIndex = 4;
            this.cboChartType.SelectedIndexChanged += new System.EventHandler(this.CboChartTypeSelectedIndexChanged);
            // 
            // lblStatType
            // 
            this.lblStatType.AutoSize = true;
            this.lblStatType.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblStatType.Location = new System.Drawing.Point(20, 18);
            this.lblStatType.Name = "lblStatType";
            this.lblStatType.Size = new System.Drawing.Size(100, 20);
            this.lblStatType.TabIndex = 3;
            this.lblStatType.Text = "Thống kê theo:";
            // 
            // btnByType
            // 
            this.btnByType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.btnByType.FlatAppearance.BorderSize = 0;
            this.btnByType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnByType.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnByType.ForeColor = System.Drawing.Color.White;
            this.btnByType.Location = new System.Drawing.Point(240, 12);
            this.btnByType.Name = "btnByType";
            this.btnByType.Size = new System.Drawing.Size(100, 32);
            this.btnByType.TabIndex = 1;
            this.btnByType.Text = "Định dạng";
            this.btnByType.UseVisualStyleBackColor = false;
            this.btnByType.Click += new System.EventHandler(this.BtnByTypeClick);
            // 
            // btnByTimeline
            // 
            this.btnByTimeline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnByTimeline.FlatAppearance.BorderSize = 0;
            this.btnByTimeline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnByTimeline.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnByTimeline.ForeColor = System.Drawing.Color.White;
            this.btnByTimeline.Location = new System.Drawing.Point(350, 12);
            this.btnByTimeline.Name = "btnByTimeline";
            this.btnByTimeline.Size = new System.Drawing.Size(100, 32);
            this.btnByTimeline.TabIndex = 6;
            this.btnByTimeline.Text = "7 ngày";
            this.btnByTimeline.UseVisualStyleBackColor = false;
            this.btnByTimeline.Click += new System.EventHandler(this.BtnByTimelineClick);
            // 
            // btnByMonth
            // 
            this.btnByMonth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnByMonth.FlatAppearance.BorderSize = 0;
            this.btnByMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnByMonth.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnByMonth.ForeColor = System.Drawing.Color.White;
            this.btnByMonth.Location = new System.Drawing.Point(460, 12);
            this.btnByMonth.Name = "btnByMonth";
            this.btnByMonth.Size = new System.Drawing.Size(100, 32);
            this.btnByMonth.TabIndex = 7;
            this.btnByMonth.Text = "Theo tháng";
            this.btnByMonth.UseVisualStyleBackColor = false;
            this.btnByMonth.Click += new System.EventHandler(this.BtnByMonthClick);
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(184)))), ((int)(((byte)(166)))));
            this.lblTotal.Location = new System.Drawing.Point(1000, 14);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(180, 28);
            this.lblTotal.TabIndex = 2;
            this.lblTotal.Text = "Tổng: 0 tài liệu";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlCharts
            // 
            this.pnlCharts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlCharts.Controls.Add(this.splitContainer);
            this.pnlCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCharts.Location = new System.Drawing.Point(0, 216);
            this.pnlCharts.Name = "pnlCharts";
            this.pnlCharts.Padding = new System.Windows.Forms.Padding(16);
            this.pnlCharts.Size = new System.Drawing.Size(1200, 456);
            this.pnlCharts.TabIndex = 2;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(16, 16);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.pnlChart);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pnlTimelineChart);
            this.splitContainer.Size = new System.Drawing.Size(1168, 424);
            this.splitContainer.SplitterDistance = 580;
            this.splitContainer.SplitterWidth = 8;
            this.splitContainer.TabIndex = 0;
            // 
            // pnlChart
            // 
            this.pnlChart.BackColor = System.Drawing.Color.White;
            this.pnlChart.Controls.Add(this.chart);
            this.pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChart.Location = new System.Drawing.Point(0, 0);
            this.pnlChart.Name = "pnlChart";
            this.pnlChart.Padding = new System.Windows.Forms.Padding(12);
            this.pnlChart.Size = new System.Drawing.Size(580, 424);
            this.pnlChart.TabIndex = 0;
            // 
            // chart
            // 
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(12, 12);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(556, 400);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart";
            // 
            // pnlTimelineChart
            // 
            this.pnlTimelineChart.BackColor = System.Drawing.Color.White;
            this.pnlTimelineChart.Controls.Add(this.lblTimelineTitle);
            this.pnlTimelineChart.Controls.Add(this.chartTimeline);
            this.pnlTimelineChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTimelineChart.Location = new System.Drawing.Point(0, 0);
            this.pnlTimelineChart.Name = "pnlTimelineChart";
            this.pnlTimelineChart.Padding = new System.Windows.Forms.Padding(12);
            this.pnlTimelineChart.Size = new System.Drawing.Size(580, 424);
            this.pnlTimelineChart.TabIndex = 0;
            // 
            // lblTimelineTitle
            // 
            this.lblTimelineTitle.AutoSize = true;
            this.lblTimelineTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblTimelineTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTimelineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTimelineTitle.Name = "lblTimelineTitle";
            this.lblTimelineTitle.Size = new System.Drawing.Size(220, 25);
            this.lblTimelineTitle.TabIndex = 1;
            this.lblTimelineTitle.Text = "Tài liệu thêm theo thời gian";
            // 
            // chartTimeline
            // 
            this.chartTimeline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.chartTimeline.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartTimeline.Legends.Add(legend2);
            this.chartTimeline.Location = new System.Drawing.Point(12, 40);
            this.chartTimeline.Name = "chartTimeline";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartTimeline.Series.Add(series2);
            this.chartTimeline.Size = new System.Drawing.Size(556, 372);
            this.chartTimeline.TabIndex = 0;
            this.chartTimeline.Text = "chartTimeline";
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 672);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 28);
            this.statusStrip.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(49, 22);
            this.lblStatus.Text = "Sẵn sàng";
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.pnlCharts);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.pnlStats);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "Report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dashboard Thống kê";
            this.Load += new System.EventHandler(this.Report_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlStats.ResumeLayout(false);
            this.pnlStatCard1.ResumeLayout(false);
            this.pnlStatCard2.ResumeLayout(false);
            this.pnlStatCard3.ResumeLayout(false);
            this.pnlStatCard4.ResumeLayout(false);
            this.pnlStatCard5.ResumeLayout(false);
            this.pnlStatCard6.ResumeLayout(false);
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.pnlCharts.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.pnlTimelineChart.ResumeLayout(false);
            this.pnlTimelineChart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTimeline)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlStats;
        private System.Windows.Forms.Panel pnlStatCard1;
        private System.Windows.Forms.Label lblStatValue1;
        private System.Windows.Forms.Label lblStatLabel1;
        private System.Windows.Forms.Panel pnlStatCard2;
        private System.Windows.Forms.Label lblStatValue2;
        private System.Windows.Forms.Label lblStatLabel2;
        private System.Windows.Forms.Panel pnlStatCard3;
        private System.Windows.Forms.Label lblStatValue3;
        private System.Windows.Forms.Label lblStatLabel3;
        private System.Windows.Forms.Panel pnlStatCard4;
        private System.Windows.Forms.Label lblStatValue4;
        private System.Windows.Forms.Label lblStatLabel4;
        private System.Windows.Forms.Panel pnlStatCard5;
        private System.Windows.Forms.Label lblStatValue5;
        private System.Windows.Forms.Label lblStatLabel5;
        private System.Windows.Forms.Panel pnlStatCard6;
        private System.Windows.Forms.Label lblStatValue6;
        private System.Windows.Forms.Label lblStatLabel6;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Label lblStatType;
        private System.Windows.Forms.Button btnBySubject;
        private System.Windows.Forms.Button btnByType;
        private System.Windows.Forms.Button btnByTimeline;
        private System.Windows.Forms.Button btnByMonth;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Panel pnlCharts;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Panel pnlTimelineChart;
        private System.Windows.Forms.Label lblTimelineTitle;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTimeline;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblChartType;
        private System.Windows.Forms.ComboBox cboChartType;
    }
}

