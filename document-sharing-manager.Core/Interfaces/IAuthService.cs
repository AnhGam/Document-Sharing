using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.DTOs;

namespace document_sharing_manager.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResponse> RefreshTokenAsync(RefreshRequest request, CancellationToken ct = default);
    }
}
