using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace study_document_manager
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public class ToastNotification : Form
    {
        private static readonly List<ToastNotification> ActiveToasts = new List<ToastNotification>();
        private static readonly object LockObject = new object();
        private static Form _parentForm;
        
        private readonly Timer _closeTimer;
        private readonly Timer _fadeTimer;
        private readonly ToastType _toastType;
        private readonly string _message;
        private const int ToastWidth = 320;
        private const int ToastHeight = 48;
        private const int ToastMargin = 12;
        private const int AnimationInterval = 16;
        private const double FadeStep = 0.12;
        private const int BorderRadius = 8;

        private static readonly Color SuccessBg = Color.FromArgb(240, 253, 244);
        private static readonly Color SuccessAccent = Color.FromArgb(34, 197, 94);
        private static readonly Color SuccessText = Color.FromArgb(21, 128, 61);
        
        private static readonly Color ErrorBg = Color.FromArgb(254, 242, 242);
        private static readonly Color ErrorAccent = Color.FromArgb(239, 68, 68);
        private static readonly Color ErrorText = Color.FromArgb(185, 28, 28);
        
        private static readonly Color WarningBg = Color.FromArgb(255, 251, 235);
        private static readonly Color WarningAccent = Color.FromArgb(245, 158, 11);
        private static readonly Color WarningText = Color.FromArgb(180, 83, 9);
        
        private static readonly Color InfoBg = Color.FromArgb(239, 246, 255);
        private static readonly Color InfoAccent = Color.FromArgb(59, 130, 246);
        private static readonly Color InfoText = Color.FromArgb(29, 78, 216);

        private static readonly Color CloseButtonColor = Color.FromArgb(156, 163, 175);

        private ToastNotification(string message, ToastType type, int durationMs = 3000)
        {
            _toastType = type;
            _message = message;
            
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(ToastWidth, ToastHeight);
            Opacity = 0;
            BackColor = GetBackgroundColor(type);
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            _closeTimer = new Timer { Interval = durationMs };
            _closeTimer.Tick += (s, e) =>
            {
                _closeTimer.Stop();
                StartFadeOut();
            };

            _fadeTimer = new Timer { Interval = AnimationInterval };

            Click += (s, e) => StartFadeOut();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                if (IsDisposed) return base.CreateParams;
                
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);
            
            using (var path = CreateRoundedRectangle(bounds, BorderRadius))
            {
                Region = new Region(path);
                
                using (var bgBrush = new SolidBrush(GetBackgroundColor(_toastType)))
                {
                    g.FillPath(bgBrush, path);
                }
                
                DrawLeftAccent(g);
                DrawIcon(g);
                DrawMessage(g);
                DrawCloseButton(g);
            }
        }

        private void DrawLeftAccent(Graphics g)
        {
            var accentColor = GetAccentColor(_toastType);
            var accentRect = new Rectangle(0, 0, 4, Height);
            
            using (var accentBrush = new SolidBrush(accentColor))
            using (var accentPath = CreateRoundedRectangle(accentRect, BorderRadius, true, false, false, true))
            {
                g.FillPath(accentBrush, accentPath);
            }
        }

        private void DrawIcon(Graphics g)
        {
            var accentColor = GetAccentColor(_toastType);
            var iconText = GetIconText(_toastType);
            
            using (var iconFont = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var iconBrush = new SolidBrush(accentColor))
            {
                var iconSize = g.MeasureString(iconText, iconFont);
                var iconX = 16;
                var iconY = (Height - iconSize.Height) / 2;
                g.DrawString(iconText, iconFont, iconBrush, iconX, iconY);
            }
        }

        private void DrawMessage(Graphics g)
        {
            var textColor = GetTextColor(_toastType);
            var messageRect = new RectangleF(38, 0, ToastWidth - 70, Height);
            
            using (var messageFont = new Font("Segoe UI", 9.5f, FontStyle.Regular))
            using (var messageBrush = new SolidBrush(textColor))
            {
                var sf = new StringFormat 
                { 
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };
                g.DrawString(_message, messageFont, messageBrush, messageRect, sf);
            }
        }

        private void DrawCloseButton(Graphics g)
        {
            var closeRect = new Rectangle(ToastWidth - 32, (Height - 20) / 2, 20, 20);
            var mousePos = PointToClient(MousePosition);
            bool isHovered = closeRect.Contains(mousePos);
            
            using (var closeFont = new Font("Segoe UI", 10, FontStyle.Regular))
            using (var closeBrush = new SolidBrush(isHovered ? GetAccentColor(_toastType) : CloseButtonColor))
            {
                var closeSize = g.MeasureString("×", closeFont);
                var closeX = closeRect.X + (closeRect.Width - closeSize.Width) / 2;
                var closeY = closeRect.Y + (closeRect.Height - closeSize.Height) / 2;
                g.DrawString("×", closeFont, closeBrush, closeX, closeY);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius, 
            bool topLeft = true, bool topRight = true, bool bottomRight = true, bool bottomLeft = true)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            
            if (topLeft)
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            else
                path.AddLine(bounds.X, bounds.Y + diameter, bounds.X, bounds.Y);
            
            if (topRight)
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            else
                path.AddLine(bounds.Right - diameter, bounds.Y, bounds.Right, bounds.Y);
            
            if (bottomRight)
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            else
                path.AddLine(bounds.Right, bounds.Bottom - diameter, bounds.Right, bounds.Bottom);
            
            if (bottomLeft)
                path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            else
                path.AddLine(bounds.X + diameter, bounds.Bottom, bounds.X, bounds.Bottom);
            
            path.CloseFigure();
            return path;
        }

        private static Color GetBackgroundColor(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success: return SuccessBg;
                case ToastType.Error: return ErrorBg;
                case ToastType.Warning: return WarningBg;
                case ToastType.Info: return InfoBg;
                default: return InfoBg;
            }
        }

        private static Color GetAccentColor(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success: return SuccessAccent;
                case ToastType.Error: return ErrorAccent;
                case ToastType.Warning: return WarningAccent;
                case ToastType.Info: return InfoAccent;
                default: return InfoAccent;
            }
        }

        private static Color GetTextColor(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success: return SuccessText;
                case ToastType.Error: return ErrorText;
                case ToastType.Warning: return WarningText;
                case ToastType.Info: return InfoText;
                default: return InfoText;
            }
        }

        private static string GetIconText(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success: return "✓";
                case ToastType.Error: return "✕";
                case ToastType.Warning: return "!";
                case ToastType.Info: return "i";
                default: return "i";
            }
        }

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
                int y = _parentForm.Top + ToastMargin + yOffset + 32;
                Location = new Point(x, y);
            }
            else
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(
                    screen.Right - ToastWidth - ToastMargin,
                    screen.Top + ToastMargin + yOffset
                );
            }
        }

        private void StartFadeIn()
        {
            _fadeTimer.Tick += FadeIn_Tick;
            _fadeTimer.Start();
        }

        private void FadeIn_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) 
            {
                _fadeTimer.Stop();
                return;
            }
            
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
            _fadeTimer.Tick -= FadeIn_Tick;
            _fadeTimer.Tick += FadeOut_Tick;
            _fadeTimer.Start();
        }

        private void FadeOut_Tick(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                _fadeTimer.Stop();
                return;
            }
            
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
            
            try
            {
                Close();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed, ignore
            }
        }

        private static void RepositionToasts()
        {
            lock (LockObject)
            {
                foreach (var toast in ActiveToasts)
                {
                    if (!toast.IsDisposed)
                    {
                        toast.PositionToast();
                    }
                }
            }
        }

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
            var closeRect = new Rectangle(ToastWidth - 32, (Height - 20) / 2, 20, 20);
            if (closeRect.Contains(e.Location))
            {
                StartFadeOut();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _closeTimer?.Dispose();
            _fadeTimer?.Dispose();
            base.OnFormClosed(e);
        }

        public static void Show(string message, ToastType type = ToastType.Info, int durationMs = 3000)
        {
            if (Application.OpenForms.Count == 0)
                return;

            _parentForm = Form.ActiveForm ?? Application.OpenForms[0];
            
            if (_parentForm.InvokeRequired)
            {
                _parentForm.BeginInvoke(new Action(() => ShowInternal(message, type, durationMs)));
            }
            else
            {
                ShowInternal(message, type, durationMs);
            }
        }

        private static void ShowInternal(string message, ToastType type, int durationMs)
        {
            var toast = new ToastNotification(message, type, durationMs);
            
            lock (LockObject)
            {
                ActiveToasts.Add(toast);
            }
            
            toast.PositionToast();
            toast.Show(_parentForm);
            toast.StartFadeIn();
        }

        public static void Success(string message, int durationMs = 3000)
        {
            Show(message, ToastType.Success, durationMs);
        }

        public static void Error(string message, int durationMs = 4000)
        {
            Show(message, ToastType.Error, durationMs);
        }

        public static void Warning(string message, int durationMs = 3500)
        {
            Show(message, ToastType.Warning, durationMs);
        }

        public static void Info(string message, int durationMs = 3000)
        {
            Show(message, ToastType.Info, durationMs);
        }

        public static void CloseAll()
        {
            lock (LockObject)
            {
                var toastsToClose = new List<ToastNotification>(ActiveToasts);
                foreach (var toast in toastsToClose)
                {
                    toast.CloseToast();
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ToastNotification
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ToastNotification";
            this.Load += new System.EventHandler(this.ToastNotification_Load);
            this.ResumeLayout(false);

        }

        private void ToastNotification_Load(object sender, EventArgs e)
        {

        }
    }
}
