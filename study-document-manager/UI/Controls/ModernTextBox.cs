using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace study_document_manager.UI.Controls
{
    [DefaultEvent("_TextChanged")]
    public class ModernTextBox : UserControl
    {
        #region === ENUMS ===
        public enum ValidationState
        {
            None,
            Valid,
            Invalid,
            Warning
        }
        #endregion

        #region === FIELDS ===
        // Appearance
        private Color borderColor = AppTheme.BorderMedium;
        private Color borderFocusColor = AppTheme.Primary;
        private Color borderHoverColor = AppTheme.InputBorderHover;
        private int borderSize = 1;
        private bool underlinedStyle = false;
        private int borderRadius = 6;

        // State
        private bool isFocused = false;
        private bool isHovered = false;
        private bool isPasswordChar = false;

        // Placeholder
        private Color placeholderColor = AppTheme.TextMuted;
        private string placeholderText = "";
        private bool isPlaceholder = false;

        // Validation
        private ValidationState _validationState = ValidationState.None;
        private string _validationMessage = "";
        private Label _validationLabel;
        private PictureBox _validationIcon;
        private bool _showValidationMessage = true;
        private bool _showValidationIcon = true;

        // Internal control
        private TextBox textBox1;
        #endregion

        #region === EVENTS ===
        public event EventHandler _TextChanged;
        public event EventHandler ValidationStateChanged;
        #endregion

        #region === APPEARANCE PROPERTIES ===
        [Category("Modern UI Code")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        public Color BorderFocusColor
        {
            get => borderFocusColor;
            set => borderFocusColor = value;
        }

        [Category("Modern UI Code")]
        public Color BorderHoverColor
        {
            get => borderHoverColor;
            set => borderHoverColor = value;
        }

        [Category("Modern UI Code")]
        public int BorderSize
        {
            get => borderSize;
            set { borderSize = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        public bool UnderlinedStyle
        {
            get => underlinedStyle;
            set { underlinedStyle = value; Invalidate(); }
        }

        [Category("Modern UI Code")]
        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }
        #endregion

        #region === TEXT PROPERTIES ===
        [Category("Modern UI Code")]
        public bool PasswordChar
        {
            get => isPasswordChar;
            set { isPasswordChar = value; textBox1.UseSystemPasswordChar = value; }
        }

        [Category("Modern UI Code")]
        public bool Multiline
        {
            get => textBox1.Multiline;
            set { textBox1.Multiline = value; }
        }

        [Category("Modern UI Code")]
        public override Color BackColor
        {
            get => base.BackColor;
            set { base.BackColor = value; textBox1.BackColor = value; }
        }

        [Category("Modern UI Code")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set { base.ForeColor = value; textBox1.ForeColor = value; }
        }

        [Category("Modern UI Code")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                textBox1.Font = value;
                if (DesignMode) UpdateControlHeight();
            }
        }

        [Category("Modern UI Code")]
        public string Texts
        {
            get => isPlaceholder ? "" : textBox1.Text;
            set { textBox1.Text = value; }
        }

        [Category("Modern UI Code")]
        public Color PlaceholderColor
        {
            get => placeholderColor;
            set { placeholderColor = value; if (isPlaceholder) textBox1.ForeColor = value; }
        }

        [Category("Modern UI Code")]
        public string PlaceholderText
        {
            get => placeholderText;
            set { placeholderText = value; textBox1.Text = ""; SetPlaceholder(); }
        }

        [Category("Modern UI Code")]
        public bool ReadOnly
        {
            get => textBox1.ReadOnly;
            set => textBox1.ReadOnly = value;
        }

        [Category("Modern UI Code")]
        public int MaxLength
        {
            get => textBox1.MaxLength;
            set => textBox1.MaxLength = value;
        }
        #endregion

        #region === VALIDATION PROPERTIES ===
        [Category("Modern UI Code - Validation")]
        [Description("Current validation state of the control")]
        public ValidationState Validation
        {
            get => _validationState;
            set
            {
                if (_validationState != value)
                {
                    _validationState = value;
                    UpdateValidationVisual();
                    ValidationStateChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        [Category("Modern UI Code - Validation")]
        [Description("Validation message to display")]
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                UpdateValidationLabel();
            }
        }

        [Category("Modern UI Code - Validation")]
        [Description("Show validation message below the control")]
        public bool ShowValidationMessage
        {
            get => _showValidationMessage;
            set { _showValidationMessage = value; UpdateValidationLabel(); }
        }

        [Category("Modern UI Code - Validation")]
        [Description("Show validation icon inside the control")]
        public bool ShowValidationIcon
        {
            get => _showValidationIcon;
            set { _showValidationIcon = value; UpdateValidationIcon(); }
        }
        #endregion

        #region === CONSTRUCTOR ===
        public ModernTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Create internal TextBox
            textBox1 = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill
            };

            this.Controls.Add(textBox1);

            // Wire up events
            textBox1.Enter += TextBox1_Enter;
            textBox1.Leave += TextBox1_Leave;
            textBox1.TextChanged += TextBox1_TextChanged;
            textBox1.Click += TextBox1_Click;
            textBox1.MouseEnter += TextBox1_MouseEnter;
            textBox1.MouseLeave += TextBox1_MouseLeave;
            textBox1.KeyPress += TextBox1_KeyPress;

            // Default styling
            this.AutoScaleMode = AutoScaleMode.None;
            this.Padding = new Padding(10, 7, 10, 7);
            this.Size = new Size(250, 30);
            this.BackColor = AppTheme.InputBackground;
            this.ForeColor = AppTheme.TextPrimary;
            this.Font = AppTheme.FontInput;

            // Create validation controls
            CreateValidationControls();
        }
        #endregion

        #region === VALIDATION METHODS ===
        /// <summary>
        /// Set control to valid state with optional message
        /// </summary>
        public void SetValid(string message = "")
        {
            Validation = ValidationState.Valid;
            ValidationMessage = message;
        }

        /// <summary>
        /// Set control to invalid state with error message
        /// </summary>
        public void SetInvalid(string message)
        {
            Validation = ValidationState.Invalid;
            ValidationMessage = message;
        }

        /// <summary>
        /// Set control to warning state with message
        /// </summary>
        public void SetWarning(string message)
        {
            Validation = ValidationState.Warning;
            ValidationMessage = message;
        }

        /// <summary>
        /// Clear validation state
        /// </summary>
        public void ClearValidation()
        {
            Validation = ValidationState.None;
            ValidationMessage = "";
        }

        private void CreateValidationControls()
        {
            // Validation icon
            _validationIcon = new PictureBox
            {
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Visible = false
            };

            // Validation label
            _validationLabel = new Label
            {
                AutoSize = true,
                Font = AppTheme.FontCaption,
                Visible = false,
                BackColor = Color.Transparent
            };
        }

        private void UpdateValidationVisual()
        {
            // Update border color and background based on validation state
            switch (_validationState)
            {
                case ValidationState.Invalid:
                    borderColor = AppTheme.ValidationError;
                    // Subtle tint - don't change BackColor to avoid affecting text visibility
                    break;

                case ValidationState.Valid:
                    borderColor = AppTheme.ValidationSuccess;
                    break;

                case ValidationState.Warning:
                    borderColor = AppTheme.ValidationWarning;
                    break;

                default:
                    borderColor = AppTheme.BorderMedium;
                    break;
            }

            UpdateValidationIcon();
            UpdateValidationLabel();
        }

        private void UpdateValidationIcon()
        {
            if (_validationIcon == null) return;

            // Remove from parent first
            if (_validationIcon.Parent != null)
                _validationIcon.Parent.Controls.Remove(_validationIcon);

            if (!_showValidationIcon || _validationState == ValidationState.None)
            {
                _validationIcon.Visible = false;
                // Reset textbox padding
                textBox1.Margin = new Padding(0);
                return;
            }

            // Add to this control
            this.Controls.Add(_validationIcon);
            _validationIcon.BringToFront();

            // Position at right edge
            int iconX = this.Width - _validationIcon.Width - Padding.Right - 2;
            int iconY = (this.Height - _validationIcon.Height) / 2;
            _validationIcon.Location = new Point(iconX, iconY);

            // Set icon based on state
            _validationIcon.Image = CreateValidationIcon(_validationState);
            _validationIcon.Visible = true;

            // Add right margin to textbox to avoid overlapping icon
            textBox1.Margin = new Padding(0, 0, 20, 0);
        }

        private Image CreateValidationIcon(ValidationState state)
        {
            int size = 14;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Color iconColor;
                switch (state)
                {
                    case ValidationState.Valid:
                        iconColor = AppTheme.ValidationSuccess;
                        // Draw checkmark
                        using (var pen = new Pen(iconColor, 2))
                        {
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            g.DrawLine(pen, 3, 7, 6, 10);
                            g.DrawLine(pen, 6, 10, 11, 4);
                        }
                        break;

                    case ValidationState.Invalid:
                        iconColor = AppTheme.ValidationError;
                        // Draw X
                        using (var pen = new Pen(iconColor, 2))
                        {
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            g.DrawLine(pen, 4, 4, 10, 10);
                            g.DrawLine(pen, 10, 4, 4, 10);
                        }
                        break;

                    case ValidationState.Warning:
                        iconColor = AppTheme.ValidationWarning;
                        // Draw exclamation mark
                        using (var pen = new Pen(iconColor, 2))
                        using (var brush = new SolidBrush(iconColor))
                        {
                            g.DrawLine(pen, 7, 3, 7, 8);
                            g.FillEllipse(brush, 6, 10, 2, 2);
                        }
                        break;

                    default:
                        return null;
                }
            }
            return bmp;
        }

        private void UpdateValidationLabel()
        {
            if (_validationLabel == null) return;

            // Remove from parent first
            if (_validationLabel.Parent != null && _validationLabel.Parent != this.Parent)
                _validationLabel.Parent.Controls.Remove(_validationLabel);

            if (!_showValidationMessage || string.IsNullOrEmpty(_validationMessage) || _validationState == ValidationState.None)
            {
                _validationLabel.Visible = false;
                return;
            }

            // Add to parent control
            if (this.Parent != null)
            {
                if (!this.Parent.Controls.Contains(_validationLabel))
                    this.Parent.Controls.Add(_validationLabel);

                // Position below this control
                _validationLabel.Location = new Point(this.Left + 2, this.Bottom + 2);
                _validationLabel.Text = _validationMessage;

                // Set color based on state
                switch (_validationState)
                {
                    case ValidationState.Invalid:
                        _validationLabel.ForeColor = AppTheme.ValidationError;
                        break;
                    case ValidationState.Warning:
                        _validationLabel.ForeColor = AppTheme.ValidationWarning;
                        break;
                    case ValidationState.Valid:
                        _validationLabel.ForeColor = AppTheme.ValidationSuccess;
                        break;
                    default:
                        _validationLabel.ForeColor = AppTheme.TextMuted;
                        break;
                }

                _validationLabel.Visible = true;
                _validationLabel.BringToFront();
            }
        }
        #endregion

        #region === PLACEHOLDER METHODS ===
        private void SetPlaceholder()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrEmpty(placeholderText))
            {
                isPlaceholder = true;
                textBox1.Text = placeholderText;
                textBox1.ForeColor = placeholderColor;
                if (isPasswordChar) textBox1.UseSystemPasswordChar = false;
            }
        }

        private void RemovePlaceholder()
        {
            if (isPlaceholder && !string.IsNullOrEmpty(placeholderText))
            {
                isPlaceholder = false;
                textBox1.Text = "";
                textBox1.ForeColor = this.ForeColor;
                if (isPasswordChar) textBox1.UseSystemPasswordChar = true;
            }
        }
        #endregion

        #region === PAINTING ===
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Determine current border color
            Color currentBorderColor = GetCurrentBorderColor();

            if (borderRadius > 1) // Rounded TextBox
            {
                var rectBorderSmooth = this.ClientRectangle;
                var rectBorder = Rectangle.Inflate(rectBorderSmooth, -borderSize, -borderSize);
                int smoothSize = borderSize > 0 ? borderSize : 1;

                using (var pathBorderSmooth = GetFigurePath(rectBorderSmooth, borderRadius))
                using (var pathBorder = GetFigurePath(rectBorder, borderRadius - borderSize))
                using (var penBorderSmooth = new Pen(this.Parent?.BackColor ?? BackColor, smoothSize))
                using (var penBorder = new Pen(currentBorderColor, borderSize))
                {
                    this.Region = new Region(pathBorderSmooth);
                    if (borderRadius > 15) SetTextBoxRoundedRegion();

                    g.DrawPath(penBorderSmooth, pathBorderSmooth);

                    if (underlinedStyle)
                        g.DrawLine(penBorder, 0, this.Height - 1, this.Width, this.Height - 1);
                    else
                        g.DrawPath(penBorder, pathBorder);
                }

                // Draw focus ring when focused
                if (isFocused && _validationState == ValidationState.None)
                {
                    using (var focusPath = GetFigurePath(Rectangle.Inflate(rectBorderSmooth, 2, 2), borderRadius + 2))
                    using (var focusPen = new Pen(Color.FromArgb(40, AppTheme.Primary), 3))
                    {
                        g.DrawPath(focusPen, focusPath);
                    }
                }
            }
            else // Square/Normal TextBox
            {
                using (var penBorder = new Pen(currentBorderColor, borderSize))
                {
                    this.Region = new Region(this.ClientRectangle);
                    penBorder.Alignment = PenAlignment.Inset;

                    if (underlinedStyle)
                        g.DrawLine(penBorder, 0, this.Height - 1, this.Width, this.Height - 1);
                    else
                        g.DrawRectangle(penBorder, 0, 0, this.Width - 0.5F, this.Height - 0.5F);
                }
            }
        }

        private Color GetCurrentBorderColor()
        {
            // Validation state takes priority
            if (_validationState != ValidationState.None)
            {
                switch (_validationState)
                {
                    case ValidationState.Invalid:
                        return AppTheme.ValidationError;
                    case ValidationState.Valid:
                        return AppTheme.ValidationSuccess;
                    case ValidationState.Warning:
                        return AppTheme.ValidationWarning;
                    default:
                        return borderColor;
                }
            }

            // Then focus state
            if (isFocused)
                return borderFocusColor;

            // Then hover state
            if (isHovered)
                return borderHoverColor;

            // Default
            return borderColor;
        }

        private void SetTextBoxRoundedRegion()
        {
            GraphicsPath pathTxt;
            if (Multiline)
            {
                pathTxt = GetFigurePath(textBox1.ClientRectangle, borderRadius - borderSize);
                textBox1.Region = new Region(pathTxt);
            }
            else
            {
                pathTxt = GetFigurePath(textBox1.ClientRectangle, borderSize * 2);
                textBox1.Region = new Region(pathTxt);
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

            float diameter = radius * 2;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
        #endregion

        #region === TEXTBOX EVENTS ===
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            // Clear validation when user types (optional behavior)
            if (!isPlaceholder && _validationState != ValidationState.None)
            {
                // Keep validation until explicitly cleared or re-validated
            }

            _TextChanged?.Invoke(sender, e);
        }

        private void TextBox1_Click(object sender, EventArgs e) => OnClick(e);

        private void TextBox1_MouseEnter(object sender, EventArgs e)
        {
            isHovered = true;
            Invalidate();
            OnMouseEnter(e);
        }

        private void TextBox1_MouseLeave(object sender, EventArgs e)
        {
            isHovered = false;
            Invalidate();
            OnMouseLeave(e);
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e) => OnKeyPress(e);

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            isFocused = true;
            Invalidate();
            RemovePlaceholder();
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            isFocused = false;
            Invalidate();
            SetPlaceholder();
        }
        #endregion

        #region === LIFECYCLE ===
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode) UpdateControlHeight();
            UpdateValidationIcon();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateControlHeight();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            UpdateValidationLabel();
        }

        private void UpdateControlHeight()
        {
            if (!textBox1.Multiline)
            {
                int txtHeight = TextRenderer.MeasureText("Text", this.Font).Height + 1;
                textBox1.Multiline = true;
                textBox1.MinimumSize = new Size(0, txtHeight);
                textBox1.Multiline = false;
                this.Height = textBox1.Height + this.Padding.Top + this.Padding.Bottom;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _validationIcon?.Dispose();
                _validationLabel?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region === PUBLIC METHODS ===
        /// <summary>
        /// Focus the internal textbox
        /// </summary>
        public new void Focus()
        {
            textBox1.Focus();
        }

        /// <summary>
        /// Select all text
        /// </summary>
        public void SelectAll()
        {
            textBox1.SelectAll();
        }

        /// <summary>
        /// Clear all text
        /// </summary>
        public void Clear()
        {
            textBox1.Clear();
            SetPlaceholder();
        }
        #endregion
    }
}
