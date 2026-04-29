using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Common;
using document_sharing_manager.Core.DTOs;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IDocumentService
    {
        Task<Result<DocumentDto>> GetDocumentAsync(int id, CancellationToken ct = default);
        Task<Result<IEnumerable<DocumentDto>>> GetUserDocumentsAsync(int userId, CancellationToken ct = default);
        Task<Result<bool>> ShareDocumentAsync(int docId, int targetUserId, CancellationToken ct = default);
        Task<Result<bool>> SoftDeleteDocumentAsync(int id, CancellationToken ct = default);
    }
}
