using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace document_sharing_manager.UI.Controls
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
        private int _borderRadius = 8;
        private Color _borderColor = Color.Transparent;
        private int _borderSize = 0;
        private ButtonVariant _variant = ButtonVariant.Primary;

        // State tracking
        private bool _isHovered = false;
        private bool _isPressed = false;

        // Colors (cached)
        private Color _baseBackColor;
        private Color _baseTextColor;
        #endregion

        #region === PROPERTIES ===
        [Category("Modern UI")]
        [Description("Border radius of the button")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("Modern UI")]
        public Color BackgroundColor
        {
            get => _baseBackColor;
            set { _baseBackColor = value; Invalidate(); }
        }

        [Category("Modern UI")]
        public Color TextColor
        {
            get => _baseTextColor;
            set { _baseTextColor = value; Invalidate(); }
        }

        [Category("Modern UI")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Modern UI")]
        public int BorderSize
        {
            get => _borderSize;
            set { _borderSize = value; Invalidate(); }
        }

        [Category("Modern UI")]
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
            this.Size = new Size(150, 40);
            this.Cursor = Cursors.Hand;
            this.Font = AppTheme.FontButton;

            this.Resize += Button_Resize;

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
                    _borderSize = 0;
                    break;

                case ButtonVariant.Secondary:
                    _baseBackColor = AppTheme.BackgroundCard;
                    _baseTextColor = AppTheme.TextPrimary;
                    _borderColor = AppTheme.BorderMedium;
                    _borderSize = 1;
                    break;

                case ButtonVariant.Success:
                    _baseBackColor = AppTheme.StatusSuccess;
                    _baseTextColor = AppTheme.TextWhite;
                    _borderSize = 0;
                    break;

                case ButtonVariant.Danger:
                    _baseBackColor = AppTheme.StatusError;
                    _baseTextColor = AppTheme.TextWhite;
                    _borderSize = 0;
                    break;

                case ButtonVariant.Warning:
                    _baseBackColor = AppTheme.StatusWarning;
                    _baseTextColor = AppTheme.TextWhite;
                    _borderSize = 0;
                    break;

                case ButtonVariant.Outline:
                    _baseBackColor = AppTheme.BackgroundCard;
                    _baseTextColor = AppTheme.Primary;
                    _borderColor = AppTheme.Primary;
                    _borderSize = 1;
                    break;

                case ButtonVariant.Ghost:
                    _baseBackColor = Color.Transparent;
                    _baseTextColor = AppTheme.Primary;
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
                return _variant == ButtonVariant.Ghost
                    ? AppTheme.PrimaryLighter
                    : ControlPaint.Dark(baseColor, 0.1f);
            }

            if (_isHovered)
            {
                return _variant == ButtonVariant.Ghost
                    ? Color.FromArgb(25, AppTheme.Primary)
                    : ControlPaint.Light(baseColor, 0.06f);
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
                return AppTheme.PrimaryDark;

            if (_isHovered && _variant == ButtonVariant.Secondary)
                return AppTheme.Secondary;

            return _borderColor;
        }
        #endregion

        #region === MOUSE EVENTS ===
        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
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

        #region === PAINTING ===
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var rect = new RectangleF(0, 0, Width - 1, Height - 1);
            var adjustedRadius = Math.Min(_borderRadius, Math.Min(Width, Height) / 2);

            // Background
            Color bgColor = GetStateBackColor();
            using (var path = GetFigurePath(rect, adjustedRadius))
            {
                if (bgColor != Color.Transparent)
                {
                    using (var brush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }

                this.Region = new Region(path);

                // Border
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

                // Anti-alias edge
                if (Parent != null && adjustedRadius > 2)
                {
                    using (var pen = new Pen(Parent.BackColor, 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }

            // Text
            Color textColor = GetStateTextColor();
            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                Rectangle.Round(rect),
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
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
        #endregion
    }
}

