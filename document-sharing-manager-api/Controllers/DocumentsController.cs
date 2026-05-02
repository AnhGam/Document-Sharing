using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace document_sharing_manager_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _repository;

        public DocumentsController(IDocumentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetAll(CancellationToken ct)
        {
            var documents = await _repository.GetAllAsync(ct);
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetById(int id, CancellationToken ct)
        {
            var document = await _repository.GetByIdAsync(id, ct);
            if (document == null)
                return NotFound();

            return Ok(document);
        }

        [HttpPost]
        public async Task<ActionResult<Document>> Create(Document document, CancellationToken ct)
        {
            await _repository.AddAsync(document, ct);
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Document document, CancellationToken ct)
        {
            if (id != document.Id)
                return BadRequest();

            await _repository.UpdateAsync(document, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _repository.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Document>>> Search([FromQuery] string keyword, CancellationToken ct)
        {
            // Note: Currently calling synchronous Search from repo as placeholder
            // In a real scenario, we should have SearchAsync
            var results = _repository.Search(keyword);
            return Ok(results);
        }
    }
}
