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
    [Route("api/audit-logs")]
    [Authorize]
    public class AuditLogController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/audit-logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogs([FromQuery] int page = 1, [FromQuery] int limit = 50, CancellationToken ct = default)
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(ct);
                
            return Ok(logs);
        }
    }
}
