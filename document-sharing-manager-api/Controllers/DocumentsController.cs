using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using document_sharing_manager.Infrastructure.Persistence;
using document_sharing_manager.Core.DTOs;

namespace document_sharing_manager_api.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController(IDocumentRepository repository, IStorageService storageService, AppDbContext context) : ControllerBase
    {
        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "document";
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }

        private readonly IDocumentRepository _repository = repository;
        private readonly IStorageService _storageService = storageService;
        private readonly AppDbContext _context = context;

        private async Task<bool> IsMemberOfServerAsync(int serverId, CancellationToken ct)
        {
            return await _context.Servers.AnyAsync(s => s.UserId == CurrentUserId && s.Id == serverId, ct);
        }

        private async Task<List<int>> GetUserServerIdsAsync(CancellationToken ct)
        {
            return await _context.Servers
                .Where(s => s.UserId == CurrentUserId)
                .Select(s => s.Id)
                .ToListAsync(ct);
        }

        private int CurrentUserId
        {
            get
            {
                var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (claim == null || !int.TryParse(claim.Value, out var userId))
                {
                    // This will be caught by GlobalExceptionHandler and returned as 401 Unauthorized
                    throw new UnauthorizedAccessException("User identification claim is missing or invalid.");
                }
                return userId;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetAll(CancellationToken ct)
        {
            var serverIds = await GetUserServerIdsAsync(ct);
            var documents = await _repository.GetSharedWithUserAsync(CurrentUserId, serverIds, ct);
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetById(int id, CancellationToken ct)
        {
            var serverIds = await GetUserServerIdsAsync(ct);
            var document = await _repository.GetByIdSharedWithUserAsync(id, CurrentUserId, serverIds, ct);
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

            if (document.Version != existing.Version)
                return Conflict(new { Message = "Concurrency conflict: The document has been modified by another user." });

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

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id, CancellationToken ct)
        {
            var serverIds = await GetUserServerIdsAsync(ct);
            var document = await _repository.GetByIdSharedWithUserAsync(id, CurrentUserId, serverIds, ct);
            if (document == null)
                return NotFound();

            return await SendFileResponse(document, ct);
        }

        [HttpGet("remote/{remoteId}/download")]
        public async Task<IActionResult> DownloadByRemoteId(Guid remoteId, CancellationToken ct)
        {
            var serverIds = await GetUserServerIdsAsync(ct);
            var document = await _repository.GetByRemoteIdAsync(remoteId, ct);
            
            if (document == null || document.IsDeleted) return NotFound();

            // Check permission
            bool hasAccess = document.UserId == CurrentUserId || (document.ServerId.HasValue && serverIds.Contains(document.ServerId.Value));
            if (!hasAccess) return Forbid();

            return await SendFileResponse(document, ct);
        }

        [HttpDelete("remote/{remoteId}")]
        public async Task<IActionResult> DeleteByRemoteId(Guid remoteId, CancellationToken ct)
        {
            var document = await _repository.GetByRemoteIdAsync(remoteId, ct);
            if (document == null || document.UserId != CurrentUserId)
                return NotFound();

            await _repository.DeleteByRemoteIdAsync(remoteId, ct);
            return NoContent();
        }

        private static readonly Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider _contentTypeProvider = new();

        private async Task<IActionResult> SendFileResponse(Document document, CancellationToken ct)
        {
            try
            {
                var stream = await _storageService.GetFileAsync(document.DuongDan, ct);
                if (!_contentTypeProvider.TryGetContentType(document.DuongDan, out string? contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(stream, contentType, document.Ten);
            }
            catch (System.IO.FileNotFoundException)
            {
                return NotFound(new { Message = "File not found on server storage." });
            }
        }

        [HttpPost("sync-stream")]
        public async Task<ActionResult<SyncResponse>> SyncStream([FromForm] Guid remoteId, [FromForm] int localVersion, [FromForm] int serverId, [FromForm] string? ten, [FromForm] string? ghiChu, IFormFile? file, CancellationToken ct)
        {
            if (!await IsMemberOfServerAsync(serverId, ct)) return Forbid();

            var document = await _repository.GetByRemoteIdAsync(remoteId, ct);
            
            // If document doesn't exist, create it (First-time sync/upload)
            if (document == null)
            {
                document = new Document
                {
                    RemoteId = remoteId,
                    UserId = CurrentUserId,
                    ServerId = serverId,
                    Version = 0, // Will be incremented to 1 below
                    Ten = ten ?? "Unnamed Document",
                    GhiChu = ghiChu ?? "",
                    DinhDang = file != null ? Path.GetExtension(file.FileName).TrimStart('.') : "bin",
                    KichThuoc = file != null ? (decimal)file.Length : 0,
                    DuongDan = "" // Will be set below
                };
                await _repository.AddAsync(document, ct);
            }
            else if (document.UserId != CurrentUserId)
            {
                return NotFound(new SyncResponse { Success = false, Message = "Document not found or access denied." });
            }

            if (localVersion < document.Version)
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
            if (!string.IsNullOrEmpty(ten)) document.Ten = ten!;
            if (ghiChu != null) document.GhiChu = ghiChu;

            // Handle file stream if provided
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                string extension = System.IO.Path.GetExtension(file.FileName)?.TrimStart('.') ?? 
                                   System.IO.Path.GetExtension(document.DuongDan)?.TrimStart('.') ?? "bin";

                string safeName = SanitizeFileName(document.Ten);
                if (safeName.Length > 200) safeName = safeName[..200];

                string fileName = $"{safeName}_v{document.Version}.{extension}";
                
                document.DuongDan = await _storageService.UploadFileAsync(stream, fileName, "sync", ct);
            }

            try 
            {
                await _repository.UpdateAsync(document, ct);
            }
            catch (Exception ex) when (ex is System.Data.DBConcurrencyException || ex is DbUpdateConcurrencyException)
            {
                return Conflict(new SyncResponse { Success = false, Message = "A concurrency conflict occurred. Please refresh and try again." });
            }

            return Ok(new SyncResponse 
            { 
                Success = true, 
                CurrentVersion = document.Version, 
                Message = "Synchronized successfully via stream." 
            });
        }

        [HttpPost("sync")]
        public async Task<ActionResult<SyncResponse>> Sync([FromBody] SyncRequest request, CancellationToken ct)
        {
            if (!await IsMemberOfServerAsync(request.ServerId, ct)) return Forbid();

            var document = await _repository.GetByRemoteIdAsync(request.RemoteId, ct);
            
            // If document doesn't exist, create it
            if (document == null)
            {
                document = new Document
                {
                    RemoteId = request.RemoteId,
                    UserId = CurrentUserId,
                    ServerId = request.ServerId,
                    Version = 0,
                    Ten = request.Ten ?? "Unnamed Document",
                    GhiChu = request.GhiChu ?? "",
                    DinhDang = "bin",
                    KichThuoc = 0,
                    DuongDan = ""
                };
                await _repository.AddAsync(document, ct);
            }
            else if (document.UserId != CurrentUserId)
            {
                return NotFound(new SyncResponse { Success = false, Message = "Document not found or access denied." });
            }

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

            // Handle content synchronization if provided
            if (!string.IsNullOrEmpty(request.Content))
            {
                byte[] data;
                try
                {
                    // For security and binary safety, we strictly expect Base64-encoded string
                    data = Convert.FromBase64String(request.Content);
                }
                catch (FormatException)
                {
                    return BadRequest(new { Message = "Content must be a valid Base64-encoded string for binary safety." });
                }

                using var stream = new MemoryStream(data);
                string extension = System.IO.Path.GetExtension(document.DuongDan)?.TrimStart('.') ?? "bin";
                if (string.IsNullOrEmpty(extension)) extension = "bin";

                string safeName = SanitizeFileName(document.Ten);
                if (safeName.Length > 200) safeName = safeName[..200];

                string fileName = $"{safeName}_v{document.Version}.{extension}";
                
                document.DuongDan = await _storageService.UploadFileAsync(stream, fileName, "sync", ct);
            }

            try 
            {
                await _repository.UpdateAsync(document, ct);
            }
            catch (Exception ex) when (ex is System.Data.DBConcurrencyException || ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                return Conflict(new { Message = "A concurrency conflict occurred. Please refresh and try again." });
            }

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
            var serverIds = await GetUserServerIdsAsync(ct);
            var results = await _repository.SearchAdvancedAsync(keyword, "", null, null, null, null, null, CurrentUserId, serverIds, null, ct);
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
            [FromQuery] int? serverId,
            CancellationToken ct)
        {
            var serverIds = await GetUserServerIdsAsync(ct);
            var results = await _repository.SearchAdvancedAsync(keyword ?? "", format ?? "", fromDate, toDate, minSize, maxSize, isImportant, CurrentUserId, serverIds, serverId, ct);
            return Ok(results);
        }
    }
}
