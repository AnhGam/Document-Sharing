using System;
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
    [Route("api/[controller]")]
    public class InviteController(AppDbContext context, IAuditService auditService) : ControllerBase
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
                    return 0; // For anonymous requests
                }
                return userId;
            }
        }

        private string CurrentUserName => User.Identity?.Name ?? "Anonymous";

        // POST: api/invite
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<InviteLink>> Create([FromBody] CreateInviteRequest request, CancellationToken ct)
        {
            var invite = new InviteLink
            {
                Code = Guid.NewGuid().ToString("N").Substring(0, 10), // Random 10-char code
                CreatedByUserId = CurrentUserId,
                RequiresApproval = request.RequiresApproval,
                ServerId = request.ServerId,
                ExpiresAt = null,
                MaxUses = null,
                UseCount = 0,
                IsRevoked = false
            };

            _context.InviteLinks.Add(invite);
            await _context.SaveChangesAsync(ct);

            await _auditService.LogAsync(CurrentUserId, CurrentUserName, "CreateInvite", "InviteLink", invite.Id.ToString(), $"Code: {invite.Code}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", ct);

            return Ok(invite);
        }

        // GET: api/invite
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InviteLink>>> GetInvites(CancellationToken ct)
        {
            var invites = await _context.InviteLinks.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
            return Ok(invites);
        }

        // DELETE: api/invite/{code}
        [HttpDelete("{code}")]
        [Authorize]
        public async Task<IActionResult> Revoke(string code, CancellationToken ct)
        {
            var invite = await _context.InviteLinks.FirstOrDefaultAsync(x => x.Code == code, ct);
            if (invite == null) return NotFound();

            invite.IsRevoked = true;
            await _context.SaveChangesAsync(ct);

            await _auditService.LogAsync(CurrentUserId, CurrentUserName, "RevokeInvite", "InviteLink", invite.Id.ToString(), $"Code: {invite.Code}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", ct);

            return NoContent();
        }

        // GET: api/invite/{code}/info
        [HttpGet("{code}/info")]
        public async Task<ActionResult> GetInfo(string code, CancellationToken ct)
        {
            var invite = await _context.InviteLinks.FirstOrDefaultAsync(x => x.Code == code, ct);
            if (invite == null || invite.IsRevoked)
                return NotFound(new { message = "Invalid or revoked invite code." });

            return Ok(new { 
                invite.Code, 
                invite.RequiresApproval
            });
        }

        // POST: api/invite/{code}/join
        [HttpPost("{code}/join")]
        public async Task<ActionResult> Join(string code, [FromBody] JoinRequestPayload payload, CancellationToken ct)
        {
            var invite = await _context.InviteLinks.FirstOrDefaultAsync(x => x.Code == code, ct);
            if (invite == null || invite.IsRevoked)
                return BadRequest(new { message = "Invalid or revoked invite code." });

            var displayName = payload.DisplayName?.Trim();
            if (string.IsNullOrEmpty(displayName))
                return BadRequest(new { message = "Display name is required." });

            invite.UseCount++;
            
            var joinRequest = new JoinRequest
            {
                UserId = CurrentUserId, // 0 if not logged in
                DisplayName = displayName,
                InviteCode = code,
                ServerId = invite.ServerId,
                Status = invite.RequiresApproval ? JoinRequestStatus.Pending : JoinRequestStatus.Approved
            };

            _context.JoinRequests.Add(joinRequest);
            
            // Nếu không cần kiểm duyệt, tự động kết nối thành viên vào Kênh chia sẻ
            if (joinRequest.Status == JoinRequestStatus.Approved && invite.ServerId.HasValue)
            {
                var targetChannel = await _context.Servers.FirstOrDefaultAsync(s => s.Id == invite.ServerId.Value, ct);
                if (targetChannel != null)
                {
                    bool alreadyMember = await _context.Servers.AnyAsync(s => s.UserId == CurrentUserId && s.BaseUrl == targetChannel.BaseUrl, ct);
                    if (!alreadyMember)
                    {
                        var membership = new ManagedServer
                        {
                            Name = targetChannel.Name,
                            BaseUrl = targetChannel.BaseUrl,
                            ServerPassword = targetChannel.ServerPassword,
                            UserId = CurrentUserId,
                            IsActive = true,
                            ConnectionStatus = 0
                        };
                        _context.Servers.Add(membership);
                    }
                }
            }

            await _context.SaveChangesAsync(ct);

            await _auditService.LogAsync(CurrentUserId, displayName, "JoinServer", "JoinRequest", joinRequest.Id.ToString(), $"Code: {code}, Status: {joinRequest.Status}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", ct);

            return Ok(new { 
                status = joinRequest.Status.ToString(),
                message = invite.RequiresApproval ? "Yêu cầu đã gửi, vui lòng chờ duyệt." : "Tham gia thành công."
            });
        }
    }

    public class CreateInviteRequest
    {
        public bool RequiresApproval { get; set; }
        public int ServerId { get; set; }
    }

    public class JoinRequestPayload
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}
