using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Common;
using document_sharing_manager.Core.DTOs;

namespace document_sharing_manager.Core.Interfaces
{
    public interface ISyncService
    {
        Task<Result> SyncAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<DocumentDto>>> GetPendingUploadsAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<DocumentDto>>> GetPendingDownloadsAsync(CancellationToken ct = default);
    }
}
