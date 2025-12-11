using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.Account.Incoming;
using security.sharedUtils.Dtos.Account.Outgoing;
using System.Security.Claims;
using System.Web;

namespace security.api.Controllers
{
    [ApiController()]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public AccountController(IAccountService accountService, IIdentityService identityService, IConfiguration configuration)
        {
            _accountService = accountService;
            _identityService = identityService;
            _configuration = configuration;
        }

        /// <summary>
        /// Redirects the user to the external authentication provider (e.g., Keycloak).
        /// </summary>
        [AllowAnonymous]
        [HttpGet("authenticate")]
        public IActionResult Authenticate()
        {
            var authorizationUrl = _configuration["Authentication:AuthorizationUrl"];
            var clientId = _configuration["Keycloak:resource"];
            var redirectUri = _configuration["Authentication:RedirectUri"];

            var queryParams = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "redirect_uri", redirectUri },
                { "response_type", "code" },
                { "scope", "openid profile" }
            };

            var keycloakRedirectUrl = BuildUrlWithParams(authorizationUrl, queryParams);

            return Redirect(keycloakRedirectUrl);
        }

        /// <summary>
        /// Exchanges the authorization code for an access token.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("exchange-token")]
        public async Task<ActionResult<TokenResponseDto>> ExchangeAuthorizationCode([FromQuery] string? code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Authorization code is missing.");

            var redirectUri = _configuration["Authentication:RedirectUri"];
            var tokenResponse = await _identityService.GetAccessTokenStandardFlowAsync(code, redirectUri);

            if (tokenResponse == null)
                return Unauthorized("Failed to retrieve access token.");

            return Ok(tokenResponse);
        }

        [AllowAnonymous]
        [HttpPost("log-in")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginCredentialsDto credentials)
        {
            try
            {
                var tokenResponse = await _accountService.LogIn(credentials);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while logging in the user: {ex.Message}");
            }
        }

        [HttpPost("log-out")]
        public async Task<ActionResult> LogOut([FromBody] string refreshToken)
        {
            try
            {
                await _accountService.LogOut(refreshToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while logging out the user: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the user information from the claims in the current context.
        /// </summary>
        [HttpGet("userinfo")]
        public ActionResult<UserInfoDto> GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized(new { message = "User is not authenticated" });

            var claims = identity.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(claims);
        }

        /// <summary>
        /// Helper method to build a URL with query parameters.
        /// </summary>
        private string BuildUrlWithParams(string baseUrl, Dictionary<string, string> queryParams)
        {
            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            return $"{baseUrl}?{queryString}";
        }
    }
}
