using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using document_sharing_manager.UI;

namespace document_sharing_manager.UI.Controls
{
    public class DocumentPreviewPanel : UserControl
    {
        private PictureBox pictureBox;
        private WebBrowser webBrowser;
        private Label lblNoPreview;
        private Label lblFileName;
        private Panel pnlHeader;
        private Button btnOpenFile;

        private string currentFilePath;

        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".ico", ".tiff", ".tif", ".webp" };
        private static readonly string[] VideoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".webm", ".flv", ".m4v" };

        public DocumentPreviewPanel()
        {
            InitializeUI();
            ApplyTheme();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;

            // Header
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 35 };
            lblFileName = new Label
            {
                Text = "Xem trước",
                Font = new Font(AppTheme.FontFamily, 10f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };

            btnOpenFile = new Button
            {
                Text = "Mở",
                Dock = DockStyle.Right,
                Width = 60,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnOpenFile.FlatAppearance.BorderSize = 0;
            btnOpenFile.Click += BtnOpenFile_Click;

            pnlHeader.Controls.Add(lblFileName);
            pnlHeader.Controls.Add(btnOpenFile);
            this.Controls.Add(pnlHeader);

            // PictureBox for images
            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Visible = false
            };
            this.Controls.Add(pictureBox);

            // WebBrowser for video (HTML5 video player)
            webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                Visible = false,
                ScriptErrorsSuppressed = true,
                IsWebBrowserContextMenuEnabled = false
            };
            this.Controls.Add(webBrowser);

            // No preview label
            lblNoPreview = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Chọn tài liệu để xem trước",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(AppTheme.FontFamily, 11f),
                Visible = true
            };
            this.Controls.Add(lblNoPreview);

            pictureBox.BringToFront();
            webBrowser.BringToFront();
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.BackgroundCard;
            pnlHeader.BackColor = AppTheme.BackgroundSoft;
            lblFileName.ForeColor = AppTheme.TextPrimary;
            lblNoPreview.ForeColor = AppTheme.TextSecondary;

            btnOpenFile.BackColor = AppTheme.Primary;
            btnOpenFile.ForeColor = Color.White;
            btnOpenFile.Font = AppTheme.FontSmallBold;
        }

        public void LoadPreview(string filePath, string fileName)
        {
            ClearPreview();
            lblFileName.Text = fileName ?? "Xem trước";
            currentFilePath = filePath;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                lblNoPreview.Text = "File không tồn tại";
                lblNoPreview.Visible = true;
                return;
            }

            btnOpenFile.Visible = true;
            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            if (Array.Exists(ImageExtensions, e => e == ext))
            {
                LoadImage(filePath);
            }
            else if (Array.Exists(VideoExtensions, e => e == ext))
            {
                LoadVideo(filePath);
            }
            else
            {
                lblNoPreview.Text = "Không hỗ trợ xem trước\nloại file này";
                lblNoPreview.Visible = true;
            }
        }

        private void LoadImage(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                var ms = new MemoryStream(bytes);
                var oldImage = pictureBox.Image;
                pictureBox.Image = Image.FromStream(ms);
                oldImage?.Dispose();
                pictureBox.Visible = true;
            }
            catch
            {
                lblNoPreview.Text = "Không thể tải hình ảnh";
                lblNoPreview.Visible = true;
            }
        }

        private void LoadVideo(string path)
        {
            try
            {
                string fullPath = Path.GetFullPath(path).Replace("\\", "/");
                string html = $@"<!DOCTYPE html>
<html><head><style>
  body {{ margin:0; background:#000; display:flex; align-items:center; justify-content:center; height:100vh; }}
  video {{ max-width:100%; max-height:100%; }}
</style></head><body>
  <video controls autoplay src=""file:///{fullPath}""></video>
</body></html>";

                webBrowser.DocumentText = html;
                webBrowser.Visible = true;
            }
            catch
            {
                lblNoPreview.Text = "Không thể phát video";
                lblNoPreview.Visible = true;
            }
        }

        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath) || !File.Exists(currentFilePath))
            {
                ToastNotification.Warning("File không tồn tại!");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = currentFilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ToastNotification.Error("Không thể mở file: " + ex.Message);
            }
        }

        public void ClearPreview()
        {
            pictureBox.Visible = false;
            webBrowser.Visible = false;
            lblNoPreview.Visible = false;
            btnOpenFile.Visible = false;

            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }

            try { webBrowser.Navigate("about:blank"); } catch { }

            lblFileName.Text = "Xem trước";
            currentFilePath = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearPreview();
                webBrowser?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

