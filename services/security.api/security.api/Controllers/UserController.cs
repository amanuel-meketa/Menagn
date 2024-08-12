using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.business.Dtos.Incoming;

namespace security.api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            { 
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDto>> GetUser(string id)
        {
            try
            {
                var users = await _userService.GetUser(id);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }
    }
}
