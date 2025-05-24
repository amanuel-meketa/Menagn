using approvals.application.DTOs;
using approvals.application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace approvals.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IApprovalRepository _approvalRepo;

        public ApprovalsController(IApprovalRepository repo)
        {
            _approvalRepo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _approvalRepo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var item = await _approvalRepo.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApprovalDto dto)
        {
            var id = await _approvalRepo.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ApprovalDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var updated = await _approvalRepo.UpdateAsync(dto);
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
