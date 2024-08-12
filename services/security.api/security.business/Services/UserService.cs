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
    public class UserService : IUserService
    {
        private readonly IIdentityService _identityService;
        private readonly string _restApi;
        private readonly IMapper _mapper;

        public UserService(IIdentityService identityService, IConfiguration configuration, IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _restApi = configuration["Keycloak:AdminRest:RestApi"] ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetUserDto> Create(CreateUserDto user)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users";

            var userPayload = new Dictionary<string, object>
            {
                { "username", user.Username },
                { "firstName", user.FirstName },
                { "lastName", user.LastName },
                { "email", user.Email }
            };

            using var content = CreateHttpContent(userPayload);
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Post, accessToken, content);

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new InvalidOperationException("User already exists.");

            response.EnsureSuccessStatusCode();

            var location = response.Headers.Location?.ToString()
                ?? throw new InvalidOperationException("Location header missing in response.");

            return await GetUserFromLocationAsync(location);
        }

        public async Task Delete(Guid id)
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var url = $"{_restApi}/users/{id}";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Delete, accessToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("User could not be retrieved.");

            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<GetUserDto>> GetAll()
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return Enumerable.Empty<GetUserDto>();

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<GetUserDto>>(content);
        }

        public async Task<GetUserDto?> Get(Guid id)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users/{id}";

            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
                throw new Exception("User could not be retrieved.");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetUserDto>(content);
        }

        public Task<IEnumerable<GetUserDto>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<GetUserDto?> Update(Guid id, UpdateUserDto user)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users/{id}";

            var userPayload = new Dictionary<string, object>
            {
                { "firstName", user.FirstName },
                { "lastName", user.LastName },
                { "email", user.Email }
            };

            using var content = CreateHttpContent(userPayload);
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Put, accessToken, content);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("User could not be found.");

            response.EnsureSuccessStatusCode();

            return await Get(id);
        }

        private HttpContent CreateHttpContent(Dictionary<string, object> payload)
        {
            string body = JsonConvert.SerializeObject(payload);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }

        private async Task<GetUserDto?> GetUserFromLocationAsync(string location)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            var response = await _identityService.SendHttpRequestAsync(location, HttpMethod.Get, accessToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetUserDto>(content);
        }
    }
}
