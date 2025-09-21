using authorization.application.Services;
using Microsoft.AspNetCore.Mvc;

namespace authorization.api.Controllers
{
    [ApiController]
    [Route("api/user-roles")]
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
    }
}
