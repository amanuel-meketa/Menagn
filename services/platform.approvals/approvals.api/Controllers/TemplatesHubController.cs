using approvals.application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [ApiController]
    [Route("api/template-hub")]
    public class TemplatesHubController : ControllerBase
    {
        private readonly ITemplateHubService _service;

        public TemplatesHubController(ITemplateHubService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _service.ListTemplatesAsync();
            return Ok(templates);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetTemplate(string key)
        {
            var template = await _service.GetTemplateDetailsAsync(key);
            if (template == null) return NotFound();
            return Ok(template);
        }
    }

}
