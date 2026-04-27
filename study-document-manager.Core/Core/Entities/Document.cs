using System;

namespace study_document_manager.Core.Entities
{
    public class Document : BaseEntity
    {
        /// <summary>
        /// Local ID for SQLite (Shadows BaseEntity.Id for compatibility)
        /// </summary>
        public new int Id { get; set; }

        public string Ten { get; set; }
        public string DanhMuc { get; set; }
        public string DinhDang { get; set; }
        public string DuongDan { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayThem { get; set; }
        public double? KichThuoc { get; set; }
        public bool QuanTrong { get; set; }
        public string Tags { get; set; }

        #region Domain Properties (English Aliases)
        
        public string FileName 
        { 
            get => Ten; 
            set { Ten = value; MarkAsUpdated(); } 
        }

        public string FilePath 
        { 
            get => DuongDan; 
            set { DuongDan = value; MarkAsUpdated(); } 
        }

        public long FileSize 
        { 
            get => (long)(KichThuoc ?? 0); 
            set { KichThuoc = value; MarkAsUpdated(); } 
        }

        public Guid OwnerId { get; private set; }
        public int Version { get; private set; } = 1;
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

