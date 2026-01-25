using approvals.application.DTOs.ApprovalInstance;
using approvals.application.DTOs.EnumDtos;
using approvals.application.DTOs.StageDefinition;
using approvals.application.DTOs.StageInstance;
using approvals.domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("stage-definition")]
    public class StageDefinitionController : ControllerBase
    {
        private readonly IStageDefinitionService _stageDefinService;
   
        public StageDefinitionController(IStageDefinitionService stageDefinService)
        {
            _stageDefinService = stageDefinService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetStageDefinitionDto>>> GetAll()
        {
            return Ok(await _stageDefinService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetStageDefinitionDto>> Get(Guid id)
        {
            return Ok(await _stageDefinService.GetByIdAsync(id)); 
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateStageDefinitionDto[] createStageDefiDto)
        {
            var newId = await _stageDefinService.CreateApplicationTypeAsync(createStageDefiDto);
            return CreatedAtAction(nameof(Get), new { id = newId }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStageDefinitionDto updatetageDefiDto)
        {
            await _stageDefinService.UpdateAsync(id, updatetageDefiDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _stageDefinService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("Instance/{id}/action")]
        public async Task<IActionResult> ActOnStage(Guid id, [FromBody] StageActionDto actionDto)
        {
            var domainAction = actionDto.Action switch
            {
                StageInstanceStatusDto.Approved => StageInstanceStatus.Approved,
                StageInstanceStatusDto.Rejected => StageInstanceStatus.Rejected,
                StageInstanceStatusDto.Skipped => StageInstanceStatus.Skipped,
                StageInstanceStatusDto.Cancelled => StageInstanceStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            };

            await _stageDefinService.ActOnStageAsync(id, actionDto.UserId, domainAction, actionDto.Comment);

            return Ok();
        }

        [HttpGet("{templateId}/stages")]
        public async Task<ActionResult<IEnumerable<GetStageDefinitionDto>>> GetStagesByTempIdAsync(Guid templateId)
        {
            return Ok(await _stageDefinService.GetStagesByTempIdAsync(templateId));
        }

        [HttpGet("user/{userId:guid}/assigned-task")]
        public async Task<ActionResult<IEnumerable<GetAppInstanceWithStageDto>>> GetAssignedTasksAsync(Guid userId)
        {
            return Ok(await _stageDefinService.GetAssignedTasksAsync(userId));
        }
    }
}
