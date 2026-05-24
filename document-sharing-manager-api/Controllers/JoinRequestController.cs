using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace document_sharing_manager_api.Controllers
{
    [ApiController]
    [Route("api/join-requests")]
    [Authorize]
    public class JoinRequestController(AppDbContext context, IAuditService auditService) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IAuditService _auditService = auditService;

        private int CurrentUserId
        {
            get
            {
                var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out var userId))
                {
                    return 0;
                }
                return userId;
            }
        }

        private string CurrentUserName => User.Identity?.Name ?? "Anonymous";

        // GET: api/join-requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JoinRequest>>> GetPendingRequests(CancellationToken ct)
        {
            var requests = await _context.JoinRequests
                .Where(r => r.Status == JoinRequestStatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
            return Ok(requests);
        }

        // POST: api/join-requests/{id}/approve
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, CancellationToken ct)
        {
            var req = await _context.JoinRequests.FindAsync(new object[] { id }, ct);
            if (req == null) return NotFound();

            if (req.Status != JoinRequestStatus.Pending)
                return BadRequest(new { message = "Request is already processed." });

            req.Status = JoinRequestStatus.Approved;
            req.ReviewedByUserId = CurrentUserId;
            req.ReviewedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            await _auditService.LogAsync(CurrentUserId, CurrentUserName, "ApproveJoin", "JoinRequest", id.ToString(), $"Approved user: {req.DisplayName}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", ct);

            return Ok(new { message = "Request approved." });
        }

        // POST: api/join-requests/{id}/deny
        [HttpPost("{id}/deny")]
        public async Task<IActionResult> Deny(int id, CancellationToken ct)
        {
            var req = await _context.JoinRequests.FindAsync(new object[] { id }, ct);
            if (req == null) return NotFound();

            if (req.Status != JoinRequestStatus.Pending)
                return BadRequest(new { message = "Request is already processed." });

            req.Status = JoinRequestStatus.Denied;
            req.ReviewedByUserId = CurrentUserId;
            req.ReviewedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            await _auditService.LogAsync(CurrentUserId, CurrentUserName, "DenyJoin", "JoinRequest", id.ToString(), $"Denied user: {req.DisplayName}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", ct);

            return Ok(new { message = "Request denied." });
        }
    }
}
