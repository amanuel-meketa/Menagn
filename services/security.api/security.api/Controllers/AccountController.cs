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
    [Route("api")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        [HttpPost("log-in")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginCredentialsDto credential)
        {
            try
            {
                return Ok(await _accountService.LogIn(credential));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while loging users. {ex.Message}");
            }
        }

        [HttpPost("Log-out")]
        public async Task<ActionResult> LogOut([FromBody] string refreshToken)
        {
            try
            {
                await _accountService.LogOut(refreshToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while loging out the user. {ex.Message}");
            }
        }

    }
}
