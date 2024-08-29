using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.Role.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;
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
                return Created("api/role", await _rserService.CreateRole(role));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating role. {ex.Message}");
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            try
            {
                return Ok(await _rserService.GetRoles());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting roles. {ex.Message}");
            }
        }

        [HttpGet("id")]
        public async Task<ActionResult<RoleDto>> GetRole(string id)
        {
            try
            {
                return Ok(await _rserService.GetRole(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting role. {ex.Message}");
            }
        }

        [HttpPut("id")]
        public async Task<ActionResult<RoleDto>> UpdateRole(string id, UpdateRoleDto updateRoleDto)
        {
            try
            {
                return Ok(await _rserService.UpdateRole(id, updateRoleDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating role. {ex.Message}");
            }
        }
        
        [HttpDelete("id")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            try
            {
                await _rserService.DeleteRole(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting role. {ex.Message}");
            }
        }

        [HttpGet("{name}/users")]
        public async Task<ActionResult<GetUserDto>> GetRoleUsers(string name)
        {
            try
            {
                return Ok(await _rserService.GetRoleUsers(name));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting users. {ex.Message}");
            }
        }
    }
}
