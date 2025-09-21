using authorization.application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace authorization.api.Controllers
{
    [ApiController]
    [Route("api/user-roles")]
    public class UserRolesController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;

        public UserRolesController(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet("{userId}/roles")]
        public async Task<ActionResult<List<string>>> GetUserRoles(string userId)
        {
            var result = await _authorizationService.GetUserRolesAsync(userId);
            return result;
        }
    }
}
