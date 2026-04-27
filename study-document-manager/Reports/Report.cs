using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using study_document_manager.UI;

namespace study_document_manager
{
    public partial class Report : Form
    {
        private string _currentStatType = "subject";
        private int _currentTimelineDays = 7;

        public Report()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            
            pnlTop.BackColor = AppTheme.Primary;
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font(AppTheme.FontFamily, 16F, FontStyle.Bold);
            
            AppTheme.ApplyButtonDanger(btnClose);
            btnClose.Height = 32;
            
            pnlStats.BackColor = AppTheme.BackgroundSoft;
            ApplyStatCardTheme(pnlStatCard1, lblStatValue1, lblStatLabel1, AppTheme.Primary);
            ApplyStatCardTheme(pnlStatCard2, lblStatValue2, lblStatLabel2, AppTheme.AccentAmber);
            ApplyStatCardTheme(pnlStatCard3, lblStatValue3, lblStatLabel3, AppTheme.Danger);
            ApplyStatCardTheme(pnlStatCard4, lblStatValue4, lblStatLabel4, AppTheme.Info);
            ApplyStatCardTheme(pnlStatCard5, lblStatValue5, lblStatLabel5, AppTheme.TextMuted);
            ApplyStatCardTheme(pnlStatCard6, lblStatValue6, lblStatLabel6, AppTheme.Secondary);
            
            pnlOptions.BackColor = AppTheme.BackgroundMain;
            pnlOptions.BorderStyle = BorderStyle.None;
            lblStatType.ForeColor = AppTheme.TextPrimary;
            lblStatType.Font = AppTheme.FontSmallBold;
            lblChartType.ForeColor = AppTheme.TextSecondary;
            lblChartType.Font = AppTheme.FontSmall;
            
            AppTheme.ApplyButtonSuccess(btnBySubject);
            btnBySubject.Height = 32;
            AppTheme.ApplyButtonPrimary(btnByType);
            btnByType.Height = 32;
            
            btnByTimeline.BackColor = AppTheme.Info;
            btnByTimeline.ForeColor = Color.White;
            btnByTimeline.FlatStyle = FlatStyle.Flat;
            btnByTimeline.FlatAppearance.BorderSize = 0;
            btnByTimeline.Font = AppTheme.FontButton;
            btnByTimeline.Cursor = Cursors.Hand;
            
            btnByMonth.BackColor = Color.FromArgb(139, 92, 246);
            btnByMonth.ForeColor = Color.White;
            btnByMonth.FlatStyle = FlatStyle.Flat;
            btnByMonth.FlatAppearance.BorderSize = 0;
            btnByMonth.Font = AppTheme.FontButton;
            btnByMonth.Cursor = Cursors.Hand;
            
            AppTheme.ApplyComboBoxStyle(cboChartType);
            
            lblTotal.ForeColor = AppTheme.Primary;
            lblTotal.Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold);
            
            pnlCharts.BackColor = AppTheme.BackgroundSoft;
            pnlChart.BackColor = Color.White;
            pnlTimelineChart.BackColor = Color.White;
            lblTimelineTitle.ForeColor = AppTheme.TextPrimary;
            lblTimelineTitle.Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold);
            
            chart.BackColor = Color.White;
            chartTimeline.BackColor = Color.White;
            
            statusStrip.BackColor = AppTheme.BackgroundSoft;
            lblStatus.ForeColor = AppTheme.TextSecondary;
        }

        private void ApplyStatCardTheme(Panel card, Label valueLabel, Label textLabel, Color accentColor)
        {
            card.BackColor = Color.White;
            card.Padding = new Padding(12);
            
            valueLabel.Font = new Font(AppTheme.FontFamily, 24F, FontStyle.Bold);
            valueLabel.ForeColor = accentColor;
            
            textLabel.Font = AppTheme.FontSmall;
            textLabel.ForeColor = AppTheme.TextSecondary;
        }

        private void Report_Load(object sender, EventArgs e)
        {
            InitializeChartTypes();
            LoadDashboardStats();
            LoadStatisticsBySubject();
            LoadTimelineChart(7);
        }

        private void InitializeChartTypes()
        {
            cboChartType.Items.Clear();
            cboChartType.Items.Add("Cột dọc (Column)");
            cboChartType.Items.Add("Cột ngang (Bar)");
            cboChartType.Items.Add("Tròn (Pie)");
            cboChartType.Items.Add("Đường (Line)");
            cboChartType.Items.Add("Vùng (Area)");
            cboChartType.SelectedIndex = 0;
        }

        private void LoadDashboardStats()
        {
            try
            {
                var stats = DatabaseHelper.GetDashboardStatistics();
                
                lblStatValue1.Text = stats.TotalDocuments.ToString();
                lblStatValue2.Text = stats.ImportantDocuments.ToString();
                lblStatValue3.Text = stats.DeletedDocuments.ToString();
                lblStatValue4.Text = stats.TotalCategories.ToString();
                lblStatValue5.Text = stats.NoFileDocuments.ToString();
                lblStatValue6.Text = stats.TotalCollections.ToString();
                
                lblTotal.Text = $"Tổng: {stats.TotalDocuments} tài liệu";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi tải thống kê: " + ex.Message);
            }
        }

        private void BtnBySubjectClick(object sender, EventArgs e)
        {
            LoadStatisticsBySubject();
        }

        private void BtnByTypeClick(object sender, EventArgs e)
        {
            LoadStatisticsByType();
        }

        private void BtnByTimelineClick(object sender, EventArgs e)
        {
            LoadTimelineChart(7);
            lblTimelineTitle.Text = "Tài liệu thêm 7 ngày qua";
        }

        private void BtnByMonthClick(object sender, EventArgs e)
        {
            LoadMonthlyChart(12);
            lblTimelineTitle.Text = "Tài liệu thêm 12 tháng qua";
        }

        private void CboChartTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentStatType == "subject")
            {
                LoadStatisticsBySubject();
            }
            else if (_currentStatType == "type")
            {
                LoadStatisticsByType();
            }
        }

        private void LoadStatisticsBySubject()
        {
            try
            {
                _currentStatType = "subject";
                
                DataTable dt = DatabaseHelper.GetStatisticsBySubject();

                if (dt.Rows.Count == 0)
                {
                    chart.Series.Clear();
                    chart.ChartAreas.Clear();
                    chart.Titles.Clear();
                    chart.Legends.Clear();
                    ToastNotification.Info("Không có dữ liệu để hiển thị thống kê!");
                    return;
                }

                int total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToInt32(row["so_luong"]);
                }
                lblTotal.Text = $"Tổng: {total} tài liệu";

                DrawChart(dt, "Danh mục", "Số lượng tài liệu theo danh mục");

                lblStatus.Text = "Đã tải thống kê theo danh mục";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        private void LoadStatisticsByType()
        {
            try
            {
                _currentStatType = "type";

                DataTable dt = DatabaseHelper.GetStatisticsByType();

                if (dt.Rows.Count == 0)
                {
                    chart.Series.Clear();
                    chart.ChartAreas.Clear();
                    chart.Titles.Clear();
                    chart.Legends.Clear();
                    ToastNotification.Info("Không có dữ liệu để hiển thị thống kê!");
                    return;
                }

                int total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToInt32(row["so_luong"]);
                }
                lblTotal.Text = $"Tổng: {total} tài liệu";

                DrawChart(dt, "Định dạng", "Số lượng tài liệu theo định dạng");
                
                lblStatus.Text = "Đã tải thống kê theo định dạng";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi: " + ex.Message);
            }
        }

        private void LoadTimelineChart(int days)
        {
            try
            {
                _currentTimelineDays = days;
                DataTable dt = DatabaseHelper.GetDocumentsByDay(days);

                if (dt.Rows.Count == 0)
                {
                    chartTimeline.Series.Clear();
                    chartTimeline.ChartAreas.Clear();
                    chartTimeline.Titles.Clear();
                    chartTimeline.Legends.Clear();
                    ToastNotification.Info("Không có dữ liệu timeline!");
                    return;
                }

                DrawTimelineChart(dt, "ngay_format");
                lblStatus.Text = $"Đã tải biểu đồ {days} ngày";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi tải timeline: " + ex.Message);
            }
        }

        private void LoadMonthlyChart(int months)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetDocumentsByMonth(months);

                if (dt.Rows.Count == 0)
                {
                    chartTimeline.Series.Clear();
                    chartTimeline.ChartAreas.Clear();
                    chartTimeline.Titles.Clear();
                    chartTimeline.Legends.Clear();
                    ToastNotification.Info("Không có dữ liệu theo tháng!");
                    return;
                }

                DrawTimelineChart(dt, "thang_format");
                lblStatus.Text = $"Đã tải biểu đồ {months} tháng";
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi tải biểu đồ tháng: " + ex.Message);
            }
        }

        private void DrawChart(DataTable dt, string xAxisTitle, string chartTitle)
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Titles.Clear();

            ChartArea chartArea = new ChartArea("MainArea") { BackColor = Color.White };
            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisY.Title = "Số lượng";
            chartArea.AxisX.LabelStyle.Font = new Font(AppTheme.FontFamily, 9F);
            chartArea.AxisY.LabelStyle.Font = new Font(AppTheme.FontFamily, 9F);
            chartArea.AxisX.TitleFont = new Font(AppTheme.FontFamily, 10F, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font(AppTheme.FontFamily, 10F, FontStyle.Bold);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            chartArea.AxisX.LineColor = AppTheme.BorderLight;
            chartArea.AxisY.LineColor = AppTheme.BorderLight;
            
            chart.ChartAreas.Add(chartArea);

            Title title = new Title(chartTitle)
            {
                Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold),
                ForeColor = AppTheme.Primary
            };
            chart.Titles.Add(title);

            chart.Legends.Clear();
            Legend legend = new Legend("Legend")
            {
                Docking = Docking.Bottom,
                Font = new Font(AppTheme.FontFamily, 9F),
                BackColor = Color.Transparent
            };
            chart.Legends.Add(legend);

            SeriesChartType chartType = GetSelectedChartType();

            Series series = new Series("Số lượng")
            {
                ChartType = chartType,
                ChartArea = "MainArea",
                Legend = "Legend",
                Font = new Font(AppTheme.FontFamily, 9F, FontStyle.Bold)
            };
            
            Color[] colors = {
                AppTheme.Primary, AppTheme.Success, AppTheme.PrimaryLight,
                Color.FromArgb(16, 185, 129), Color.FromArgb(45, 212, 191),
                Color.FromArgb(52, 211, 153), Color.FromArgb(94, 234, 212),
                Color.FromArgb(110, 231, 183)
            };

            int colorIndex = 0;
            string categoryColumn = _currentStatType == "subject" ? "danh_muc" : "dinh_dang";
            
            foreach (DataRow row in dt.Rows)
            {
                string category = row[categoryColumn]?.ToString() ?? "Không xác định";
                int value = Convert.ToInt32(row["so_luong"]);
                
                DataPoint point = new DataPoint();
                point.SetValueXY(category, value);
                point.Color = colors[colorIndex % colors.Length];
                point.Label = value.ToString();
                point.Font = new Font(AppTheme.FontFamily, 9F, FontStyle.Bold);
                point.ToolTip = $"{category}: {value} tài liệu";
                
                series.Points.Add(point);
                colorIndex++;
            }

            if (chartType == SeriesChartType.Pie)
            {
                series["PieLabelStyle"] = "Outside";
                series.Label = "#PERCENT{P1}";
                chartArea.AxisX.Title = "";
                chartArea.AxisY.Title = "";
            }
            else
            {
                series.IsValueShownAsLabel = true;
            }

            chart.Series.Add(series);
        }

        private void DrawTimelineChart(DataTable dt, string dateColumn)
        {
            chartTimeline.Series.Clear();
            chartTimeline.ChartAreas.Clear();
            chartTimeline.Titles.Clear();
            chartTimeline.Legends.Clear();

            ChartArea chartArea = new ChartArea("TimelineArea") { BackColor = Color.White };
            chartArea.AxisX.Title = "";
            chartArea.AxisY.Title = "Số lượng";
            chartArea.AxisX.LabelStyle.Font = new Font(AppTheme.FontFamily, 8F);
            chartArea.AxisY.LabelStyle.Font = new Font(AppTheme.FontFamily, 8F);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            chartArea.AxisX.LineColor = AppTheme.BorderLight;
            chartArea.AxisY.LineColor = AppTheme.BorderLight;
            chartArea.AxisX.Interval = 1;
            
            chartTimeline.ChartAreas.Add(chartArea);

            Series series = new Series("Số lượng")
            {
                ChartType = SeriesChartType.Area,
                ChartArea = "TimelineArea",
                Color = Color.FromArgb(100, AppTheme.Primary),
                BorderColor = AppTheme.Primary,
                BorderWidth = 2,
                Font = new Font(AppTheme.FontFamily, 8F, FontStyle.Bold)
            };

            foreach (DataRow row in dt.Rows)
            {
                string dateLabel = row[dateColumn]?.ToString() ?? "";
                int value = Convert.ToInt32(row["so_luong"]);
                
                DataPoint point = new DataPoint();
                point.SetValueXY(dateLabel, value);
                point.ToolTip = $"{dateLabel}: {value} tài liệu";
                if (value > 0)
                {
                    point.Label = value.ToString();
                }
                
                series.Points.Add(point);
            }

            series.IsValueShownAsLabel = false;
            chartTimeline.Series.Add(series);
        }

        private SeriesChartType GetSelectedChartType()
        {
            switch (cboChartType.SelectedIndex)
            {
                case 0: return SeriesChartType.Column;
                case 1: return SeriesChartType.Bar;
                case 2: return SeriesChartType.Pie;
                case 3: return SeriesChartType.Line;
                case 4: return SeriesChartType.Area;
                default: return SeriesChartType.Column;
            }
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
