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

            // Dọn dẹp các tiến trình tunnel rác cũ do ứng dụng tạo ra để tránh xung đột
            KillOrphanedTunnels();

            try
            {
                string tunnelName = txtTunnelName.Text.Trim();
                bool isServeo = string.IsNullOrEmpty(tunnelName);

                _sshProcess = new Process();

                if (isServeo)
                {
                    // Xác định đường dẫn ssh.exe chính xác, bypass Wow64 redirection nếu app chạy 32-bit trên Windows 64-bit
                    string sshPath = "ssh";
                    if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                    {
                        string sysnativePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Sysnative", "OpenSSH", "ssh.exe");
                        if (System.IO.File.Exists(sysnativePath))
                        {
                            sshPath = sysnativePath;
                        }
                    }
                    else
                    {
                        string system32Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "OpenSSH", "ssh.exe");
                        if (System.IO.File.Exists(system32Path))
                        {
                            sshPath = system32Path;
                        }
                    }

                    _sshProcess.StartInfo.FileName = sshPath;
                    // Dùng localhost.run làm Tunnel ngẫu nhiên, không bị DNS Quad9 chặn và không bị WAF/Anti-Bot chặn
                    _sshProcess.StartInfo.Arguments = "-o StrictHostKeyChecking=no -R 80:localhost:5000 nokey@localhost.run";
                }
                else
                {
                    // Chạy Cloudflare cho tunnel cố định (Named Tunnel)
                    string cloudflaredPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "cloudflared.exe");
                    if (!System.IO.File.Exists(cloudflaredPath))
                    {
                        cloudflaredPath = "cloudflared"; // Fallback to PATH if not bundled
                    }
                    _sshProcess.StartInfo.FileName = cloudflaredPath;
                    _sshProcess.StartInfo.Arguments = $"tunnel run {tunnelName}";
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

                if (isServeo)
                {
                    Log("Bắt đầu khởi chạy SSH Tunnel (Localhost.run)...");
                    txtUrl.Text = "Đang lấy URL ngẫu nhiên...";
                }
                else
                {
                    Log("Bắt đầu khởi chạy Cloudflare Tunnel...");
                    string url = $"https://{tunnelName}.me";
                    txtUrl.Text = $"{url} (Tham khảo)";
                    document_sharing_manager.Core.Data.UserSession.PublicUrl = url;
                    Log($"Đang kết nối Cloudflare Tunnel cá nhân: {tunnelName}...");
                }

                btnStart.Text = "Dừng Tunnel";
                btnStart.Enabled = true;
            }
            catch (Exception ex)
            {
                Log($"Lỗi: {ex.Message}");
                Log("Hãy đảm bảo OpenSSH Client đã được cài đặt và kích hoạt trong Windows Features.");
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
                        int spaceIndex = url.IndexOf(' ');
                        if (spaceIndex > 0)
                        {
                            url = url.Substring(0, spaceIndex);
                        }
                        
                        txtUrl.Text = url;
                        document_sharing_manager.Core.Data.UserSession.PublicUrl = url;
                        Log("==> Cấp phát URL thành công (Cloudflare): " + url);
                    }
                }

                // Parse URL ngẫu nhiên (.lhr.life) từ localhost.run
                if (e.Data.Contains("lhr.life") && e.Data.Contains("https://"))
                {
                    int startIndex = e.Data.IndexOf("https://");
                    if (startIndex != -1)
                    {
                        string url = e.Data.Substring(startIndex).Trim().TrimEnd('|', ' ', '\t');
                        int spaceIndex = url.IndexOf(' ');
                        if (spaceIndex > 0)
                        {
                            url = url.Substring(0, spaceIndex);
                        }
                        
                        if (url.Contains("lhr.life"))
                        {
                            txtUrl.Text = url;
                            document_sharing_manager.Core.Data.UserSession.PublicUrl = url;
                            Log("==> Cấp phát URL thành công (Localhost.run): " + url);
                        }
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
                StopTunnelProcess();
            }
        }

        public static void StopAndDispose()
        {
            if (_instance != null)
            {
                _instance._isClosing = true;
                _instance.StopTunnelProcess();
                _instance.Dispose();
                _instance = null;
            }
            // Dọn dẹp một lần cuối trước khi tắt hẳn ứng dụng
            KillOrphanedTunnels();
        }

        private void StopTunnelProcess()
        {
            try
            {
                if (_sshProcess != null)
                {
                    try { if (!_sshProcess.HasExited) _sshProcess.Kill(); } catch { }
                    try { _sshProcess.Dispose(); } catch { }
                    _sshProcess = null;
                }
            }
            catch { }
        }

        public static void KillOrphanedTunnels()
        {
            try
            {
                // 1. Dọn dẹp tiến trình cloudflared rác
                string appDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
                var processes = Process.GetProcessesByName("cloudflared");
                foreach (var p in processes)
                {
                    try
                    {
                        string fileName = p.MainModule.FileName;
                        if (fileName.StartsWith(appDir, StringComparison.OrdinalIgnoreCase))
                        {
                            p.Kill();
                        }
                    }
                    catch { }
                }

                // 2. Dọn dẹp tiến trình ssh serveo/localhost.run rác bằng lệnh PowerShell an toàn
                KillOrphanedSsh();
            }
            catch { }
        }

        private static void KillOrphanedSsh()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-Command \"Get-CimInstance Win32_Process -Filter \\\"Name = 'ssh.exe' AND (CommandLine LIKE '%serveo.net%' OR CommandLine LIKE '%localhost.run%')\\\" | Invoke-CimMethod -MethodName Terminate\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi)?.WaitForExit(5000);
            }
            catch { }
        }
    }
}
