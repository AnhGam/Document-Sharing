using System;

namespace document_sharing_manager.Core.Domain
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public Guid RemoteId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }

        public virtual void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
