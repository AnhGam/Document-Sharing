using System;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.DTOs;
using document_sharing_manager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace document_sharing_manager_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var response = await _authService.LoginAsync(request, ct);
            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var response = await _authService.RegisterAsync(request, ct);
            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken, CancellationToken ct)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken, ct);
            return Ok(response);
        }
    }
}
