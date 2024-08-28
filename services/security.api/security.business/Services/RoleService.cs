using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.sharedUtils.Dtos.Role.Incoming;
using SharedLibrary.Utilities;
using System.Data;
using System;
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

        public async Task<RoleDto> Create(CreateRoleDto role)
        {
            var accessToken = await _identityService.GetAccessTokenAsync();
            var clientId = await _identityService.GetClientIdAsync(accessToken);

            var url = $"{_restApi}/clients/{clientId}/roles";
            var rolePayload = new
            {
                name = role.Name,
                description = role.Description
            };

           var content = HttpContentHelper.CreateHttpContent(rolePayload);

            var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Post, accessToken, content);

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new InvalidOperationException("Role already exists.");

            response.EnsureSuccessStatusCode();

            var location = response.Headers.Location?.ToString()
                ?? throw new InvalidOperationException("Location header missing in response.");

            return await GetRoleFromLocationAsync(location);
        }

        public async Task<IEnumerable<RoleDto>> GetAll()
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
