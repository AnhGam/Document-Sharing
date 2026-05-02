using System;
using System.ComponentModel.DataAnnotations;

namespace document_sharing_manager.Core.Domain
{
    public class Document : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Ten { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DinhDang { get; set; } = string.Empty;

        [Required]
        public string DuongDan { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string GhiChu { get; set; } = string.Empty;

        public DateTime NgayThem { get; set; } = DateTime.UtcNow;
        public decimal? KichThuoc { get; set; }
        public bool QuanTrong { get; set; }

        [MaxLength(500)]
        public string Tags { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int Version { get; set; } = 1;
        public int UserId { get; set; }

        public string KichThuocFormatted => FormatFileSize(KichThuoc ?? 0m);

        public static string FormatFileSize(decimal bytes)
        {
            string[] Suffix = ["B", "KB", "MB", "GB", "TB"];
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
            DeletedAt = DateTime.UtcNow;
            Update();
        }
    }
}
