using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.sharedUtils.Dtos.Account.Incoming;
using security.sharedUtils.Dtos.Account.Outgoing;
using SharedLibrary.Utilities;

namespace security.business.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly IIdentityService _identityService;
        private readonly string? _tokenEndpoint;
        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string? _logoutEndpoint;
        private readonly string? _restApi;

        public AccountService(IConfiguration configuration, HttpClient httpClient, IIdentityService identityService)
        {
            _clientId = configuration["Keycloak:resource"];
            _clientSecret = configuration["Keycloak:credentials:secret"];
            _tokenEndpoint = configuration["Keycloak:auth-rest-api"];
            _logoutEndpoint = configuration["Keycloak:logout-rest-api"];
            _httpClient = httpClient;
            _identityService = identityService;
            _restApi = configuration["Keycloak:AdminRest:RestApi"];
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

        public async Task LogOut(string refreshToken)
        {
            var logoutPayload = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "refresh_token", refreshToken }
            };

            using var content = new FormUrlEncodedContent(logoutPayload);

            var response = await _httpClient.PostAsync(_logoutEndpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Logout request failed. Status Code: {response.StatusCode}, Error: {errorResponse}");
            }
        }
    }
}
