using System;
using System.Drawing;
using System.Windows.Forms;
using document_sharing_manager.UI;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Services;

namespace document_sharing_manager.Management
{
    public partial class AuditLogForm : Form
    {
        private DataGridView dgvLogs;
        private Button btnClose;
        private readonly AuthServiceClient _authClient;

        public AuditLogForm(AuthServiceClient authClient)
        {
            _authClient = authClient;
            InitializeComponentManual();
            LoadLogs();
        }

        private void InitializeComponentManual()
        {
            this.Text = "Nhật ký hệ thống (Audit Logs)";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = AppTheme.BackgroundMain;

            dgvLogs = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = AppTheme.BackgroundCard
            };
            AppTheme.ApplyDataGridViewStyle(dgvLogs);

            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CreatedAt", HeaderText = "Thời gian", Width = 150 });
            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserName", HeaderText = "Người thực hiện", Width = 150 });
            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Action", HeaderText = "Hành động", Width = 120 });
            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EntityType", HeaderText = "Đối tượng", Width = 120 });
            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Details", HeaderText = "Chi tiết", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            btnClose = new Button { Text = "Đóng", Dock = DockStyle.Bottom, Height = 40, Cursor = Cursors.Hand };
            AppTheme.ApplyButtonSecondary(btnClose);
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(dgvLogs);
            this.Controls.Add(btnClose);
        }

        private async void LoadLogs()
        {
            var logs = await _authClient.GetAuditLogsAsync(1, 100);
            dgvLogs.DataSource = logs;
        }
    }
}
