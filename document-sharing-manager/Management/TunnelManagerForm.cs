using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using document_sharing_manager.Core.Configurations;
using document_sharing_manager.UI;

namespace document_sharing_manager.Management
{
    public class TunnelManagerForm : Form
    {
        private static TunnelManagerForm _instance;
        public static void ShowInstance()
        {
            if (_instance == null || _instance.IsDisposed)
                _instance = new TunnelManagerForm();
            _instance.Show();
            _instance.BringToFront();
        }

        private Button btnStart;
        private TextBox txtUrl;
        private TextBox txtTunnelName;
        private Button btnCopy;
        private RichTextBox rtbLog;
        private Process _sshProcess;
        private bool _isClosing = false;

        public TunnelManagerForm()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.Text = "Internet Tunnel (SSH)";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;

            var lblDesc = new Label
            {
                Text = "Khởi chạy Tunnel ngầm để truy cập Server từ xa qua Internet.",
                Location = new Point(20, 20),
                AutoSize = true
            };

            btnStart = new Button
            {
                Text = "Khởi chạy Tunnel",
                Location = new Point(20, 50),
                Size = new Size(150, 40),
                Cursor = Cursors.Hand
            };
            btnStart.Click += BtnStart_Click;

            var lblTunnelName = new Label { Text = "Tên Tunnel (để trống nếu muốn cấp random):", Location = new Point(190, 45), AutoSize = true };
            txtTunnelName = new TextBox { Location = new Point(190, 65), Width = 270 };


            var lblUrl = new Label { Text = "Public URL:", Location = new Point(20, 110), AutoSize = true };
            txtUrl = new TextBox { Location = new Point(20, 135), Width = 350, ReadOnly = true };
            
            btnCopy = new Button
            {
                Text = "Copy",
                Location = new Point(380, 133),
                Size = new Size(80, 30),
                Cursor = Cursors.Hand
            };
            btnCopy.Click += (s, e) => {
                if (!string.IsNullOrEmpty(txtUrl.Text))
                {
                    Clipboard.SetText(txtUrl.Text);
                    MessageBox.Show("Đã copy link!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            rtbLog = new RichTextBox
            {
                Location = new Point(20, 180),
                Size = new Size(440, 100),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 9)
            };

            this.Controls.AddRange(new Control[] { lblDesc, btnStart, lblTunnelName, txtTunnelName, lblUrl, txtUrl, btnCopy, rtbLog });
            this.FormClosing += TunnelManagerForm_FormClosing;
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            AppTheme.ApplyButtonPrimary(btnStart);
            AppTheme.ApplyButtonPrimary(btnCopy);
            txtUrl.BackColor = AppTheme.InputBackground;
            txtUrl.ForeColor = AppTheme.TextPrimary;
            txtTunnelName.BackColor = AppTheme.InputBackground;
            txtTunnelName.ForeColor = AppTheme.TextPrimary;
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl) lbl.ForeColor = AppTheme.TextPrimary;
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (_sshProcess != null)
                {
                    try { if (!_sshProcess.HasExited) _sshProcess.Kill(); } catch { }
                    try { _sshProcess.Dispose(); } catch { }
                    _sshProcess = null;
                    document_sharing_manager.Core.Data.UserSession.PublicUrl = null;
                    btnStart.Text = "Khởi chạy Tunnel";
                    btnStart.Enabled = true;
                    Log("Đã dừng Tunnel.");
                    return;
                }
            }
            catch { }

            btnStart.Text = "Đang kết nối...";
            txtUrl.Text = "";
            btnStart.Enabled = false;

            try
            {
                string cloudflaredPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "cloudflared.exe");
                if (!System.IO.File.Exists(cloudflaredPath))
                {
                    cloudflaredPath = "cloudflared"; // Fallback to PATH if not bundled
                }

                _sshProcess = new Process();
                _sshProcess.StartInfo.FileName = cloudflaredPath;
                
                string tunnelName = txtTunnelName.Text.Trim();
                if (!string.IsNullOrEmpty(tunnelName))
                {
                    _sshProcess.StartInfo.Arguments = $"tunnel run {tunnelName}";
                }
                else
                {
                    _sshProcess.StartInfo.Arguments = "tunnel --url http://localhost:5000";
                }
                
                _sshProcess.StartInfo.UseShellExecute = false;
                _sshProcess.StartInfo.RedirectStandardOutput = true;
                _sshProcess.StartInfo.RedirectStandardError = true;
                _sshProcess.StartInfo.CreateNoWindow = true;

                _sshProcess.OutputDataReceived += SshProcess_DataReceived;
                _sshProcess.ErrorDataReceived += SshProcess_DataReceived;

                _sshProcess.Start();
                _sshProcess.BeginOutputReadLine();
                _sshProcess.BeginErrorReadLine();

                Log("Bắt đầu khởi chạy Cloudflare Tunnel...");
                btnStart.Text = "Dừng Tunnel";
                btnStart.Enabled = true;
                
                if (!string.IsNullOrEmpty(tunnelName))
                {
                    string url = $"https://{tunnelName}.me";
                    txtUrl.Text = $"{url} (Tham khảo)";
                    document_sharing_manager.Core.Data.UserSession.PublicUrl = url;
                    Log($"Đang kết nối Cloudflare Tunnel cá nhân: {tunnelName}...");
                }
                else
                {
                    txtUrl.Text = "Đang lấy URL ngẫu nhiên...";
                    Log("Đang kết nối Cloudflare Quick Tunnel...");
                }
            }
            catch (Exception ex)
            {
                Log($"Lỗi: {ex.Message}");
                Log("Có thể máy bạn chưa cài đặt cloudflared hoặc chưa đưa vào biến môi trường PATH.");
                btnStart.Text = "Khởi chạy Tunnel";
                btnStart.Enabled = true;
                if (_sshProcess != null)
                {
                    try { _sshProcess.Dispose(); } catch { }
                    _sshProcess = null;
                }
            }
        }

        private void SshProcess_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) || _isClosing) return;

            this.Invoke(new Action(() =>
            {
                Log(e.Data);
                
                // Parse URL ngẫu nhiên (.trycloudflare.com) từ Cloudflare
                if (e.Data.Contains("trycloudflare.com") && e.Data.Contains("https://"))
                {
                    int startIndex = e.Data.IndexOf("https://");
                    if (startIndex != -1)
                    {
                        string url = e.Data.Substring(startIndex).Trim().TrimEnd('|', ' ', '\t');
                        // Nếu có dấu cách phía sau URL thì cắt đi
                        int spaceIndex = url.IndexOf(' ');
                        if (spaceIndex > 0)
                        {
                            url = url.Substring(0, spaceIndex);
                        }
                        
                        txtUrl.Text = url;
                        document_sharing_manager.Core.Data.UserSession.PublicUrl = url;
                        Log("==> Cấp phát URL thành công: " + url);
                    }
                }
            }));
        }

        private void Log(string message)
        {
            if (rtbLog.IsDisposed) return;
            rtbLog.AppendText(message + Environment.NewLine);
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
        }

        private void TunnelManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                _isClosing = true;
                try
                {
                    if (_sshProcess != null)
                    {
                        try { if (!_sshProcess.HasExited) _sshProcess.Kill(); } catch { }
                        try { _sshProcess.Dispose(); } catch { }
                    }
                }
                catch { }
            }
        }
    }
}
