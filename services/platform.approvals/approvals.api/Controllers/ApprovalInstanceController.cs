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
            var instanceId = await _appInstanceservice.StartAppInstance(request.TemplateId, request.UserId);
            return Ok(instanceId);
        }
    }
}
