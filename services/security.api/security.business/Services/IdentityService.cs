using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using security.business.Contracts;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;

    public IdentityService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GetAccessToken()
    {
        string tokenEndpoint = "http://localhost:8080/realms/master/protocol/openid-connect/token";
        var clientId = "admin-cli";
        var clientSecret = "LtkjlZ8eNiYBlriRjtRkOZ5kKzt2BZ8k";
        var username = "admin";
        var password = "admin";

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        })
        };

        using (var client = new HttpClient())
        {
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JObject.Parse(responseContent)["access_token"]?.ToString();
                return token;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Could not retrieve access token. Status Code: {response.StatusCode}, Error: {errorResponse}");
            }
        }
    }


    public async Task<HttpResponseMessage> SendHttpRequestAsync(string url, HttpMethod method, string accessToken, HttpContent content)
    {
        try
        {
            var requestMessage = new HttpRequestMessage(method, url);
            if (content != null)
                requestMessage.Content = content;

            var handler = new HttpClientHandler { UseCookies = false };
            var client = new HttpClient(handler);

            if (accessToken != null)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Request failed. Status Code: {response.StatusCode}, Error: {errorResponse}");
            }

            return response;
        }
        catch (Exception ex)
        {
            // Log detailed exception
            throw new Exception($"An error occurred while sending the HTTP request: {ex.Message}", ex);
        }
    }

}
