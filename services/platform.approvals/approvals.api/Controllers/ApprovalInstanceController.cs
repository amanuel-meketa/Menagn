using approvals.application.DTOs.ApprovalInstance;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("approval-instance")]
    public class ApprovalInstanceController : ControllerBase
    {
        private readonly IApprovalInstanceService _appInstanceservice;
   
        public ApprovalInstanceController(IApprovalInstanceService appInstanceservice)
        {
            _appInstanceservice = appInstanceservice;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartApproval([FromBody] StartApprovalRequestDto request)
        {
            var instanceId = await _appInstanceservice.StartAppInstanceAsync(request.TemplateId, request.UserId);
            return Ok(instanceId);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetApprovalInstanceDto>>> GetAll()
        {
            var list = await _appInstanceservice.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetApprovalInstanceDto>> Get(Guid id)
        {
            var item = await _appInstanceservice.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApprovaleInstanceDto updateAppDto)
        {
            await _appInstanceservice.UpdateAsync(id, updateAppDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _appInstanceservice.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("template/{templateId:guid}/instances")]
        public async Task<ActionResult<IEnumerable<GetApprovalInstanceDto>>> GetByTemplateIdAsync(Guid templateId)
        {
            return Ok(await _appInstanceservice.GetByTemplateIdAsync(templateId));
        }

        [HttpGet("user/{userId}/instances")]
        public async Task<ActionResult<IEnumerable<GetApprovalInstanceDto>>> GetMyInstancesAsync(Guid userId)
        {
            return Ok(await _appInstanceservice.GetMyAppInstances(userId));
        }
    }
}
