using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.business.Dtos;

namespace security.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}
