using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace study_document_manager.UI.Controls
{
    public class ModernPanel : Panel
    {
        #region === ENUMS ===
        public enum ElevationLevel
        {
            None,
            Low,      // Subtle shadow
            Medium,   // Standard card shadow
            High      // Floating/modal shadow
        }
        #endregion

        #region === FIELDS ===
        // Appearance
        private int _borderRadius = 0;
        private float _gradientAngle = 90F;
        private Color _gradientTopColor = Color.White;
        private Color _gradientBottomColor = Color.WhiteSmoke;
        private bool _useGradient = false;

        // Border
        private bool _showBorder = false;
        private Color _borderColor = AppTheme.BorderLight;
        private int _borderSize = 1;

        // Elevation/Shadow
        private ElevationLevel _elevation = ElevationLevel.None;
        private bool _showShadow = false;

        // State
        private bool _isHovered = false;
        private bool _hoverEffect = false;
        #endregion

        #region === CONSTRUCTOR ===
        public ModernPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            this.BackColor = AppTheme.BackgroundCard;
            this.ForeColor = AppTheme.TextPrimary;
            this.Size = new Size(350, 200);
        }
        #endregion

        #region === APPEARANCE PROPERTIES ===
        [Category("Modern UI Code")]
        [Description("Border radius of the panel")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        [Description("Gradient angle in degrees")]
        public float GradientAngle
        {
            get => _gradientAngle;
            set { _gradientAngle = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        [Description("Top color of gradient")]
        public Color GradientTopColor
        {
            get => _gradientTopColor;
            set { _gradientTopColor = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        [Description("Bottom color of gradient")]
        public Color GradientBottomColor
        {
            get => _gradientBottomColor;
            set { _gradientBottomColor = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        [Description("Use gradient background instead of solid color")]
        public bool UseGradient
        {
            get => _useGradient;
            set { _useGradient = value; Invalidate(); }
        }
        #endregion

        #region === BORDER PROPERTIES ===
        [Category("Modern UI Code - Border")]
        [Description("Show border around panel")]
        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        [Category("Modern UI Code - Border")]
        [Description("Border color")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Modern UI Code - Border")]
        [Description("Border thickness")]
        public int BorderSize
        {
            get => _borderSize;
            set { _borderSize = Math.Max(1, value); Invalidate(); }
        }
        #endregion

        #region === ELEVATION PROPERTIES ===
        [Category("Modern UI Code - Elevation")]
        [Description("Elevation level for shadow effect")]
        public ElevationLevel Elevation
        {
            get => _elevation;
            set
            {
                _elevation = value;
                _showShadow = value != ElevationLevel.None;
                Invalidate();
            }
        }

        [Category("Modern UI Code - Elevation")]
        [Description("Show shadow (legacy property, use Elevation instead)")]
        public bool ShowShadow
        {
            get => _showShadow;
            set
            {
                _showShadow = value;
                if (value && _elevation == ElevationLevel.None)
                    _elevation = ElevationLevel.Low;
                else if (!value)
                    _elevation = ElevationLevel.None;
                Invalidate();
            }
        }

        [Category("Modern UI Code - Elevation")]
        [Description("Enable hover effect (slight lift on hover)")]
        public bool HoverEffect
        {
            get => _hoverEffect;
            set { _hoverEffect = value; }
        }
        #endregion

        #region === MOUSE EVENTS ===
        protected override void OnMouseEnter(EventArgs e)
        {
            if (_hoverEffect)
            {
                _isHovered = true;
                Invalidate();
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_hoverEffect)
            {
                _isHovered = false;
                Invalidate();
            }
            base.OnMouseLeave(e);
        }
        #endregion

        #region === PAINTING ===
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Calculate effective elevation (increase on hover)
            ElevationLevel effectiveElevation = _elevation;
            if (_hoverEffect && _isHovered && _elevation != ElevationLevel.None)
            {
                effectiveElevation = (ElevationLevel)Math.Min((int)_elevation + 1, (int)ElevationLevel.High);
            }

            // Calculate shadow offset for content area
            int shadowOffset = GetShadowOffset(effectiveElevation);
            var contentRect = new Rectangle(
                shadowOffset,
                shadowOffset,
                Width - shadowOffset * 2 - 1,
                Height - shadowOffset * 2 - 1
            );

            // Layer 1: Draw shadow
            if (_showShadow && effectiveElevation != ElevationLevel.None)
            {
                DrawElevationShadow(g, contentRect, effectiveElevation);
            }

            // Layer 2: Draw background
            DrawBackground(g, contentRect);

            // Layer 3: Draw border
            if (_showBorder && _borderRadius > 2)
            {
                DrawBorder(g, contentRect);
            }

            // Set region for clipping
            if (_borderRadius > 2)
            {
                using (var path = GetFigurePath(new RectangleF(0, 0, Width, Height), _borderRadius + shadowOffset))
                {
                    this.Region = new Region(path);
                }
            }
            else
            {
                this.Region = new Region(new Rectangle(0, 0, Width, Height));
            }
        }

        private void DrawBackground(Graphics g, Rectangle rect)
        {
            if (_borderRadius > 2)
            {
                using (var path = GetFigurePath(rect, _borderRadius))
                {
                    if (_useGradient)
                    {
                        using (var brush = new LinearGradientBrush(rect, _gradientTopColor, _gradientBottomColor, _gradientAngle))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    else
                    {
                        using (var brush = new SolidBrush(BackColor))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                }
            }
            else
            {
                if (_useGradient)
                {
                    using (var brush = new LinearGradientBrush(rect, _gradientTopColor, _gradientBottomColor, _gradientAngle))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(BackColor))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
            }
        }

        private void DrawBorder(Graphics g, Rectangle rect)
        {
            var borderRect = new Rectangle(
                rect.X + _borderSize / 2,
                rect.Y + _borderSize / 2,
                rect.Width - _borderSize,
                rect.Height - _borderSize
            );

            using (var path = GetFigurePath(borderRect, Math.Max(0, _borderRadius - _borderSize)))
            using (var pen = new Pen(_borderColor, _borderSize))
            {
                g.DrawPath(pen, path);
            }
        }

        private void DrawElevationShadow(Graphics g, Rectangle contentRect, ElevationLevel level)
        {
            int layers;
            int baseAlpha;
            int offsetY;

            switch (level)
            {
                case ElevationLevel.Low:
                    layers = 3;
                    baseAlpha = 8;
                    offsetY = 1;
                    break;
                case ElevationLevel.Medium:
                    layers = 5;
                    baseAlpha = 10;
                    offsetY = 2;
                    break;
                case ElevationLevel.High:
                    layers = 8;
                    baseAlpha = 12;
                    offsetY = 4;
                    break;
                default:
                    layers = 0;
                    baseAlpha = 0;
                    offsetY = 0;
                    break;
            }

            // Draw shadow layers (from outer to inner)
            for (int i = layers; i >= 1; i--)
            {
                var shadowRect = new RectangleF(
                    contentRect.X - i + 1,
                    contentRect.Y - i + 1 + offsetY,
                    contentRect.Width + (i - 1) * 2,
                    contentRect.Height + (i - 1) * 2
                );

                int alpha = baseAlpha + (layers - i) * 2;
                float radius = _borderRadius > 0 ? _borderRadius + i : i * 2;

                using (var path = GetFigurePath(shadowRect, radius))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private int GetShadowOffset(ElevationLevel level)
        {
            switch (level)
            {
                case ElevationLevel.Low:
                    return 4;
                case ElevationLevel.Medium:
                    return 6;
                case ElevationLevel.High:
                    return 10;
                default:
                    return 0;
            }
        }

        private GraphicsPath GetFigurePath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();

            return path;
        }
        #endregion

        #region === HELPER METHODS ===
        /// <summary>
        /// Apply card styling with elevation
        /// </summary>
        public void ApplyCardStyle(ElevationLevel elevation = ElevationLevel.Low)
        {
            BackColor = AppTheme.BackgroundCard;
            BorderRadius = AppTheme.BorderRadius;
            ShowBorder = true;
            BorderColor = AppTheme.BorderLight;
            Elevation = elevation;
            Padding = new Padding(AppTheme.Space4);
        }

        /// <summary>
        /// Apply surface styling (subtle)
        /// </summary>
        public void ApplySurfaceStyle()
        {
            BackColor = AppTheme.BackgroundSoft;
            BorderRadius = AppTheme.BorderRadius;
            ShowBorder = false;
            Elevation = ElevationLevel.None;
            Padding = new Padding(AppTheme.Space4);
        }

        /// <summary>
        /// Apply modal/overlay styling
        /// </summary>
        public void ApplyModalStyle()
        {
            BackColor = AppTheme.BackgroundCard;
            BorderRadius = AppTheme.BorderRadius;
            ShowBorder = true;
            BorderColor = AppTheme.BorderLight;
            Elevation = ElevationLevel.High;
            Padding = new Padding(AppTheme.Space6);
        }
        #endregion
    }
}
