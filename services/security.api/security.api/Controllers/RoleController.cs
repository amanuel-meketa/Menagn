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
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto role)
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
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
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
        public async Task<ActionResult<RoleDto>> Get(string id)
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
    }
}
