using System;

namespace document_sharing_manager.Core.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsImportant { get; set; }
        public string Tags { get; set; }
    }
}
