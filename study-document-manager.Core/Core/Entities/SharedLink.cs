using System;

namespace study_document_manager.Core.Entities
{
    public class SharedLink : BaseEntity
    {
        public Guid DocumentId { get; private set; }
        public Guid SharedWithUserId { get; private set; }
        public SharePermission Permission { get; private set; }

        public SharedLink(Guid documentId, Guid sharedWithUserId, SharePermission permission)
        {
            DocumentId = documentId;
            SharedWithUserId = sharedWithUserId;
            Permission = permission;
        }

        public void UpdatePermission(SharePermission newPermission)
        {
            Permission = newPermission;
            MarkAsUpdated();
        }
    }
}

