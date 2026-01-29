using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace study_document_manager
{
    /// <summary>
    /// Helper class để tạo icon cho các loại tài liệu
    /// </summary>
    public static class IconHelper
    {
        private static string _iconFolder;

        /// <summary>
        /// Lấy thư mục chứa icon
        /// </summary>
        private static string GetIconFolder()
        {
            if (_iconFolder != null) return _iconFolder;

            // Thử các đường dẫn có thể
            string[] paths = {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "logo", "icon"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "assets", "logo", "icon"),
                Path.Combine(Application.StartupPath, "assets", "logo", "icon"),
            };

            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                {
                    _iconFolder = path;
                    return _iconFolder;
                }
            }

            return null;
        }

        /// <summary>
        /// Load icon từ file
        /// </summary>
        private static Bitmap LoadIcon(string fileName, int size)
        {
            try
            {
                string folder = GetIconFolder();
                if (folder == null) return null;

                string filePath = Path.Combine(folder, fileName);
                if (!File.Exists(filePath)) return null;

                using (var original = new Bitmap(filePath))
                {
                    // Resize về kích thước yêu cầu
                    Bitmap resized = new Bitmap(size, size);
                    using (Graphics g = Graphics.FromImage(resized))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawImage(original, 0, 0, size, size);
                    }
                    return resized;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy icon dựa vào đường dẫn file (ưu tiên extension)
        /// </summary>
        public static Bitmap GetDocumentIcon(string loai, int size = 24, string filePath = null)
        {
            // Đảm bảo size hợp lệ
            if (size <= 0) size = 24;

            // Lấy extension từ đường dẫn file
            string ext = GetExtension(filePath);

            Bitmap icon = null;

            // Ưu tiên extension trước
            switch (ext)
            {
                // Word
                case ".doc":
                case ".docx":
                case ".rtf":
                case ".odt":
                    icon = LoadIcon("word.png", size);
                    break;

                // Excel
                case ".xls":
                case ".xlsx":
                case ".xlsm":
                case ".csv":
                case ".ods":
                    icon = LoadIcon("excel.png", size);
                    break;

                // PDF
                case ".pdf":
                    icon = LoadIcon("pdf.png", size);
                    break;

                // PowerPoint
                case ".ppt":
                case ".pptx":
                case ".ppsx":
                case ".odp":
                    icon = LoadIcon("powerpoint.png", size);
                    break;

                // Images
                case ".jpg":
                case ".jpeg":
                    icon = LoadIcon("jpg.png", size);
                    break;
                case ".png":
                case ".gif":
                case ".bmp":
                case ".webp":
                case ".tiff":
                    icon = LoadIcon("png.png", size);
                    break;
                case ".svg":
                case ".ico":
                    icon = LoadIcon("svg.png", size);
                    break;

                // Video
                case ".mp4":
                case ".avi":
                case ".mkv":
                case ".mov":
                case ".wmv":
                case ".flv":
                case ".webm":
                    icon = LoadIcon("mp4.png", size);
                    break;
            }

            // Nếu load được icon thì trả về
            if (icon != null) return icon;

            // Fallback: dùng loại tài liệu
            string type = (loai ?? "").ToLower();
            if (type.Contains("word") || type.Contains("tài liệu") || type.Contains("báo cáo"))
            {
                icon = LoadIcon("word.png", size);
                if (icon != null) return icon;
            }
            if (type.Contains("excel") || type.Contains("bảng tính"))
            {
                icon = LoadIcon("excel.png", size);
                if (icon != null) return icon;
            }
            if (type.Contains("pdf") || type.Contains("đề thi"))
            {
                icon = LoadIcon("pdf.png", size);
                if (icon != null) return icon;
            }
            if (type.Contains("slide") || type.Contains("powerpoint"))
            {
                icon = LoadIcon("powerpoint.png", size);
                if (icon != null) return icon;
            }
            if (type.Contains("hình ảnh") || type.Contains("ảnh") || type.Contains("image"))
            {
                icon = LoadIcon("png.png", size);
                if (icon != null) return icon;
            }
            if (type.Contains("video"))
            {
                icon = LoadIcon("mp4.png", size);
                if (icon != null) return icon;
            }

            // Fallback cuối cùng: tạo icon mặc định
            return CreateDefaultIcon(size);
        }

        /// <summary>
        /// Lấy extension từ đường dẫn file
        /// </summary>
        private static string GetExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "";

            try
            {
                return Path.GetExtension(filePath).ToLower();
            }
            catch
            {
                // Fallback: tìm dấu chấm cuối cùng
                int lastDot = filePath.LastIndexOf('.');
                if (lastDot >= 0 && lastDot < filePath.Length - 1)
                    return filePath.Substring(lastDot).ToLower();
                return "";
            }
        }

        private static Bitmap CreateDefaultIcon(int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Nền xám nhạt
                using (Brush brush = new SolidBrush(Color.FromArgb(158, 158, 158)))
                    g.FillRectangle(brush, 1, 1, size - 2, size - 2);

                // Icon tài liệu
                float m = size * 0.2f;
                PointF[] doc = {
                    new PointF(m, m),
                    new PointF(size - m - size * 0.15f, m),
                    new PointF(size - m, m + size * 0.15f),
                    new PointF(size - m, size - m),
                    new PointF(m, size - m)
                };
                using (Brush fill = Brushes.White)
                    g.FillPolygon(fill, doc);
            }
            return bmp;
        }

        #region Other Icons

        /// <summary>
        /// Icon sao vàng cho quan trọng
        /// </summary>
        public static Bitmap CreateStarIcon(int size = 24)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                PointF[] star = GetStarPoints(size / 2f, size / 2f, size / 2f - 2, size / 4f);
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 193, 7)))
                    g.FillPolygon(brush, star);
            }
            return bmp;
        }

        private static PointF[] GetStarPoints(float cx, float cy, float outer, float inner)
        {
            PointF[] pts = new PointF[10];
            double angle = -Math.PI / 2;
            for (int i = 0; i < 10; i++)
            {
                float r = (i % 2 == 0) ? outer : inner;
                pts[i] = new PointF(cx + (float)(r * Math.Cos(angle)), cy + (float)(r * Math.Sin(angle)));
                angle += Math.PI / 5;
            }
            return pts;
        }

        // UI Icons
        public static Bitmap CreateEyeIcon(int size = 20, bool isOpen = true)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float cy = size / 2f;
                using (Pen pen = new Pen(Color.Gray, 1.5f))
                {
                    g.DrawEllipse(pen, size * 0.1f, cy - size * 0.2f, size * 0.8f, size * 0.4f);
                    g.FillEllipse(Brushes.DimGray, size * 0.4f, cy - size * 0.1f, size * 0.2f, size * 0.2f);
                    if (!isOpen)
                        g.DrawLine(new Pen(Color.Red, 2), 2, size - 2, size - 2, 2);
                }
            }
            return bmp;
        }

        public static Bitmap CreateCloseIcon(int size = 16, Color? color = null)
        {
            Color c = color ?? Color.Gray;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 2) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    float m = size * 0.25f;
                    g.DrawLine(pen, m, m, size - m, size - m);
                    g.DrawLine(pen, size - m, m, m, size - m);
                }
            }
            return bmp;
        }

        public static Bitmap CreateChevronDownIcon(int size = 16, Color? color = null)
        {
            Color c = color ?? Color.Gray;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 2) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawLine(pen, size * 0.25f, size * 0.35f, size * 0.5f, size * 0.65f);
                    g.DrawLine(pen, size * 0.5f, size * 0.65f, size * 0.75f, size * 0.35f);
                }
            }
            return bmp;
        }

        public static Bitmap CreateAddIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(76, 175, 80);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    float m = size * 0.2f;
                    g.DrawLine(pen, m, size / 2f, size - m, size / 2f);
                    g.DrawLine(pen, size / 2f, m, size / 2f, size - m);
                }
            }
            return bmp;
        }

        public static Bitmap CreateEditIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(33, 150, 243);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f))
                {
                    PointF[] pencil = { new PointF(size * 0.7f, size * 0.1f), new PointF(size * 0.9f, size * 0.3f), new PointF(size * 0.3f, size * 0.9f), new PointF(size * 0.1f, size * 0.7f) };
                    g.DrawPolygon(pen, pencil);
                }
            }
            return bmp;
        }

        public static Bitmap CreateDeleteIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(244, 67, 54);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f))
                {
                    g.DrawRectangle(pen, size * 0.25f, size * 0.25f, size * 0.5f, size * 0.6f);
                    g.DrawLine(pen, size * 0.15f, size * 0.25f, size * 0.85f, size * 0.25f);
                    g.DrawLine(pen, size * 0.4f, size * 0.15f, size * 0.6f, size * 0.15f);
                }
            }
            return bmp;
        }

        public static Bitmap CreateOpenIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(255, 193, 7);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Brush brush = new SolidBrush(c))
                {
                    PointF[] folder = { new PointF(size * 0.1f, size * 0.3f), new PointF(size * 0.35f, size * 0.3f), new PointF(size * 0.45f, size * 0.2f), new PointF(size * 0.9f, size * 0.2f), new PointF(size * 0.9f, size * 0.8f), new PointF(size * 0.1f, size * 0.8f) };
                    g.FillPolygon(brush, folder);
                }
            }
            return bmp;
        }

        public static Bitmap CreateExportIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(33, 150, 243);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawLine(pen, size / 2f, size * 0.15f, size / 2f, size * 0.6f);
                    g.DrawLine(pen, size / 2f, size * 0.15f, size * 0.3f, size * 0.35f);
                    g.DrawLine(pen, size / 2f, size * 0.15f, size * 0.7f, size * 0.35f);
                    g.DrawLine(pen, size * 0.15f, size * 0.5f, size * 0.15f, size * 0.85f);
                    g.DrawLine(pen, size * 0.15f, size * 0.85f, size * 0.85f, size * 0.85f);
                    g.DrawLine(pen, size * 0.85f, size * 0.85f, size * 0.85f, size * 0.5f);
                }
            }
            return bmp;
        }

        public static Bitmap CreateRefreshIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(76, 175, 80);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 2) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawArc(pen, size * 0.15f, size * 0.15f, size * 0.7f, size * 0.7f, -60, 300);
                }
            }
            return bmp;
        }

        public static Bitmap CreateRoleIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(156, 39, 176);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Brush brush = new SolidBrush(c))
                {
                    g.FillEllipse(brush, size * 0.25f, size * 0.1f, size * 0.3f, size * 0.3f);
                    g.FillEllipse(brush, size * 0.15f, size * 0.45f, size * 0.4f, size * 0.35f);
                }
            }
            return bmp;
        }

        public static Bitmap CreateChartIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(33, 150, 243);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Brush brush = new SolidBrush(c))
                {
                    g.FillRectangle(brush, size * 0.15f, size * 0.5f, size * 0.2f, size * 0.35f);
                    g.FillRectangle(brush, size * 0.4f, size * 0.3f, size * 0.2f, size * 0.55f);
                    g.FillRectangle(brush, size * 0.65f, size * 0.15f, size * 0.2f, size * 0.7f);
                }
            }
            return bmp;
        }

        public static Bitmap CreateSettingsIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.Gray;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f))
                {
                    float cx = size / 2f, cy = size / 2f, r = size * 0.2f;
                    g.DrawEllipse(pen, cx - r, cy - r, r * 2, r * 2);
                    for (int i = 0; i < 8; i++)
                    {
                        double a = i * Math.PI / 4;
                        g.DrawLine(pen, cx + (float)(r * 1.2f * Math.Cos(a)), cy + (float)(r * 1.2f * Math.Sin(a)), cx + (float)(r * 1.8f * Math.Cos(a)), cy + (float)(r * 1.8f * Math.Sin(a)));
                    }
                }
            }
            return bmp;
        }

        #endregion
    }
}
