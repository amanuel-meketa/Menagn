using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;

namespace security.api.Controllers
{
    //[Authorize(Roles = "Developer")]
    [ApiController]
    [Route("api/user")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        public async Task<ActionResult<string>> CreateUser([FromBody] CreateUserDto User)
        {
            try
            {
                return Ok(await _userService.Create(User));
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
                return Ok(await _userService.GetAll());
            }
            catch (Exception ex)
            { 
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDto>> GetUser([FromRoute] string id)
        {
            try
            {
                return Ok(await _userService.Get(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetUserDto>> UpdateUser([FromRoute] string id, [FromBody] UpdateUserDto User)
        {
            try
            {
                return Ok(await _userService.Update(id, User));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            try
            {
                await _userService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpGet("{id}/sessions")]
        public async Task<ActionResult<IEnumerable<UserSessionDto>>> Sessions([FromRoute] string id)
        {
            try
            {
                return Ok(await _userService.AllSessions(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpPost("{id}/remove-sessions")]
        public async Task<ActionResult> RemoveAllSessions([FromRoute] string id)
        {
            try
            {
                await _userService.RemoveAllSessions(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

        [HttpPut("{id}/reset-password")]
        public async Task<ActionResult> ResetPassword([FromRoute] string id, [FromBody] string newPassword)
        {
            try
            {
                await _userService.ResetPassword(id, newPassword);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

    }
}
