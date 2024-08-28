using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using security.business.Contracts;
using security.sharedUtils.Dtos.Account.Incoming;
using security.sharedUtils.Dtos.Account.Outgoing;
using System.Security.Claims;

namespace security.api.Controllers
{
    [ApiController]
    [Route("api")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

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
            var claims = User.Claims;

            // Populate it with important claims
            var importantClaims = new UserInfoDto
            {
                Exp = GetClaimValue(claims, ClaimTypes.Expiration),
                Iat = GetClaimValue(claims, "iat"),
                Jti = GetClaimValue(claims, "jti"),
                Iss = GetClaimValue(claims, "iss"),
                Aud = GetClaimValue(claims, "aud"),
                NameIdentifier = GetClaimValue(claims, ClaimTypes.NameIdentifier),
                Typ = GetClaimValue(claims, "typ"),
                Azp = GetClaimValue(claims, "azp"),
                SessionState = GetClaimValue(claims, "session_state"),
                AuthnClassReference = GetClaimValue(claims, "http://schemas.microsoft.com/claims/authnclassreference"),
                AllowedOrigins = GetClaimValue(claims, "allowed-origins"),
                RealmAccess = GetClaimValue(claims, "realm_access"),
                ResourceAccess = GetClaimValue(claims, "resource_access"),
                Scope = GetClaimValue(claims, "scope"),
                Sid = GetClaimValue(claims, "sid"),
                EmailVerified = GetClaimValue(claims, "email_verified"),
                Name = GetClaimValue(claims, "name"),
                PreferredUsername = GetClaimValue(claims, "preferred_username"),
                GivenName = GetClaimValue(claims, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"),
                Surname = GetClaimValue(claims, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"),
                EmailAddress = GetClaimValue(claims, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
            };

            return Ok(importantClaims);
        }

        private string GetClaimValue(IEnumerable<Claim> claims, string type)
        {
            return claims.FirstOrDefault(c => c.Type == type)?.Value;
        }
    }
}
