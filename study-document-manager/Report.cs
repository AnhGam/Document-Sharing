using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace study_document_manager
{
    public partial class Report : Form
    {
        private string currentStatType = "subject"; // "subject" hoặc "type"

        public Report()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load form
        /// </summary>
        private void Report_Load(object sender, EventArgs e)
        {
            InitializeChartTypes();
            LoadStatisticsBySubject();
        }

        /// <summary>
        /// Khởi tạo ComboBox kiểu biểu đồ
        /// </summary>
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

        /// <summary>
        /// Button thống kê theo môn học
        /// </summary>
        private void btnBySubject_Click(object sender, EventArgs e)
        {
            LoadStatisticsBySubject();
        }

        /// <summary>
        /// Button thống kê theo loại
        /// </summary>
        private void btnByType_Click(object sender, EventArgs e)
        {
            LoadStatisticsByType();
        }

        /// <summary>
        /// Thay đổi kiểu biểu đồ
        /// </summary>
        private void cboChartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentStatType == "subject")
            {
                LoadStatisticsBySubject();
            }
            else
            {
                LoadStatisticsByType();
            }
        }

        /// <summary>
        /// Load thống kê theo môn học
        /// </summary>
        private void LoadStatisticsBySubject()
        {
            try
            {
                currentStatType = "subject";
                
                // Lấy dữ liệu từ database
                DataTable dt = DatabaseHelper.GetStatisticsBySubject();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để hiển thị thống kê!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tổng số tài liệu
                int total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToInt32(row["so_luong"]);
                }
                lblTotal.Text = $"Tổng: {total} tài liệu";

                // Vẽ biểu đồ
                DrawChart(dt, "Môn học", "Số lượng tài liệu theo môn học");
                
                lblStatus.Text = "Đã tải thống kê theo môn học";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load thống kê theo loại
        /// </summary>
        private void LoadStatisticsByType()
        {
            try
            {
                currentStatType = "type";
                
                // Lấy dữ liệu từ database
                DataTable dt = DatabaseHelper.GetStatisticsByType();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để hiển thị thống kê!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tổng số tài liệu
                int total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToInt32(row["so_luong"]);
                }
                lblTotal.Text = $"Tổng: {total} tài liệu";

                // Vẽ biểu đồ
                DrawChart(dt, "Loại tài liệu", "Số lượng tài liệu theo loại");
                
                lblStatus.Text = "Đã tải thống kê theo loại tài liệu";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Vẽ biểu đồ
        /// </summary>
        private void DrawChart(DataTable dt, string xAxisTitle, string chartTitle)
        {
            // Clear chart
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Titles.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.BackColor = Color.White;
            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisY.Title = "Số lượng";
            chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisX.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            
            // Grid style
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            
            chart.ChartAreas.Add(chartArea);

            // Tạo Title
            Title title = new Title(chartTitle);
            title.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(52, 73, 94);
            chart.Titles.Add(title);

            // Lấy kiểu biểu đồ từ ComboBox
            SeriesChartType chartType = GetSelectedChartType();

            // Tạo Series
            Series series = new Series("Số lượng");
            series.ChartType = chartType;
            series.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            // Màu sắc đẹp mắt
            Color[] colors = new Color[]
            {
                Color.FromArgb(76, 175, 80),   // Xanh lá
                Color.FromArgb(33, 150, 243),  // Xanh dương
                Color.FromArgb(255, 152, 0),   // Cam
                Color.FromArgb(244, 67, 54),   // Đỏ
                Color.FromArgb(156, 39, 176),  // Tím
                Color.FromArgb(255, 193, 7),   // Vàng
                Color.FromArgb(0, 150, 136),   // Xanh lục
                Color.FromArgb(233, 30, 99)    // Hồng
            };

            // Thêm dữ liệu vào Series
            int colorIndex = 0;
            string categoryColumn = currentStatType == "subject" ? "mon_hoc" : "loai";
            
            foreach (DataRow row in dt.Rows)
            {
                string category = row[categoryColumn]?.ToString() ?? "Không xác định";
                int value = Convert.ToInt32(row["so_luong"]);
                
                DataPoint point = new DataPoint();
                point.SetValueXY(category, value);
                point.Color = colors[colorIndex % colors.Length];
                point.Label = value.ToString();
                point.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                
                // Tooltip
                point.ToolTip = $"{category}: {value} tài liệu";
                
                series.Points.Add(point);
                colorIndex++;
            }

            // Đặc biệt cho biểu đồ tròn
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

            // Legend
            chart.Legends.Clear();
            Legend legend = new Legend("Legend");
            legend.Docking = Docking.Bottom;
            legend.Font = new Font("Segoe UI", 9F);
            legend.BackColor = Color.Transparent;
            chart.Legends.Add(legend);

            // Enable 3D (optional)
            if (chartType == SeriesChartType.Column || chartType == SeriesChartType.Bar)
            {
                chartArea.Area3DStyle.Enable3D = false; // Tắt 3D để modern hơn
            }
        }

        /// <summary>
        /// Lấy kiểu biểu đồ được chọn
        /// </summary>
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

        /// <summary>
        /// Button đóng
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
