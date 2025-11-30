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
        private readonly Label _messageLabel;
        private readonly Label _iconLabel;
        private readonly Label _closeButton;
        private readonly ToastType _toastType;
        private const int ToastWidth = 300;
        private const int ToastHeight = 50;
        private const int ToastMargin = 10;
        private const int AnimationInterval = 20;
        private const double FadeStep = 0.08;

        private static readonly Color SuccessColor = Color.FromArgb(76, 175, 80);
        private static readonly Color ErrorColor = Color.FromArgb(244, 67, 54);
        private static readonly Color WarningColor = Color.FromArgb(255, 152, 0);
        private static readonly Color InfoColor = Color.FromArgb(33, 150, 243);

        private ToastNotification(string message, ToastType type, int durationMs = 3000)
        {
            _toastType = type;
            
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(ToastWidth, ToastHeight);
            Opacity = 0;
            
            BackColor = GetBackgroundColor(type);
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer, true);

            _iconLabel = new Label
            {
                AutoSize = false,
                Size = new Size(30, 30),
                Location = new Point(10, 10),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = GetIconText(type)
            };

            _messageLabel = new Label
            {
                AutoSize = false,
                Location = new Point(44, 8),
                Size = new Size(ToastWidth - 85, ToastHeight - 16),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = message
            };

            _closeButton = new Label
            {
                Size = new Size(20, 20),
                Location = new Point(ToastWidth - 28, 15),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Text = "×",
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _closeButton.Click += (s, e) => CloseToast();
            _closeButton.MouseEnter += (s, e) => _closeButton.ForeColor = Color.FromArgb(200, 200, 200);
            _closeButton.MouseLeave += (s, e) => _closeButton.ForeColor = Color.White;

            Controls.Add(_iconLabel);
            Controls.Add(_messageLabel);
            Controls.Add(_closeButton);

            _closeTimer = new Timer { Interval = durationMs };
            _closeTimer.Tick += (s, e) =>
            {
                _closeTimer.Stop();
                StartFadeOut();
            };

            _fadeTimer = new Timer { Interval = AnimationInterval };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            using (var path = CreateRoundedRectangle(new Rectangle(0, 0, Width, Height), 8))
            {
                Region = new Region(path);
                
                using (var brush = new SolidBrush(BackColor))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }

        private static Color GetBackgroundColor(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success: return SuccessColor;
                case ToastType.Error: return ErrorColor;
                case ToastType.Warning: return WarningColor;
                case ToastType.Info: return InfoColor;
                default: return InfoColor;
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
            int yOffset;
            
            lock (LockObject)
            {
                int index = ActiveToasts.IndexOf(this);
                yOffset = (ToastHeight + ToastMargin) * index;
            }

            if (_parentForm != null && !_parentForm.IsDisposed)
            {
                int x = _parentForm.Left + _parentForm.Width - ToastWidth - ToastMargin;
                int y = _parentForm.Top + ToastMargin + yOffset + 30;
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
            if (Opacity < 1)
            {
                Opacity += FadeStep;
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
            _fadeTimer.Tick += FadeOut_Tick;
            _fadeTimer.Start();
        }

        private void FadeOut_Tick(object sender, EventArgs e)
        {
            if (Opacity > 0)
            {
                Opacity -= FadeStep;
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
            lock (LockObject)
            {
                ActiveToasts.Remove(this);
                RepositionToasts();
            }
            
            _closeTimer.Stop();
            _fadeTimer.Stop();
            Close();
            Dispose();
        }

        private static void RepositionToasts()
        {
            lock (LockObject)
            {
                foreach (var toast in ActiveToasts)
                {
                    toast.PositionToast();
                }
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
    }
}
