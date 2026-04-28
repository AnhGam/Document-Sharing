using System;
using System.ComponentModel;

namespace document_sharing_manager.Core.Entities
{
    public abstract class BaseEntity
    {
        [Browsable(false)]
        public Guid Id { get; protected set; } = Guid.NewGuid();

        [Browsable(false)]
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        [Browsable(false)]
        public DateTime? UpdatedAt { get; protected set; }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

