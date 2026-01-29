using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    /// <summary>
    /// Modern Toast Notification with AppTheme Integration
    /// Design: Teal Primary Theme, Glassmorphism, Progress Indicator
    /// </summary>
    public class ToastNotification : Form
    {
        #region Static Management
        private static readonly List<ToastNotification> ActiveToasts = new List<ToastNotification>();
        private static readonly object LockObject = new object();
        private static Form _parentForm;
        #endregion

        #region Instance Fields
        private readonly Timer _closeTimer;
        private readonly Timer _fadeTimer;
        private readonly Timer _progressTimer;
        private readonly ToastType _toastType;
        private readonly string _message;
        private readonly int _totalDuration;
        private int _elapsedMs;
        private bool _isHovered;
        private bool _isClosing;
        private readonly float _dpiScale;
        #endregion

        #region Design Constants - Base Values (at 96 DPI)
        private const int BaseToastWidth = 400;
        private const int BaseToastHeight = 80;
        private const int BaseToastMargin = 16;
        private const int BaseProgressHeight = 4;
        private const int AnimationInterval = 16; // ~60fps
        private const double FadeStep = 0.12;

        // Instance-based scaled values (cached DPI)
        private int ToastWidthScaled => (int)(BaseToastWidth * _dpiScale);
        private int ToastHeightScaled => (int)(BaseToastHeight * _dpiScale);
        private int ToastMarginScaled => (int)(BaseToastMargin * _dpiScale);
        private int BorderRadiusScaled => (int)(AppTheme.BorderRadius * _dpiScale);
        private int ProgressHeightScaled => (int)(BaseProgressHeight * _dpiScale);

        // Static values for positioning
        private static float _cachedDpiScale = 0;
        private static float CachedDpiScale
        {
            get
            {
                if (_cachedDpiScale == 0)
                {
                    using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        _cachedDpiScale = g.DpiX / 96f;
                    }
                }
                return _cachedDpiScale;
            }
        }
        private static int ToastWidth => (int)(BaseToastWidth * CachedDpiScale);
        private static int ToastHeight => (int)(BaseToastHeight * CachedDpiScale);
        private static int ToastMargin => (int)(BaseToastMargin * CachedDpiScale);
        #endregion

        #region Constructor
        private ToastNotification(string message, ToastType type, int durationMs = 3000)
        {
            _toastType = type;
            _message = message;
            _totalDuration = durationMs;
            _elapsedMs = 0;

            // Calculate DPI scale once from parent form or primary monitor
            if (_parentForm != null && !_parentForm.IsDisposed)
            {
                using (var g = _parentForm.CreateGraphics())
                {
                    _dpiScale = g.DpiX / 96f;
                }
            }
            else
            {
                _dpiScale = CachedDpiScale;
            }

            // Form Setup
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(ToastWidthScaled, ToastHeightScaled);
            Opacity = 0;
            BackColor = Color.White;

            // High-quality rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            // Close timer
            _closeTimer = new Timer { Interval = durationMs };
            _closeTimer.Tick += (s, e) =>
            {
                _closeTimer.Stop();
                StartFadeOut();
            };

            // Fade animation timer
            _fadeTimer = new Timer { Interval = AnimationInterval };

            // Progress timer
            _progressTimer = new Timer { Interval = 50 };
            _progressTimer.Tick += (s, e) =>
            {
                if (!_isHovered && !_isClosing)
                {
                    _elapsedMs += 50;
                    Invalidate();
                }
            };

            // Mouse events
            MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
            Click += (s, e) => StartFadeOut();
        }
        #endregion

        #region Windows API - Drop Shadow
        protected override CreateParams CreateParams
        {
            get
            {
                if (IsDisposed) return base.CreateParams;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000; // CS_DROPSHADOW
                return cp;
            }
        }
        #endregion

        #region Paint - Main Rendering
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            var palette = GetPalette(_toastType);

            // Layer 1: Glow Effect
            DrawGlowEffect(g, bounds, palette.Glow);

            // Layer 2: Background with gradient
            DrawBackground(g, bounds, palette.LightBg);

            // Layer 3: Left Accent Bar
            DrawAccentBar(g, bounds, palette.Primary, palette.PrimaryDark);

            // Layer 4: Icon Circle
            DrawIcon(g, bounds, palette.Primary, palette.PrimaryDark, palette.LightBg);

            // Layer 5: Message Text
            DrawMessage(g, bounds, palette.Primary);

            // Layer 6: Close Button
            DrawCloseButton(g, bounds);

            // Layer 7: Progress Bar
            DrawProgressBar(g, bounds, palette.Primary, palette.PrimaryDark);

            // Layer 8: Border
            DrawBorder(g, bounds, palette.Border);
        }

        private void DrawGlowEffect(Graphics g, Rectangle bounds, Color glowColor)
        {
            int glowSize = 10;
            var glowRect = new Rectangle(bounds.X + glowSize, bounds.Y + glowSize,
                                          bounds.Width - glowSize * 2, bounds.Height - glowSize);
            using (var path = CreateRoundedRectangle(glowRect, BorderRadiusScaled))
            using (var brush = new SolidBrush(glowColor))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawBackground(Graphics g, Rectangle bounds, Color bgColor)
        {
            using (var path = CreateRoundedRectangle(bounds, BorderRadiusScaled))
            {
                // Gradient from light tint to white
                using (var brush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(250, bgColor),
                    AppTheme.BackgroundMain,
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawAccentBar(Graphics g, Rectangle bounds, Color startColor, Color endColor)
        {
            int barWidth = (int)(6 * _dpiScale);
            int barMargin = (int)(14 * _dpiScale);
            int barHeight = bounds.Height - barMargin * 2 - ProgressHeightScaled;
            var barRect = new Rectangle(barMargin, barMargin, barWidth, barHeight);

            using (var path = CreateRoundedRectangle(barRect, barWidth / 2))
            using (var brush = new LinearGradientBrush(
                barRect,
                startColor,
                endColor,
                LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawIcon(Graphics g, Rectangle bounds, Color startColor, Color endColor, Color tintColor)
        {
            int iconSize = (int)(44 * _dpiScale);
            int iconX = (int)(32 * _dpiScale);
            int iconY = (bounds.Height - iconSize - ProgressHeightScaled) / 2;
            var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);

            // Outer circle with tint
            using (var brush = new SolidBrush(Color.FromArgb(160, tintColor)))
            {
                g.FillEllipse(brush, iconRect);
            }

            // Inner circle with gradient
            int innerPadding = (int)(5 * _dpiScale);
            var innerRect = new Rectangle(iconX + innerPadding, iconY + innerPadding,
                                           iconSize - innerPadding * 2, iconSize - innerPadding * 2);
            using (var brush = new LinearGradientBrush(
                innerRect,
                startColor,
                endColor,
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillEllipse(brush, innerRect);
            }

            // Icon symbol
            string symbol = GetIconSymbol(_toastType);
            float fontSize = 13 * _dpiScale;
            using (var font = new Font(AppTheme.FontFamily, fontSize, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(symbol, font, brush, innerRect, sf);
            }
        }

        private void DrawMessage(Graphics g, Rectangle bounds, Color accentColor)
        {
            int textLeft = (int)(90 * _dpiScale);
            int textRight = (int)(50 * _dpiScale);
            int availableHeight = Height - ProgressHeightScaled;

            // Title
            string title = GetTitle(_toastType);
            float titleSize = 9.5f * _dpiScale;
            using (var titleFont = new Font(AppTheme.FontFamily + " Semibold", titleSize))
            using (var brush = new SolidBrush(accentColor))
            {
                var titleRect = new RectangleF(textLeft, 14 * _dpiScale, Width - textLeft - textRight, 20 * _dpiScale);
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
                g.DrawString(title, titleFont, brush, titleRect, sf);
            }

            // Message
            float msgSize = 10.5f * _dpiScale;
            using (var msgFont = new Font(AppTheme.FontFamily, msgSize))
            using (var brush = new SolidBrush(AppTheme.TextPrimary))
            {
                var msgRect = new RectangleF(textLeft, 38 * _dpiScale, Width - textLeft - textRight, 28 * _dpiScale);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                g.DrawString(_message, msgFont, brush, msgRect, sf);
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle bounds)
        {
            int btnSize = (int)(30 * _dpiScale);
            int btnMargin = (int)(12 * _dpiScale);
            var btnRect = new Rectangle(bounds.Right - btnSize - btnMargin, btnMargin, btnSize, btnSize);

            var mousePos = PointToClient(MousePosition);
            bool isHovered = btnRect.Contains(mousePos);

            // Hover background
            if (isHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, AppTheme.TextSecondary)))
                {
                    g.FillEllipse(brush, btnRect);
                }
            }

            // X symbol
            float fontSize = 11 * _dpiScale;
            using (var font = new Font(AppTheme.FontFamily, fontSize, FontStyle.Regular))
            using (var brush = new SolidBrush(isHovered ? AppTheme.TextPrimary : AppTheme.TextMuted))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("✕", font, brush, btnRect, sf);
            }
        }

        private void DrawProgressBar(Graphics g, Rectangle bounds, Color startColor, Color endColor)
        {
            // Background track
            var trackRect = new Rectangle(0, bounds.Height - ProgressHeightScaled, bounds.Width, ProgressHeightScaled);

            // Round bottom corners
            using (var clipPath = CreateBottomRoundedRectangle(trackRect, BorderRadiusScaled))
            {
                g.SetClip(clipPath);

                using (var brush = new SolidBrush(Color.FromArgb(40, startColor)))
                {
                    g.FillRectangle(brush, trackRect);
                }

                // Progress fill
                float progress = 1f - ((float)_elapsedMs / _totalDuration);
                if (progress < 0) progress = 0;
                if (progress > 1) progress = 1;

                int progressWidth = (int)(bounds.Width * progress);
                var progressRect = new Rectangle(0, bounds.Height - ProgressHeightScaled, progressWidth, ProgressHeightScaled);

                if (progressWidth > 0)
                {
                    using (var brush = new LinearGradientBrush(
                        new Rectangle(0, bounds.Height - ProgressHeightScaled, bounds.Width, ProgressHeightScaled),
                        startColor,
                        endColor,
                        LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(brush, progressRect);
                    }
                }

                g.ResetClip();
            }
        }

        private void DrawBorder(Graphics g, Rectangle bounds, Color borderColor)
        {
            using (var path = CreateRoundedRectangle(bounds, BorderRadiusScaled))
            using (var pen = new Pen(Color.FromArgb(100, borderColor), 1.5f))
            {
                g.DrawPath(pen, path);
            }
        }
        #endregion

        #region Helper Methods
        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private GraphicsPath CreateBottomRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            // Top left - no radius
            path.AddLine(bounds.X, bounds.Y, bounds.Right, bounds.Y);
            // Top right - no radius
            path.AddLine(bounds.Right, bounds.Y, bounds.Right, bounds.Bottom - radius);
            // Bottom right arc
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            // Bottom left arc
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private struct ColorPalette
        {
            public Color Primary;
            public Color PrimaryDark;
            public Color LightBg;
            public Color Border;
            public Color Glow;
        }

        private static ColorPalette GetPalette(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success:
                    return new ColorPalette
                    {
                        Primary = AppTheme.StatusSuccess,
                        PrimaryDark = Color.FromArgb(22, 163, 74), // Green-600
                        LightBg = AppTheme.ValidationSuccessLight,
                        Border = AppTheme.ValidationSuccessBorder,
                        Glow = AppTheme.GlowSuccess
                    };
                case ToastType.Error:
                    return new ColorPalette
                    {
                        Primary = AppTheme.StatusError,
                        PrimaryDark = Color.FromArgb(220, 38, 38), // Red-600
                        LightBg = AppTheme.ValidationErrorLight,
                        Border = AppTheme.ValidationErrorBorder,
                        Glow = AppTheme.GlowDanger
                    };
                case ToastType.Warning:
                    return new ColorPalette
                    {
                        Primary = AppTheme.StatusWarning,
                        PrimaryDark = Color.FromArgb(202, 138, 4), // Yellow-600
                        LightBg = AppTheme.ValidationWarningLight,
                        Border = AppTheme.ValidationWarningBorder,
                        Glow = AppTheme.GlowWarning
                    };
                default: // Info
                    return new ColorPalette
                    {
                        Primary = AppTheme.StatusInfo,
                        PrimaryDark = Color.FromArgb(37, 99, 235), // Blue-600
                        LightBg = Color.FromArgb(239, 246, 255), // Blue-50
                        Border = Color.FromArgb(147, 197, 253), // Blue-300
                        Glow = Color.FromArgb(60, 59, 130, 246) // Info glow
                    };
            }
        }

        private static string GetIconSymbol(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success: return "✓";
                case ToastType.Error: return "✕";
                case ToastType.Warning: return "!";
                default: return "i";
            }
        }

        private static string GetTitle(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success: return "THANH CONG";
                case ToastType.Error: return "LOI";
                case ToastType.Warning: return "CANH BAO";
                default: return "THONG BAO";
            }
        }
        #endregion

        #region Positioning
        private void PositionToast()
        {
            if (IsDisposed) return;

            int yOffset;
            lock (LockObject)
            {
                int index = ActiveToasts.IndexOf(this);
                if (index < 0) return;
                yOffset = (ToastHeight + ToastMargin) * index;
            }

            if (_parentForm != null && !_parentForm.IsDisposed)
            {
                int x = _parentForm.Left + _parentForm.Width - ToastWidth - ToastMargin;
                int y = _parentForm.Top + ToastMargin + yOffset + 50;
                Location = new Point(x, y);
            }
            else
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(screen.Right - ToastWidth - ToastMargin, screen.Top + ToastMargin + yOffset);
            }
        }

        private static void RepositionToasts()
        {
            lock (LockObject)
            {
                foreach (var toast in ActiveToasts)
                    if (!toast.IsDisposed) toast.PositionToast();
            }
        }
        #endregion

        #region Animation
        private void StartFadeIn()
        {
            _fadeTimer.Tick += FadeIn_Tick;
            _fadeTimer.Start();
            _progressTimer.Start();
        }

        private void FadeIn_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) { _fadeTimer.Stop(); return; }
            if (Opacity < 1)
            {
                Opacity = Math.Min(1, Opacity + FadeStep);
            }
            else
            {
                Opacity = 1;
                _fadeTimer.Stop();
                _fadeTimer.Tick -= FadeIn_Tick;
                _closeTimer.Start();
            }
        }

        private void StartFadeOut()
        {
            if (_isClosing) return;
            _isClosing = true;
            _closeTimer.Stop();
            _progressTimer.Stop();
            _fadeTimer.Tick -= FadeIn_Tick;
            _fadeTimer.Tick += FadeOut_Tick;
            _fadeTimer.Start();
        }

        private void FadeOut_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) { _fadeTimer.Stop(); return; }
            if (Opacity > 0)
            {
                Opacity = Math.Max(0, Opacity - FadeStep);
            }
            else
            {
                _fadeTimer.Stop();
                _fadeTimer.Tick -= FadeOut_Tick;
                CloseToast();
            }
        }

        private void CloseToast()
        {
            if (IsDisposed) return;
            lock (LockObject)
            {
                ActiveToasts.Remove(this);
                RepositionToasts();
            }
            _closeTimer?.Stop();
            _fadeTimer?.Stop();
            _progressTimer?.Stop();
            try { Close(); } catch (ObjectDisposedException) { }
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            StartFadeOut();
        }
        #endregion

        #region Lifecycle
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _closeTimer?.Dispose();
            _fadeTimer?.Dispose();
            _progressTimer?.Dispose();
            base.OnFormClosed(e);
        }
        #endregion

        #region Static API
        public static void Show(string message, ToastType type = ToastType.Info, int durationMs = 3000)
        {
            if (Application.OpenForms.Count == 0) return;
            _parentForm = Form.ActiveForm ?? Application.OpenForms[0];

            if (_parentForm.InvokeRequired)
                _parentForm.BeginInvoke(new Action(() => ShowInternal(message, type, durationMs)));
            else
                ShowInternal(message, type, durationMs);
        }

        private static void ShowInternal(string message, ToastType type, int durationMs)
        {
            var toast = new ToastNotification(message, type, durationMs);
            lock (LockObject) { ActiveToasts.Add(toast); }
            toast.PositionToast();
            toast.Show(_parentForm);
            toast.StartFadeIn();
        }

        // Convenience methods
        public static void Success(string message, int durationMs = 3000) => Show(message, ToastType.Success, durationMs);
        public static void Error(string message, int durationMs = 4000) => Show(message, ToastType.Error, durationMs);
        public static void Warning(string message, int durationMs = 3500) => Show(message, ToastType.Warning, durationMs);
        public static void Info(string message, int durationMs = 3000) => Show(message, ToastType.Info, durationMs);

        public static void CloseAll()
        {
            lock (LockObject)
            {
                var toastsToClose = new List<ToastNotification>(ActiveToasts);
                foreach (var toast in toastsToClose)
                {
                    if (!toast.IsDisposed) toast.StartFadeOut();
                }
            }
        }
        #endregion
    }
}
