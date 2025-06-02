using approvals.application.DTOs.ApplicationType;
using approvals.domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("api/approvals-type")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IApplicationTypeService _appTypeservice;
   
        public ApprovalsController(IApplicationTypeService appTypeservice)
        {
            _appTypeservice = appTypeservice;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetApplicationTypeDto>>> GetAll()
        {
            var list = await _appTypeservice.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetApplicationTypeDto>> Get(Guid id)
        {
            var item = await _appTypeservice.GetByIdAsync(id);
            return Ok(item); 
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateApplicationTypeDto createAppDto)
        {
            var newId = await _appTypeservice.CreateApplicationTypeAsync(createAppDto);
            return CreatedAtAction(nameof(Get), new { id = newId }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApplicationTypeDto updateAppDto)
        {
            await _appTypeservice.UpdateAsync(id, updateAppDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _appTypeservice.DeleteAsync(id);
            return NoContent();
        }
    }
}
