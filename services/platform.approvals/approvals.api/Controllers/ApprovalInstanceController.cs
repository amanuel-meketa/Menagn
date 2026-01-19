using approvals.application.DTOs.ApprovalInstance;
using approvals.domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("approval-instance")]
    [ApiController]
    public class ApprovalInstanceController : ControllerBase
    {
        private readonly IApprovalInstanceService _appInstanceservice;

        public ApprovalInstanceController(IApprovalInstanceService appInstanceservice )
        {
            _appInstanceservice = appInstanceservice;
        }

        [HttpPost("{templateId}/start")]
        public async Task<IActionResult> StartApproval(Guid templateId, [FromBody] UserInfoDto userInfoDto)
        {
            var instanceId = await _appInstanceservice.StartAppInstanceAsync(templateId, userInfoDto);
            return Ok(instanceId);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAppInstanceWithStageDto>>> GetAll()
        {
            var list = await _appInstanceservice.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetAppInstanceWithStageDto>> Get(Guid id)
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
        public async Task<ActionResult<IEnumerable<GetAppInstanceWithStageDto>>> GetByTemplateIdAsync(Guid templateId)
        {
            return Ok(await _appInstanceservice.GetByTemplateIdAsync(templateId));
        }

        [HttpGet("user/{userId}/instances")]
        public async Task<ActionResult<IEnumerable<GetMyApprovalInstanceDto>>> GetMyInstancesAsync(Guid userId)
        {
            return Ok(await _appInstanceservice.GetMyAppInstances(userId));
        }
    }
}
