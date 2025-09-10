using approvals.application.DTOs.ApprovalInstance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("approval-instance")]
    [ApiController]
    public class ApprovalInstanceController : ControllerBase
    {
        private readonly IApprovalInstanceService _appInstanceservice;
        private readonly IAuthorizationService _authorizationService;

        public ApprovalInstanceController(IApprovalInstanceService appInstanceservice, IAuthorizationService authorizationService)
        {
            _appInstanceservice = appInstanceservice;
            _authorizationService = authorizationService;
        }

        [HttpPost("start")]
        [Authorize]
        public async Task<IActionResult> StartApproval([FromBody] StartApprovalRequestDto request)
        {
            var policyName = "ApprovalInstance#start";
            var authzResult = await _authorizationService.AuthorizeAsync(User, policyName);
            if (!authzResult.Succeeded) return Forbid();

            var instanceId = await _appInstanceservice.StartAppInstanceAsync(request.TemplateId, request.UserId);
            return Ok(instanceId);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetApprovalInstanceDto>>> GetAll()
        {
            var policyName = "ApprovalInstance#list";
            var authzResult = await _authorizationService.AuthorizeAsync(User, policyName);
            if (!authzResult.Succeeded) return Forbid();

            var list = await _appInstanceservice.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GetApprovalInstanceDto>> Get(Guid id)
        {
            var policyName = "ApprovalInstance#read";
            var authzResult = await _authorizationService.AuthorizeAsync(User, policyName);
            if (!authzResult.Succeeded) return Forbid();

            var item = await _appInstanceservice.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApprovaleInstanceDto updateAppDto)
        {
            var policyName = "ApprovalInstance#update";
            var authzResult = await _authorizationService.AuthorizeAsync(User, policyName);
            if (!authzResult.Succeeded) return Forbid();

            await _appInstanceservice.UpdateAsync(id, updateAppDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var policyName = "ApprovalInstance#delete";
            var authzResult = await _authorizationService.AuthorizeAsync(User, policyName);
            if (!authzResult.Succeeded) return Forbid();

            await _appInstanceservice.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("template/{templateId:guid}/instances")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetApprovalInstanceDto>>> GetByTemplateIdAsync(Guid templateId)
        {
            var policyName = "ApprovalTemplate#read-instances";
            var authzResult = await _authorizationService.AuthorizeAsync(User, policyName);
            if (!authzResult.Succeeded) return Forbid();

            return Ok(await _appInstanceservice.GetByTemplateIdAsync(templateId));
        }

        [HttpGet("user/{userId}/instances")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetMyApprovalInstanceDto>>> GetMyInstancesAsync(Guid userId)
        {
            var currentUserId = User.FindFirst("sub")?.Value;
            if (string.Equals(currentUserId, userId.ToString(), StringComparison.OrdinalIgnoreCase))
                return Ok(await _appInstanceservice.GetMyAppInstances(userId));

            // Non-owner: check broader permission at resource level
            var policyName = "ApprovalInstance#read";
            var authzResult = await _authorizationService.AuthorizeAsync(User, policyName);
            if (!authzResult.Succeeded) return Forbid();

            return Ok(await _appInstanceservice.GetMyAppInstances(userId));
        }
    }
}
