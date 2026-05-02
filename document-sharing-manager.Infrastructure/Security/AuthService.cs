using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.DTOs;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace document_sharing_manager.Infrastructure.Security
{
    public class AuthService(AppDbContext context, ITokenService tokenService, Microsoft.Extensions.Configuration.IConfiguration config) : IAuthService
    {
        private readonly AppDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config = config;

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower(), ct);

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is deactivated.");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(int.TryParse(_config["JWT:RefreshTokenDurationInDays"], out var days) ? days : 7)
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity, ct);
            await _context.SaveChangesAsync(ct);

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == request.Username.ToLower(), ct))
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BC.HashPassword(request.Password, int.TryParse(_config["Security:BCryptWorkFactor"], out var factor) ? factor : 10),
                Role = UserRole.User
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);

            return await LoginAsync(new LoginRequest { Username = request.Username, Password = request.Password }, ct);
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshRequest request, CancellationToken ct = default)
        {
            var tokenEntity = await _context.RefreshTokens
                .Include(u => u.User)
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, ct);

            if (tokenEntity == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            if (tokenEntity.IsRevoked)
            {
                // Reuse detection: revoke all tokens for this user
                var activeTokens = await _context.RefreshTokens
                    .Where(t => t.UserId == tokenEntity.UserId && !t.IsRevoked)
                    .ToListAsync(ct);

                foreach (var token in activeTokens)
                {
                    token.IsRevoked = true;
                }

                await _context.SaveChangesAsync(ct);
                throw new UnauthorizedAccessException("Refresh token reuse detected. All sessions revoked.");
            }

            if (tokenEntity.IsExpired)
            {
                throw new UnauthorizedAccessException("Refresh token has expired.");
            }

            var user = tokenEntity.User!;
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is deactivated.");
            }

            // Revoke old token
            tokenEntity.IsRevoked = true;
            
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(int.TryParse(_config["JWT:RefreshTokenDurationInDays"], out var days) ? days : 7)
            };

            await _context.RefreshTokens.AddAsync(newRefreshTokenEntity, ct);
            await _context.SaveChangesAsync(ct);

            return new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Username = user.Username,
                Role = user.Role.ToString()
            };
        }
    }
}
