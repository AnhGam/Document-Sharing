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
        private Button btnStart;
        private TextBox txtUrl;
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

            this.Controls.AddRange(new Control[] { lblDesc, btnStart, lblUrl, txtUrl, btnCopy, rtbLog });
            this.FormClosing += TunnelManagerForm_FormClosing;
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            AppTheme.ApplyButtonPrimary(btnStart);
            AppTheme.ApplyButtonPrimary(btnCopy);
            txtUrl.BackColor = AppTheme.InputBackground;
            txtUrl.ForeColor = AppTheme.TextPrimary;
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl) lbl.ForeColor = AppTheme.TextPrimary;
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (_sshProcess != null && !_sshProcess.HasExited)
            {
                _sshProcess.Kill();
                _sshProcess.Dispose();
                _sshProcess = null;
                btnStart.Text = "Khởi chạy Tunnel";
                Log("Đã tắt Tunnel.");
                return;
            }

            btnStart.Text = "Đang kết nối...";
            txtUrl.Text = "";
            btnStart.Enabled = false;

            try
            {
                _sshProcess = new Process();
                _sshProcess.StartInfo.FileName = "ssh";
                _sshProcess.StartInfo.Arguments = "-R 80:localhost:5000 nokey@localhost.run -o StrictHostKeyChecking=no";
                _sshProcess.StartInfo.UseShellExecute = false;
                _sshProcess.StartInfo.RedirectStandardOutput = true;
                _sshProcess.StartInfo.RedirectStandardError = true;
                _sshProcess.StartInfo.CreateNoWindow = true;

                _sshProcess.OutputDataReceived += SshProcess_DataReceived;
                _sshProcess.ErrorDataReceived += SshProcess_DataReceived;

                _sshProcess.Start();
                _sshProcess.BeginOutputReadLine();
                _sshProcess.BeginErrorReadLine();

                Log("Bắt đầu khởi tạo SSH Tunnel...");
                btnStart.Text = "Dừng Tunnel";
                btnStart.Enabled = true;
            }
            catch (Exception ex)
            {
                Log($"Lỗi: {ex.Message}");
                btnStart.Text = "Khởi chạy Tunnel";
                btnStart.Enabled = true;
            }
        }

        private void SshProcess_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) || _isClosing) return;

            this.Invoke(new Action(() =>
            {
                Log(e.Data);
                // Check for localhost.run url
                var match = Regex.Match(e.Data, @"https://[a-zA-Z0-9-]+\.lhr\.life");
                if (match.Success)
                {
                    txtUrl.Text = match.Value;
                    Log("==> Lấy URL thành công: " + match.Value);
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
            _isClosing = true;
            if (_sshProcess != null && !_sshProcess.HasExited)
            {
                try
                {
                    _sshProcess.Kill();
                    _sshProcess.Dispose();
                }
                catch { }
            }
        }
    }
}
