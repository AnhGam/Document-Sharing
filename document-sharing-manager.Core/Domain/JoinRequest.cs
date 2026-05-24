using System;

namespace document_sharing_manager.Core.Domain
{
    public enum JoinRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Denied = 2
    }

    public class JoinRequest : BaseEntity
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string InviteCode { get; set; } = string.Empty;
        public JoinRequestStatus Status { get; set; }
        public int? ReviewedByUserId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        
        public User? User { get; set; }
    }
}
