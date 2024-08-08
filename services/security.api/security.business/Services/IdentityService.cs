using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using security.business.Contracts;
using System.Net.Http.Headers;

public class IdentityService : IIdentityService
{
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _username;
    private readonly string _password;

    public IdentityService(IConfiguration configuration)
    {
        var identityProviderSection = configuration.GetSection("Keycloak:AdminRest");
        _tokenEndpoint = $"{identityProviderSection.GetValue<string>("RestAuthority")}{identityProviderSection.GetValue<string>("RestTokenStub")}";
        _clientId = identityProviderSection.GetValue<string>("RestClientId");
        _username = identityProviderSection.GetValue<string>("RestUsername");
        _password = identityProviderSection.GetValue<string>("RestPassword");
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

    public async Task<HttpResponseMessage> SendHttpRequestAsync(string url, HttpMethod method, string accessToken, HttpContent content = null)
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
}
