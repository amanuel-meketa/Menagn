using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using security.business.Contracts;
using security.data.Roles;
using System.Data;
using System;

[ApiController]
[Route("[controller]")]
public class IdentityAdminController : ControllerBase
{
    private readonly IIdentityService _tokenService;
    private readonly IConfiguration _configuration;

    public IdentityAdminController(IIdentityService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpGet("Roles")]
    public async Task<IEnumerable<Role>> GetRoles()
    {
        string accessToken = await _tokenService.GetAccessToken();
        string restApi = _configuration.GetValue<string>("IdentityProvider:RestApi");
        string clientId = await GetClientId(accessToken);

        string url = $"http://localhost:8080/admin/realms/Menagn/clients/{clientId}/roles";
        var response = await _tokenService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Roles could not be retreived.");

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Role>>(content);
    }

    private async Task<string> GetClientId(string accessToken)
    {
        string restApi = "http://localhost:8080/admin/realms/Menagn";
        string clientId = _configuration.GetValue<string>("IdentityProvider:ClientId");
        string url = string.Format("{0}/clients", restApi);
        var response = await _tokenService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Client could not be retreived.");

        var content = await response.Content.ReadAsStringAsync();
        List<JObject> clients = JsonConvert.DeserializeObject<List<JObject>>(content);
        JObject sessionClient = clients.Where(m => ((string)m.SelectToken("clientId") == clientId)).ToList().FirstOrDefault();
        return sessionClient.GetValue("id").ToString();
    }
}
