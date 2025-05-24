using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using security.business.Contracts;
using security.sharedUtils.Dtos.Account.Outgoing;
using System.Net.Http.Headers;

public class IdentityService : IIdentityService
{
    private readonly string _restApi;
    private readonly string _resource;
    private readonly string _tokenEndpoint;
    private readonly string? _clientId;
    private readonly string? _username;
    private readonly string? _password;
    private readonly string? _clientSecret;
    private readonly string? _tokenUrl;

    public IdentityService(IConfiguration configuration)
    {
        var identityProviderSection = configuration.GetSection("Keycloak:AdminRest");
        var identityProviderSecretSection = configuration.GetSection("Keycloak:credentials");
        _tokenEndpoint = $"{identityProviderSection.GetValue<string>("RestAuthority")}{identityProviderSection.GetValue<string>("RestTokenStub")}";
        _clientId = identityProviderSection.GetValue<string>("RestClientId");
        _username = identityProviderSection.GetValue<string>("RestUsername");
        _password = identityProviderSection.GetValue<string>("RestPassword");
        _restApi = identityProviderSection.GetValue<string>("RestApi");
        _clientSecret = identityProviderSecretSection.GetValue<string>("secret");
        _resource = configuration["Keycloak:resource"];
        _tokenUrl = configuration["Keycloak:TokenUrl"];
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("username", _username),
            new KeyValuePair<string, string>("password", _password)
        });

        using var client = new HttpClient();
        var response = await client.PostAsync(_tokenEndpoint, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Could not retrieve access token. Status Code: {response.StatusCode}, Error: {errorResponse}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JObject.Parse(responseContent)["access_token"]?.ToString();
    }

    public async Task<TokenResponseDto> GetAccessTokenStandardFlowAsync(string code, string redirectUri)
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        new KeyValuePair<string, string>("client_id", _resource),
        new KeyValuePair<string, string>("client_secret", _clientSecret),
        new KeyValuePair<string, string>("code", code),
        new KeyValuePair<string, string>("redirect_uri", redirectUri)
    });

    using var client = new HttpClient();
    var response = await client.PostAsync(_tokenUrl, requestContent);

    if (!response.IsSuccessStatusCode)
    {
        var errorResponse = await response.Content.ReadAsStringAsync();
        throw new Exception($"Could not retrieve access token. Status Code: {response.StatusCode}, Error: {errorResponse}");
    }

        var responseContent = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<TokenResponseDto>(responseContent)
                    ?? throw new Exception("Failed to parse token response.");

       return token;
    }

    public async Task<HttpResponseMessage> SendHttpRequestAsync(string url, HttpMethod method, string? accessToken, HttpContent? content = null)
    {
        try
        {
            using var client = new HttpClient(new HttpClientHandler { UseCookies = false });
            var requestMessage = new HttpRequestMessage(method, url)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(accessToken))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Request failed. Status Code: {response.StatusCode}, Error: {errorResponse}");
            }

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while sending the HTTP request: {ex.Message}", ex);
        }
    }

    public async Task<string> GetClientIdAsync(string accessToken)
    {
        string url = $"{_restApi}/clients";
        var response = await SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Client could not be retreived.");

        var content = await response.Content.ReadAsStringAsync();
        List<JObject> clients = JsonConvert.DeserializeObject<List<JObject>>(content);
        JObject sessionClient = clients.Where(m => ((string)m.SelectToken("clientId") == _resource)).ToList().FirstOrDefault();
        return sessionClient.GetValue("id").ToString();
    }
}
