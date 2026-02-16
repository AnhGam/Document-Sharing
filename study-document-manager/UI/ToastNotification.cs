using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using study_document_manager.UI;

namespace study_document_manager
{
    public enum ToastType { Success, Error, Warning, Info }

    /// <summary>
    /// Modern Toast Notification v3 — Light theme, inside parent form, top-right
    /// </summary>
    public class ToastNotification : Form
    {
        // ── Static ──
        private static readonly List<ToastNotification> _active = new List<ToastNotification>();
        private static readonly object _lock = new object();
        private static Form _parentForm;

        // ── Instance ──
        private readonly ToastType _type;
        private readonly string _message;
        private readonly int _durationMs;
        private readonly Timer _progressTimer;
        private readonly Timer _animTimer;
        private int _elapsed;
        private double _slideOffset; // pixels from target, for slide-in
        private bool _closing;

        // ── Layout ──
        private const int W = 340, H = 62, GAP = 6, MARGIN = 12;
        private const int BAR_H = 3, ICON_SIZE = 30, RADIUS = 8;
        private const int SLIDE_START = 60; // slide-in distance

        private ToastNotification(string message, ToastType type, int durationMs)
        {
            _type = type;
            _message = message;
            _durationMs = durationMs;
            _slideOffset = SLIDE_START;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(W, H);
            Opacity = 0;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Progress countdown
            _progressTimer = new Timer { Interval = 40 };
            _progressTimer.Tick += (s, e) =>
            {
                _elapsed += 40;
                if (_elapsed >= _durationMs) { _progressTimer.Stop(); BeginClose(); }
                Invalidate();
            };

            // Slide + fade animation
            _animTimer = new Timer { Interval = 14 };
            _animTimer.Tick += OnAnimTick;

            Click += (s, e) => BeginClose();
        }

        // ── Drop Shadow ──
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000;
                return cp;
            }
        }

        // ── Color Scheme ──
        private struct Scheme
        {
            public Color Accent, LightBg, Border;
            public string Symbol;
        }

        private static Scheme GetScheme(ToastType t)
        {
            switch (t)
            {
                case ToastType.Success:
                    return new Scheme
                    {
                        Accent = Color.FromArgb(22, 163, 74),    // Green-600
                        LightBg = Color.FromArgb(240, 253, 244), // Green-50
                        Border = Color.FromArgb(187, 247, 208),   // Green-200
                        Symbol = "✓"
                    };
                case ToastType.Error:
                    return new Scheme
                    {
                        Accent = Color.FromArgb(220, 38, 38),    // Red-600
                        LightBg = Color.FromArgb(254, 242, 242), // Red-50
                        Border = Color.FromArgb(254, 202, 202),   // Red-200
                        Symbol = "✕"
                    };
                case ToastType.Warning:
                    return new Scheme
                    {
                        Accent = Color.FromArgb(202, 138, 4),     // Amber-600
                        LightBg = Color.FromArgb(255, 251, 235), // Amber-50
                        Border = Color.FromArgb(253, 230, 138),   // Amber-200
                        Symbol = "!"
                    };
                default:
                    return new Scheme
                    {
                        Accent = Color.FromArgb(37, 99, 235),    // Blue-600
                        LightBg = Color.FromArgb(239, 246, 255), // Blue-50
                        Border = Color.FromArgb(191, 219, 254),   // Blue-200
                        Symbol = "i"
                    };
            }
        }

        // ── Paint ──
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var sc = GetScheme(_type);
            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);

            // Background
            using (var path = RoundRect(bounds, RADIUS))
            using (var bg = new SolidBrush(sc.LightBg))
            {
                g.FillPath(bg, path);
            }

            // Left accent strip
            g.SetClip(new Rectangle(0, 0, 5, Height));
            using (var path = RoundRect(bounds, RADIUS))
            using (var br = new SolidBrush(sc.Accent))
            {
                g.FillPath(br, path);
            }
            g.ResetClip();

            // Icon circle
            int iconX = 14, iconY = (H - BAR_H - ICON_SIZE) / 2;
            var iconRect = new Rectangle(iconX, iconY, ICON_SIZE, ICON_SIZE);
            using (var br = new SolidBrush(Color.FromArgb(35, sc.Accent)))
                g.FillEllipse(br, iconRect);
            using (var font = new Font("Segoe UI", 11f, FontStyle.Bold))
            using (var br = new SolidBrush(sc.Accent))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(sc.Symbol, font, br, iconRect, sf);
            }

            // Message text
            int textX = iconX + ICON_SIZE + 10;
            int textW = Width - textX - 12;
            var msgRect = new RectangleF(textX, 0, textW, H - BAR_H);
            using (var font = new Font("Segoe UI", 9.5f))
            using (var br = new SolidBrush(Color.FromArgb(55, 65, 81))) // Gray-700
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                g.DrawString(_message, font, br, msgRect, sf);
            }

            // Progress bar
            float progress = Math.Max(0, 1f - (float)_elapsed / _durationMs);
            int barW = (int)(Width * progress);
            var barBounds = new Rectangle(0, Height - BAR_H, Width, BAR_H);

            // Track
            using (var br = new SolidBrush(Color.FromArgb(40, sc.Accent)))
                g.FillRectangle(br, barBounds);

            // Fill
            if (barW > 1)
            {
                var barFill = new Rectangle(0, Height - BAR_H, barW, BAR_H);
                using (var br = new SolidBrush(Color.FromArgb(180, sc.Accent)))
                    g.FillRectangle(br, barFill);
            }

            // Border
            using (var path = RoundRect(bounds, RADIUS))
            using (var pen = new Pen(sc.Border, 1f))
            {
                g.DrawPath(pen, path);
            }
        }

        private static GraphicsPath RoundRect(Rectangle r, int rad)
        {
            var p = new GraphicsPath();
            int d = rad * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        // ── Positioning: inside parent form, top-right ──
        private void SetPosition()
        {
            if (IsDisposed) return;
            int index;
            lock (_lock) { index = _active.IndexOf(this); }
            if (index < 0) return;

            int yOffset = (H + GAP) * index;
            int xSlide = (int)_slideOffset;

            if (_parentForm != null && !_parentForm.IsDisposed)
            {
                // Inside parent form, top-right corner
                int x = _parentForm.Right - W - MARGIN + xSlide;
                int y = _parentForm.Top + MARGIN + yOffset + 60; // below title bar + toolbar
                Location = new Point(x, y);
            }
            else
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(screen.Right - W - MARGIN + xSlide, screen.Top + MARGIN + yOffset);
            }
        }

        private static void RepositionAll()
        {
            lock (_lock)
            {
                foreach (var t in _active)
                    if (!t.IsDisposed) t.SetPosition();
            }
        }

        // ── Animation ──
        private void OnAnimTick(object sender, EventArgs e)
        {
            if (IsDisposed) { _animTimer.Stop(); return; }

            if (!_closing)
            {
                // Slide in from right + fade in
                _slideOffset += (0 - _slideOffset) * 0.22;
                Opacity = Math.Min(0.97, Opacity + 0.1);

                if (_slideOffset < 0.5 && Opacity >= 0.95)
                {
                    _slideOffset = 0;
                    Opacity = 0.97;
                    _animTimer.Stop();
                    _progressTimer.Start();
                }
                SetPosition();
            }
            else
            {
                // Slide out to right + fade out
                _slideOffset += (SLIDE_START - _slideOffset) * 0.25;
                Opacity = Math.Max(0, Opacity - 0.08);
                if (Opacity <= 0.02)
                {
                    _animTimer.Stop();
                    DoClose();
                    return;
                }
                SetPosition();
            }
        }

        private void BeginClose()
        {
            if (_closing) return;
            _closing = true;
            _progressTimer.Stop();
            _animTimer.Start();
        }

        private void DoClose()
        {
            if (IsDisposed) return;
            lock (_lock) { _active.Remove(this); }
            RepositionAll();
            _progressTimer?.Dispose();
            _animTimer?.Dispose();
            try { Close(); } catch { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _progressTimer?.Dispose();
            _animTimer?.Dispose();
            base.OnFormClosed(e);
        }

        // ── Static API ──
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
            lock (_lock) { _active.Add(toast); }
            toast.SetPosition();
            toast.Show(_parentForm);
            toast._animTimer.Start();
        }

        public static void Success(string msg, int ms = 3000) => Show(msg, ToastType.Success, ms);
        public static void Error(string msg, int ms = 4000) => Show(msg, ToastType.Error, ms);
        public static void Warning(string msg, int ms = 3500) => Show(msg, ToastType.Warning, ms);
        public static void Info(string msg, int ms = 3000) => Show(msg, ToastType.Info, ms);

        public static void CloseAll()
        {
            lock (_lock)
            {
                var copy = new List<ToastNotification>(_active);
                foreach (var t in copy)
                    if (!t.IsDisposed) t.BeginClose();
            }
        }
    }
}
