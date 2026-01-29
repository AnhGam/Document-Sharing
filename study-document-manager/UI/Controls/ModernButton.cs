using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace study_document_manager.UI.Controls
{
    public class ModernButton : Button
    {
        #region === ENUMS ===
        public enum ButtonVariant
        {
            Primary,
            Secondary,
            Success,
            Danger,
            Warning,
            Outline,
            Ghost
        }
        #endregion

        #region === FIELDS ===
        private int _borderRadius = 10;
        private Color _borderColor = Color.Transparent;
        private int _borderSize = 0;
        private ButtonVariant _variant = ButtonVariant.Primary;
        private bool _enableGlow = true;
        private bool _enableAnimation = true;

        // State tracking
        private bool _isHovered = false;
        private bool _isPressed = false;
        private bool _isLoading = false;

        // Animation
        private Timer _glowTimer;
        private int _glowOpacity = 0;
        private const int GlowMaxOpacity = 80;
        private const int GlowStep = 12;

        // Colors (cached)
        private Color _baseBackColor;
        private Color _baseTextColor;
        private Color _glowColor;
        #endregion

        #region === PROPERTIES ===
        [Category("Modern UI Code")]
        [Description("Border radius of the button")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        public Color BackgroundColor
        {
            get => _baseBackColor;
            set { _baseBackColor = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        public Color TextColor
        {
            get => _baseTextColor;
            set { _baseTextColor = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        public int BorderSize
        {
            get => _borderSize;
            set { _borderSize = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        [Description("Button style variant")]
        public ButtonVariant Variant
        {
            get => _variant;
            set
            {
                _variant = value;
                ApplyVariantStyle();
                Invalidate();
            }
        }

        [Category("Modern UI Code")]
        [Description("Enable glow effect on hover")]
        public bool EnableGlow
        {
            get => _enableGlow;
            set { _enableGlow = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        [Description("Enable hover/press animations")]
        public bool EnableAnimation
        {
            get => _enableAnimation;
            set { _enableAnimation = value; }
        }

        [Category("Modern UI Code")]
        [Description("Show loading state")]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                Enabled = !value;
                Invalidate();
            }
        }
        #endregion

        #region === CONSTRUCTOR ===
        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.OptimizedDoubleBuffer, true);

            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 44);
            this.Cursor = Cursors.Hand;
            this.Font = AppTheme.FontButton;

            this.Resize += Button_Resize;

            // Apply default variant
            ApplyVariantStyle();
        }
        #endregion

        #region === VARIANT STYLES ===
        private void ApplyVariantStyle()
        {
            switch (_variant)
            {
                case ButtonVariant.Primary:
                    _baseBackColor = AppTheme.Primary;
                    _baseTextColor = AppTheme.TextWhite;
                    _glowColor = AppTheme.GlowPrimary;
                    _borderSize = 0;
                    break;

                case ButtonVariant.Secondary:
                    _baseBackColor = AppTheme.BackgroundSoft;
                    _baseTextColor = AppTheme.TextSecondary;
                    _glowColor = AppTheme.ShadowMedium;
                    _borderColor = AppTheme.BorderMedium;
                    _borderSize = 1;
                    break;

                case ButtonVariant.Success:
                    _baseBackColor = AppTheme.StatusSuccess;
                    _baseTextColor = AppTheme.TextWhite;
                    _glowColor = AppTheme.GlowSuccess;
                    _borderSize = 0;
                    break;

                case ButtonVariant.Danger:
                    _baseBackColor = AppTheme.StatusError;
                    _baseTextColor = AppTheme.TextWhite;
                    _glowColor = AppTheme.GlowDanger;
                    _borderSize = 0;
                    break;

                case ButtonVariant.Warning:
                    _baseBackColor = AppTheme.StatusWarning;
                    _baseTextColor = AppTheme.TextPrimary;
                    _glowColor = AppTheme.GlowWarning;
                    _borderSize = 0;
                    break;

                case ButtonVariant.Outline:
                    _baseBackColor = AppTheme.BackgroundMain;
                    _baseTextColor = AppTheme.Primary;
                    _glowColor = AppTheme.GlowPrimary;
                    _borderColor = AppTheme.Primary;
                    _borderSize = 2;
                    break;

                case ButtonVariant.Ghost:
                    _baseBackColor = Color.Transparent;
                    _baseTextColor = AppTheme.Primary;
                    _glowColor = AppTheme.PrimaryLighter;
                    _borderSize = 0;
                    break;
            }

            this.BackColor = _baseBackColor;
            this.ForeColor = _baseTextColor;
        }

        private Color GetStateBackColor()
        {
            if (!Enabled)
                return AppTheme.DisabledBg;

            Color baseColor = _baseBackColor;

            if (_isPressed)
            {
                // Darken on press
                return _variant == ButtonVariant.Ghost
                    ? AppTheme.PrimaryLighter
                    : ControlPaint.Dark(baseColor, 0.08f);
            }

            if (_isHovered)
            {
                // Lighten on hover
                return _variant == ButtonVariant.Ghost
                    ? Color.FromArgb(40, AppTheme.Primary)
                    : ControlPaint.Light(baseColor, 0.08f);
            }

            return baseColor;
        }

        private Color GetStateTextColor()
        {
            if (!Enabled)
                return AppTheme.DisabledText;

            return _baseTextColor;
        }

        private Color GetStateBorderColor()
        {
            if (!Enabled)
                return AppTheme.DisabledBorder;

            if (_isHovered && _variant == ButtonVariant.Outline)
                return AppTheme.PrimaryLight;

            return _borderColor;
        }
        #endregion

        #region === MOUSE EVENTS ===
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            if (_enableGlow && _enableAnimation)
                StartGlowAnimation();
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            if (_enableGlow && _enableAnimation)
                StartGlowAnimation(); // Will fade out
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }
        #endregion

        #region === GLOW ANIMATION ===
        private void StartGlowAnimation()
        {
            if (_glowTimer == null)
            {
                _glowTimer = new Timer { Interval = 16 }; // ~60fps
                _glowTimer.Tick += GlowTimer_Tick;
            }
            _glowTimer.Start();
        }

        private void GlowTimer_Tick(object sender, EventArgs e)
        {
            if (_isHovered && _glowOpacity < GlowMaxOpacity)
            {
                _glowOpacity = Math.Min(GlowMaxOpacity, _glowOpacity + GlowStep);
            }
            else if (!_isHovered && _glowOpacity > 0)
            {
                _glowOpacity = Math.Max(0, _glowOpacity - GlowStep);
            }

            if ((_glowOpacity == 0 && !_isHovered) || (_glowOpacity == GlowMaxOpacity && _isHovered))
            {
                _glowTimer?.Stop();
            }

            Invalidate();
        }
        #endregion

        #region === PAINTING ===
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var rect = new RectangleF(0, 0, Width - 1, Height - 1);
            var adjustedRadius = Math.Min(_borderRadius, Math.Min(Width, Height) / 2);

            // Layer 1: Glow effect (when hovered and enabled)
            if (_enableGlow && _glowOpacity > 0 && Enabled)
            {
                DrawGlowEffect(e.Graphics, rect, adjustedRadius);
            }

            // Layer 2: Background with state colors
            Color bgColor = GetStateBackColor();
            using (var path = GetFigurePath(rect, adjustedRadius))
            {
                // Fill background
                if (bgColor != Color.Transparent)
                {
                    using (var brush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }

                // Set region for clipping
                this.Region = new Region(path);

                // Layer 3: Border
                if (_borderSize >= 1)
                {
                    var borderRect = new RectangleF(
                        _borderSize / 2f,
                        _borderSize / 2f,
                        Width - _borderSize - 1,
                        Height - _borderSize - 1
                    );
                    using (var borderPath = GetFigurePath(borderRect, adjustedRadius - 1))
                    using (var pen = new Pen(GetStateBorderColor(), _borderSize))
                    {
                        pen.Alignment = PenAlignment.Center;
                        e.Graphics.DrawPath(pen, borderPath);
                    }
                }

                // Draw parent background around edges for anti-aliasing
                if (Parent != null && adjustedRadius > 2)
                {
                    using (var pen = new Pen(Parent.BackColor, 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }

            // Layer 4: Text or loading indicator
            DrawContent(e.Graphics, rect);
        }

        private void DrawGlowEffect(Graphics g, RectangleF bounds, float radius)
        {
            int glowSize = 8;
            var glowRect = new RectangleF(
                bounds.X - glowSize / 2f,
                bounds.Y - glowSize / 2f,
                bounds.Width + glowSize,
                bounds.Height + glowSize
            );

            // Multiple layers for soft glow
            for (int i = 3; i >= 1; i--)
            {
                var layerRect = new RectangleF(
                    glowRect.X - i,
                    glowRect.Y - i,
                    glowRect.Width + i * 2,
                    glowRect.Height + i * 2
                );

                using (var path = GetFigurePath(layerRect, radius + i + 2))
                {
                    int layerAlpha = (int)((_glowOpacity / 100.0) * (25 - i * 6));
                    if (layerAlpha > 0)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(layerAlpha, _glowColor)))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                }
            }
        }

        private void DrawContent(Graphics g, RectangleF bounds)
        {
            Color textColor = GetStateTextColor();

            if (_isLoading)
            {
                // Draw loading spinner (simple dots animation)
                DrawLoadingIndicator(g, bounds, textColor);
            }
            else
            {
                // Draw text
                TextRenderer.DrawText(
                    g,
                    Text,
                    Font,
                    Rectangle.Round(bounds),
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            }
        }

        private void DrawLoadingIndicator(Graphics g, RectangleF bounds, Color color)
        {
            int dotCount = 3;
            int dotSize = 6;
            int spacing = 4;
            int totalWidth = dotCount * dotSize + (dotCount - 1) * spacing;

            float startX = bounds.X + (bounds.Width - totalWidth) / 2;
            float centerY = bounds.Y + bounds.Height / 2;

            for (int i = 0; i < dotCount; i++)
            {
                float alpha = (float)(0.3 + 0.7 * Math.Sin(DateTime.Now.Millisecond / 150.0 + i * 0.8));
                using (var brush = new SolidBrush(Color.FromArgb((int)(alpha * 255), color)))
                {
                    g.FillEllipse(brush,
                        startX + i * (dotSize + spacing),
                        centerY - dotSize / 2,
                        dotSize,
                        dotSize);
                }
            }
        }
        #endregion

        #region === HELPER METHODS ===
        private void Button_Resize(object sender, EventArgs e)
        {
            if (_borderRadius > this.Height)
                _borderRadius = this.Height;
        }

        private GraphicsPath GetFigurePath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            float diameter = radius * 2;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (Parent != null)
                Parent.BackColorChanged += Container_BackColorChanged;
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            if (this.DesignMode)
                this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _glowTimer?.Stop();
                _glowTimer?.Dispose();
                _glowTimer = null;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
