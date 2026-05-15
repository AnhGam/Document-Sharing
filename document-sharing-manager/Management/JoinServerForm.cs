using System;
using System.Drawing;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.Core.Data;
using document_sharing_manager.Core.Domain;
using System.Net.Http;
using System.Threading.Tasks;

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
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public JoinServerForm()
        {
            InitializeComponent();
            SetupUI();
            ApplyTheme();
        }

        private void SetupUI()
        {
            this.Text = "Kết nối Server mới";
            this.Size = new Size(400, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int left = 30;
            int top = 30;
            int width = 320;

            var lblName = new Label { Text = "Tên Server (Gợi nhớ):", Location = new Point(left, top), AutoSize = true };
            txtName = new TextBox { Location = new Point(left, top + 25), Width = width };

            var lblUrl = new Label { Text = "Địa chỉ Server (IP/URL):", Location = new Point(left, top + 65), AutoSize = true };
            txtUrl = new TextBox { Location = new Point(left, top + 90), Width = width };

            var lblPass = new Label { Text = "Mật khẩu (nếu có):", Location = new Point(left, top + 130), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(left, top + 155), Width = width, UseSystemPasswordChar = true };

            lblStatus = new Label { Text = "", Location = new Point(left, top + 190), Width = width, AutoSize = false, Height = 20, ForeColor = AppTheme.StatusInfo };

            btnJoin = new Button { Text = "Kết nối ngay", Location = new Point(left + 110, top + 220), Size = new Size(100, 36), Cursor = Cursors.Hand };
            btnCancel = new Button { Text = "Hủy", Location = new Point(left + 220, top + 220), Size = new Size(100, 36), Cursor = Cursors.Hand };

            btnJoin.Click += BtnJoin_Click;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange([lblName, txtName, lblUrl, txtUrl, lblPass, txtPassword, lblStatus, btnJoin, btnCancel]);
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
            string password = txtPassword.Text;

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
                // Thử kết nối và kiểm tra server
                bool isConnected = await TestServerConnection(url);
                if (isConnected)
                {
                    // Lưu vào DB
                    DatabaseHelper.InsertServer(name, url, password: password);
                    Success = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.Text = "Không thể kết nối hoặc sai mật khẩu!";
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

        private async Task<bool> TestServerConnection(string url)
        {
            try
            {
                // Gọi thử endpoint Health để check
                // NOTE: In a real production scenario, we should also validate the server password 
                // by attempting a specialized 'test-auth' or login request if the server requires it.
                var response = await _httpClient.GetAsync($"{url.TrimEnd('/')}/api/Health");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
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
