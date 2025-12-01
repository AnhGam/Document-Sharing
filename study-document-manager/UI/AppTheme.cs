using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace study_document_manager.UI
{
    public static class AppTheme
    {
        public static readonly Color PrimaryDark = Color.FromArgb(13, 148, 136);
        public static readonly Color Primary = Color.FromArgb(20, 184, 166);
        public static readonly Color PrimaryLight = Color.FromArgb(45, 212, 191);
        public static readonly Color PrimaryLighter = Color.FromArgb(204, 251, 241);

        public static readonly Color Secondary = Color.FromArgb(16, 185, 129);
        public static readonly Color SecondaryLight = Color.FromArgb(52, 211, 153);

        public static readonly Color AccentOrange = Color.FromArgb(251, 146, 60);
        public static readonly Color AccentRose = Color.FromArgb(244, 63, 94);
        public static readonly Color AccentAmber = Color.FromArgb(245, 158, 11);
        public static readonly Color AccentSky = Color.FromArgb(14, 165, 233);

        public static readonly Color BackgroundMain = Color.FromArgb(255, 255, 255);
        public static readonly Color BackgroundSoft = Color.FromArgb(248, 250, 252);
        public static readonly Color BackgroundWarm = Color.FromArgb(254, 252, 248);
        public static readonly Color BackgroundCard = Color.FromArgb(255, 255, 255);

        public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
        public static readonly Color TextSecondary = Color.FromArgb(71, 85, 105);
        public static readonly Color TextMuted = Color.FromArgb(148, 163, 184);
        public static readonly Color TextWhite = Color.FromArgb(255, 255, 255);

        public static readonly Color BorderLight = Color.FromArgb(226, 232, 240);
        public static readonly Color BorderMedium = Color.FromArgb(203, 213, 225);
        public static readonly Color BorderFocus = Color.FromArgb(20, 184, 166);

        public static readonly Color StatusSuccess = Color.FromArgb(34, 197, 94);
        public static readonly Color StatusWarning = Color.FromArgb(234, 179, 8);
        public static readonly Color StatusError = Color.FromArgb(239, 68, 68);
        public static readonly Color StatusInfo = Color.FromArgb(59, 130, 246);

        public static readonly Color InputBackground = Color.FromArgb(255, 255, 255);
        public static readonly Color InputBorder = Color.FromArgb(203, 213, 225);
        public static readonly Color InputBorderFocus = Color.FromArgb(20, 184, 166);
        public static readonly Color InputBorderHover = Color.FromArgb(148, 163, 184);

        public static readonly Color GridHeaderBg = Color.FromArgb(248, 250, 252);
        public static readonly Color GridHeaderFg = Color.FromArgb(51, 65, 85);
        public static readonly Color GridRowAlt = Color.FromArgb(248, 250, 252);
        public static readonly Color GridRowSelected = Color.FromArgb(204, 251, 241);
        public static readonly Color GridBorder = Color.FromArgb(226, 232, 240);

        public static readonly Font FontTitle = new Font("Segoe UI", 20F, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 14F, FontStyle.Regular);
        public static readonly Font FontHeading = new Font("Segoe UI Semibold", 12F);
        public static readonly Font FontBody = new Font("Segoe UI", 10F);
        public static readonly Font FontBodyBold = new Font("Segoe UI Semibold", 10F);
        public static readonly Font FontSmall = new Font("Segoe UI", 9F);
        public static readonly Font FontSmallBold = new Font("Segoe UI Semibold", 9F);
        public static readonly Font FontCaption = new Font("Segoe UI", 8F);
        public static readonly Font FontInput = new Font("Segoe UI", 11F);
        public static readonly Font FontButton = new Font("Segoe UI Semibold", 10F);

        public static readonly int BorderRadius = 12;
        public static readonly int ButtonRadius = 8;
        public static readonly int InputRadius = 8;
        public static readonly int PaddingSmall = 8;
        public static readonly int PaddingMedium = 16;
        public static readonly int PaddingLarge = 24;

        public static void ApplyFormStyle(Form form, bool isMainForm = false)
        {
            form.BackColor = BackgroundMain;
            form.Font = FontBody;
            form.ForeColor = TextPrimary;
            if (isMainForm)
            {
                form.MinimumSize = new Size(1200, 700);
            }
        }

        public static void ApplyButtonPrimary(Button btn)
        {
            btn.BackColor = Primary;
            btn.ForeColor = TextWhite;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = 44;

            btn.MouseEnter += (s, e) => btn.BackColor = PrimaryLight;
            btn.MouseLeave += (s, e) => btn.BackColor = Primary;
        }

        public static void ApplyButtonSuccess(Button btn)
        {
            btn.BackColor = StatusSuccess;
            btn.ForeColor = TextWhite;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = 44;

            btn.MouseEnter += (s, e) => btn.BackColor = SecondaryLight;
            btn.MouseLeave += (s, e) => btn.BackColor = StatusSuccess;
        }

        public static void ApplyButtonDanger(Button btn)
        {
            btn.BackColor = StatusError;
            btn.ForeColor = TextWhite;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = 44;

            Color hoverColor = Color.FromArgb(220, 38, 38);
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = StatusError;
        }

        public static void ApplyButtonSecondary(Button btn)
        {
            btn.BackColor = BackgroundSoft;
            btn.ForeColor = TextSecondary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = BorderMedium;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = 44;

            btn.MouseEnter += (s, e) => { btn.BackColor = BorderLight; btn.FlatAppearance.BorderColor = TextMuted; };
            btn.MouseLeave += (s, e) => { btn.BackColor = BackgroundSoft; btn.FlatAppearance.BorderColor = BorderMedium; };
        }

        public static void ApplyButtonOutline(Button btn)
        {
            btn.BackColor = BackgroundMain;
            btn.ForeColor = Primary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Primary;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = 44;

            btn.MouseEnter += (s, e) => { btn.BackColor = PrimaryLighter; };
            btn.MouseLeave += (s, e) => { btn.BackColor = BackgroundMain; };
        }

        public static void ApplyTextBoxStyle(TextBox txt)
        {
            txt.Font = FontInput;
            txt.BackColor = InputBackground;
            txt.ForeColor = TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void ApplyComboBoxStyle(ComboBox cbo)
        {
            cbo.Font = FontInput;
            cbo.BackColor = InputBackground;
            cbo.ForeColor = TextPrimary;
            cbo.FlatStyle = FlatStyle.Flat;
        }

        public static void ApplyDataGridViewStyle(DataGridView dgv)
        {
            dgv.BackgroundColor = BackgroundMain;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = GridBorder;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderBg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = GridHeaderFg;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontSmallBold;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 14, 12, 14);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 48;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgv.DefaultCellStyle.BackColor = BackgroundMain;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = FontSmall;
            dgv.DefaultCellStyle.Padding = new Padding(12, 10, 12, 10);
            dgv.DefaultCellStyle.SelectionBackColor = GridRowSelected;
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridRowAlt;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridRowSelected;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;

            dgv.RowTemplate.Height = 48;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static void ApplyGroupBoxStyle(GroupBox grp)
        {
            grp.Font = FontSmallBold;
            grp.ForeColor = TextPrimary;
            grp.BackColor = BackgroundMain;
        }

        public static void ApplyLabelTitle(Label lbl)
        {
            lbl.Font = FontTitle;
            lbl.ForeColor = TextPrimary;
        }

        public static void ApplyLabelSubtitle(Label lbl)
        {
            lbl.Font = FontSubtitle;
            lbl.ForeColor = TextSecondary;
        }

        public static void ApplyLabelNormal(Label lbl)
        {
            lbl.Font = FontSmall;
            lbl.ForeColor = TextSecondary;
        }

        public static void ApplyStatusStripStyle(StatusStrip status)
        {
            status.BackColor = BackgroundSoft;
            status.ForeColor = TextSecondary;
            status.SizingGrip = false;
        }

        public static void ApplyMenuStripStyle(MenuStrip menu)
        {
            menu.BackColor = BackgroundMain;
            menu.ForeColor = TextPrimary;
            menu.Font = FontSmall;
            menu.Renderer = new ModernMenuRenderer();
        }

        public static void ApplyToolStripStyle(ToolStrip tool)
        {
            tool.BackColor = BackgroundMain;
            tool.GripStyle = ToolStripGripStyle.Hidden;
            tool.Font = FontSmall;
            tool.Padding = new Padding(PaddingSmall, 4, PaddingSmall, 4);
            tool.Renderer = new ModernToolStripRenderer();
        }

        public static void ApplyTabControlStyle(TabControl tab)
        {
            tab.Font = FontSmallBold;
        }

        public static LinearGradientBrush CreateGradientBrush(Rectangle rect, Color startColor, Color endColor, float angle = 135f)
        {
            return new LinearGradientBrush(rect, startColor, endColor, angle);
        }

        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private class ModernMenuRenderer : ToolStripProfessionalRenderer
        {
            public ModernMenuRenderer() : base(new ModernMenuColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected)
                {
                    using (SolidBrush brush = new SolidBrush(PrimaryLighter))
                    {
                        e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                    }
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
        }

        private class ModernMenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => PrimaryLighter;
            public override Color MenuItemBorder => Primary;
            public override Color MenuBorder => BorderLight;
            public override Color ToolStripDropDownBackground => BackgroundMain;
            public override Color ImageMarginGradientBegin => BackgroundSoft;
            public override Color ImageMarginGradientMiddle => BackgroundSoft;
            public override Color ImageMarginGradientEnd => BackgroundSoft;
        }

        private class ModernToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ModernToolStripRenderer() : base(new ModernToolStripColorTable()) { }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected || e.Item.Pressed)
                {
                    using (SolidBrush brush = new SolidBrush(PrimaryLighter))
                    {
                        e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                    }
                }
            }
        }

        private class ModernToolStripColorTable : ProfessionalColorTable
        {
            public override Color ToolStripBorder => BorderLight;
            public override Color ToolStripGradientBegin => BackgroundMain;
            public override Color ToolStripGradientMiddle => BackgroundMain;
            public override Color ToolStripGradientEnd => BackgroundMain;
            public override Color ButtonSelectedHighlight => PrimaryLighter;
            public override Color ButtonSelectedBorder => Primary;
        }
    }

    public class RoundedButton : Button
    {
        private int _borderRadius = 8;
        private Color _borderColor = Color.Transparent;

        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = AppTheme.CreateRoundedRectangle(new Rectangle(0, 0, Width - 1, Height - 1), _borderRadius))
            {
                Region = new Region(path);

                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                if (_borderColor != Color.Transparent)
                {
                    using (Pen pen = new Pen(_borderColor, 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public class RoundedPanel : Panel
    {
        private int _borderRadius = 12;
        private Color _borderColor = Color.Transparent;
        private bool _showShadow = false;

        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        public bool ShowShadow
        {
            get => _showShadow;
            set { _showShadow = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            if (_showShadow)
            {
                for (int i = 1; i <= 4; i++)
                {
                    using (Pen pen = new Pen(Color.FromArgb(10, 0, 0, 0), 1))
                    {
                        e.Graphics.DrawRectangle(pen, rect.X + i, rect.Y + i, rect.Width, rect.Height);
                    }
                }
                rect = new Rectangle(0, 0, Width - 5, Height - 5);
            }

            using (GraphicsPath path = AppTheme.CreateRoundedRectangle(rect, _borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                if (_borderColor != Color.Transparent)
                {
                    using (Pen pen = new Pen(_borderColor, 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }
    }
}
