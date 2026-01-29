using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace study_document_manager.UI
{
    public static class AppTheme
    {
        #region === PRIMARY PALETTE (Teal) ===
        public static readonly Color PrimaryDark = Color.FromArgb(13, 148, 136);     // Teal-700
        public static readonly Color Primary = Color.FromArgb(20, 184, 166);          // Teal-500
        public static readonly Color PrimaryLight = Color.FromArgb(45, 212, 191);     // Teal-400
        public static readonly Color PrimaryLighter = Color.FromArgb(204, 251, 241);  // Teal-100
        #endregion

        #region === SECONDARY PALETTE (Emerald) ===
        public static readonly Color Secondary = Color.FromArgb(16, 185, 129);        // Emerald-500
        public static readonly Color SecondaryLight = Color.FromArgb(52, 211, 153);   // Emerald-400
        public static readonly Color SecondaryLighter = Color.FromArgb(209, 250, 229);// Emerald-100
        #endregion

        #region === ACCENT COLORS ===
        public static readonly Color AccentOrange = Color.FromArgb(251, 146, 60);     // Orange-400
        public static readonly Color AccentRose = Color.FromArgb(244, 63, 94);        // Rose-500
        public static readonly Color AccentAmber = Color.FromArgb(245, 158, 11);      // Amber-500
        public static readonly Color AccentSky = Color.FromArgb(14, 165, 233);        // Sky-500
        #endregion

        #region === VALIDATION STATES ===
        public static readonly Color ValidationError = Color.FromArgb(239, 68, 68);           // Red-500
        public static readonly Color ValidationErrorLight = Color.FromArgb(254, 226, 226);    // Red-100
        public static readonly Color ValidationErrorBorder = Color.FromArgb(252, 165, 165);   // Red-300
        public static readonly Color ValidationSuccess = Color.FromArgb(34, 197, 94);         // Green-500
        public static readonly Color ValidationSuccessLight = Color.FromArgb(220, 252, 231);  // Green-100
        public static readonly Color ValidationSuccessBorder = Color.FromArgb(134, 239, 172); // Green-300
        public static readonly Color ValidationWarning = Color.FromArgb(245, 158, 11);        // Amber-500
        public static readonly Color ValidationWarningLight = Color.FromArgb(254, 243, 199);  // Amber-100
        public static readonly Color ValidationWarningBorder = Color.FromArgb(252, 211, 77);  // Amber-300
        #endregion

        #region === INTERACTION STATES ===
        public static readonly Color HoverOverlay = Color.FromArgb(20, 0, 0, 0);              // 8% black
        public static readonly Color PressedOverlay = Color.FromArgb(40, 0, 0, 0);            // 16% black
        public static readonly Color FocusRing = Color.FromArgb(80, 20, 184, 166);            // Primary 30%
        public static readonly Color DisabledBg = Color.FromArgb(241, 245, 249);              // Slate-100
        public static readonly Color DisabledText = Color.FromArgb(148, 163, 184);            // Slate-400
        public static readonly Color DisabledBorder = Color.FromArgb(226, 232, 240);          // Slate-200
        #endregion

        #region === SHADOW & GLOW EFFECTS ===
        public static readonly Color ShadowLight = Color.FromArgb(15, 0, 0, 0);               // 6% black
        public static readonly Color ShadowMedium = Color.FromArgb(25, 0, 0, 0);              // 10% black
        public static readonly Color ShadowHeavy = Color.FromArgb(40, 0, 0, 0);               // 16% black
        public static readonly Color GlowPrimary = Color.FromArgb(60, 20, 184, 166);          // Primary glow
        public static readonly Color GlowSuccess = Color.FromArgb(60, 34, 197, 94);           // Success glow
        public static readonly Color GlowDanger = Color.FromArgb(60, 239, 68, 68);            // Danger glow
        public static readonly Color GlowWarning = Color.FromArgb(60, 245, 158, 11);          // Warning glow
        #endregion

        #region === DARK MODE FOUNDATION ===
        public static readonly Color DarkBackground = Color.FromArgb(15, 23, 42);             // Slate-900
        public static readonly Color DarkSurface = Color.FromArgb(30, 41, 59);                // Slate-800
        public static readonly Color DarkSurfaceLight = Color.FromArgb(51, 65, 85);           // Slate-700
        public static readonly Color DarkBorder = Color.FromArgb(71, 85, 105);                // Slate-600
        public static readonly Color DarkTextPrimary = Color.FromArgb(241, 245, 249);         // Slate-100
        public static readonly Color DarkTextSecondary = Color.FromArgb(148, 163, 184);       // Slate-400
        #endregion

        #region === BACKGROUND COLORS ===
        public static readonly Color BackgroundMain = Color.FromArgb(255, 255, 255);
        public static readonly Color BackgroundSoft = Color.FromArgb(248, 250, 252);
        public static readonly Color BackgroundWarm = Color.FromArgb(254, 252, 248);
        public static readonly Color BackgroundCard = Color.FromArgb(255, 255, 255);
        #endregion

        #region === TEXT COLORS ===
        public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
        public static readonly Color TextSecondary = Color.FromArgb(71, 85, 105);
        public static readonly Color TextMuted = Color.FromArgb(148, 163, 184);
        public static readonly Color TextWhite = Color.FromArgb(255, 255, 255);
        #endregion

        #region === BORDER COLORS ===
        public static readonly Color BorderLight = Color.FromArgb(226, 232, 240);
        public static readonly Color BorderMedium = Color.FromArgb(203, 213, 225);
        public static readonly Color BorderFocus = Color.FromArgb(20, 184, 166);
        #endregion

        #region === STATUS COLORS ===
        public static readonly Color StatusSuccess = Color.FromArgb(34, 197, 94);
        public static readonly Color StatusWarning = Color.FromArgb(234, 179, 8);
        public static readonly Color StatusError = Color.FromArgb(239, 68, 68);
        public static readonly Color StatusInfo = Color.FromArgb(59, 130, 246);

        // Shorthand aliases for common colors
        public static readonly Color Success = StatusSuccess;
        public static readonly Color Warning = StatusWarning;
        public static readonly Color Danger = StatusError;
        public static readonly Color Info = StatusInfo;
        #endregion

        #region === INPUT COLORS ===
        public static readonly Color InputBackground = Color.FromArgb(255, 255, 255);
        public static readonly Color InputBorder = Color.FromArgb(203, 213, 225);
        public static readonly Color InputBorderFocus = Color.FromArgb(20, 184, 166);
        public static readonly Color InputBorderHover = Color.FromArgb(148, 163, 184);
        #endregion

        #region === GRID COLORS ===
        public static readonly Color GridHeaderBg = Color.FromArgb(15, 118, 110);  // Teal-700
        public static readonly Color GridHeaderFg = Color.FromArgb(255, 255, 255);
        public static readonly Color GridRowAlt = Color.FromArgb(248, 250, 252);
        public static readonly Color GridRowSelected = Color.FromArgb(204, 251, 241);
        public static readonly Color GridBorder = Color.FromArgb(226, 232, 240);
        #endregion

        #region === TYPOGRAPHY SCALE ===
        public const string FontFamily = "Segoe UI";
        public static readonly Font FontDisplay = new Font(FontFamily, 28F, FontStyle.Bold);      // Hero text
        public static readonly Font FontH1 = new Font(FontFamily, 24F, FontStyle.Bold);           // Page titles
        public static readonly Font FontH2 = new Font(FontFamily, 20F, FontStyle.Bold);           // Section headers
        public static readonly Font FontH3 = new Font(FontFamily + " Semibold", 16F);             // Card titles
        public static readonly Font FontH4 = new Font(FontFamily + " Semibold", 14F);             // Subsections
        public static readonly Font FontTitle = new Font("Segoe UI", 20F, FontStyle.Bold);        // Legacy alias
        public static readonly Font FontSubtitle = new Font("Segoe UI", 14F, FontStyle.Regular);
        public static readonly Font FontHeading = new Font("Segoe UI Semibold", 12F);
        public static readonly Font FontBody = new Font("Segoe UI", 10F);
        public static readonly Font FontBodyLarge = new Font("Segoe UI", 11F);
        public static readonly Font FontBodyBold = new Font("Segoe UI Semibold", 10F);
        public static readonly Font FontSmall = new Font("Segoe UI", 9F);
        public static readonly Font FontSmallBold = new Font("Segoe UI Semibold", 9F);
        public static readonly Font FontCaption = new Font("Segoe UI", 8F);
        public static readonly Font FontLabel = new Font("Segoe UI Semibold", 9F);                // Form labels
        public static readonly Font FontInput = new Font("Segoe UI", 11F);
        public static readonly Font FontButton = new Font("Segoe UI Semibold", 10F);
        public static readonly Font FontCode = new Font("Cascadia Code", 9F);                     // Code/paths
        #endregion

        #region === SPACING SCALE (4px base) ===
        public const int Space0 = 0;
        public const int Space1 = 4;    // xs
        public const int Space2 = 8;    // sm
        public const int Space3 = 12;   // md-sm
        public const int Space4 = 16;   // md
        public const int Space5 = 20;   // md-lg
        public const int Space6 = 24;   // lg
        public const int Space8 = 32;   // xl
        public const int Space10 = 40;  // 2xl
        public const int Space12 = 48;  // 3xl
        public const int Space16 = 64;  // 4xl

        // Legacy aliases
        public static readonly int PaddingSmall = Space2;   // 8px
        public static readonly int PaddingMedium = Space4;  // 16px
        public static readonly int PaddingLarge = Space6;   // 24px
        #endregion

        #region === COMPONENT SIZES ===
        public const int ButtonHeightSmall = 32;
        public const int ButtonHeightMedium = 40;
        public const int ButtonHeightLarge = 48;
        public const int ButtonHeightDefault = 44;
        public const int InputHeight = 44;
        public const int IconSizeSmall = 16;
        public const int IconSizeMedium = 20;
        public const int IconSizeLarge = 24;
        public const int IconSizeXL = 32;
        #endregion

        #region === BORDER RADIUS ===
        public static readonly int BorderRadius = 12;       // Panels, cards
        public static readonly int ButtonRadius = 8;        // Buttons
        public static readonly int InputRadius = 8;         // Inputs
        public static readonly int RadiusSmall = 4;         // Small elements
        public static readonly int RadiusFull = 9999;       // Pill/circular
        #endregion

        #region === ANIMATION TIMING (ms) ===
        public const int AnimationFast = 150;
        public const int AnimationNormal = 250;
        public const int AnimationSlow = 400;
        public const int AnimationXSlow = 600;
        #endregion

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

        public static void ApplyButtonWarning(Button btn)
        {
            btn.BackColor = StatusWarning;
            btn.ForeColor = TextPrimary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = 44;

            Color hoverColor = Color.FromArgb(202, 138, 4);
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = StatusWarning;
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

        #region === ModernButton Overloads ===
        /// <summary>
        /// Apply Primary variant to ModernButton
        /// </summary>
        public static void ApplyButtonPrimary(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Primary;
            btn.BorderRadius = BorderRadius;
            btn.EnableGlow = true;
        }

        /// <summary>
        /// Apply Success variant to ModernButton
        /// </summary>
        public static void ApplyButtonSuccess(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Success;
            btn.BorderRadius = BorderRadius;
            btn.EnableGlow = true;
        }

        /// <summary>
        /// Apply Danger variant to ModernButton
        /// </summary>
        public static void ApplyButtonDanger(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Danger;
            btn.BorderRadius = BorderRadius;
            btn.EnableGlow = true;
        }

        /// <summary>
        /// Apply Warning variant to ModernButton
        /// </summary>
        public static void ApplyButtonWarning(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Warning;
            btn.BorderRadius = BorderRadius;
            btn.EnableGlow = true;
        }

        /// <summary>
        /// Apply Secondary variant to ModernButton
        /// </summary>
        public static void ApplyButtonSecondary(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Secondary;
            btn.BorderRadius = BorderRadius;
            btn.EnableGlow = false;
        }

        /// <summary>
        /// Apply Outline variant to ModernButton
        /// </summary>
        public static void ApplyButtonOutline(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Outline;
            btn.BorderRadius = BorderRadius;
            btn.EnableGlow = true;
        }

        /// <summary>
        /// Apply Ghost variant to ModernButton
        /// </summary>
        public static void ApplyButtonGhost(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Ghost;
            btn.BorderRadius = BorderRadius;
            btn.EnableGlow = false;
        }
        #endregion

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
