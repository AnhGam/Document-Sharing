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

        public static Bitmap CreateUpdateIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.White;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    // Arrow pointing UP
                    float m = size * 0.2f;
                    float cx = size / 2f;

                    // Vertical line
                    g.DrawLine(pen, cx, size - m, cx, m);

                    // Arrow head
                    g.DrawLine(pen, cx, m, cx - size * 0.25f, m + size * 0.25f);
                    g.DrawLine(pen, cx, m, cx + size * 0.25f, m + size * 0.25f);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Icon Import: Thư mục với mũi tên xuống
        /// </summary>
        public static Bitmap CreateImportIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(16, 185, 129);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    // Hộp/tray bên dưới
                    g.DrawLine(pen, size * 0.15f, size * 0.55f, size * 0.15f, size * 0.85f);
                    g.DrawLine(pen, size * 0.15f, size * 0.85f, size * 0.85f, size * 0.85f);
                    g.DrawLine(pen, size * 0.85f, size * 0.85f, size * 0.85f, size * 0.55f);
                    // Mũi tên xuống
                    float cx = size / 2f;
                    g.DrawLine(pen, cx, size * 0.1f, cx, size * 0.6f);
                    g.DrawLine(pen, cx, size * 0.6f, cx - size * 0.2f, size * 0.4f);
                    g.DrawLine(pen, cx, size * 0.6f, cx + size * 0.2f, size * 0.4f);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Icon Recycle Bin: Thùng rác với nắp
        /// </summary>
        public static Bitmap CreateRecycleBinIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(245, 158, 11);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    // Nắp thùng rác
                    g.DrawLine(pen, size * 0.12f, size * 0.25f, size * 0.88f, size * 0.25f);
                    g.DrawLine(pen, size * 0.38f, size * 0.25f, size * 0.38f, size * 0.12f);
                    g.DrawLine(pen, size * 0.38f, size * 0.12f, size * 0.62f, size * 0.12f);
                    g.DrawLine(pen, size * 0.62f, size * 0.12f, size * 0.62f, size * 0.25f);
                    // Thân thùng (hình thang)
                    g.DrawLine(pen, size * 0.2f, size * 0.25f, size * 0.25f, size * 0.88f);
                    g.DrawLine(pen, size * 0.25f, size * 0.88f, size * 0.75f, size * 0.88f);
                    g.DrawLine(pen, size * 0.75f, size * 0.88f, size * 0.8f, size * 0.25f);
                    // Vạch dọc bên trong
                    g.DrawLine(pen, size * 0.4f, size * 0.38f, size * 0.4f, size * 0.75f);
                    g.DrawLine(pen, size * 0.6f, size * 0.38f, size * 0.6f, size * 0.75f);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Icon Bulk/Checklist: Danh sách với checkbox
        /// </summary>
        public static Bitmap CreateChecklistIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(59, 130, 246);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    float boxSize = size * 0.2f;
                    // Checkbox 1 (checked)
                    float y1 = size * 0.15f;
                    g.DrawRectangle(pen, size * 0.1f, y1, boxSize, boxSize);
                    g.DrawLine(pen, size * 0.13f, y1 + boxSize * 0.5f, size * 0.2f, y1 + boxSize * 0.85f);
                    g.DrawLine(pen, size * 0.2f, y1 + boxSize * 0.85f, size * 0.28f, y1 + boxSize * 0.15f);
                    g.DrawLine(pen, size * 0.4f, y1 + boxSize * 0.5f, size * 0.85f, y1 + boxSize * 0.5f);
                    // Checkbox 2 (checked)
                    float y2 = size * 0.42f;
                    g.DrawRectangle(pen, size * 0.1f, y2, boxSize, boxSize);
                    g.DrawLine(pen, size * 0.13f, y2 + boxSize * 0.5f, size * 0.2f, y2 + boxSize * 0.85f);
                    g.DrawLine(pen, size * 0.2f, y2 + boxSize * 0.85f, size * 0.28f, y2 + boxSize * 0.15f);
                    g.DrawLine(pen, size * 0.4f, y2 + boxSize * 0.5f, size * 0.7f, y2 + boxSize * 0.5f);
                    // Checkbox 3 (unchecked)
                    float y3 = size * 0.69f;
                    g.DrawRectangle(pen, size * 0.1f, y3, boxSize, boxSize);
                    g.DrawLine(pen, size * 0.4f, y3 + boxSize * 0.5f, size * 0.75f, y3 + boxSize * 0.5f);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Icon Clock/Recent: Đồng hồ
        /// </summary>
        public static Bitmap CreateClockIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(14, 165, 233);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float m = size * 0.12f;
                using (Pen pen = new Pen(c, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    // Mặt đồng hồ
                    g.DrawEllipse(pen, m, m, size - m * 2, size - m * 2);
                    float cx = size / 2f, cy = size / 2f;
                    // Kim giờ (chỉ 10h)
                    g.DrawLine(pen, cx, cy, cx - size * 0.12f, cy - size * 0.2f);
                    // Kim phút (chỉ 2)
                    g.DrawLine(pen, cx, cy, cx + size * 0.22f, cy - size * 0.08f);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Icon Backup/Save: Ổ đĩa floppy disk
        /// </summary>
        public static Bitmap CreateBackupIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(99, 102, 241);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    float m = size * 0.12f;
                    // Hình đĩa mềm (floppy disk outline)
                    g.DrawLine(pen, m, m, size * 0.65f, m);                    // top-left to notch
                    g.DrawLine(pen, size * 0.65f, m, size - m, m + size * 0.2f); // notch diagonal
                    g.DrawLine(pen, size - m, m + size * 0.2f, size - m, size - m); // right side
                    g.DrawLine(pen, size - m, size - m, m, size - m);            // bottom
                    g.DrawLine(pen, m, size - m, m, m);                          // left side
                    // Khe đĩa bên trong (label area)
                    g.DrawRectangle(pen, size * 0.3f, size * 0.5f, size * 0.4f, size * 0.35f);
                    // Khe trên (metal slider)
                    g.DrawLine(pen, size * 0.35f, m, size * 0.35f, size * 0.3f);
                    g.DrawLine(pen, size * 0.35f, size * 0.3f, size * 0.6f, size * 0.3f);
                    g.DrawLine(pen, size * 0.6f, size * 0.3f, size * 0.6f, m);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Icon Duplicate/Copy: Hai tài liệu chồng lên nhau
        /// </summary>
        public static Bitmap CreateDuplicateIcon(int size = 20, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(234, 179, 8);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(c, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    // Tài liệu phía sau (offset phải-dưới)
                    g.DrawRectangle(pen, size * 0.3f, size * 0.3f, size * 0.58f, size * 0.58f);
                    // Tài liệu phía trước (offset trái-trên)
                    using (Brush fill = new SolidBrush(Color.FromArgb(40, c)))
                        g.FillRectangle(fill, size * 0.12f, size * 0.12f, size * 0.58f, size * 0.58f);
                    g.DrawRectangle(pen, size * 0.12f, size * 0.12f, size * 0.58f, size * 0.58f);
                    // Dòng text trên tài liệu trước
                    g.DrawLine(pen, size * 0.22f, size * 0.32f, size * 0.55f, size * 0.32f);
                    g.DrawLine(pen, size * 0.22f, size * 0.48f, size * 0.45f, size * 0.48f);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Icon thư mục cho tree category
        /// </summary>
        public static Bitmap CreateFolderIcon(int size = 16, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(202, 138, 4);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float s = size / 16f;
                // Tab
                var tab = new PointF[] {
                    new PointF(1*s, 5*s), new PointF(1*s, 3*s),
                    new PointF(6*s, 3*s), new PointF(8*s, 5*s)
                };
                using (var brush = new SolidBrush(c))
                    g.FillPolygon(brush, tab);
                // Body
                using (var brush = new SolidBrush(c))
                    g.FillRectangle(brush, 1*s, 5*s, 14*s, 9*s);
                // Front face (lighter)
                Color lighter = Color.FromArgb(
                    Math.Min(255, c.R + 30),
                    Math.Min(255, c.G + 30),
                    Math.Min(255, c.B + 30));
                using (var brush = new SolidBrush(lighter))
                    g.FillRectangle(brush, 1*s, 7*s, 14*s, 7*s);
            }
            return bmp;
        }

        /// <summary>
        /// Icon home cho node "Tat ca tai lieu"
        /// </summary>
        public static Bitmap CreateHomeIcon(int size = 16, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(37, 99, 235);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float s = size / 16f;
                // Roof
                var roof = new PointF[] {
                    new PointF(8*s, 1*s), new PointF(15*s, 7*s), new PointF(1*s, 7*s)
                };
                using (var brush = new SolidBrush(c))
                    g.FillPolygon(brush, roof);
                // Body
                using (var brush = new SolidBrush(c))
                    g.FillRectangle(brush, 3*s, 7*s, 10*s, 8*s);
                // Door cutout
                using (var brush = new SolidBrush(Color.White))
                    g.FillRectangle(brush, 6*s, 10*s, 4*s, 5*s);
            }
            return bmp;
        }

        /// <summary>
        /// Icon bookmark cho bo suu tap
        /// </summary>
        public static Bitmap CreateBookmarkIcon(int size = 16, Color? color = null)
        {
            Color c = color ?? Color.FromArgb(51, 65, 85);
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float s = size / 16f;
                var points = new PointF[] {
                    new PointF(3*s, 1*s), new PointF(13*s, 1*s),
                    new PointF(13*s, 14*s), new PointF(8*s, 10*s),
                    new PointF(3*s, 14*s)
                };
                using (var brush = new SolidBrush(c))
                    g.FillPolygon(brush, points);
            }
            return bmp;
        }

        #endregion
    }
}
