using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace study_document_manager.UI
{
    public static class AppTheme
    {
        #region === PRIMARY PALETTE (Slate) ===
        public static readonly Color PrimaryDark = Color.FromArgb(30, 41, 59);       // Slate-800
        public static readonly Color Primary = Color.FromArgb(51, 65, 85);            // Slate-700
        public static readonly Color PrimaryLight = Color.FromArgb(100, 116, 139);    // Slate-500
        public static readonly Color PrimaryLighter = Color.FromArgb(226, 232, 240);  // Slate-200
        #endregion

        #region === SECONDARY PALETTE (Stone) ===
        public static readonly Color Secondary = Color.FromArgb(120, 113, 108);       // Stone-500
        public static readonly Color SecondaryLight = Color.FromArgb(168, 162, 158);  // Stone-400
        public static readonly Color SecondaryLighter = Color.FromArgb(245, 245, 244);// Stone-100
        #endregion

        #region === ACCENT COLORS (Warm) ===
        public static readonly Color AccentOrange = Color.FromArgb(217, 119, 6);      // Amber-600
        public static readonly Color AccentRose = Color.FromArgb(190, 18, 60);        // Rose-700
        public static readonly Color AccentAmber = Color.FromArgb(202, 138, 4);       // Yellow-600
        public static readonly Color AccentSky = Color.FromArgb(37, 99, 235);         // Blue-600
        #endregion

        #region === VALIDATION STATES ===
        public static readonly Color ValidationError = Color.FromArgb(220, 38, 38);            // Red-600
        public static readonly Color ValidationErrorLight = Color.FromArgb(254, 226, 226);     // Red-100
        public static readonly Color ValidationErrorBorder = Color.FromArgb(252, 165, 165);    // Red-300
        public static readonly Color ValidationSuccess = Color.FromArgb(22, 163, 74);          // Green-600
        public static readonly Color ValidationSuccessLight = Color.FromArgb(220, 252, 231);   // Green-100
        public static readonly Color ValidationSuccessBorder = Color.FromArgb(134, 239, 172);  // Green-300
        public static readonly Color ValidationWarning = Color.FromArgb(202, 138, 4);          // Yellow-600
        public static readonly Color ValidationWarningLight = Color.FromArgb(254, 249, 195);   // Yellow-100
        public static readonly Color ValidationWarningBorder = Color.FromArgb(253, 224, 71);   // Yellow-300
        #endregion

        #region === INTERACTION STATES ===
        public static readonly Color HoverOverlay = Color.FromArgb(12, 0, 0, 0);               // 5% black
        public static readonly Color PressedOverlay = Color.FromArgb(25, 0, 0, 0);              // 10% black
        public static readonly Color FocusRing = Color.FromArgb(80, 51, 65, 85);                // Primary 30%
        public static readonly Color DisabledBg = Color.FromArgb(245, 245, 244);                // Stone-100
        public static readonly Color DisabledText = Color.FromArgb(168, 162, 158);              // Stone-400
        public static readonly Color DisabledBorder = Color.FromArgb(231, 229, 228);            // Stone-200
        #endregion

        #region === SHADOW EFFECTS ===
        public static readonly Color ShadowLight = Color.FromArgb(8, 0, 0, 0);                 // 3% black
        public static readonly Color ShadowMedium = Color.FromArgb(15, 0, 0, 0);               // 6% black
        public static readonly Color ShadowHeavy = Color.FromArgb(25, 0, 0, 0);                // 10% black
        #endregion

        #region === BACKGROUND COLORS ===
        public static readonly Color BackgroundMain = Color.FromArgb(250, 250, 249);  // Stone-50
        public static readonly Color BackgroundSoft = Color.FromArgb(245, 245, 244);  // Stone-100
        public static readonly Color BackgroundWarm = Color.FromArgb(250, 250, 249);  // Stone-50
        public static readonly Color BackgroundCard = Color.FromArgb(255, 255, 255);  // White
        #endregion

        #region === TEXT COLORS ===
        public static readonly Color TextPrimary = Color.FromArgb(28, 25, 23);        // Stone-900
        public static readonly Color TextSecondary = Color.FromArgb(120, 113, 108);    // Stone-500
        public static readonly Color TextMuted = Color.FromArgb(168, 162, 158);        // Stone-400
        public static readonly Color TextWhite = Color.FromArgb(255, 255, 255);
        #endregion

        #region === BORDER COLORS ===
        public static readonly Color BorderLight = Color.FromArgb(231, 229, 228);      // Stone-200
        public static readonly Color BorderMedium = Color.FromArgb(214, 211, 209);     // Stone-300
        public static readonly Color BorderFocus = Color.FromArgb(51, 65, 85);         // Slate-700
        #endregion

        #region === STATUS COLORS ===
        public static readonly Color StatusSuccess = Color.FromArgb(22, 163, 74);      // Green-600
        public static readonly Color StatusWarning = Color.FromArgb(202, 138, 4);      // Yellow-600
        public static readonly Color StatusError = Color.FromArgb(220, 38, 38);        // Red-600
        public static readonly Color StatusInfo = Color.FromArgb(37, 99, 235);         // Blue-600

        // Shorthand aliases
        public static readonly Color Success = StatusSuccess;
        public static readonly Color Warning = StatusWarning;
        public static readonly Color Danger = StatusError;
        public static readonly Color Info = StatusInfo;
        #endregion

        #region === INPUT COLORS ===
        public static readonly Color InputBackground = Color.FromArgb(255, 255, 255);
        public static readonly Color InputBorder = Color.FromArgb(214, 211, 209);      // Stone-300
        public static readonly Color InputBorderFocus = Color.FromArgb(51, 65, 85);    // Slate-700
        public static readonly Color InputBorderHover = Color.FromArgb(168, 162, 158); // Stone-400
        #endregion

        #region === GRID COLORS ===
        public static readonly Color GridHeaderBg = Color.FromArgb(51, 65, 85);        // Slate-700
        public static readonly Color GridHeaderFg = Color.FromArgb(255, 255, 255);
        public static readonly Color GridRowAlt = Color.FromArgb(250, 250, 249);       // Stone-50
        public static readonly Color GridRowSelected = Color.FromArgb(226, 232, 240);  // Slate-200
        public static readonly Color GridBorder = Color.FromArgb(231, 229, 228);       // Stone-200
        #endregion

        #region === TYPOGRAPHY SCALE ===
        public const string FontFamily = "Segoe UI";
        public static readonly Font FontDisplay = new Font(FontFamily, 26F, FontStyle.Bold);
        public static readonly Font FontH1 = new Font(FontFamily, 22F, FontStyle.Bold);
        public static readonly Font FontH2 = new Font(FontFamily, 18F, FontStyle.Bold);
        public static readonly Font FontH3 = new Font(FontFamily + " Semibold", 15F);
        public static readonly Font FontH4 = new Font(FontFamily + " Semibold", 13F);
        public static readonly Font FontTitle = new Font(FontFamily, 18F, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font(FontFamily, 13F, FontStyle.Regular);
        public static readonly Font FontHeading = new Font(FontFamily + " Semibold", 11F);
        public static readonly Font FontBody = new Font(FontFamily, 10F);
        public static readonly Font FontBodyLarge = new Font(FontFamily, 11F);
        public static readonly Font FontBodyBold = new Font(FontFamily + " Semibold", 10F);
        public static readonly Font FontSmall = new Font(FontFamily, 9F);
        public static readonly Font FontSmallBold = new Font(FontFamily + " Semibold", 9F);
        public static readonly Font FontCaption = new Font(FontFamily, 8F);
        public static readonly Font FontLabel = new Font(FontFamily + " Semibold", 9F);
        public static readonly Font FontInput = new Font(FontFamily, 11F);
        public static readonly Font FontButton = new Font(FontFamily + " Semibold", 10F);
        public static readonly Font FontCode = new Font("Cascadia Code", 9F);
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
        public const int ButtonHeightMedium = 38;
        public const int ButtonHeightLarge = 44;
        public const int ButtonHeightDefault = 40;
        public const int InputHeight = 40;
        public const int IconSizeSmall = 16;
        public const int IconSizeMedium = 20;
        public const int IconSizeLarge = 24;
        public const int IconSizeXL = 32;
        #endregion

        #region === BORDER RADIUS ===
        public static readonly int BorderRadius = 8;
        public static readonly int ButtonRadius = 6;
        public static readonly int InputRadius = 6;
        public static readonly int RadiusSmall = 4;
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

            Color hoverColor = Color.FromArgb(21, 128, 61); // Green-700
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

            Color hoverColor = Color.FromArgb(185, 28, 28); // Red-700
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

            Color hoverColor = Color.FromArgb(161, 98, 7); // Yellow-700
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

            btn.MouseEnter += (s, e) => { btn.BackColor = BackgroundSoft; btn.FlatAppearance.BorderColor = Secondary; };
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
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 12, 12, 12);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 44;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgv.DefaultCellStyle.BackColor = BackgroundCard;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = FontSmall;
            dgv.DefaultCellStyle.Padding = new Padding(12, 8, 12, 8);
            dgv.DefaultCellStyle.SelectionBackColor = GridRowSelected;
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridRowAlt;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridRowSelected;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;

            dgv.RowTemplate.Height = 44;
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
            public override Color MenuItemBorder => BorderMedium;
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
                var btn = e.Item as ToolStripButton;
                if (btn != null && btn.BackColor != Color.Empty && btn.BackColor != Color.Transparent && btn.BackColor != BackgroundCard)
                {
                    using (SolidBrush brush = new SolidBrush(btn.BackColor))
                    {
                        e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                    }
                }

                if (e.Item.Selected || e.Item.Pressed)
                {
                    Color hoverColor = PrimaryLighter;
                    if (btn != null && btn.BackColor != Color.Empty && btn.BackColor != Color.Transparent && btn.BackColor != BackgroundCard)
                    {
                        hoverColor = Color.FromArgb(40, 255, 255, 255); // White overlay for colored buttons
                    }

                    using (SolidBrush brush = new SolidBrush(hoverColor))
                    {
                        e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                    }
                }
            }
        }

        private class ModernToolStripColorTable : ProfessionalColorTable
        {
            public override Color ToolStripBorder => BorderLight;
            public override Color ToolStripGradientBegin => BackgroundCard;
            public override Color ToolStripGradientMiddle => BackgroundCard;
            public override Color ToolStripGradientEnd => BackgroundCard;
            public override Color ButtonSelectedHighlight => PrimaryLighter;
            public override Color ButtonSelectedBorder => BorderMedium;
        }
    }

    // ===================================================================
    // ROUNDED BUTTON (Legacy support)
    // ===================================================================

    public class RoundedButton : Button
    {
        private int _borderRadius = 6;
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
