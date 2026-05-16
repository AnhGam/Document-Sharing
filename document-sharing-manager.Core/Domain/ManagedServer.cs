using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace document_sharing_manager.Core.Domain
{
    public class ManagedServer : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public int UserId { get; set; }
        [NotMapped]
        public int? CloudId { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        [JsonPropertyName("serverPassword")]
        [JsonProperty("serverPassword")]
        public string ServerPassword { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastSyncDate { get; set; }
        
        // Trạng thái kết nối
        public int ConnectionStatus { get; set; } // 0: Connected, 1: Unauthorized, 2: Offline

        // Local-only properties
        private Services.AuthServiceClient? _authClient;
        public Services.AuthServiceClient GetAuthClient()
        {
            return _authClient ??= new Services.AuthServiceClient(BaseUrl);
        }
    }
}
