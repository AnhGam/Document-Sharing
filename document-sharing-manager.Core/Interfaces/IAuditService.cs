using System.Threading;
using System.Threading.Tasks;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int userId, string userName, string action, string entityType, string entityId, string details, string ipAddress, CancellationToken ct = default);
    }
}
