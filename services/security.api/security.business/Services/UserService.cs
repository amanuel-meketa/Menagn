using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.business.Dtos;
using System.Net;

namespace security.business.Services
{
    public class UserService : IUserService
    {
        private readonly IIdentityService _identityService;
        private readonly string? _RestApi;

        public UserService(IIdentityService identityService, IConfiguration configuration)
        {
           _identityService = identityService;
            _RestApi = configuration["Keycloak:AdminRest:RestApi"];
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
    }
}
