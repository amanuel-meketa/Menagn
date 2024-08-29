using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.sharedUtils.Dtos.Role.Incoming;
using security.sharedUtils.Dtos.Role.Outgoing;
using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;
using SharedLibrary.Utilities;
using System.Data;
using System.Net;

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

            var content = HttpContentHelper.CreateHttpContent(userPayload);
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Post, accessToken, content);

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new InvalidOperationException("User already exists.");

            response.EnsureSuccessStatusCode();

            var location = response.Headers.Location?.ToString()
                ?? throw new InvalidOperationException("Location header missing in response.");

            return await GetUserFromLocationAsync(location);
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

        public async Task<GetUserDto?> Get(string id)
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

        public async Task<GetUserDto?> Update(string id, UpdateUserDto user)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users/{id}";

            var userPayload = new Dictionary<string, object>
            {
                { "firstName", user.FirstName },
                { "lastName", user.LastName },
                { "email", user.Email }
            };

            var content = HttpContentHelper.CreateHttpContent(userPayload);
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Put, accessToken, content);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("User could not be found.");

            response.EnsureSuccessStatusCode();

            return await Get(id);
        }

        public async Task Delete(string id)
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var url = $"{_restApi}/users/{id}";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Delete, accessToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("User could not be retrieved.");

            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<UserSessionDto>> AllSessions(string id)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users/{id}/sessions";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var sessions = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(content);

            // Convert each session's timestamps to formatted strings
            return sessions.Select(session => new UserSessionDto
            {
                IpAddress = session.ipAddress,
                Start = DateTimeOffset.FromUnixTimeMilliseconds((long)session.start).ToString("M/d/yyyy, h:mm:ss tt"),
                LastAccess = DateTimeOffset.FromUnixTimeMilliseconds((long)session.lastAccess).ToString("M/d/yyyy, h:mm:ss tt")
            });
        }

        public async Task RemoveAllSessions(string id)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users/{id}/logout";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Post, accessToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task ResetPassword(string id, string newPassword)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string url = $"{_restApi}/users/{id}/reset-password";

            var passwordPayload = new Dictionary<string, object>
            {
                { "type", "password" },
                { "value", newPassword },
                { "temporary", false }
            };

            var content = HttpContentHelper.CreateHttpContent(passwordPayload);
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Put, accessToken, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<GetRoleDto>> AssignedRoles(string id)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string clientId = await _identityService.GetClientIdAsync(accessToken);
            string url = $"{_restApi}/users/{id}/role-mappings/clients/{clientId}";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return Enumerable.Empty<GetRoleDto>();

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<GetRoleDto>>(content);
        }

        public async Task<IEnumerable<GetRoleDto>> UnAssignedRoles(string id)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string clientId = await _identityService.GetClientIdAsync(accessToken);
            string url = $"{_restApi}/users/{id}/role-mappings/clients/{clientId}/available";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return Enumerable.Empty<GetRoleDto>();

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<GetRoleDto>>(content);
        }

        public async Task AssignRoles(string userId, RoleDto[] roles)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string clientId = await _identityService.GetClientIdAsync(accessToken);

            var roleAssignments = roles.Select(role => new Dictionary<string, object>
            {
                { "clientRole", true },
                { "id", role.Id },
                { "name", role.Name }
            }).ToList();

            var content = HttpContentHelper.CreateHttpContent(roleAssignments);

            string requestUrl = $"{_restApi}/users/{userId}/role-mappings/clients/{clientId}";

            var response = await _identityService.SendHttpRequestAsync(requestUrl, HttpMethod.Post, accessToken, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.NotFound => "User could not be found.",
                    _ => "Role could not be assigned."
                };

                throw new Exception(errorMessage);
            }
        }
        
        public async Task RemoveRoles(string userId, RoleDto[] roles)
        {
            string accessToken = await _identityService.GetAccessTokenAsync();
            string clientId = await _identityService.GetClientIdAsync(accessToken);

            var removeroles = roles.Select(role => new Dictionary<string, object>
            {
                { "clientRole", true },
                { "id", role.Id },
                { "name", role.Name }
            }).ToList();

            var content = HttpContentHelper.CreateHttpContent(removeroles);

            string requestUrl = $"{_restApi}/users/{userId}/role-mappings/clients/{clientId}";

            var response = await _identityService.SendHttpRequestAsync(requestUrl, HttpMethod.Delete, accessToken, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.NotFound => "User could not be found.",
                    _ => "Role could not be removed."
                };

                throw new Exception(errorMessage);
            }
        }
    }
}
