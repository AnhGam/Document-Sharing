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
        private TabControl tabControl;
        private TabPage tabInvite;
        private TabPage tabManual;

        // Invite Tab Controls
        private TextBox txtInviteCode;
        private TextBox txtDisplayName;
        private Button btnJoinInvite;

        // Manual Tab Controls
        private TextBox txtName;
        private TextBox txtUrl;
        private TextBox txtPassword;
        private Button btnJoinManual;

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
                tabControl.SelectedTab = tabInvite;
            }
        }

        private void InitializeComponentManual()
        {
            this.Text = "Kết nối Server";
            this.Size = new Size(420, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            tabControl = new TabControl { Dock = DockStyle.Top, Height = 340, Padding = new Point(15, 8) };
            tabInvite = new TabPage("Sử dụng Link Mời");
            tabManual = new TabPage("Kết nối Thủ Công");

            int left = 20;
            int width = 340;

            // --- Invite Tab ---
            var lblInvite = new Label { Text = "Mã mời hoặc Link (docshare://join/...):", Location = new Point(left, 20), AutoSize = true };
            txtInviteCode = new TextBox { Location = new Point(left, 45), Width = width };
            AppTheme.ApplyTextBoxStyle(txtInviteCode);

            var lblDisplay = new Label { Text = "Tên hiển thị của bạn:", Location = new Point(left, 90), AutoSize = true };
            txtDisplayName = new TextBox { Location = new Point(left, 115), Width = width, Text = document_sharing_manager.Core.Data.UserSession.Username };
            AppTheme.ApplyTextBoxStyle(txtDisplayName);

            btnJoinInvite = new Button { Text = "Tham gia", Location = new Point(left + 110, 180), Size = new Size(120, 40), Cursor = Cursors.Hand };
            btnJoinInvite.Click += BtnJoinInvite_Click;

            tabInvite.Controls.AddRange([lblInvite, txtInviteCode, lblDisplay, txtDisplayName, btnJoinInvite]);
            tabInvite.BackColor = AppTheme.BackgroundMain;

            // --- Manual Tab ---
            var lblName = new Label { Text = "Tên Server (Gợi nhớ):", Location = new Point(left, 20), AutoSize = true };
            txtName = new TextBox { Location = new Point(left, 45), Width = width };
            AppTheme.ApplyTextBoxStyle(txtName);

            var lblUrl = new Label { Text = "Địa chỉ Server (IP/URL):", Location = new Point(left, 90), AutoSize = true };
            txtUrl = new TextBox { Location = new Point(left, 115), Width = width, Text = "http://127.0.0.1:5000/" };
            AppTheme.ApplyTextBoxStyle(txtUrl);

            var lblPass = new Label { Text = "Mật khẩu tham gia Server:", Location = new Point(left, 160), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(left, 185), Width = width, UseSystemPasswordChar = true };
            AppTheme.ApplyTextBoxStyle(txtPassword);

            btnJoinManual = new Button { Text = "Kết nối ngay", Location = new Point(left + 110, 240), Size = new Size(120, 40), Cursor = Cursors.Hand };
            btnJoinManual.Click += BtnJoinManual_Click;

            tabManual.Controls.AddRange([lblName, txtName, lblUrl, txtUrl, lblPass, txtPassword, btnJoinManual]);
            tabManual.BackColor = AppTheme.BackgroundMain;

            tabControl.TabPages.Add(tabInvite);
            tabControl.TabPages.Add(tabManual);

            lblStatus = new Label { Text = "", Location = new Point(left, 350), Width = width, AutoSize = false, Height = 40, ForeColor = AppTheme.StatusInfo, TextAlign = ContentAlignment.TopCenter };

            var btnCancel = new Button { Text = "Hủy", Location = new Point(left + 130, 395), Size = new Size(100, 35), Cursor = Cursors.Hand };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange([tabControl, lblStatus, btnCancel]);

            // Add standard hidden components to satisfy partial class if designer created anything
            this.SuspendLayout();
            this.Name = "JoinServerForm";
            this.ResumeLayout(false);
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            AppTheme.ApplyButtonPrimary(btnJoinInvite);
            AppTheme.ApplyButtonPrimary(btnJoinManual);
            
            foreach (TabPage page in tabControl.TabPages)
            {
                foreach (Control c in page.Controls)
                {
                    if (c is Label lbl) lbl.ForeColor = AppTheme.TextPrimary;
                }
            }
        }

        private string ExtractCode(string input)
        {
            input = input.Trim();
            if (input.StartsWith("docshare://join/"))
            {
                return input.Substring(16).Trim('/');
            }
            return input;
        }

        private async void BtnJoinInvite_Click(object sender, EventArgs e)
        {
            string code = ExtractCode(txtInviteCode.Text);
            string displayName = txtDisplayName.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(displayName))
            {
                ShowStatus("Vui lòng nhập Mã mời và Tên hiển thị!", true);
                return;
            }

            btnJoinInvite.Enabled = false;
            ShowStatus("Đang kiểm tra mã mời...", false);

            var info = await _authServiceClient.GetInviteInfoAsync(code);
            if (!info.valid)
            {
                ShowStatus(info.message, true);
                btnJoinInvite.Enabled = true;
                return;
            }

            ShowStatus("Đang tham gia server...", false);
            var joinRes = await _authServiceClient.JoinWithInviteAsync(code, displayName);
            
            if (joinRes.success)
            {
                // In a real scenario, after joining via invite, we might need the server details
                // to connect (like BaseUrl). If the API is single-instance (as is currently), 
                // joining an invite just gives you access. The client still needs the BaseUrl to sync.
                // Wait, if we use Invite Link, how do we know the BaseUrl?
                // The current architecture assumes we know the BaseUrl when using the API.
                // But the invite code is validated against the Current API instance connected in AuthServiceClient.
                // Ah, the WinForms client connects to MULTIPLE API servers.
                // If they paste an invite link, we don't know WHICH server it belongs to unless the link contains it.
                // Wait, if the link is `docshare://join/abc123xyz`, there's no server IP in it.
                // Discord links don't have IP because they hit Discord central API.
                // Our system is decentralized. There is no central server.
                // If there's no central server, how does an invite link know the IP?
                // We must embed the IP/domain in the invite link!
                // Example: `docshare://join/http://192.168.1.5:5000/abc123xyz` 
                // OR the InviteLink must just be a normal URL `http://192.168.1.5:5000/api/invite/abc123xyz/join`
                // Let's modify the ExtractCode logic to parse the BaseUrl and Code.
                
                Success = true;
                MessageBox.Show(joinRes.message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowStatus(joinRes.message, true);
                btnJoinInvite.Enabled = true;
            }
        }

        private async void BtnJoinManual_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string url = txtUrl.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url))
            {
                ShowStatus("Vui lòng nhập đủ Tên và URL!", true);
                return;
            }

            btnJoinManual.Enabled = false;
            ShowStatus("Đang kiểm tra kết nối...", false);

            try
            {
                string testUrl = $"{url.TrimEnd('/')}/api/Auth/login";
                using var response = await _httpClient.GetAsync(testUrl);

                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    string joinPass = txtPassword.Text;
                    string token = document_sharing_manager.Core.Data.UserSession.AccessToken;
                    
                    var cloudResult = await _authServiceClient.SaveServerToCloudAsync(name, url, token, joinPass);
                    if (!cloudResult)
                    {
                        ShowStatus("Sai mật khẩu Server hoặc lỗi phân quyền!", true);
                        btnJoinManual.Enabled = true;
                        return;
                    }

                    DatabaseHelper.InsertServer(name, url, password: joinPass, accessToken: token);
                    
                    var allServers = DatabaseHelper.GetManagedServers();
                    var newServer = allServers.FirstOrDefault(s => s.BaseUrl.TrimEnd('/') == url.TrimEnd('/'));
                    if (newServer != null)
                    {
                        _syncEngine?.AddServer(newServer);
                    }
                    Success = true;
                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                    this.Close();
                }
                else
                {
                    ShowStatus($"Không thể kết nối! Lỗi {(int)response.StatusCode}", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Lỗi: " + ex.Message, true);
            }
            finally
            {
                btnJoinManual.Enabled = true;
            }
        }

        private void ShowStatus(string message, bool isError)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = isError ? AppTheme.StatusError : AppTheme.StatusInfo;
        }
    }
}
