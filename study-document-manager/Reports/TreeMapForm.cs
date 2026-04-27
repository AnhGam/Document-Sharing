using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using study_document_manager.UI;
using study_document_manager.UI.Controls;

namespace study_document_manager
{
    public class TreeMapForm : Form
    {
        private TreeMapPanel treeMapPanel;
        private Panel pnlHeader;
        private Panel pnlToolbar;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnBySubject;
        private Button btnByType;
        private Button btnClose;
        private Panel pnlLegend;
        private string _currentMode = "subject";

        private static readonly Color[] Palette = new Color[]
        {
            Color.FromArgb(0, 103, 192),        // Win11 Blue (Primary)
            Color.FromArgb(22, 163, 74),        // Green-600
            Color.FromArgb(124, 58, 237),       // Violet-600
            Color.FromArgb(234, 88, 12),        // Orange-600
            Color.FromArgb(190, 18, 60),        // Rose-700
            Color.FromArgb(14, 165, 233),       // Sky-500
            Color.FromArgb(217, 119, 6),        // Amber-600
            Color.FromArgb(168, 85, 247),       // Purple-500
            Color.FromArgb(20, 184, 166),       // Teal-500
            Color.FromArgb(244, 63, 94),        // Rose-500
            Color.FromArgb(79, 70, 229),        // Indigo-600
            Color.FromArgb(5, 150, 105),        // Emerald-600
            Color.FromArgb(236, 72, 153),       // Pink-500
            Color.FromArgb(107, 114, 128),      // Grey-500
            Color.FromArgb(202, 138, 4),        // Yellow-600
            Color.FromArgb(0, 84, 166),         // Blue-800
        };

        public TreeMapForm()
        {
            InitializeForm();
            LoadData("subject");
        }

        private void InitializeForm()
        {
            Text = "TreeMap - Phân bố tài liệu";
            Size = new Size(900, 620);
            MinimumSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.BackgroundMain;
            Font = new Font(AppTheme.FontFamily, 9F);
            FormBorderStyle = FormBorderStyle.Sizable;
            ShowInTaskbar = false;

            // Header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = AppTheme.PrimaryDark,
                Padding = new Padding(20, 0, 20, 0)
            };

            lblTitle = new Label
            {
                Text = "TreeMap",
                Font = new Font(AppTheme.FontFamily, 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 12)
            };

            lblSubtitle = new Label
            {
                Text = "Phân bố tài liệu theo danh mục",
                Font = new Font(AppTheme.FontFamily, 9.5F),
                ForeColor = Color.FromArgb(180, 226, 232, 240),
                AutoSize = true,
                Location = new Point(22, 42)
            };

            btnClose = new Button
            {
                Text = "Đóng",
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 255, 255, 255),
                ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 9F),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Close();

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle, btnClose });
            pnlHeader.Resize += (s, e) =>
            {
                btnClose.Location = new Point(pnlHeader.Width - btnClose.Width - 20, 18);
            };

            // Toolbar
            pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = AppTheme.BackgroundCard,
                Padding = new Padding(16, 10, 16, 10)
            };
            pnlToolbar.Paint += (s, e) =>
            {
                using (var pen = new Pen(AppTheme.BorderLight))
                    e.Graphics.DrawLine(pen, 0, pnlToolbar.Height - 1,
                        pnlToolbar.Width, pnlToolbar.Height - 1);
            };

            var lblMode = new Label
            {
                Text = "Hiển thị theo:",
                Font = new Font(AppTheme.FontFamily, 9F),
                ForeColor = AppTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(16, 16)
            };

            btnBySubject = CreateToolButton("Danh mục", 110, true);
            btnBySubject.Click += (s, e) => LoadData("subject");

            btnByType = CreateToolButton("Định dạng", 200, false);
            btnByType.Click += (s, e) => LoadData("type");

            pnlToolbar.Controls.AddRange(new Control[] { lblMode, btnBySubject, btnByType });

            // Legend panel (bottom)
            pnlLegend = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = AppTheme.BackgroundCard,
                Padding = new Padding(16, 8, 16, 8)
            };
            pnlLegend.Paint += (s, e) =>
            {
                using (var pen = new Pen(AppTheme.BorderLight))
                    e.Graphics.DrawLine(pen, 0, 0, pnlLegend.Width, 0);
            };

            // TreeMap panel (fill)
            treeMapPanel = new TreeMapPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            // Add in correct order (bottom-up for Dock)
            Controls.Add(treeMapPanel);
            Controls.Add(pnlLegend);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlHeader);
        }

        private Button CreateToolButton(string text, int x, bool active)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(80, 30),
                Location = new Point(x, 11),
                FlatStyle = FlatStyle.Flat,
                Font = new Font(AppTheme.FontFamily, 8.5F),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 1;

            if (active)
            {
                btn.BackColor = AppTheme.PrimaryDark;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = AppTheme.PrimaryDark;
            }
            else
            {
                btn.BackColor = AppTheme.BackgroundCard;
                btn.ForeColor = AppTheme.TextPrimary;
                btn.FlatAppearance.BorderColor = AppTheme.BorderMedium;
            }

            return btn;
        }

        private void SetActiveButton(Button active)
        {
            foreach (var btn in new[] { btnBySubject, btnByType })
            {
                if (btn == active)
                {
                    btn.BackColor = AppTheme.PrimaryDark;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = AppTheme.PrimaryDark;
                }
                else
                {
                    btn.BackColor = Color.White;
                    btn.ForeColor = AppTheme.TextPrimary;
                    btn.FlatAppearance.BorderColor = AppTheme.BorderMedium;
                }
            }
        }

        private void LoadData(string mode)
        {
            _currentMode = mode;

            try
            {
                DataTable dt;
                string categoryColumn;

                if (mode == "subject")
                {
                    dt = DatabaseHelper.GetStatisticsBySubject();
                    categoryColumn = "danh_muc";
                    lblSubtitle.Text = "Phân bố tài liệu theo danh mục";
                    SetActiveButton(btnBySubject);
                }
                else
                {
                    dt = DatabaseHelper.GetStatisticsByType();
                    categoryColumn = "dinh_dang";
                    lblSubtitle.Text = "Phân bố tài liệu theo định dạng";
                    SetActiveButton(btnByType);
                }

                var items = new List<TreeMapItem>();
                int colorIndex = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string label = row[categoryColumn]?.ToString() ?? "Không xác định";
                    int count = Convert.ToInt32(row["so_luong"]);

                    items.Add(new TreeMapItem
                    {
                        Label = label,
                        Value = count,
                        Color = Palette[colorIndex % Palette.Length]
                    });
                    colorIndex++;
                }

                treeMapPanel.SetData(items);
                BuildLegend(items);
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Lỗi tải dữ liệu TreeMap: " + ex.Message);
            }
        }

        private void BuildLegend(List<TreeMapItem> items)
        {
            pnlLegend.Controls.Clear();

            int x = 16;
            int y = 10;
            int totalValue = 0;
            foreach (var item in items) totalValue += (int)item.Value;

            foreach (var item in items)
            {
                string text = $"{item.Label} ({(int)item.Value})";

                var legendItem = new Panel { Size = new Size(14, 14), Location = new Point(x, y + 2) };
                legendItem.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var brush = new SolidBrush(item.Color))
                    {
                        var path = new GraphicsPath();
                        path.AddArc(0, 0, 6, 6, 180, 90);
                        path.AddArc(8, 0, 6, 6, 270, 90);
                        path.AddArc(8, 8, 6, 6, 0, 90);
                        path.AddArc(0, 8, 6, 6, 90, 90);
                        path.CloseFigure();
                        e.Graphics.FillPath(brush, path);
                        path.Dispose();
                    }
                };

                var lbl = new Label
                {
                    Text = text,
                    Font = new Font(AppTheme.FontFamily, 8F),
                    ForeColor = AppTheme.TextSecondary,
                    AutoSize = true,
                    Location = new Point(x + 18, y)
                };

                pnlLegend.Controls.Add(legendItem);
                pnlLegend.Controls.Add(lbl);

                var textSize = TextRenderer.MeasureText(text, lbl.Font);
                x += 18 + textSize.Width + 16;

                if (x > pnlLegend.Width - 100 && items.IndexOf(item) < items.Count - 1)
                {
                    x = 16;
                    y += 22;
                    if (pnlLegend.Height < y + 22)
                    {
                        pnlLegend.Height = y + 26;
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_currentMode != null && pnlLegend != null)
            {
                LoadData(_currentMode);
            }
        }
    }
}
