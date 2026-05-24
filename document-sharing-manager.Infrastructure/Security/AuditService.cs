using System;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Infrastructure.Persistence;

namespace document_sharing_manager.Infrastructure.Security
{
    public class AuditService(AppDbContext context) : IAuditService
    {
        private readonly AppDbContext _context = context;

        public async Task LogAsync(int userId, string userName, string action, string entityType, string entityId, string details, string ipAddress, CancellationToken ct = default)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                IpAddress = ipAddress
            };

            await _context.AuditLogs.AddAsync(auditLog, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
