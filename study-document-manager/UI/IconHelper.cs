using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace study_document_manager
{
    /// <summary>
    /// Helper class để tạo icon động cho các loại tài liệu
    /// </summary>
    public static class IconHelper
    {
        /// <summary>
        /// Lấy icon theo loại tài liệu và tên file
        /// </summary>
        public static Bitmap GetDocumentIcon(string loai, int size = 24, string fileName = null)
        {
            loai = loai?.ToLower() ?? "";
            fileName = fileName?.ToLower() ?? "";
            string fileExt = System.IO.Path.GetExtension(fileName) ?? "";

            if (fileExt == ".xlsx" || fileExt == ".xls" || fileExt == ".xlsm" || fileExt == ".csv" ||
                fileName.EndsWith("xlsx") || fileName.EndsWith("xls") || fileName.EndsWith("xlsm") ||
                loai.Contains("excel") || loai.Contains("xls"))
            {
                return CreateExcelIcon(size);
            }
            else if (fileExt == ".pdf" || fileName.EndsWith("pdf") ||
                     loai.Contains("đề thi") || loai.Contains("pdf"))
            {
                return CreatePdfIcon(size);
            }
            else if (fileExt == ".pptx" || fileExt == ".ppt" || fileExt == ".ppsx" ||
                     fileName.EndsWith("pptx") || fileName.EndsWith("ppt") ||
                     loai.Contains("slide") || loai.Contains("ppt") || loai.Contains("powerpoint"))
            {
                return CreatePowerPointIcon(size);
            }
            else if (fileExt == ".docx" || fileExt == ".doc" || fileExt == ".rtf" ||
                     fileName.EndsWith("docx") || fileName.EndsWith("doc") ||
                     loai.Contains("bài tập") || loai.Contains("word") || loai.Contains("doc"))
            {
                return CreateWordIcon(size);
            }
            else
            {
                return CreateDefaultIcon(size);
            }
        }

        /// <summary>
        /// Tạo icon sao vàng
        /// </summary>
        public static Bitmap CreateStarIcon(int size = 24)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Vẽ sao vàng
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 202, 40)))
                {
                    PointF[] starPoints = GetStarPoints(size / 2, size / 2, size / 2 - 2, size / 4);
                    g.FillPolygon(brush, starPoints);
                }

                // Viền vàng đậm
                using (Pen pen = new Pen(Color.FromArgb(255, 193, 7), 1.5f))
                {
                    PointF[] starPoints = GetStarPoints(size / 2, size / 2, size / 2 - 2, size / 4);
                    g.DrawPolygon(pen, starPoints);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tính toán điểm cho hình sao 5 cánh
        /// </summary>
        private static PointF[] GetStarPoints(float centerX, float centerY, float outerRadius, float innerRadius)
        {
            PointF[] points = new PointF[10];
            double angle = -Math.PI / 2; // Bắt đầu từ trên
            double angleStep = Math.PI / 5; // 36 độ

            for (int i = 0; i < 10; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                points[i] = new PointF(
                    centerX + (float)(radius * Math.Cos(angle)),
                    centerY + (float)(radius * Math.Sin(angle))
                );
                angle += angleStep;
            }
            return points;
        }

        /// <summary>
        /// Tạo icon PDF (màu đỏ)
        /// </summary>
        private static Bitmap CreatePdfIcon(int size)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Nền đỏ
                using (Brush brush = new SolidBrush(Color.FromArgb(244, 67, 54)))
                {
                    g.FillRectangle(brush, 2, 2, size - 4, size - 4);
                }

                // Viền đỏ đậm
                using (Pen pen = new Pen(Color.FromArgb(211, 47, 47), 2))
                {
                    g.DrawRectangle(pen, 2, 2, size - 4, size - 4);
                }

                // Chữ PDF
                using (Font font = new Font("Arial", size / 4, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("PDF", font, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon Word (màu xanh dương)
        /// </summary>
        private static Bitmap CreateWordIcon(int size)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Nền xanh dương
                using (Brush brush = new SolidBrush(Color.FromArgb(33, 150, 243)))
                {
                    g.FillRectangle(brush, 2, 2, size - 4, size - 4);
                }

                // Viền xanh đậm
                using (Pen pen = new Pen(Color.FromArgb(25, 118, 210), 2))
                {
                    g.DrawRectangle(pen, 2, 2, size - 4, size - 4);
                }

                // Chữ W
                using (Font font = new Font("Arial", size / 3, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("W", font, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon PowerPoint (màu cam)
        /// </summary>
        private static Bitmap CreatePowerPointIcon(int size)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Nền cam
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 152, 0)))
                {
                    g.FillRectangle(brush, 2, 2, size - 4, size - 4);
                }

                // Viền cam đậm
                using (Pen pen = new Pen(Color.FromArgb(245, 124, 0), 2))
                {
                    g.DrawRectangle(pen, 2, 2, size - 4, size - 4);
                }

                // Chữ P
                using (Font font = new Font("Arial", size / 3, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("P", font, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon Excel (màu xanh lá)
        /// </summary>
        private static Bitmap CreateExcelIcon(int size)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Nền xanh lá
                using (Brush brush = new SolidBrush(Color.FromArgb(76, 175, 80)))
                {
                    g.FillRectangle(brush, 2, 2, size - 4, size - 4);
                }

                // Viền xanh đậm
                using (Pen pen = new Pen(Color.FromArgb(56, 142, 60), 2))
                {
                    g.DrawRectangle(pen, 2, 2, size - 4, size - 4);
                }

                // Chữ X
                using (Font font = new Font("Arial", size / 3, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("X", font, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon mặc định (icon tài liệu)
        /// </summary>
        private static Bitmap CreateDefaultIcon(int size)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                float margin = size * 0.15f;
                float width = size - 2 * margin;
                float height = size - 2 * margin;
                float foldSize = size * 0.2f;

                // Vẽ hình tài liệu với góc gập
                PointF[] docShape = new PointF[]
                {
                    new PointF(margin, margin),
                    new PointF(margin + width - foldSize, margin),
                    new PointF(margin + width, margin + foldSize),
                    new PointF(margin + width, margin + height),
                    new PointF(margin, margin + height)
                };

                // Nền trắng
                using (Brush brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
                {
                    g.FillPolygon(brush, docShape);
                }

                // Góc gập
                PointF[] foldShape = new PointF[]
                {
                    new PointF(margin + width - foldSize, margin),
                    new PointF(margin + width - foldSize, margin + foldSize),
                    new PointF(margin + width, margin + foldSize)
                };
                using (Brush foldBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
                {
                    g.FillPolygon(foldBrush, foldShape);
                }

                // Viền xám
                using (Pen pen = new Pen(Color.FromArgb(158, 158, 158), 1.5f))
                {
                    g.DrawPolygon(pen, docShape);
                    g.DrawPolygon(pen, foldShape);
                }

                // Các đường kẻ ngang giả lập text
                using (Pen linePen = new Pen(Color.FromArgb(180, 180, 180), 1f))
                {
                    float lineY = margin + foldSize + size * 0.1f;
                    float lineSpacing = size * 0.12f;
                    for (int i = 0; i < 3; i++)
                    {
                        float lineWidth = (i == 2) ? width * 0.5f : width * 0.7f;
                        g.DrawLine(linePen, margin + size * 0.1f, lineY, margin + size * 0.1f + lineWidth, lineY);
                        lineY += lineSpacing;
                    }
                }
            }
            return bitmap;
        }

        #region UI Action Icons

        /// <summary>
        /// Tạo icon con mắt (show/hide password)
        /// </summary>
        public static Bitmap CreateEyeIcon(int size = 20, bool isOpen = true)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                float centerY = size / 2f;
                float eyeWidth = size * 0.8f;
                float eyeHeight = size * 0.4f;
                float startX = (size - eyeWidth) / 2f;

                using (Pen pen = new Pen(Color.FromArgb(100, 100, 100), 1.5f))
                {
                    // Vẽ hình mắt (ellipse)
                    g.DrawEllipse(pen, startX, centerY - eyeHeight / 2, eyeWidth, eyeHeight);
                    
                    // Vẽ con ngươi
                    float pupilSize = size * 0.25f;
                    g.FillEllipse(Brushes.DimGray, 
                        size / 2f - pupilSize / 2, 
                        centerY - pupilSize / 2, 
                        pupilSize, pupilSize);

                    // Nếu đóng, vẽ đường gạch chéo
                    if (!isOpen)
                    {
                        using (Pen strikePen = new Pen(Color.FromArgb(244, 67, 54), 2f))
                        {
                            g.DrawLine(strikePen, 2, size - 2, size - 2, 2);
                        }
                    }
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon X (close/clear)
        /// </summary>
        public static Bitmap CreateCloseIcon(int size = 16, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(158, 158, 158);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    
                    float margin = size * 0.25f;
                    g.DrawLine(pen, margin, margin, size - margin, size - margin);
                    g.DrawLine(pen, size - margin, margin, margin, size - margin);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon mũi tên xuống (chevron down)
        /// </summary>
        public static Bitmap CreateChevronDownIcon(int size = 16, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(100, 100, 100);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    
                    float margin = size * 0.25f;
                    float midX = size / 2f;
                    float topY = size * 0.35f;
                    float bottomY = size * 0.65f;
                    
                    g.DrawLine(pen, margin, topY, midX, bottomY);
                    g.DrawLine(pen, midX, bottomY, size - margin, topY);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon dấu cộng (add)
        /// </summary>
        public static Bitmap CreateAddIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(76, 175, 80);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 2.5f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    
                    float margin = size * 0.2f;
                    float mid = size / 2f;
                    
                    // Đường ngang
                    g.DrawLine(pen, margin, mid, size - margin, mid);
                    // Đường dọc
                    g.DrawLine(pen, mid, margin, mid, size - margin);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon bút chì (edit)
        /// </summary>
        public static Bitmap CreateEditIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(33, 150, 243);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 1.5f))
                {
                    // Thân bút
                    PointF[] pencilBody = new PointF[]
                    {
                        new PointF(size * 0.7f, size * 0.1f),
                        new PointF(size * 0.9f, size * 0.3f),
                        new PointF(size * 0.3f, size * 0.9f),
                        new PointF(size * 0.1f, size * 0.7f)
                    };
                    g.DrawPolygon(pen, pencilBody);
                    g.FillPolygon(new SolidBrush(Color.FromArgb(80, iconColor)), pencilBody);
                    
                    // Đầu bút
                    g.DrawLine(pen, size * 0.1f, size * 0.7f, size * 0.05f, size * 0.95f);
                    g.DrawLine(pen, size * 0.05f, size * 0.95f, size * 0.3f, size * 0.9f);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon thùng rác (delete)
        /// </summary>
        public static Bitmap CreateDeleteIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(244, 67, 54);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 1.5f))
                {
                    float left = size * 0.2f;
                    float right = size * 0.8f;
                    float top = size * 0.25f;
                    float bottom = size * 0.85f;
                    
                    // Thân thùng rác
                    g.DrawLine(pen, left, top, left + (right - left) * 0.1f, bottom);
                    g.DrawLine(pen, right, top, right - (right - left) * 0.1f, bottom);
                    g.DrawLine(pen, left + (right - left) * 0.1f, bottom, right - (right - left) * 0.1f, bottom);
                    
                    // Nắp
                    g.DrawLine(pen, size * 0.15f, top, size * 0.85f, top);
                    g.DrawLine(pen, size * 0.35f, top, size * 0.35f, size * 0.15f);
                    g.DrawLine(pen, size * 0.65f, top, size * 0.65f, size * 0.15f);
                    g.DrawLine(pen, size * 0.35f, size * 0.15f, size * 0.65f, size * 0.15f);
                    
                    // Các đường dọc trong thùng
                    g.DrawLine(pen, size * 0.35f, size * 0.4f, size * 0.35f, size * 0.75f);
                    g.DrawLine(pen, size * 0.5f, size * 0.4f, size * 0.5f, size * 0.75f);
                    g.DrawLine(pen, size * 0.65f, size * 0.4f, size * 0.65f, size * 0.75f);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon folder mở (open file)
        /// </summary>
        public static Bitmap CreateOpenIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(255, 193, 7);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Brush brush = new SolidBrush(iconColor))
                using (Pen pen = new Pen(Color.FromArgb(245, 127, 23), 1.5f))
                {
                    // Folder shape
                    PointF[] folder = new PointF[]
                    {
                        new PointF(size * 0.1f, size * 0.3f),
                        new PointF(size * 0.35f, size * 0.3f),
                        new PointF(size * 0.45f, size * 0.2f),
                        new PointF(size * 0.9f, size * 0.2f),
                        new PointF(size * 0.9f, size * 0.8f),
                        new PointF(size * 0.1f, size * 0.8f)
                    };
                    g.FillPolygon(brush, folder);
                    g.DrawPolygon(pen, folder);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon export (mũi tên ra ngoài)
        /// </summary>
        public static Bitmap CreateExportIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(33, 150, 243);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 1.5f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    
                    // Mũi tên lên
                    float midX = size / 2f;
                    g.DrawLine(pen, midX, size * 0.15f, midX, size * 0.6f);
                    g.DrawLine(pen, midX, size * 0.15f, size * 0.3f, size * 0.35f);
                    g.DrawLine(pen, midX, size * 0.15f, size * 0.7f, size * 0.35f);
                    
                    // Khay
                    g.DrawLine(pen, size * 0.15f, size * 0.5f, size * 0.15f, size * 0.85f);
                    g.DrawLine(pen, size * 0.15f, size * 0.85f, size * 0.85f, size * 0.85f);
                    g.DrawLine(pen, size * 0.85f, size * 0.85f, size * 0.85f, size * 0.5f);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon refresh (mũi tên xoay tròn)
        /// </summary>
        public static Bitmap CreateRefreshIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(76, 175, 80);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    
                    // Arc
                    float margin = size * 0.15f;
                    g.DrawArc(pen, margin, margin, size - 2 * margin, size - 2 * margin, -60, 300);
                    
                    // Mũi tên
                    float arrowX = size * 0.75f;
                    float arrowY = size * 0.25f;
                    g.DrawLine(pen, arrowX, arrowY - size * 0.15f, arrowX, arrowY);
                    g.DrawLine(pen, arrowX, arrowY, arrowX + size * 0.15f, arrowY);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon người dùng với gear (role management)
        /// </summary>
        public static Bitmap CreateRoleIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(156, 39, 176);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Pen pen = new Pen(iconColor, 1.5f))
                using (Brush brush = new SolidBrush(iconColor))
                {
                    // Đầu người
                    float headSize = size * 0.3f;
                    g.FillEllipse(brush, size * 0.2f, size * 0.1f, headSize, headSize);
                    
                    // Thân người
                    g.FillEllipse(brush, size * 0.1f, size * 0.45f, size * 0.45f, size * 0.4f);
                    
                    // Gear nhỏ ở góc
                    float gearX = size * 0.6f;
                    float gearY = size * 0.5f;
                    float gearSize = size * 0.35f;
                    
                    // Vẽ gear đơn giản
                    g.DrawEllipse(pen, gearX, gearY, gearSize, gearSize);
                    g.FillEllipse(Brushes.White, gearX + gearSize * 0.25f, gearY + gearSize * 0.25f, gearSize * 0.5f, gearSize * 0.5f);
                    
                    // Răng gear
                    float cx = gearX + gearSize / 2;
                    float cy = gearY + gearSize / 2;
                    for (int i = 0; i < 6; i++)
                    {
                        double angle = i * Math.PI / 3;
                        float x1 = cx + (float)(gearSize / 2 * Math.Cos(angle));
                        float y1 = cy + (float)(gearSize / 2 * Math.Sin(angle));
                        float x2 = cx + (float)((gearSize / 2 + 2) * Math.Cos(angle));
                        float y2 = cy + (float)((gearSize / 2 + 2) * Math.Sin(angle));
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon thống kê (chart)
        /// </summary>
        public static Bitmap CreateChartIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(33, 150, 243);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (Brush brush = new SolidBrush(iconColor))
                {
                    // Các cột biểu đồ
                    g.FillRectangle(brush, size * 0.15f, size * 0.5f, size * 0.2f, size * 0.35f);
                    g.FillRectangle(brush, size * 0.4f, size * 0.3f, size * 0.2f, size * 0.55f);
                    g.FillRectangle(brush, size * 0.65f, size * 0.15f, size * 0.2f, size * 0.7f);
                }

                using (Pen pen = new Pen(iconColor, 1.5f))
                {
                    // Trục
                    g.DrawLine(pen, size * 0.1f, size * 0.1f, size * 0.1f, size * 0.9f);
                    g.DrawLine(pen, size * 0.1f, size * 0.9f, size * 0.9f, size * 0.9f);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Tạo icon cài đặt (gear)
        /// </summary>
        public static Bitmap CreateSettingsIcon(int size = 20, Color? color = null)
        {
            Color iconColor = color ?? Color.FromArgb(100, 100, 100);
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                float cx = size / 2f;
                float cy = size / 2f;
                float outerR = size * 0.4f;
                float innerR = size * 0.2f;

                using (Pen pen = new Pen(iconColor, 1.5f))
                {
                    // Vòng tròn giữa
                    g.DrawEllipse(pen, cx - innerR, cy - innerR, innerR * 2, innerR * 2);
                    
                    // Răng gear
                    for (int i = 0; i < 8; i++)
                    {
                        double angle = i * Math.PI / 4;
                        float x1 = cx + (float)(innerR * 1.2f * Math.Cos(angle));
                        float y1 = cy + (float)(innerR * 1.2f * Math.Sin(angle));
                        float x2 = cx + (float)(outerR * Math.Cos(angle));
                        float y2 = cy + (float)(outerR * Math.Sin(angle));
                        g.DrawLine(pen, x1, y1, x2, y2);
                        
                        // Đầu răng
                        double angle1 = angle - Math.PI / 16;
                        double angle2 = angle + Math.PI / 16;
                        float tx1 = cx + (float)(outerR * Math.Cos(angle1));
                        float ty1 = cy + (float)(outerR * Math.Sin(angle1));
                        float tx2 = cx + (float)(outerR * Math.Cos(angle2));
                        float ty2 = cy + (float)(outerR * Math.Sin(angle2));
                        g.DrawLine(pen, tx1, ty1, tx2, ty2);
                    }
                }
            }
            return bitmap;
        }

        #endregion
    }
}
