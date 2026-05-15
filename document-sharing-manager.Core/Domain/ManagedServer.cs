using System;

namespace document_sharing_manager.Core.Domain
{
    public class ManagedServer : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string ServerPassword { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastSyncDate { get; set; }
        
        // Trạng thái kết nối
        public int ConnectionStatus { get; set; } // 0: Connected, 1: Unauthorized, 2: Offline
    }
}
