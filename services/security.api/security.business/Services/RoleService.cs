using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.sharedUtils.Dtos.Role.Incoming;
using SharedLibrary.Utilities;
using System.Net;

namespace security.business.Services
{
    public class RoleService : IRoleService
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly string _restApi;

        public RoleService(IIdentityService identityService, IConfiguration configuration, IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _restApi = configuration["Keycloak:AdminRest:RestApi"] ?? throw new ArgumentNullException(nameof(configuration), "Rest API URL not configured.");
        }

        public async Task<RoleDto> CreateRole(CreateRoleDto role)
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var clientId = await _identityService.GetClientIdAsync(accessToken);
            var url = $"{_restApi}/clients/{clientId}/roles";

            var roleUpdatePayload = new Dictionary<string, string>();
            roleUpdatePayload.Add("name", role.Name);
            roleUpdatePayload.Add("description", role.Description);

            var content = HttpContentHelper.CreateHttpContent(roleUpdatePayload);
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Post, accessToken, content);

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new InvalidOperationException("Role already exists.");

            response.EnsureSuccessStatusCode();

            var location = response.Headers.Location?.ToString()
                ?? throw new InvalidOperationException("Location header missing in response.");

            return await GetRoleFromLocationAsync(location);
        }

        public async Task<IEnumerable<RoleDto>> GetRoles()
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var clientId = await _identityService.GetClientIdAsync(accessToken);
            var url = $"{_restApi}/clients/{clientId}/roles";

            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Roles could not be retreived.");

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<RoleDto>>(content);
        }

        public async Task<RoleDto> GetRole(string id)
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var url = $"{_restApi}/roles-by-id/{id}";
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Role could not be found.");

                throw new Exception("Role could not be retreived.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoleDto>(content);
        }

        public async Task<RoleDto> UpdateRole(string id, UpdateRoleDto role)
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var url = $"{_restApi}/roles-by-id/{id}";

            var roleUpdatePayload = new Dictionary<string, string>();
            roleUpdatePayload.Add("name", role.Name);
            roleUpdatePayload.Add("description", role.Description);

            var content = HttpContentHelper.CreateHttpContent(roleUpdatePayload);
            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Put, accessToken, content);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Role could not be found.");

                throw new Exception("Role could not be updated.");
            }

            var updatedRole = new RoleDto
            { 
                 Id = id,   
                 Name = role.Name,
                 Description = role.Description
            };

            return updatedRole;
        }
        private async Task<RoleDto?> GetRoleFromLocationAsync(string location)
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var response = await _identityService.SendHttpRequestAsync(location, HttpMethod.Get, accessToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoleDto>(content);
        }
    }
}
