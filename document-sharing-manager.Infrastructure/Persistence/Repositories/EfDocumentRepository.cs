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

        public async Task<BaseEntity> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return (await _context.Documents.FindAsync([id], ct))!;
        }

        public async Task<IEnumerable<BaseEntity>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Documents.AsNoTracking().ToListAsync(ct);
        }

        public async Task AddAsync(BaseEntity entity, CancellationToken ct = default)
        {
            if (entity is not Document doc)
                throw new ArgumentException("Entity must be of type Document", nameof(entity));

            await _context.Documents.AddAsync(doc, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(BaseEntity entity, CancellationToken ct = default)
        {
            var tracked = _context.ChangeTracker.Entries<BaseEntity>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (tracked == null)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            
            // Prevent CreatedAt from being updated
            _context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            
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

        public async Task<IEnumerable<BaseEntity>> GetFilesByOwnerAsync(int ownerId, CancellationToken ct = default)
        {
            // Note: Currently Document doesn't have OwnerId, placeholder for future
            return await _context.Documents.AsNoTracking().ToListAsync(ct);
        }

        public Task<BaseEntity> GetByVersionAsync(int docId, int version, CancellationToken ct = default)
        {
            throw new NotImplementedException("Version control is in roadmap.");
        }

        public async Task<List<Document>> SearchAsync(string keyword, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return await _context.Documents.AsNoTracking().ToListAsync(ct);
            }

            // Accuracy: Escape SQL wildcards to prevent incorrect search results
            var escapedKeyword = keyword.Replace("%", "\\%").Replace("_", "\\_");
            var searchPattern = $"%{escapedKeyword}%";

            return await _context.Documents
                .AsNoTracking()
                .Where(d => EF.Functions.ILike(d.Ten, searchPattern) || (d.GhiChu != null && EF.Functions.ILike(d.GhiChu, searchPattern)))
                .ToListAsync(ct);
        }

        public async Task<List<Document>> SearchAdvancedAsync(string keyword, string format, DateTime? fromDate, DateTime? toDate, decimal? minSize, decimal? maxSize, bool? isImportant, CancellationToken ct = default)
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

            return await query.ToListAsync(ct);
        }

        // --- Synchronous Legacy Implementation ---

        public List<Document> GetAll()
        {
            return _context.Documents.AsNoTracking().ToList();
        }

        public Document GetById(int id)
        {
            return _context.Documents.Find(id)!;
        }

        public List<Document> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            return _context.Documents
                .AsNoTracking()
                .Where(d => EF.Functions.ILike(d.Ten, $"%{keyword}%") || (d.GhiChu != null && EF.Functions.ILike(d.GhiChu, $"%{keyword}%")))
                .ToList();
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

            return query.ToList();
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
            return _context.Documents
                .AsNoTracking()
                .Select(d => d.DinhDang)
                .Distinct()
                .ToList();
        }
    }
}
