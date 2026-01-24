using approvals.application.DTOs.StageDefinition;
using approvals.application.DTOs.StageInstance;
using approvals.application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("stage-instances")]
    [ApiController]
    public class StageInstanceController : ControllerBase
    {
        private readonly IStageInstanceService _stageInstanceService;

        public StageInstanceController(IStageInstanceService stageInstanceService)
        {
            _stageInstanceService = stageInstanceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetStageInstanceDto>>> GetAll()
        {
            var list = await _stageInstanceService.GetAllAsync();
            return Ok(list);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignApprover([FromBody] AssignmentDto assignment)
        {
            await _stageInstanceService.AssignApproverAsync(assignment);
            return Ok(new { message = "Approver assigned successfully" });
        }

        [HttpPost("unassign")]
        public async Task<IActionResult> UnassignApprover([FromBody] AssignmentDto assignment)
        {
            await _stageInstanceService.UnassignApproverAsync(assignment);
            return Ok(new { message = "Approver unassigned successfully" });
        }
    }
}
