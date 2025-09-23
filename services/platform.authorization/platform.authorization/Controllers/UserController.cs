using authorization.application.Services;
using authorization.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace authorization.api.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UserController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;

        public UserController(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet("user/{userId}/roles")]
        public async Task<IReadOnlyList<string>> GetUserRoles(string userId)
        {
            return await _authorizationService.GetUserRolesAsync(userId);
        }

        [HttpPost("user/{userId}/roles/{roleName}")]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            await _authorizationService.AssignRoleToUserAsync(userId, roleName);
            return Ok(new { Message = $"Role '{roleName}' assigned to user '{userId}' successfully." });
        }

        [HttpDelete("user/{userId}/roles/{roleName}")]
        public async Task<IActionResult> UnassignRoleFromUser(string userId, string roleName)
        {
            await _authorizationService.UnassignRoleFromUserAsync(userId, roleName);
            return Ok(new { Message = $"Role '{roleName}' unassigned from user '{userId}' successfully." });
        }

        [HttpPost("user/{userId}/assigne-resource/scopes")]
        public async Task<IActionResult> AssignUserToResourceAsync(UserResourceAssignment userResourceAssignment)
        {
            await _authorizationService.AssignUserToResourceAsync(userResourceAssignment);
            return Ok(new { Message = $"User '{userResourceAssignment.UserId}' assigned to resource '{userResourceAssignment.Resource}' with scops successfully." });
        }

        [HttpDelete("user/{userId}/unassigne-resource/scopes")]
        public async Task<IActionResult> UnassignUserFromResourceAsync(UserResourceAssignment userResourceAssignment)
        {
            await _authorizationService.UnassignUserFromResourceAsync(userResourceAssignment);
            return Ok(new { Message = $"User '{userResourceAssignment.UserId}' unassigned to resource '{userResourceAssignment.Resource}' with scops successfully." });
        }

        [HttpPost("role/assigne-resource/scopes")]
        public async Task<IActionResult> AssignRoleToResourceAsync(RoleResourceAssignment assignment)
        {
            await _authorizationService.AssignRoleToResourceAsync(assignment);
            return Ok(new { Message = $"Role '{assignment.Role}' unassigned to resource '{assignment.Resource}' with scops successfully." });
        }

        [HttpDelete("role/unassigne-resource/scopes")]
        public async Task<IActionResult> UnassignRoleFromResourceAsync(RoleResourceAssignment assignment)
        {
            await _authorizationService.UnassignRoleFromResourceAsync(assignment);
            return Ok(new { Message = $"Role '{assignment.Role}' unassigned to resource '{assignment.Resource}' with scops successfully." });
        }
    }
}
