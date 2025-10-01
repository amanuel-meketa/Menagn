using authorization.application.Services;
using authorization.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace authorization.api.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;

        public UserController(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        // ------------------ USER ROLES ------------------

        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetUserRolesAsync(string userId)
        {
            var roles = await _authorizationService.GetUserRolesAsync(userId);
            return Ok(roles);
        }

        [HttpPost("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> AddRoleToUserAsync(string userId, string roleName)
        {
            await _authorizationService.AssignRoleToUserAsync(userId, roleName);
            return CreatedAtAction(nameof(GetUserRolesAsync), new { userId }, new { Message = $"Role '{roleName}' assigned to user '{userId}'." });
        }

        [HttpDelete("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            await _authorizationService.UnassignRoleFromUserAsync(userId, roleName);
            return NoContent();
        }

        // ------------------ USER RESOURCE ASSIGNMENTS ------------------

        [HttpPost("users/{userId}/resources")]
        public async Task<IActionResult> AssignUserToResourceAsync(Guid userId, [FromBody] UserResourceAssignment assignment)
        {
            await _authorizationService.AssignUserToResourceAsync(userId, assignment);
            return Created($"api/users/{userId}/resources", new { Message = $"User '{userId}' assigned to resource '{assignment.Resource}'." });
        }

        [HttpDelete("users/{userId}/resources")]
        public async Task<IActionResult> UnassignUserFromResourceAsync(Guid userId, [FromBody] UserResourceAssignment assignment)
        {
            await _authorizationService.UnassignUserFromResourceAsync(userId, assignment);
            return NoContent();
        }

        // ------------------ ROLE RESOURCE ASSIGNMENTS ------------------

        [HttpGet("roles/{roleId}/assignments")]
        public async Task<ActionResult<List<AccessAssignment>>> GetRoleAssignmentsAsync(string roleId)
        {
            var result = await _authorizationService.GetRoleAssignmentsAsync(roleId);
            return Ok(result);
        }

        [HttpPost("roles/{roleName}/resources")]
        public async Task<IActionResult> AssignRoleToResourceAsync(string roleName, [FromBody] RoleResourceAssignment assignment)
        {
            await _authorizationService.AssignRoleToResourceAsync(roleName, assignment);
            return Created($"api/roles/{roleName}/resources", new { Message = $"Role '{roleName}' assigned to resource '{assignment.Resource}'." });
        }

        [HttpDelete("roles/{roleName}/resources")]
        public async Task<IActionResult> UnassignRoleFromResourceAsync(string roleName, [FromBody] RoleResourceAssignment assignment)
        {
            await _authorizationService.UnassignRoleFromResourceAsync(roleName, assignment);
            return NoContent();
        }

        // ------------------ ACCESS CHECK ------------------

        [HttpPost("access/check")]
        public async Task<ActionResult<bool>> CheckAccessAsync([FromBody] CheckAccess checkAccess)
        {
            var result = await _authorizationService.CheckAccessAsync(checkAccess);
            return Ok(result);
        }

        // ------------------ ASSIGNMENT RETRIEVAL ------------------

        [HttpGet("resources/{resourceId}/assignments")]
        public async Task<IActionResult> GetResourceAssignmentsAsync(string resourceId)
        {
            var result = await _authorizationService.ListAssignmentsAsync(resourceId);
            return Ok(result);
        }

        [HttpGet("resources/assignments")]
        public async Task<IActionResult> GetAllAssignmentsAsync()
        {
            var result = await _authorizationService.GetAllTuplesAsync();
            return Ok(result);
        }
    }
}
