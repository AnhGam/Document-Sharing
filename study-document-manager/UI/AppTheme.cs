using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace study_document_manager.UI
{
    public static class AppTheme
    {
        #region === PRIMARY PALETTE (Win11 Accent Blue) ===
        public static readonly Color PrimaryDark = Color.FromArgb(0, 84, 166);        // Blue-800
        public static readonly Color Primary = Color.FromArgb(0, 103, 192);            // Win11 Accent Blue
        public static readonly Color PrimaryLight = Color.FromArgb(96, 165, 250);      // Blue-400
        public static readonly Color PrimaryLighter = Color.FromArgb(219, 234, 254);   // Blue-100
        #endregion

        #region === SECONDARY PALETTE (Neutral Grey) ===
        public static readonly Color Secondary = Color.FromArgb(107, 114, 128);        // Grey-500
        public static readonly Color SecondaryLight = Color.FromArgb(156, 163, 175);   // Grey-400
        public static readonly Color SecondaryLighter = Color.FromArgb(243, 244, 246); // Grey-100
        #endregion

        #region === ACCENT COLORS ===
        public static readonly Color AccentOrange = Color.FromArgb(234, 88, 12);       // Orange-600
        public static readonly Color AccentRose = Color.FromArgb(190, 18, 60);         // Rose-700
        public static readonly Color AccentAmber = Color.FromArgb(180, 120, 0);        // Amber darker
        public static readonly Color AccentSky = Color.FromArgb(0, 103, 192);          // Same as Primary for consistency
        #endregion

        #region === VALIDATION STATES ===
        public static readonly Color ValidationError = Color.FromArgb(197, 40, 40);             // Red-700
        public static readonly Color ValidationErrorLight = Color.FromArgb(254, 226, 226);      // Red-100
        public static readonly Color ValidationErrorBorder = Color.FromArgb(252, 165, 165);     // Red-300
        public static readonly Color ValidationSuccess = Color.FromArgb(22, 163, 74);           // Green-600
        public static readonly Color ValidationSuccessLight = Color.FromArgb(220, 252, 231);    // Green-100
        public static readonly Color ValidationSuccessBorder = Color.FromArgb(134, 239, 172);   // Green-300
        public static readonly Color ValidationWarning = Color.FromArgb(180, 120, 0);           // Amber-700
        public static readonly Color ValidationWarningLight = Color.FromArgb(254, 243, 199);    // Amber-100
        public static readonly Color ValidationWarningBorder = Color.FromArgb(253, 211, 77);    // Amber-300
        #endregion

        #region === INTERACTION STATES ===
        public static readonly Color HoverOverlay = Color.FromArgb(10, 0, 0, 0);                // 4% black
        public static readonly Color PressedOverlay = Color.FromArgb(20, 0, 0, 0);              // 8% black
        public static readonly Color FocusRing = Color.FromArgb(80, 0, 103, 192);               // Primary 30%
        public static readonly Color DisabledBg = Color.FromArgb(243, 244, 246);                // Grey-100
        public static readonly Color DisabledText = Color.FromArgb(156, 163, 175);              // Grey-400
        public static readonly Color DisabledBorder = Color.FromArgb(229, 231, 235);            // Grey-200
        #endregion

        #region === SHADOW EFFECTS ===
        public static readonly Color ShadowLight = Color.FromArgb(8, 0, 0, 0);                  // 3% black
        public static readonly Color ShadowMedium = Color.FromArgb(15, 0, 0, 0);                // 6% black
        public static readonly Color ShadowHeavy = Color.FromArgb(25, 0, 0, 0);                 // 10% black
        #endregion

        #region === BACKGROUND COLORS (Win11 Mica-like layers) ===
        public static readonly Color BackgroundMain = Color.FromArgb(243, 243, 243);   // Win11 Mica base
        public static readonly Color BackgroundSoft = Color.FromArgb(238, 238, 238);   // Slightly darker
        public static readonly Color BackgroundWarm = Color.FromArgb(243, 243, 243);   // Same as Main
        public static readonly Color BackgroundCard = Color.FromArgb(255, 255, 255);   // Pure white card
        #endregion

        #region === TEXT COLORS ===
        public static readonly Color TextPrimary = Color.FromArgb(28, 28, 28);         // Near-black
        public static readonly Color TextSecondary = Color.FromArgb(96, 96, 96);       // Medium grey
        public static readonly Color TextMuted = Color.FromArgb(148, 148, 148);        // Light grey
        public static readonly Color TextWhite = Color.FromArgb(255, 255, 255);
        #endregion

        #region === BORDER COLORS ===
        public static readonly Color BorderLight = Color.FromArgb(229, 229, 229);      // Subtle divider
        public static readonly Color BorderMedium = Color.FromArgb(210, 210, 210);     // Standard border
        public static readonly Color BorderFocus = Color.FromArgb(0, 103, 192);        // Win11 accent focus
        #endregion

        #region === STATUS COLORS ===
        public static readonly Color StatusSuccess = Color.FromArgb(16, 124, 16);      // Win11 green
        public static readonly Color StatusWarning = Color.FromArgb(154, 99, 0);       // Win11 amber
        public static readonly Color StatusError = Color.FromArgb(196, 43, 28);        // Win11 red
        public static readonly Color StatusInfo = Color.FromArgb(0, 103, 192);         // Win11 blue

        // Shorthand aliases
        public static readonly Color Success = StatusSuccess;
        public static readonly Color Warning = StatusWarning;
        public static readonly Color Danger = StatusError;
        public static readonly Color Info = StatusInfo;
        #endregion

        #region === INPUT COLORS ===
        public static readonly Color InputBackground = Color.FromArgb(255, 255, 255);
        public static readonly Color InputBorder = Color.FromArgb(210, 210, 210);
        public static readonly Color InputBorderFocus = Color.FromArgb(0, 103, 192);   // Win11 accent
        public static readonly Color InputBorderHover = Color.FromArgb(148, 148, 148);
        #endregion

        #region === GRID COLORS ===
        public static readonly Color GridHeaderBg = Color.FromArgb(243, 243, 243);     // Match Mica background
        public static readonly Color GridHeaderFg = Color.FromArgb(28, 28, 28);        // Dark text
        public static readonly Color GridRowAlt = Color.FromArgb(249, 249, 249);       // Very subtle alt row
        public static readonly Color GridRowSelected = Color.FromArgb(204, 228, 247);  // Win11 blue selection
        public static readonly Color GridBorder = Color.FromArgb(229, 229, 229);
        #endregion

        #region === TYPOGRAPHY SCALE ===
        public const string FontFamily = "Segoe UI Variable";
        public const string FontFamilyFallback = "Segoe UI";
        public static readonly Font FontDisplay = CreateFont(26F, FontStyle.Bold);
        public static readonly Font FontH1 = CreateFont(22F, FontStyle.Bold);
        public static readonly Font FontH2 = CreateFont(18F, FontStyle.Bold);
        public static readonly Font FontH3 = CreateFont(15F, FontStyle.Regular);
        public static readonly Font FontH4 = CreateFont(13F, FontStyle.Regular);
        public static readonly Font FontTitle = CreateFont(18F, FontStyle.Bold);
        public static readonly Font FontSubtitle = CreateFont(13F, FontStyle.Regular);
        public static readonly Font FontHeading = CreateFont(11F, FontStyle.Regular);
        public static readonly Font FontBody = CreateFont(10F, FontStyle.Regular);
        public static readonly Font FontBodyLarge = CreateFont(11F, FontStyle.Regular);
        public static readonly Font FontBodyBold = CreateFont(10F, FontStyle.Bold);
        public static readonly Font FontSmall = CreateFont(9F, FontStyle.Regular);
        public static readonly Font FontSmallBold = CreateFont(9F, FontStyle.Bold);
        public static readonly Font FontCaption = CreateFont(8F, FontStyle.Regular);
        public static readonly Font FontLabel = CreateFont(9F, FontStyle.Bold);
        public static readonly Font FontInput = CreateFont(10F, FontStyle.Regular);
        public static readonly Font FontButton = CreateFont(10F, FontStyle.Regular);
        public static readonly Font FontCode = new Font("Cascadia Code", 9F);

        private static Font CreateFont(float size, FontStyle style)
        {
            try { return new Font(FontFamily, size, style); }
            catch { return new Font(FontFamilyFallback, size, style); }
        }
        #endregion

        #region === SPACING SCALE (4px base) ===
        public const int Space0 = 0;
        public const int Space1 = 4;
        public const int Space2 = 8;
        public const int Space3 = 12;
        public const int Space4 = 16;
        public const int Space5 = 20;
        public const int Space6 = 24;
        public const int Space8 = 32;
        public const int Space10 = 40;
        public const int Space12 = 48;
        public const int Space16 = 64;

        public static readonly int PaddingSmall = Space2;
        public static readonly int PaddingMedium = Space4;
        public static readonly int PaddingLarge = Space6;
        #endregion

        #region === COMPONENT SIZES ===
        public const int ButtonHeightSmall = 32;
        public const int ButtonHeightMedium = 36;
        public const int ButtonHeightLarge = 44;
        public const int ButtonHeightDefault = 36;
        public const int InputHeight = 36;
        public const int IconSizeSmall = 16;
        public const int IconSizeMedium = 20;
        public const int IconSizeLarge = 24;
        public const int IconSizeXL = 32;
        #endregion

        #region === BORDER RADIUS (Win11 style) ===
        public static readonly int BorderRadius = 12;
        public static readonly int ButtonRadius = 8;        // Increased from 4 for more modern look
        public static readonly int InputRadius = 6;
        public static readonly int RadiusSmall = 6;
        public static readonly int RadiusFull = 9999;
        #endregion

        #region === ANIMATION TIMING (ms) ===
        public const int AnimationFast = 120;
        public const int AnimationNormal = 200;
        public const int AnimationSlow = 350;
        public const int AnimationXSlow = 500;
        #endregion

        // ===================================================================
        // FORM STYLING
        // ===================================================================

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

        // ===================================================================
        // BUTTON STYLING (Standard System.Windows.Forms.Button)
        // ===================================================================

        public static void ApplyButtonPrimary(Button btn)
        {
            btn.BackColor = Primary;
            btn.ForeColor = TextWhite;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = ButtonHeightDefault;

            btn.MouseEnter += (s, e) => btn.BackColor = PrimaryDark;
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
            btn.Height = ButtonHeightDefault;

            Color hoverColor = Color.FromArgb(13, 96, 13); // Green darker
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
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
            btn.Height = ButtonHeightDefault;

            Color hoverColor = Color.FromArgb(164, 32, 20); // Red darker
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = StatusError;
        }

        public static void ApplyButtonWarning(Button btn)
        {
            btn.BackColor = StatusWarning;
            btn.ForeColor = TextWhite;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = ButtonHeightDefault;

            Color hoverColor = Color.FromArgb(120, 76, 0); // Amber darker
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = StatusWarning;
        }

        public static void ApplyButtonSecondary(Button btn)
        {
            btn.BackColor = BackgroundCard;
            btn.ForeColor = TextPrimary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = BorderMedium;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = ButtonHeightDefault;

            btn.MouseEnter += (s, e) => { btn.BackColor = SecondaryLighter; btn.FlatAppearance.BorderColor = Secondary; };
            btn.MouseLeave += (s, e) => { btn.BackColor = BackgroundCard; btn.FlatAppearance.BorderColor = BorderMedium; };
        }

        public static void ApplyButtonOutline(Button btn)
        {
            btn.BackColor = BackgroundCard;
            btn.ForeColor = Primary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Primary;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.Height = ButtonHeightDefault;

            btn.MouseEnter += (s, e) => { btn.BackColor = PrimaryLighter; };
            btn.MouseLeave += (s, e) => { btn.BackColor = BackgroundCard; };
        }

        // ===================================================================
        // MODERN BUTTON OVERLOADS
        // ===================================================================

        #region === ModernButton Overloads ===
        public static void ApplyButtonPrimary(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Primary;
            btn.BorderRadius = ButtonRadius;
        }

        public static void ApplyButtonSuccess(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Success;
            btn.BorderRadius = ButtonRadius;
        }

        public static void ApplyButtonDanger(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Danger;
            btn.BorderRadius = ButtonRadius;
        }

        public static void ApplyButtonWarning(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Warning;
            btn.BorderRadius = ButtonRadius;
        }

        public static void ApplyButtonSecondary(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Secondary;
            btn.BorderRadius = ButtonRadius;
        }

        public static void ApplyButtonOutline(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Outline;
            btn.BorderRadius = ButtonRadius;
        }

        public static void ApplyButtonGhost(Controls.ModernButton btn)
        {
            btn.Variant = Controls.ModernButton.ButtonVariant.Ghost;
            btn.BorderRadius = ButtonRadius;
        }
        #endregion

        // ===================================================================
        // INPUT STYLING
        // ===================================================================

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

        // ===================================================================
        // DATAGRIDVIEW STYLING
        // ===================================================================

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
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 10, 12, 10);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgv.DefaultCellStyle.BackColor = BackgroundCard;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = FontSmall;
            dgv.DefaultCellStyle.Padding = new Padding(12, 8, 12, 8);
            dgv.DefaultCellStyle.SelectionBackColor = GridRowSelected;
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridRowAlt;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridRowSelected;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;

            dgv.RowTemplate.Height = 40;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Protect fixed-width columns (e.g. Icon) from Fill mode override
            if (dgv.Columns.Contains("Icon"))
            {
                dgv.Columns["Icon"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns["Icon"].Width = 40;
                dgv.Columns["Icon"].MinimumWidth = 40;
                dgv.Columns["Icon"].Resizable = DataGridViewTriState.False;
            }
        }

        // ===================================================================
        // MISC COMPONENT STYLING
        // ===================================================================

        public static void ApplyGroupBoxStyle(GroupBox grp)
        {
            grp.Font = FontSmallBold;
            grp.ForeColor = TextPrimary;
            grp.BackColor = BackgroundCard;
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
            menu.BackColor = BackgroundCard;
            menu.ForeColor = TextPrimary;
            menu.Font = FontSmall;
            menu.Renderer = new ModernMenuRenderer();
        }

        public static void ApplyToolStripStyle(ToolStrip tool)
        {
            tool.BackColor = BackgroundCard;
            tool.GripStyle = ToolStripGripStyle.Hidden;
            tool.Font = FontSmall;
            tool.Padding = new Padding(PaddingSmall, 4, PaddingSmall, 4);
            tool.Renderer = new ModernToolStripRenderer();
        }

        public static void ApplyTabControlStyle(TabControl tab)
        {
            tab.Font = FontSmallBold;
        }

        // ===================================================================
        // UTILITY METHODS
        // ===================================================================

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

        // ===================================================================
        // MENU RENDERER
        // ===================================================================

        private class ModernMenuRenderer : ToolStripProfessionalRenderer
        {
            public ModernMenuRenderer() : base(new ModernMenuColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected)
                {
                    var rect = new Rectangle(4, 2, e.Item.Width - 8, e.Item.Height - 4);
                    using (var path = new GraphicsPath())
                    {
                        int r = 4;
                        path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                        path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                        path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                        path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                        path.CloseFigure();
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (SolidBrush brush = new SolidBrush(PrimaryLighter))
                        {
                            e.Graphics.FillPath(brush, path);
                        }
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
            public override Color MenuItemBorder => BorderLight;
            public override Color MenuBorder => BorderLight;
            public override Color ToolStripDropDownBackground => BackgroundCard;
            public override Color ImageMarginGradientBegin => BackgroundSoft;
            public override Color ImageMarginGradientMiddle => BackgroundSoft;
            public override Color ImageMarginGradientEnd => BackgroundSoft;
        }

        // ===================================================================
        // TOOLBAR RENDERER
        // ===================================================================

        private class ModernToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ModernToolStripRenderer() : base(new ModernToolStripColorTable()) { }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected || e.Item.Pressed)
                {
                    var rect = new Rectangle(2, 2, e.Item.Width - 4, e.Item.Height - 4);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var path = new GraphicsPath())
                    {
                        int r = 4;
                        path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                        path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                        path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                        path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                        path.CloseFigure();

                        Color hoverBg = e.Item.Pressed
                            ? Color.FromArgb(20, 0, 0, 0)
                            : Color.FromArgb(10, 0, 0, 0);
                        using (SolidBrush brush = new SolidBrush(hoverBg))
                        {
                            e.Graphics.FillPath(brush, path);
                        }
                    }
                }
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                int x = e.Item.Width / 2;
                using (Pen pen = new Pen(BorderLight, 1))
                {
                    e.Graphics.DrawLine(pen, x, 4, x, e.Item.Height - 4);
                }
            }
        }

        private class ModernToolStripColorTable : ProfessionalColorTable
        {
            public override Color ToolStripBorder => BorderLight;
            public override Color ToolStripGradientBegin => BackgroundCard;
            public override Color ToolStripGradientMiddle => BackgroundCard;
            public override Color ToolStripGradientEnd => BackgroundCard;
            public override Color ButtonSelectedHighlight => Color.FromArgb(10, 0, 0, 0);
            public override Color ButtonSelectedBorder => Color.Transparent;
            public override Color ButtonPressedHighlight => Color.FromArgb(20, 0, 0, 0);
            public override Color ButtonPressedBorder => Color.Transparent;
            public override Color ButtonCheckedHighlight => PrimaryLighter;
        }
    }

    // ===================================================================
    // ROUNDED BUTTON (Legacy support)
    // ===================================================================

    public class RoundedButton : Button
    {
        private int _borderRadius = 4;
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

    // ===================================================================
    // ROUNDED PANEL (Legacy support)
    // ===================================================================

    public class RoundedPanel : Panel
    {
        private int _borderRadius = 8;
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
                for (int i = 1; i <= 3; i++)
                {
                    using (Pen pen = new Pen(Color.FromArgb(6, 0, 0, 0), 1))
                    {
                        e.Graphics.DrawRectangle(pen, rect.X + i, rect.Y + i, rect.Width, rect.Height);
                    }
                }
                rect = new Rectangle(0, 0, Width - 4, Height - 4);
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
