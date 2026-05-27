using System;
using System.Drawing;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace document_sharing_manager.Management
{
    public partial class InviteManagementForm : Form
    {
        private DataGridView dgvInvites;
        private Button btnCreatePublic;
        private Button btnCreateApproval;
        private Button btnClose;
        private readonly AuthServiceClient _authClient;
        private readonly string _serverBaseUrl;
        private int _serverId;

        public InviteManagementForm(AuthServiceClient authClient, string serverBaseUrl, int serverId = 0)
        {
            _authClient = authClient;
            _serverBaseUrl = serverBaseUrl.TrimEnd('/');
            _serverId = serverId;

            if (_serverId == 0)
            {
                try
                {
                    var server = document_sharing_manager.Core.Data.DatabaseHelper.GetManagedServers()
                        .FirstOrDefault(x => x.BaseUrl.TrimEnd('/').Equals(_serverBaseUrl, StringComparison.OrdinalIgnoreCase));
                    if (server != null && server.CloudId.HasValue)
                    {
                        _serverId = server.CloudId.Value;
                    }
                }
                catch { }
            }

            InitializeComponentManual();
            LoadInvites();
        }

        private void InitializeComponentManual()
        {
            this.Text = "Quản lý Link Mời";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = AppTheme.BackgroundMain;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            btnCreatePublic = new Button { Text = "Tạo Link (Tự động duyệt)", Width = 200, Dock = DockStyle.Left, Cursor = Cursors.Hand };
            btnCreateApproval = new Button { Text = "Tạo Link (Cần phê duyệt)", Width = 200, Dock = DockStyle.Left, Cursor = Cursors.Hand };
            
            AppTheme.ApplyButtonPrimary(btnCreatePublic);
            AppTheme.ApplyButtonWarning(btnCreateApproval);
            
            // Fix text alignment issues
            btnCreatePublic.TextAlign = ContentAlignment.MiddleCenter;
            btnCreateApproval.TextAlign = ContentAlignment.MiddleCenter;

            // Attach event handlers
            btnCreatePublic.Click += async (s, e) => await CreateInvite(false);
            btnCreateApproval.Click += async (s, e) => await CreateInvite(true);
            
            var padding = new Panel { Dock = DockStyle.Left, Width = 10 };
            pnlTop.Controls.Add(btnCreateApproval);
            pnlTop.Controls.Add(padding);
            pnlTop.Controls.Add(btnCreatePublic);

            dgvInvites = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = AppTheme.BackgroundCard
            };
            AppTheme.ApplyDataGridViewStyle(dgvInvites);

            dgvInvites.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Code", HeaderText = "Mã Mời", Width = 100 });
            dgvInvites.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RequiresApprovalText", HeaderText = "Loại", Width = 120 });
            dgvInvites.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UseCount", HeaderText = "Số lần dùng", Width = 90 });
            dgvInvites.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StatusText", HeaderText = "Trạng thái", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            var ctxMenu = new ContextMenuStrip();
            var tsmiCopy = new ToolStripMenuItem("Copy Link Mời");
            var tsmiRevoke = new ToolStripMenuItem("Thu hồi Link");
            
            tsmiCopy.Click += TsmiCopy_Click;
            tsmiRevoke.Click += TsmiRevoke_Click;
            
            ctxMenu.Items.Add(tsmiCopy);
            ctxMenu.Items.Add(new ToolStripSeparator());
            ctxMenu.Items.Add(tsmiRevoke);
            dgvInvites.ContextMenuStrip = ctxMenu;

            btnClose = new Button { Text = "Đóng", Dock = DockStyle.Bottom, Height = 40, Cursor = Cursors.Hand };
            AppTheme.ApplyButtonSecondary(btnClose);

            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(dgvInvites);
            this.Controls.Add(pnlTop);
            this.Controls.Add(btnClose);
        }

        private async void LoadInvites()
        {
            var invites = await _authClient.GetInvitesAsync();
            var displayList = new List<dynamic>();
            foreach (var inv in invites)
            {
                displayList.Add(new {
                    inv.Code,
                    RequiresApprovalText = inv.RequiresApproval ? "Cần duyệt" : "Tự do",
                    inv.UseCount,
                    StatusText = inv.IsRevoked ? "Đã thu hồi" : "Đang hoạt động",
                    IsRevoked = inv.IsRevoked
                });
            }
            dgvInvites.DataSource = displayList;
        }

        private async System.Threading.Tasks.Task CreateInvite(bool requiresApproval)
        {
            var inv = await _authClient.CreateInviteAsync(requiresApproval, _serverId);
            if (inv != null)
            {
                MessageBox.Show("Tạo link thành công!\nChuột phải vào danh sách để Copy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadInvites();
            }
            else
            {
                MessageBox.Show("Lỗi khi tạo link.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiCopy_Click(object sender, EventArgs e)
        {
            if (dgvInvites.SelectedRows.Count > 0)
            {
                var row = dgvInvites.SelectedRows[0];
                string code = row.Cells[0].Value.ToString();
                
                // Construct universal deep link
                string serverUrl = document_sharing_manager.Core.Data.UserSession.PublicUrl ?? _serverBaseUrl;
                string encodedUrl = Uri.EscapeDataString(serverUrl);
                string deepLink = $"docshare://join?url={encodedUrl}&code={code}";
                
                Clipboard.SetText(deepLink);
                MessageBox.Show("Đã copy link mời vào Clipboard:\n\n" + deepLink, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void TsmiRevoke_Click(object sender, EventArgs e)
        {
            if (dgvInvites.SelectedRows.Count > 0)
            {
                var row = dgvInvites.SelectedRows[0];
                bool isRevoked = (bool)((dynamic)row.DataBoundItem).IsRevoked;
                if (isRevoked) return;

                string code = row.Cells[0].Value.ToString();
                if (MessageBox.Show($"Bạn có chắc muốn thu hồi link {code}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool success = await _authClient.RevokeInviteAsync(code);
                    if (success) LoadInvites();
                    else MessageBox.Show("Không thể thu hồi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
