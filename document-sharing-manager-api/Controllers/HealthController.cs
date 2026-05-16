using Microsoft.AspNetCore.Mvc;

namespace document_sharing_manager_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Status = "Healthy", Version = "1.0.0" });
        }
    }
}
