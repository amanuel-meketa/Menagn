using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.Account;
using security.sharedUtils.Dtos.Role.Incoming;
using security.sharedUtils.Dtos.Role.Outgoing;
using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;

namespace security.api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/account")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        [HttpPost]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginCredentialsDto credential)
        {
            try
            {
                return Ok(await _accountService.LogIn(credential));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users. {ex.Message}");
            }
        }

    }
}
