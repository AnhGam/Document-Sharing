using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace document_sharing_manager_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServersController(AppDbContext context, IConfiguration configuration) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        private int CurrentUserId
        {
            get
            {
                var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out var userId))
                {
                    throw new System.UnauthorizedAccessException("User identification claim is missing or invalid.");
                }
                return userId;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManagedServer>>> GetAll(CancellationToken ct)
        {
            var servers = await _context.Servers
                .Where(s => s.UserId == CurrentUserId)
                .ToListAsync(ct);
            return Ok(servers);
        }

        [HttpPost]
        public async Task<ActionResult<ManagedServer>> Create([FromBody] ManagedServer server, CancellationToken ct)
        {
            // Validate Server Password
            string? requiredPassword = _configuration["Server:JoinPassword"]?.Trim();
            string incomingPassword = server.ServerPassword?.Trim() ?? string.Empty;

            if (!string.IsNullOrEmpty(requiredPassword) && incomingPassword != requiredPassword)
            {
                return Unauthorized(new { message = "Invalid server join password." });
            }

            // Check if already exists for this user
            var existing = await _context.Servers
                .FirstOrDefaultAsync(s => s.UserId == CurrentUserId && s.BaseUrl == server.BaseUrl, ct);

            if (existing != null)
            {
                existing.Name = server.Name;
                existing.AccessToken = server.AccessToken;
                existing.IsActive = true;
                _context.Servers.Update(existing);
                await _context.SaveChangesAsync(ct);
                return Ok(existing);
            }

            server.UserId = CurrentUserId;
            _context.Servers.Add(server);
            await _context.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetAll), new { id = server.Id }, server);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var server = await _context.Servers
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == CurrentUserId, ct);

            if (server == null) return NotFound();

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync(ct);

            return NoContent();
        }
    }
}
