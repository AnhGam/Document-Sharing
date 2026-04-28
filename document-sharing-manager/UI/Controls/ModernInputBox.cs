using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using document_sharing_manager.UI;

namespace document_sharing_manager.UI.Controls
{
    public static class ModernInputBox
    {
        public static string Show(string title, string label, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = AppTheme.BackgroundMain,
                ShowInTaskbar = false
            };

            Label lblText = new Label()
            {
                Left = 20,
                Top = 20,
                Text = label,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true
            };

            TextBox txtInput = new TextBox()
            {
                Left = 20,
                Top = 45,
                Width = 340,
                Font = AppTheme.FontInput,
                Text = defaultValue,
                BackColor = AppTheme.InputBackground,
                ForeColor = AppTheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnOk = new Button()
            {
                Text = "OK",
                Left = 180,
                Width = 80,
                Top = 85,
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyButtonSuccess(btnOk);
            btnOk.Height = 35; // Adjust height for dialog

            Button btnCancel = new Button()
            {
                Text = "Hủy",
                Left = 270,
                Width = 80,
                Top = 85,
                DialogResult = DialogResult.Cancel,
                Cursor = Cursors.Hand
            };
            AppTheme.ApplyButtonSecondary(btnCancel);
            btnCancel.Height = 35;

            prompt.Controls.Add(lblText);
            prompt.Controls.Add(txtInput);
            prompt.Controls.Add(btnOk);
            prompt.Controls.Add(btnCancel);
            prompt.AcceptButton = btnOk;
            prompt.CancelButton = btnCancel;

            txtInput.SelectAll();

            // Set focus to textbox when shown
            prompt.Shown += (s, e) => txtInput.Focus();

            return prompt.ShowDialog() == DialogResult.OK ? txtInput.Text.Trim() : "";
        }
    }
}

