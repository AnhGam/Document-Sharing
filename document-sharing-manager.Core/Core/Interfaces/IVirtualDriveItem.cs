using System;

namespace document_sharing_manager.Core.Interfaces
{
    /// <summary>
    /// Ghost Concept for Dokan.Net integration without polluting Domain
    /// </summary>
    public interface IVirtualDriveItem
    {
        Guid Id { get; }
        string Name { get; }
        string Path { get; }
        bool IsDirectory { get; }
        long Size { get; }
        DateTime CreatedAt { get; }
        DateTime? ModifiedAt { get; }
    }
}

