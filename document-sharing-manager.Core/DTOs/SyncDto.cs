using System.ComponentModel.DataAnnotations;

namespace document_sharing_manager.Core.DTOs
{
    public class SyncRequest
    {
        [Required]
        public int DocumentId { get; set; }

        [Required]
        public int LocalVersion { get; set; }

        public string? Content { get; set; } // Simplified for now, or use IFormFile for actual uploads
    }

    public class SyncResponse
    {
        public bool Success { get; set; }
        public int CurrentVersion { get; set; }
        public string? Message { get; set; }
        public bool Conflict { get; set; }
    }
}
