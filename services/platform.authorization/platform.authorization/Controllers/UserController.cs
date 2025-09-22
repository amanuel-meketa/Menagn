using authorization.application.Services;
using authorization.data.Models;
using Microsoft.AspNetCore.Mvc;

namespace authorization.api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;

        public UserController(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet("{userId}/roles")]
        public async Task<IReadOnlyList<string>> GetUserRoles(string userId)
        {
            return await _authorizationService.GetUserRolesAsync(userId);
        }

        [HttpPost("{userId}/roles/{roleName}")]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            await _authorizationService.AssignRoleToUserAsync(userId, roleName);
            return Ok(new { Message = $"Role '{roleName}' assigned to user '{userId}' successfully." });
        }

        [HttpDelete("{userId}/roles/{roleName}")]
        public async Task<IActionResult> UnassignRoleFromUser(string userId, string roleName)
        {
            await _authorizationService.UnassignRoleFromUserAsync(userId, roleName);
            return Ok(new { Message = $"Role '{roleName}' unassigned from user '{userId}' successfully." });
        }

        [HttpPost("{userId}/assigneResourceWithScopes")]
        public async Task<IActionResult> AssignUserToResourceAsync(ResourceAssignment resourceAssignment)
        {
            await _authorizationService.AssignUserToResourceAsync(resourceAssignment);
            return Ok(new { Message = $"User '{resourceAssignment.UserId}' assigned to resource '{resourceAssignment.Resource}' with scops successfully." });
        }
    }
}
