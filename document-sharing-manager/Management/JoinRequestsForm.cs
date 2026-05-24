using System;
using System.Drawing;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Services;
using System.Collections.Generic;

namespace document_sharing_manager.Management
{
    public partial class JoinRequestsForm : Form
    {
        private DataGridView dgvRequests;
        private Button btnClose;
        private readonly AuthServiceClient _authClient;

        public JoinRequestsForm(AuthServiceClient authClient)
        {
            _authClient = authClient;
            InitializeComponentManual();
            LoadRequests();
        }

        private void InitializeComponentManual()
        {
            this.Text = "Yêu cầu tham gia";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = AppTheme.BackgroundMain;

            dgvRequests = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = AppTheme.BackgroundCard
            };
            AppTheme.ApplyDataGridViewStyle(dgvRequests);

            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DisplayName", HeaderText = "Tên hiển thị", Width = 150 });
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "InviteCode", HeaderText = "Mã Mời", Width = 100 });
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CreatedAt", HeaderText = "Ngày yêu cầu", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            var ctxMenu = new ContextMenuStrip();
            var tsmiApprove = new ToolStripMenuItem("Phê duyệt");
            var tsmiDeny = new ToolStripMenuItem("Từ chối");
            
            tsmiApprove.Click += TsmiApprove_Click;
            tsmiDeny.Click += TsmiDeny_Click;
            
            ctxMenu.Items.Add(tsmiApprove);
            ctxMenu.Items.Add(tsmiDeny);
            dgvRequests.ContextMenuStrip = ctxMenu;

            btnClose = new Button { Text = "Đóng", Dock = DockStyle.Bottom, Height = 40, Cursor = Cursors.Hand };
            AppTheme.ApplyButtonSecondary(btnClose);
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(dgvRequests);
            this.Controls.Add(btnClose);
        }

        private async void LoadRequests()
        {
            var requests = await _authClient.GetPendingJoinRequestsAsync();
            dgvRequests.DataSource = requests;
        }

        private async void TsmiApprove_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count > 0)
            {
                var row = dgvRequests.SelectedRows[0];
                int id = ((JoinRequest)row.DataBoundItem).Id;
                
                bool success = await _authClient.ApproveJoinRequestAsync(id);
                if (success) LoadRequests();
                else MessageBox.Show("Không thể phê duyệt.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TsmiDeny_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count > 0)
            {
                var row = dgvRequests.SelectedRows[0];
                int id = ((JoinRequest)row.DataBoundItem).Id;
                
                bool success = await _authClient.DenyJoinRequestAsync(id);
                if (success) LoadRequests();
                else MessageBox.Show("Không thể từ chối.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
