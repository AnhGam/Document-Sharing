using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.DTOs;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using document_sharing_manager.Core.Configurations;
using Microsoft.Extensions.Options;
using BC = BCrypt.Net.BCrypt;

namespace document_sharing_manager.Infrastructure.Security
{
    public class AuthService(AppDbContext context, ITokenService tokenService, IOptions<JwtSettings> jwtOptions, IOptions<SecuritySettings> securityOptions) : IAuthService
    {
        private readonly AppDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;
        private readonly SecuritySettings _securitySettings = securityOptions.Value;

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var normalizedUsername = request.Username?.ToLowerInvariant() ?? string.Empty;
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == normalizedUsername, ct);

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is deactivated.");
            }

            return await CreateAuthResponseAsync(user, ct);
        }

        public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var normalizedUsername = request.Username?.ToLowerInvariant() ?? string.Empty;
            var normalizedEmail = request.Email?.ToLowerInvariant() ?? string.Empty;
            
            if (await _context.Users.AnyAsync(u => u.Username == normalizedUsername || u.Email.ToLower() == normalizedEmail, ct))
            {
                throw new InvalidOperationException("Username or Email already exists.");
            }

            var user = new User
            {
                Username = normalizedUsername,
                Email = normalizedEmail,
                PasswordHash = BC.HashPassword(request.Password!, _securitySettings.BCryptWorkFactor),
                Role = UserRole.User
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);

            return new UserDto
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                Message = "User registered successfully. Please login to get your access token."
            };
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
                await _context.RefreshTokens
                    .Where(t => t.UserId == tokenEntity.UserId && !t.IsRevoked)
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true), ct);

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
            return await CreateAuthResponseAsync(user, ct);
        }

        private async Task<AuthResponse> CreateAuthResponseAsync(User user, CancellationToken ct)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays)
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
    }
}
