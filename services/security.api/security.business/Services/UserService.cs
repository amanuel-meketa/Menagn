using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;
using System.Net;
using System.Text;

namespace security.business.Services
{
    public class UserService(IIdentityService identityService, IConfiguration configuration, IMapper mapper) : IUserService
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly string? _RestApi = configuration["Keycloak:AdminRest:RestApi"];
        private readonly IMapper _mapper = mapper;

        public async Task<GetUserDto> CreateUser(CreateUserDto user)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_RestApi}/users";

            var userPayload = new Dictionary<string, object>
            {
                { "username", user.Username },
                { "firstName", user.FirstName },
                { "lastName", user.LastName },
                { "email", user.Email }
            };

            string body = JsonConvert.SerializeObject(userPayload);
            using var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Post, accessToken, content);

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new InvalidOperationException("User already exists.");

            response.EnsureSuccessStatusCode();

            string location = response.Headers.Location?.ToString()
                ?? throw new InvalidOperationException("Location header missing in response.");

            return await GetUserFromLocationAsync(location);
        }

        public async Task<IEnumerable<GetUserDto>> GetUsers()
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_RestApi}/users";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                    throw new Exception("User could not be retreived.");
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<GetUserDto>>(content);
        }

        public async Task<GetUserDto> GetUser(string id)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_RestApi}/users/{id}";

            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                    throw new Exception("User could not be retreived.");
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<GetUserDto>(content);
        }

        private async Task<GetUserDto> GetUserFromLocationAsync(string location)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();

            string url = location;
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonConvert.DeserializeObject<GetUserDto>(content);
        }
    }
}
