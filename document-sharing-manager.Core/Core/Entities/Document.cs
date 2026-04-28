using System;
using System.ComponentModel;

namespace document_sharing_manager.Core.Entities
{
    public class Document : BaseEntity
    {
        /// <summary>
        /// Local ID for SQLite (Shadows BaseEntity.Id for compatibility)
        /// </summary>
        [Browsable(false)]
        public new int Id { get; set; }

        public string Ten { get; set; }
        public string DinhDang { get; set; }

        public string DuongDan { get; set; }

        [Browsable(false)]
        public string GhiChu { get; set; }

        public DateTime NgayThem { get; set; }

        [Browsable(false)]
        public double? KichThuoc { get; set; }
        public bool QuanTrong { get; set; }

        [Browsable(false)]
        public string Tags { get; set; }

        public string KichThuocFormatted => FormatFileSize(KichThuoc * 1024 * 1024);

        public static string FormatFileSize(double? bytes)
        {
            if (!bytes.HasValue || bytes.Value <= 0) return "0 B";
            
            double val = bytes.Value;
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            int unitIndex = 0;
            while (val >= 1024 && unitIndex < units.Length - 1)
            {
                val /= 1024;
                unitIndex++;
            }
            return $"{val:0.##} {units[unitIndex]}";
        }

        #region Domain Properties (English Aliases)
        
        [Browsable(false)]
        public string FileName 
        { 
            get => Ten; 
            set { Ten = value; MarkAsUpdated(); } 
        }

        [Browsable(false)]
        public string FilePath 
        { 
            get => DuongDan; 
            set { DuongDan = value; MarkAsUpdated(); } 
        }

        [Browsable(false)]
        public long FileSize 
        { 
            get => (long)(KichThuoc ?? 0); 
            set { KichThuoc = value; MarkAsUpdated(); } 
        }

        [Browsable(false)]
        public Guid OwnerId { get; private set; }

        [Browsable(false)]
        public int Version { get; private set; } = 1;

        [Browsable(false)]
        public bool IsDeleted { get; private set; } = false;

        #endregion

        public Document()
        {
            NgayThem = DateTime.Now;
            QuanTrong = false;
        }

        #region Domain Methods

        public void IncrementVersion()
        {
            Version++;
            MarkAsUpdated();
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            MarkAsUpdated();
        }

        public void SetOwner(Guid ownerId)
        {
            OwnerId = ownerId;
            MarkAsUpdated();
        }

        #endregion
    }
}

