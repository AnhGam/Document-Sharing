using System;

namespace document_sharing_manager.Core.Domain
{
    public class Document : BaseEntity
    {
        public string Ten { get; set; }
        public string DinhDang { get; set; }
        public string DuongDan { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayThem { get; set; } = DateTime.Now;
        public decimal? KichThuoc { get; set; }
        public bool QuanTrong { get; set; }
        public string Tags { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string KichThuocFormatted => FormatFileSize(KichThuoc ?? 0m);

        public static string FormatFileSize(decimal bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            for (i = 0; i < Suffix.Length - 1 && bytes >= 1024m; i++)
            {
                bytes /= 1024.0m;
            }

            return String.Format("{0:0.##} {1}", bytes, Suffix[i]);
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.Now;
            Update();
        }
    }
}
