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
        private TextBox txtName;
        private TextBox txtUrl;
        private TextBox txtPassword;
        private Button btnJoin;
        private Button btnCancel;
        private Label lblStatus;
        
        public bool Success { get; private set; }
        private static readonly HttpClient _httpClient = new(new HttpClientHandler { UseProxy = false }) { Timeout = TimeSpan.FromSeconds(10) };
        private readonly document_sharing_manager.Core.Services.SyncEngine _syncEngine;
        private readonly document_sharing_manager.Core.Services.AuthServiceClient _authServiceClient;

        public JoinServerForm(document_sharing_manager.Core.Services.SyncEngine syncEngine, document_sharing_manager.Core.Services.AuthServiceClient authClient)
        {
            _syncEngine = syncEngine;
            _authServiceClient = authClient;
            InitializeComponent();
            SetupUI();
            ApplyTheme();
        }

        private void SetupUI()
        {
            this.Text = "Kết nối Server mới";
            this.Size = new Size(400, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int left = 30;
            int top = 30;
            int width = 320;

            var lblName = new Label { Text = "Tên Server (Gợi nhớ):", Location = new Point(left, top), AutoSize = true };
            txtName = new TextBox { Location = new Point(left, top + 25), Width = width };
            AppTheme.ApplyTextBoxStyle(txtName);

            var lblUrl = new Label { Text = "Địa chỉ Server (IP/URL):", Location = new Point(left, top + 65), AutoSize = true };
            txtUrl = new TextBox { Location = new Point(left, top + 90), Width = width };
            AppTheme.ApplyTextBoxStyle(txtUrl);

            var lblPass = new Label { Text = "Mật khẩu tham gia Server:", Location = new Point(left, top + 130), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(left, top + 155), Width = width, UseSystemPasswordChar = true };
            AppTheme.ApplyTextBoxStyle(txtPassword);

            lblStatus = new Label { Text = "", Location = new Point(left, top + 195), Width = width, AutoSize = false, Height = 40, ForeColor = AppTheme.StatusInfo };

            btnJoin = new Button { Text = "Kết nối ngay", Location = new Point(left + 80, top + 245), Size = new Size(120, 40), Cursor = Cursors.Hand };
            btnCancel = new Button { Text = "Hủy", Location = new Point(left + 210, top + 245), Size = new Size(100, 40), Cursor = Cursors.Hand };

            btnJoin.Click += BtnJoin_Click;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange([lblName, txtName, lblUrl, txtUrl, lblPass, txtPassword, lblStatus, btnJoin, btnCancel]);
            this.Size = new Size(width + 100, top + 350);
            txtUrl.Text = "http://127.0.0.1:5000/";
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundMain;
            AppTheme.ApplyButtonPrimary(btnJoin);
            AppTheme.ApplyButtonSecondary(btnCancel);
            
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl) lbl.ForeColor = AppTheme.TextPrimary;
            }
        }

        private async void BtnJoin_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string url = txtUrl.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url))
            {
                lblStatus.Text = "Vui lòng nhập đủ Tên và URL!";
                lblStatus.ForeColor = AppTheme.StatusError;
                return;
            }

            btnJoin.Enabled = false;
            lblStatus.Text = "Đang kiểm tra kết nối...";
            lblStatus.ForeColor = AppTheme.StatusInfo;

            try
            {
                // Dùng endpoint Auth/login để check vì chắc chắn nó tồn tại
                string testUrl = $"{url.TrimEnd('/')}/api/Auth/login";
                using var response = await _httpClient.GetAsync(testUrl);

                // Nếu không phải 404 thì nghĩa là Server đã phản hồi (có thể là 405 Method Not Allowed nhưng vẫn là có kết nối)
                // Nếu không phải 404 thì nghĩa là Server đã phản hồi
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    string joinPass = txtPassword.Text;
                    string token = document_sharing_manager.Core.Data.UserSession.AccessToken;
                    
                    // Upload to cloud for persistence across account logins
                    var cloudResult = await _authServiceClient.SaveServerToCloudAsync(name, url, token, joinPass);
                    if (!cloudResult)
                    {
                        lblStatus.Text = "Sai mật khẩu Server hoặc lỗi phân quyền!";
                        lblStatus.ForeColor = AppTheme.StatusError;
                        btnJoin.Enabled = true;
                        return;
                    }

                    // Lưu vào DB local
                    DatabaseHelper.InsertServer(name, url, password: joinPass, accessToken: token);
                    
                    // Fetch the newly inserted server to add it to SyncEngine
                    var allServers = DatabaseHelper.GetManagedServers();
                    var newServer = allServers.FirstOrDefault(s => s.BaseUrl.TrimEnd('/') == url.TrimEnd('/'));
                    if (newServer != null)
                    {
                        _syncEngine?.AddServer(newServer);
                    }
                    Success = true;
                    this.DialogResult = DialogResult.OK;
                    this.Hide(); // Hide immediately for better UX
                    this.Close();
                }
                else
                {
                    string errorDetail = $"Lỗi { (int)response.StatusCode } ({ response.ReasonPhrase })";
                    lblStatus.Text = $"Không thể kết nối! {errorDetail}";
                    lblStatus.ForeColor = AppTheme.StatusError;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi: " + ex.Message;
                lblStatus.ForeColor = AppTheme.StatusError;
            }
            finally
            {
                btnJoin.Enabled = true;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(400, 320);
            this.Name = "JoinServerForm";
            this.ResumeLayout(false);
        }
    }
}
