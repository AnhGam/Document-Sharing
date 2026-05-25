using System;

namespace document_sharing_manager.Core.Domain
{
    public class InviteLink : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public bool RequiresApproval { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUses { get; set; }
        public int UseCount { get; set; }
        public bool IsRevoked { get; set; }
    }
}
