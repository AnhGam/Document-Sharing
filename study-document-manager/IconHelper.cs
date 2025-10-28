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
        /// Lấy icon theo loại tài liệu
        /// </summary>
        public static Bitmap GetDocumentIcon(string loai, int size = 24)
        {
            loai = loai?.ToLower() ?? "";

            if (loai.Contains("slide") || loai.Contains("ppt") || loai.Contains("powerpoint"))
            {
                return CreatePowerPointIcon(size);
            }
            else if (loai.Contains("bài tập") || loai.Contains("word") || loai.Contains("doc"))
            {
                return CreateWordIcon(size);
            }
            else if (loai.Contains("đề thi") || loai.Contains("pdf"))
            {
                return CreatePdfIcon(size);
            }
            else if (loai.Contains("excel") || loai.Contains("xls"))
            {
                return CreateExcelIcon(size);
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
        /// Tạo icon mặc định (màu xám)
        /// </summary>
        private static Bitmap CreateDefaultIcon(int size)
        {
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Nền xám
                using (Brush brush = new SolidBrush(Color.FromArgb(158, 158, 158)))
                {
                    g.FillRectangle(brush, 2, 2, size - 4, size - 4);
                }

                // Viền xám đậm
                using (Pen pen = new Pen(Color.FromArgb(117, 117, 117), 2))
                {
                    g.DrawRectangle(pen, 2, 2, size - 4, size - 4);
                }

                // Icon file
                using (Font font = new Font("Segoe UI Symbol", size / 3, FontStyle.Regular))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("??", font, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bitmap;
        }
    }
}
