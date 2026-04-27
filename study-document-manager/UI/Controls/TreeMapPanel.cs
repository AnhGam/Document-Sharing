using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager.UI.Controls
{
    public class TreeMapItem
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public Color Color { get; set; }
        public RectangleF Bounds { get; internal set; }
    }

    public class TreeMapPanel : Panel
    {
        private List<TreeMapItem> _items = new List<TreeMapItem>();
        private TreeMapItem _hoveredItem;
        private TreeMapItem _selectedItem;
        private ToolTip _tooltip;
        private const int CellPadding = 3;
        private const int CornerRadius = 8;
        private double _totalValue;

        public event EventHandler<TreeMapItem> ItemClicked;

        public TreeMapPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            BackColor = AppTheme.BackgroundMain;
            _tooltip = new ToolTip
            {
                InitialDelay = 300,
                ReshowDelay = 100,
                AutoPopDelay = 5000,
                BackColor = AppTheme.PrimaryDark,
                ForeColor = Color.White,
                OwnerDraw = true
            };
            _tooltip.Draw += Tooltip_Draw;
            _tooltip.Popup += Tooltip_Popup;
        }

        public void SetData(List<TreeMapItem> items)
        {
            _items = items ?? new List<TreeMapItem>();
            _totalValue = _items.Sum(i => i.Value);
            _hoveredItem = null;
            _selectedItem = null;
            LayoutTreeMap();
            Invalidate();
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            LayoutTreeMap();
            Invalidate();
        }

        private void LayoutTreeMap()
        {
            if (_items.Count == 0 || Width < 10 || Height < 10) return;

            var sorted = _items.OrderByDescending(i => i.Value).ToList();
            var rect = new RectangleF(CellPadding, CellPadding,
                Width - CellPadding * 2, Height - CellPadding * 2);

            Squarify(sorted, new List<TreeMapItem>(), rect);
        }

        // Squarified Treemap Algorithm
        private void Squarify(List<TreeMapItem> remaining, List<TreeMapItem> currentRow, RectangleF bounds)
        {
            if (remaining.Count == 0)
            {
                LayoutRow(currentRow, bounds);
                return;
            }

            if (currentRow.Count == 0)
            {
                currentRow.Add(remaining[0]);
                Squarify(remaining.Skip(1).ToList(), currentRow, bounds);
                return;
            }

            var withNext = new List<TreeMapItem>(currentRow) { remaining[0] };

            if (WorstAspectRatio(currentRow, bounds) >= WorstAspectRatio(withNext, bounds))
            {
                Squarify(remaining.Skip(1).ToList(), withNext, bounds);
            }
            else
            {
                var remainingBounds = LayoutRow(currentRow, bounds);
                Squarify(remaining, new List<TreeMapItem>(), remainingBounds);
            }
        }

        private double WorstAspectRatio(List<TreeMapItem> row, RectangleF bounds)
        {
            double totalArea = bounds.Width * bounds.Height;
            double rowSum = row.Sum(i => i.Value);
            double areaFraction = (_totalValue > 0) ? rowSum / _totalValue : 0;
            double rowArea = totalArea * areaFraction;

            bool horizontal = bounds.Width >= bounds.Height;
            double sideLength = horizontal ? bounds.Height : bounds.Width;

            if (sideLength <= 0 || rowSum <= 0) return double.MaxValue;

            double stripWidth = rowArea / sideLength;

            double worst = 0;
            foreach (var item in row)
            {
                double itemFraction = item.Value / rowSum;
                double itemLength = sideLength * itemFraction;
                double ratio = Math.Max(stripWidth / itemLength, itemLength / stripWidth);
                worst = Math.Max(worst, ratio);
            }
            return worst;
        }

        private RectangleF LayoutRow(List<TreeMapItem> row, RectangleF bounds)
        {
            if (row.Count == 0) return bounds;

            double totalArea = bounds.Width * bounds.Height;
            double rowSum = row.Sum(i => i.Value);
            double areaFraction = (_totalValue > 0) ? rowSum / _totalValue : 0;
            double rowArea = totalArea * areaFraction;

            bool horizontal = bounds.Width >= bounds.Height;
            double sideLength = horizontal ? bounds.Height : bounds.Width;

            if (sideLength <= 0 || rowSum <= 0) return bounds;

            double stripWidth = (float)(rowArea / sideLength);

            float offset = 0;
            foreach (var item in row)
            {
                double itemFraction = item.Value / rowSum;
                float itemLength = (float)(sideLength * itemFraction);

                if (horizontal)
                {
                    item.Bounds = new RectangleF(
                        bounds.X, bounds.Y + offset,
                        (float)stripWidth, itemLength);
                }
                else
                {
                    item.Bounds = new RectangleF(
                        bounds.X + offset, bounds.Y,
                        itemLength, (float)stripWidth);
                }
                offset += itemLength;
            }

            if (horizontal)
            {
                return new RectangleF(
                    bounds.X + (float)stripWidth, bounds.Y,
                    bounds.Width - (float)stripWidth, bounds.Height);
            }
            else
            {
                return new RectangleF(
                    bounds.X, bounds.Y + (float)stripWidth,
                    bounds.Width, bounds.Height - (float)stripWidth);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_items.Count == 0)
            {
                using (var brush = new SolidBrush(AppTheme.TextMuted))
                using (var font = new Font(AppTheme.FontFamily, 11F))
                {
                    var text = "Không có dữ liệu để hiển thị TreeMap";
                    var size = g.MeasureString(text, font);
                    g.DrawString(text, font,brush,
                        (Width - size.Width) / 2, (Height - size.Height) / 2);
                }
                return;
            }

            foreach (var item in _items)
            {
                DrawCell(g, item);
            }
        }

        private void DrawCell(Graphics g, TreeMapItem item)
        {
            var cellRect = item.Bounds;
            if (cellRect.Width < 2 || cellRect.Height < 2) return;

            // Inset for gap between cells
            var inset = new RectangleF(
                cellRect.X + 2, cellRect.Y + 2,
                cellRect.Width - 4, cellRect.Height - 4);

            if (inset.Width < 1 || inset.Height < 1) return;

            var path = CreateRoundedRect(inset, CornerRadius);

            // Fill
            using (var brush = new LinearGradientBrush(inset, item.Color,
                DarkenColor(item.Color, 0.15f), LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(brush, path);
            }

            // Hover overlay
            if (item == _hoveredItem)
            {
                using (var hoverBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                {
                    g.FillPath(hoverBrush, path);
                }
                using (var pen = new Pen(Color.FromArgb(180, 255, 255, 255), 2f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Selected border
            if (item == _selectedItem)
            {
                using (var pen = new Pen(Color.White, 3f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Label
            DrawLabel(g, item, inset);

            path.Dispose();
        }

        private void DrawLabel(Graphics g, TreeMapItem item, RectangleF rect)
        {
            if (rect.Width < 30 || rect.Height < 20) return;

            var padding = new RectangleF(rect.X + 8, rect.Y + 6,
                rect.Width - 16, rect.Height - 12);

            if (padding.Width < 10 || padding.Height < 10) return;

            bool isLarge = rect.Width > 100 && rect.Height > 50;
            bool isMedium = rect.Width > 60 && rect.Height > 35;

            using (var whiteBrush = new SolidBrush(Color.White))
            using (var semiWhite = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                if (isLarge)
                {
                    using (var titleFont = new Font(AppTheme.FontFamily, 10F))
                    using (var valueFont = new Font(AppTheme.FontFamily, 20F, FontStyle.Bold))
                    using (var pctFont = new Font(AppTheme.FontFamily, 9F))
                    {
                        // Title at top
                        var titleSize = g.MeasureString(item.Label, titleFont, (int)padding.Width);
                        g.DrawString(item.Label, titleFont, semiWhite, padding,
                            new StringFormat { Trimming = StringTrimming.EllipsisCharacter });

                        // Value in center
                        string valueStr = ((int)item.Value).ToString();
                        var valueSize = g.MeasureString(valueStr, valueFont);
                        float valueY = padding.Y + titleSize.Height + 4;
                        if (valueY + valueSize.Height < padding.Bottom)
                        {
                            g.DrawString(valueStr, valueFont, whiteBrush, padding.X, valueY);

                            // Percentage below value
                            double pct = (_totalValue > 0) ? item.Value / _totalValue * 100 : 0;
                            string pctStr = pct.ToString("F1") + "%";
                            float pctY = valueY + valueSize.Height - 2;
                            if (pctY + 16 < padding.Bottom)
                            {
                                g.DrawString(pctStr, pctFont, semiWhite, padding.X, pctY);
                            }
                        }
                    }
                }
                else if (isMedium)
                {
                    using (var titleFont = new Font(AppTheme.FontFamily, 8.5F))
                    using (var valueFont = new Font(AppTheme.FontFamily, 13F, FontStyle.Bold))
                    {
                        g.DrawString(item.Label, titleFont, semiWhite, padding,
                            new StringFormat { Trimming = StringTrimming.EllipsisCharacter });

                        var titleSize = g.MeasureString(item.Label, titleFont, (int)padding.Width);
                        string valueStr = ((int)item.Value).ToString();
                        float valueY = padding.Y + titleSize.Height + 2;
                        if (valueY + 18 < padding.Bottom)
                        {
                            g.DrawString(valueStr, valueFont, whiteBrush, padding.X, valueY);
                        }
                    }
                }
                else
                {
                    using (var smallFont = new Font(AppTheme.FontFamily, 7.5F))
                    {
                        string shortLabel = ((int)item.Value).ToString();
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };
                        g.DrawString(shortLabel, smallFont, whiteBrush, padding, sf);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var hit = HitTest(e.Location);
            if (hit != _hoveredItem)
            {
                _hoveredItem = hit;
                Invalidate();

                if (hit != null)
                {
                    double pct = (_totalValue > 0) ? hit.Value / _totalValue * 100 : 0;
                    _tooltip.SetToolTip(this,
                        $"{hit.Label}\n{(int)hit.Value} tài liệu ({pct:F1}%)");
                }
                else
                {
                    _tooltip.SetToolTip(this, null);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredItem != null)
            {
                _hoveredItem = null;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            var hit = HitTest(e.Location);
            if (hit != null)
            {
                _selectedItem = (_selectedItem == hit) ? null : hit;
                Invalidate();
                ItemClicked?.Invoke(this, hit);
            }
        }

        private TreeMapItem HitTest(Point pt)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i].Bounds.Contains(pt))
                    return _items[i];
            }
            return null;
        }

        private void Tooltip_Popup(object sender, PopupEventArgs e)
        {
            var text = _tooltip.GetToolTip(this);
            if (string.IsNullOrEmpty(text)) return;
            using (var font = new Font(AppTheme.FontFamily, 9F))
            {
                var size = TextRenderer.MeasureText(text, font);
                e.ToolTipSize = new Size(size.Width + 20, size.Height + 12);
            }
        }

        private void Tooltip_Draw(object sender, DrawToolTipEventArgs e)
        {
            using (var bgBrush = new SolidBrush(AppTheme.PrimaryDark))
            using (var font = new Font(AppTheme.FontFamily, 9F))
            using (var textBrush = new SolidBrush(Color.White))
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var path = CreateRoundedRect(new RectangleF(0, 0,
                    e.Bounds.Width, e.Bounds.Height), 6);
                g.FillPath(bgBrush, path);

                g.DrawString(e.ToolTipText, font, textBrush, 10, 6);
                path.Dispose();
            }
        }

        private static GraphicsPath CreateRoundedRect(RectangleF rect, int radius)
        {
            var path = new GraphicsPath();
            float d = radius * 2f;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color DarkenColor(Color color, float amount)
        {
            int r = Math.Max(0, (int)(color.R * (1 - amount)));
            int g = Math.Max(0, (int)(color.G * (1 - amount)));
            int b = Math.Max(0, (int)(color.B * (1 - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tooltip?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

