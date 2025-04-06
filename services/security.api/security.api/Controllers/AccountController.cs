using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.Account.Incoming;
using security.sharedUtils.Dtos.Account.Outgoing;
using System.Security.Claims;
using System.Web;

namespace security.api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/account")]
    public class AccountController(IAccountService accountService, IIdentityService iIdentityService, IConfiguration _configuration) : ControllerBase
    {
        private readonly IIdentityService _IIdentityService = iIdentityService;
        private readonly IAccountService _accountService = accountService;

        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<ActionResult<TokenResponseDto>> GetToken([FromQuery] string? code)
        {
            var authorizationUrl = _configuration["Authentication:AuthorizationUrl"]!;
            var clientId = _configuration["Keycloak:resource"]!;

            // Construct the redirect URI dynamically
            var redirectUri = $"{Request.Scheme}://{Request.Host}{Request.Path}";

            if (string.IsNullOrEmpty(code))
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "redirect_uri", redirectUri },
                    { "response_type", "code" },
                    { "scope", "openid profile" }
                };

                var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
                var keycloakRedirectUrl = $"{authorizationUrl}?{queryString}";

                Console.WriteLine("🔁 Redirecting to Keycloak with URI: " + redirectUri);
                return Redirect(keycloakRedirectUrl);
            }

            var responseBody = await _IIdentityService.GetAccessTokenStandardFlowAsync(code, redirectUri);

            return Ok(responseBody);
        }

        [AllowAnonymous]
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

        [HttpGet("userinfo")]
        public ActionResult<UserInfoDto> GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }

            var claims = identity.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(claims);
        }
    }
}
