using System.Collections.Generic;
using System.Security.Claims;
using document_sharing_manager.Core.Domain;

namespace document_sharing_manager.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
