using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;

namespace security.api.Controllers
{
    //[Authorize(Roles = "Developer")]
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        public async Task<ActionResult<string>> CreatUser(CreateUserDto User)
        {
            try
            {
                return Ok(await _userService.CreateUser(User));
            }
            catch (Exception ex)
            { 
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> GetUsers()
        {
            try
            {
                return Ok(await _userService.GetUsers());
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
                return Ok(await _userService.GetUser(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }
    }
}
