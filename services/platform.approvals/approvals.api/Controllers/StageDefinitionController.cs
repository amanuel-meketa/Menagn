using approvals.application.DTOs.StageDefinition;
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
            var list = await _stageDefinService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetStageDefinitionDto>> Get(Guid id)
        {
            var item = await _stageDefinService.GetByIdAsync(id);
            return Ok(item); 
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateStageDefinitionDto createStageDefiDto)
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
    }
}
