using approvals.application.DTOs.ApplicationType;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("approval-template")]
    public class ApprovalTemplateController : ControllerBase
    {
        private readonly IApprovalTemplateService _appTemplateservice;
   
        public ApprovalTemplateController(IApprovalTemplateService appTemplateservice)
        {
            _appTemplateservice = appTemplateservice;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAppTemplateDto>>> GetAll()
        {
            var list = await _appTemplateservice.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetAppTemplateDto>> Get(Guid id)
        {
            var item = await _appTemplateservice.GetByIdAsync(id);
            return Ok(item); 
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateApprovalTemplateDto createAppDto)
        {
            var newId = await _appTemplateservice.CreateApplicationTypeAsync(createAppDto);
            return CreatedAtAction(nameof(Get), new { id = newId }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatAppemplateDto updateAppDto)
        {
            await _appTemplateservice.UpdateAsync(id, updateAppDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _appTemplateservice.DeleteAsync(id);
            return NoContent();
        }
    }
}
