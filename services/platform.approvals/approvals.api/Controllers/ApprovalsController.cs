using approvals.application.DTOs.ApplicationType;
using approvals.application.DTOs.ApplicationType.Validator;
using approvals.application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("api/approvalsType")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IApprovalRepository _approvalRepo;

        public ApprovalsController(IApprovalRepository repo)
        {
            _approvalRepo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<GetApplicationTypeDto>> GetAll()
        {
            var list = await _approvalRepo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetApplicationTypeDto>> Get(Guid id)
        {
            var item = await _approvalRepo.GetByIdAsync(id);
            return Ok(item); 
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApplicationTypeDto updateApp)
        {

            var id = await _approvalRepo.CreateAsync(updateApp);
            return CreatedAtAction(nameof(Get), new { id }, updateApp);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateApplicationTypeDto updateApp)
        {
            
            var updated = await _approvalRepo.UpdateAsync(updateApp);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _approvalRepo.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
