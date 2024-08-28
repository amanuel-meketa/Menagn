using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.Role.Incoming;
namespace security.api.Controllers
{
    //[Authorize(Roles = "Developer")]
    [ApiController]
    [Route("api/role")]
    public class RoleController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _rserService = roleService;

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role), "Role object cannot be null.");
            try
            {
                return Created("api/role", await _rserService.Create(role));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating role. {ex.Message}");
            }
        }
    }
}
