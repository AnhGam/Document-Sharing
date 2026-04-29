using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IDocumentRepository : IRepository<BaseEntity>
    {
        Task<IEnumerable<BaseEntity>> GetFilesByOwnerAsync(int ownerId, CancellationToken ct = default);
        Task<BaseEntity> GetByVersionAsync(int docId, int version, CancellationToken ct = default);

        // Synchronous Legacy methods for UI compatibility
        List<Document> GetAll();
        Document GetById(int id);
        List<Document> Search(string keyword);
        List<Document> SearchAdvanced(string keyword, string format, DateTime? fromDate, DateTime? toDate, double? minSize, double? maxSize, bool? isImportant);
        bool Update(Document doc);
        bool Delete(int id);
        List<string> GetDistinctFormats();
    }
}
