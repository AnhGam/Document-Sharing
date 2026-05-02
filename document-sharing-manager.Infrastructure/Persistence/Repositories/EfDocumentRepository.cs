using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document_sharing_manager.Infrastructure.Persistence.Repositories
{
    public class EfDocumentRepository(AppDbContext context) : IDocumentRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Document?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Documents.FindAsync([id], ct);
        }

        public async Task<IEnumerable<Document>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Documents.AsNoTracking().ToListAsync(ct);
        }

        public async Task AddAsync(Document entity, CancellationToken ct = default)
        {
            await _context.Documents.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Document entity, CancellationToken ct = default)
        {
            var trackedEntry = _context.ChangeTracker.Entries<Document>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (trackedEntry != null && trackedEntry.Entity != entity)
            {
                _context.Entry(trackedEntry.Entity).CurrentValues.SetValues(entity);
            }
            else if (trackedEntry == null)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            
            var entry = trackedEntry ?? _context.Entry(entity);
            entry.Property(x => x.CreatedAt).IsModified = false;
            entry.Property(x => x.RemoteId).IsModified = false; // Never change RemoteId
            
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Documents.FindAsync([id], ct);
            if (entity != null)
            {
                entity.SoftDelete();
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<IEnumerable<Document>> GetAllByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Documents
                .AsNoTracking()
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<Document?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken ct = default)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId && !d.IsDeleted, ct);
        }

        public async Task<Document?> GetByVersionAsync(int docId, int version, int userId, CancellationToken ct = default)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == docId && d.Version == version && d.UserId == userId, ct);
        }

        public async Task<Document?> GetByPathAsync(string path, CancellationToken ct = default)
        {
            return await _context.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DuongDan == path && !d.IsDeleted, ct);
        }

        public async Task<Document?> GetByRemoteIdAsync(Guid remoteId, CancellationToken ct = default)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d => d.RemoteId == remoteId && !d.IsDeleted, ct);
        }

        public async Task<List<Document>> SearchAsync(string keyword, int userId, CancellationToken ct = default)
        {
            var query = _context.Documents.AsNoTracking().Where(d => d.UserId == userId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.Ten.Contains(keyword) || (d.GhiChu != null && d.GhiChu.Contains(keyword)));
            }

            return await query.ToListAsync(ct);
        }

        public async Task<List<Document>> SearchAdvancedAsync(string keyword, string format, DateTime? fromDate, DateTime? toDate, decimal? minSize, decimal? maxSize, bool? isImportant, int userId, CancellationToken ct = default)
        {
            var query = _context.Documents.AsNoTracking().Where(d => d.UserId == userId).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(d => d.Ten.Contains(keyword));

            if (!string.IsNullOrEmpty(format))
                query = query.Where(d => d.DinhDang == format);

            if (fromDate.HasValue)
                query = query.Where(d => d.NgayThem >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(d => d.NgayThem <= toDate.Value);

            if (minSize.HasValue)
                query = query.Where(d => d.KichThuoc >= minSize.Value);

            if (maxSize.HasValue)
                query = query.Where(d => d.KichThuoc <= maxSize.Value);

            if (isImportant.HasValue)
                query = query.Where(d => d.QuanTrong == isImportant.Value);

            return await query.ToListAsync(ct);
        }

        // --- Synchronous Legacy Implementation ---

        public List<Document> GetAll()
        {
            return [.. _context.Documents.AsNoTracking()];
        }

        public Document? GetById(int id)
        {
            return _context.Documents.Find(id);
        }

        public List<Document> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            return [.. _context.Documents
                .AsNoTracking()
                .Where(d => d.Ten.Contains(keyword) || (d.GhiChu != null && d.GhiChu.Contains(keyword)))];
        }

        public List<Document> SearchAdvanced(string keyword, string format, DateTime? fromDate, DateTime? toDate, decimal? minSize, decimal? maxSize, bool? isImportant)
        {
            var query = _context.Documents.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(d => d.Ten.Contains(keyword));

            if (!string.IsNullOrEmpty(format))
                query = query.Where(d => d.DinhDang == format);

            if (fromDate.HasValue)
                query = query.Where(d => d.NgayThem >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(d => d.NgayThem <= toDate.Value);

            if (minSize.HasValue)
                query = query.Where(d => d.KichThuoc >= minSize.Value);

            if (maxSize.HasValue)
                query = query.Where(d => d.KichThuoc <= maxSize.Value);

            if (isImportant.HasValue)
                query = query.Where(d => d.QuanTrong == isImportant.Value);

            return [.. query];
        }

        public bool Update(Document doc)
        {
            _context.Documents.Update(doc);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var doc = _context.Documents.Find(id);
            if (doc != null)
            {
                doc.SoftDelete();
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public List<string> GetDistinctFormats()
        {
            return [.. _context.Documents
                .AsNoTracking()
                .Select(d => d.DinhDang)
                .Distinct()];
        }
        public async Task<List<Document>> GetPendingSyncDocumentsAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Documents
                .Where(d => d.UserId == userId && d.SyncStatus != 0 && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task UpdateSyncStatusAsync(int id, int syncStatus, int? newVersion = null, int? expectedVersion = null, int? newLocalVersion = null, CancellationToken ct = default)
        {
            var doc = await _context.Documents.FindAsync([id], ct);
            if (doc != null)
            {
                if (expectedVersion.HasValue && doc.Version != expectedVersion.Value)
                {
                    return; 
                }
                
                doc.SyncStatus = syncStatus;
                if (newVersion.HasValue) doc.Version = newVersion.Value;
                if (newLocalVersion.HasValue) doc.LocalVersion = newLocalVersion.Value;
                
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}
