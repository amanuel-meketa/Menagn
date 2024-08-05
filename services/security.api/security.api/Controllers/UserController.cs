using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using security.data.Roles;
using security.data.User;
using System.Security.Claims;
using System.Text.Json;

namespace security.api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetUserInfo")]
        public ActionResult<UserInfo> Get()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return Unauthorized("User is not authenticated.");

            var userInfo = new UserInfo
            {
                Name = GetClaimValue(claimsIdentity, ClaimTypes.Name),
                PreferredUsername = GetClaimValue(claimsIdentity, "preferred_username"),
                Email = GetClaimValue(claimsIdentity, ClaimTypes.Email),
                Roles = ExtractRolesFromResourceAccess(GetClaimValue(claimsIdentity, "resource_access"))
            };

            return Ok(userInfo);
        }

        private static string? GetClaimValue(ClaimsIdentity claimsIdentity, string claimType)
        {
            return claimsIdentity.FindFirst(claimType)?.Value;
        }

        private IEnumerable<Role> ExtractRolesFromResourceAccess(string? resourceAccessClaim)
        {
            var roles = new List<Role>();

            if (string.IsNullOrWhiteSpace(resourceAccessClaim))
                return roles;

            try
            {
                using var doc = JsonDocument.Parse(resourceAccessClaim);
                foreach (var resource in doc.RootElement.EnumerateObject())
                {
                    if (resource.Value.TryGetProperty("roles", out JsonElement rolesElement))
                    {
                        roles.AddRange(rolesElement.EnumerateArray().Select(role => new Role
                        {
                            Name = role.GetString()
                        }));
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing resource access claim.");
            }

            return roles;
        }
    }
}
