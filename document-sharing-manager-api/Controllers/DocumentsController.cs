using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using document_sharing_manager.Core.DTOs;

namespace document_sharing_manager_api.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController(IDocumentRepository repository) : ControllerBase
    {
        private readonly IDocumentRepository _repository = repository;

        private int CurrentUserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
            ?? throw new UnauthorizedAccessException("Required user claim is missing."));

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetAll(CancellationToken ct)
        {
            var documents = await _repository.GetAllByUserIdAsync(CurrentUserId, ct);
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetById(int id, CancellationToken ct)
        {
            var document = await _repository.GetByIdAndUserIdAsync(id, CurrentUserId, ct);
            if (document == null)
                return NotFound();

            return Ok(document);
        }

        [HttpPost]
        public async Task<ActionResult<Document>> Create(Document document, CancellationToken ct)
        {
            document.UserId = CurrentUserId;
            document.Version = 1;
            await _repository.AddAsync(document, ct);
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Document document, CancellationToken ct)
        {
            var existing = await _repository.GetByIdAndUserIdAsync(id, CurrentUserId, ct);
            if (existing == null)
                return NotFound();

            if (id != document.Id)
                return BadRequest();

            document.UserId = CurrentUserId;
            document.Version = existing.Version + 1; // Increment version on update
            
            await _repository.UpdateAsync(document, ct);
            return Ok(document);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var existing = await _repository.GetByIdAndUserIdAsync(id, CurrentUserId, ct);
            if (existing == null)
                return NotFound();

            await _repository.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpPost("sync")]
        public async Task<ActionResult<SyncResponse>> Sync([FromBody] SyncRequest request, CancellationToken ct)
        {
            var document = await _repository.GetByIdAndUserIdAsync(request.DocumentId, CurrentUserId, ct);
            if (document == null)
                return NotFound(new SyncResponse { Success = false, Message = "Document not found." });

            if (request.LocalVersion < document.Version)
            {
                return Conflict(new SyncResponse 
                { 
                    Success = false, 
                    Conflict = true, 
                    CurrentVersion = document.Version, 
                    Message = "A newer version exists on the server." 
                });
            }

            // Perform sync update
            document.Version++;
            if (!string.IsNullOrEmpty(request.Ten)) document.Ten = request.Ten!;
            if (request.GhiChu != null) document.GhiChu = request.GhiChu;

            await _repository.UpdateAsync(document, ct);

            return Ok(new SyncResponse 
            { 
                Success = true, 
                CurrentVersion = document.Version, 
                Message = "Synchronized successfully." 
            });
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Document>>> Search([FromQuery] string keyword, CancellationToken ct)
        {
            var results = await _repository.SearchAsync(keyword, CurrentUserId, ct);
            return Ok(results);
        }

        [HttpGet("search-advanced")]
        public async Task<ActionResult<IEnumerable<Document>>> SearchAdvanced(
            [FromQuery] string? keyword,
            [FromQuery] string? format,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] decimal? minSize,
            [FromQuery] decimal? maxSize,
            [FromQuery] bool? isImportant,
            CancellationToken ct)
        {
            var results = await _repository.SearchAdvancedAsync(keyword ?? "", format ?? "", fromDate, toDate, minSize, maxSize, isImportant, CurrentUserId, ct);
            return Ok(results);
        }
    }
}
