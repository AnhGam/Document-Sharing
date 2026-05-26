using System;
using System.Drawing;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.Core.Data;
using document_sharing_manager.Core.Domain;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;

namespace document_sharing_manager.Management
{
    public partial class JoinServerForm : Form
    {
        // Invite Controls
        private TextBox txtInviteCode;
        private TextBox txtDisplayName;
        private Button btnJoinInvite;

        private Label lblStatus;
        
        public bool Success { get; private set; }
        private static readonly HttpClient _httpClient = new(new HttpClientHandler { UseProxy = false }) { Timeout = TimeSpan.FromSeconds(10) };
        private readonly document_sharing_manager.Core.Services.SyncEngine _syncEngine;
        private readonly document_sharing_manager.Core.Services.AuthServiceClient _authServiceClient;

        public JoinServerForm(document_sharing_manager.Core.Services.SyncEngine syncEngine, document_sharing_manager.Core.Services.AuthServiceClient authClient, string defaultInviteCode = "")
        {
            _syncEngine = syncEngine;
            _authServiceClient = authClient;
            InitializeComponentManual();
            ApplyTheme();
            
            if (!string.IsNullOrEmpty(defaultInviteCode))
            {
                txtInviteCode.Text = defaultInviteCode;
            }
        }

        private void InitializeComponentManual()
        {
            this.Text = "Tham gia Kênh chia sẻ";
            this.Size = new Size(400, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = AppTheme.BackgroundMain;

            int left = 25;
            int width = 330;

            var lblInvite = new Label { Text = "Mã mời hoặc Link (docshare://join/...):", Location = new Point(left, 20), AutoSize = true, ForeColor = AppTheme.TextPrimary };
            txtInviteCode = new TextBox { Location = new Point(left, 45), Width = width };
            AppTheme.ApplyTextBoxStyle(txtInviteCode);

            var lblDisplay = new Label { Text = "Tên hiển thị của bạn:", Location = new Point(left, 95), AutoSize = true, ForeColor = AppTheme.TextPrimary };
            txtDisplayName = new TextBox { Location = new Point(left, 120), Width = width, Text = document_sharing_manager.Core.Data.UserSession.Username };
            AppTheme.ApplyTextBoxStyle(txtDisplayName);

            btnJoinInvite = new Button { Text = "Tham gia Kênh", Location = new Point(left + 90, 185), Size = new Size(150, 40), Cursor = Cursors.Hand };
            btnJoinInvite.Click += BtnJoinInvite_Click;
            AppTheme.ApplyButtonPrimary(btnJoinInvite);

            lblStatus = new Label { Text = "", Location = new Point(left, 240), Width = width, AutoSize = false, Height = 40, ForeColor = AppTheme.StatusInfo, TextAlign = ContentAlignment.TopCenter };

            var btnCancel = new Button { Text = "Hủy", Location = new Point(left + 115, 280), Size = new Size(100, 32), Cursor = Cursors.Hand };
            btnCancel.Click += (s, e) => this.Close();
            AppTheme.ApplyButtonSecondary(btnCancel);

            this.Controls.AddRange([lblInvite, txtInviteCode, lblDisplay, txtDisplayName, btnJoinInvite, lblStatus, btnCancel]);

            // Add standard hidden components to satisfy partial class if designer created anything
            this.SuspendLayout();
            this.Name = "JoinServerForm";
            this.ResumeLayout(false);
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            AppTheme.ApplyButtonPrimary(btnJoinInvite);
        }

        private (string code, string url) ParseInviteInput(string input)
        {
            input = input.Trim();
            
            // 1. Dạng deep link: docshare://join?url=...&code=...
            if (input.StartsWith("docshare://join?"))
            {
                try
                {
                    string query = input.Substring(input.IndexOf('?') + 1);
                    return ParseQueryString(query);
                }
                catch { }
            }
            
            // 2. Dạng deep link cũ: docshare://join/{code}
            if (input.StartsWith("docshare://join/"))
            {
                return (input.Substring(16).Trim('/'), null);
            }

            // 3. Dạng URL thông thường: http://domain/join?code=xxx hoặc https://domain/join?code=xxx
            // Hoặc chỉ đơn giản là https://domain&code=xxx (do chat client highlight hyperlink)
            if (input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                input.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string url = "";
                    string code = "";
                    
                    int codeIndex = input.IndexOf("code=", StringComparison.OrdinalIgnoreCase);
                    if (codeIndex != -1)
                    {
                        code = input.Substring(codeIndex + 5).Split('&')[0];
                        
                        int delimiterIndex = input.IndexOf('?');
                        if (delimiterIndex == -1) delimiterIndex = input.IndexOf('&');
                        
                        if (delimiterIndex != -1)
                        {
                            url = input.Substring(0, delimiterIndex);
                        }
                        else
                        {
                            url = input;
                        }
                        return (code, url);
                    }
                }
                catch { }
            }

            // 4. Trường hợp copy bị mất https:// ở đầu (ví dụ: domain.com&code=xxx)
            if (input.Contains("code="))
            {
                try
                {
                    string code = "";
                    string url = "";
                    int codeIndex = input.IndexOf("code=", StringComparison.OrdinalIgnoreCase);
                    code = input.Substring(codeIndex + 5).Split('&')[0];
                    
                    int delimiterIndex = input.IndexOf('?');
                    if (delimiterIndex == -1) delimiterIndex = input.IndexOf('&');
                    
                    if (delimiterIndex > 0)
                    {
                        string domain = input.Substring(0, delimiterIndex);
                        url = "https://" + domain; // Mặc định dùng https cho các Tunnel
                    }
                    return (code, url);
                }
                catch { }
            }

            return (input, null);
        }

        private (string code, string url) ParseQueryString(string query)
        {
            var parts = query.Split('&');
            string url = "";
            string code = "";
            foreach (var part in parts)
            {
                var kv = part.Split('=');
                if (kv.Length == 2)
                {
                    if (kv[0].ToLower() == "url") url = Uri.UnescapeDataString(kv[1]);
                    if (kv[0].ToLower() == "code") code = Uri.UnescapeDataString(kv[1]);
                }
            }
            return (code, url);
        }

        private async void BtnJoinInvite_Click(object sender, EventArgs e)
        {
            var parsed = ParseInviteInput(txtInviteCode.Text);
            string code = parsed.code;
            string displayName = txtDisplayName.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(displayName))
            {
                ShowStatus("Vui lòng nhập Mã mời và Tên hiển thị!", true);
                return;
            }

            string originalUrl = _authServiceClient.BaseUrl;
            if (!string.IsNullOrEmpty(parsed.url))
            {
                _authServiceClient.UpdateBaseUrl(parsed.url);
            }

            btnJoinInvite.Enabled = false;
            ShowStatus("Đang kiểm tra mã mời...", false);

            var info = await _authServiceClient.GetInviteInfoAsync(code);
            if (!info.valid)
            {
                ShowStatus(info.message, true);
                _authServiceClient.UpdateBaseUrl(originalUrl); // Khôi phục URL cũ khi thất bại
                btnJoinInvite.Enabled = true;
                return;
            }

            ShowStatus("Đang tham gia server...", false);
            var joinRes = await _authServiceClient.JoinWithInviteAsync(code, displayName);
            
            if (joinRes.success)
            {
                // Lưu server vào database local (giống như luồng kết nối thủ công)
                string serverUrl = parsed.url ?? _authServiceClient.BaseUrl;
                string serverName = "";
                try
                {
                    var uri = new Uri(serverUrl);
                    serverName = uri.Host; // Dùng hostname làm tên server
                }
                catch { serverName = serverUrl; }

                string token = document_sharing_manager.Core.Data.UserSession.AccessToken;
                
                // Đăng ký server lên API cloud để tránh lỗi 403 Forbidden khi sync và lấy Cloud Server ID
                int? cloudId = await _authServiceClient.SaveServerToCloudAsync(serverName, serverUrl, token);
                
                DatabaseHelper.InsertServer(serverName, serverUrl, accessToken: token, remoteId: cloudId);

                // Thêm server vào SyncEngine để hiện ở sidebar
                var allServers = DatabaseHelper.GetManagedServers();
                var newServer = allServers.FirstOrDefault(s => s.BaseUrl.TrimEnd('/') == serverUrl.TrimEnd('/'));
                if (newServer != null)
                {
                    _syncEngine?.AddServer(newServer);
                }

                Success = true;
                MessageBox.Show(joinRes.message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowStatus(joinRes.message, true);
                _authServiceClient.UpdateBaseUrl(originalUrl); // Khôi phục URL cũ khi thất bại
                btnJoinInvite.Enabled = true;
            }
        }


        private void ShowStatus(string message, bool isError)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = isError ? AppTheme.StatusError : AppTheme.StatusInfo;
        }
    }
}
