using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task<IEnumerable<Document>> GetAllByUserIdAsync(int userId, CancellationToken ct = default);
        Task<Document?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken ct = default);
        Task<Document?> GetByVersionAsync(int docId, int version, int userId, CancellationToken ct = default);
        Task<Document?> GetByPathAsync(string path, CancellationToken ct = default);
        Task<Document?> GetByRemoteIdAsync(Guid remoteId, CancellationToken ct = default);

        Task<List<Document>> SearchAsync(string keyword, int userId, CancellationToken ct = default);
        Task<List<Document>> SearchAdvancedAsync(string keyword, string format, DateTime? fromDate, DateTime? toDate, decimal? minSize, decimal? maxSize, bool? isImportant, int userId, CancellationToken ct = default);
        Task<List<Document>> GetPendingSyncDocumentsAsync(int userId, CancellationToken ct = default);
        Task UpdateSyncStatusAsync(int id, int syncStatus, int? newVersion = null, int? expectedVersion = null, int? newLocalVersion = null, CancellationToken ct = default);

        // Synchronous Legacy methods for UI compatibility
        List<Document> GetAll();
        Document? GetById(int id);
        List<Document> Search(string keyword);
        List<Document> SearchAdvanced(string keyword, string format, DateTime? fromDate, DateTime? toDate, decimal? minSize, decimal? maxSize, bool? isImportant);
        bool Update(Document doc);
        bool Delete(int id);
        List<string> GetDistinctFormats();
    }
}
