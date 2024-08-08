using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using security.business.Contracts;
using security.data.Roles;

[ApiController]
[Route("[controller]")]
public class IdentityAdminController(IIdentityService identityService, IConfiguration configuration) : ControllerBase
{
    private readonly IIdentityService _identityService = identityService;
    private readonly string? _restApi = configuration.GetValue<string>("Keycloak:AdminRest:RestApi");
    private readonly string? _clientId = configuration.GetValue<string>("Keycloak:resource");

    [HttpGet("Roles")]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        try
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var clientId = await GetClientIdAsync(accessToken);
            var roles = await FetchRolesAsync(clientId, accessToken);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private async Task<string> GetClientIdAsync(string accessToken)
    {
        var url = $"{_restApi}/clients";
        var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var clients = JsonConvert.DeserializeObject<List<JObject>>(content);
        var client = clients.FirstOrDefault(c => c["clientId"]?.ToString() == _clientId);

        if (client == null)
            throw new Exception("Client could not be retrieved.");
        
        return client["id"]?.ToString();
    }

    private async Task<IEnumerable<Role>> FetchRolesAsync(string clientId, string accessToken)
    {
        var url = $"{_restApi}/clients/{clientId}/roles";
        var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Role>>(content);
    }
}
