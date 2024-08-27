using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.sharedUtils.Dtos.Account;

namespace security.business.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _tokenEndpoint;
        private readonly string? _clientId;
        private readonly string? _clientSecret;

        public AccountService(IConfiguration configuration, HttpClient httpClient)
        {
            _clientId = configuration["Keycloak:resource"];
            _clientSecret = configuration["Keycloak:credentials:secret"];
            _tokenEndpoint = configuration["Keycloak:auth-rest-api"];
            _httpClient = httpClient;
        }
        public async Task<TokenResponseDto?> LogIn(LoginCredentialsDto credential)
        {
            var userPayload = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "username", credential.Username },
                { "password", credential.Password }
            };

            using var content = new FormUrlEncodedContent(userPayload);

            var response = await _httpClient.PostAsync(_tokenEndpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Request failed. Status Code: {response.StatusCode}, Error: {errorResponse}");
            }

            var tokenResponse = await response.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<TokenResponseDto>(tokenResponse);
            return tokenData;
        }
    }
}
