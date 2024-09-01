using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using security.business.Contracts;
using security.sharedUtils.Dtos.Event.Event;

public class EventLogService : IEventLogService
{
    private readonly HttpClient _httpClient;
    private readonly IIdentityService _identityService;
    private readonly string _restApi;

    public EventLogService(IIdentityService identityService, IConfiguration configuration, HttpClient httpClient)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _restApi = configuration["Keycloak:AdminRest:RestApi"] ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<EventLogDto>?> GetEventsAsync()
    {
        string accessToken = await _identityService.GetAccessTokenAsync();
        string url = $"{_restApi}/events";
        var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<EventLogDto>>(content);
    }

    public async Task<IEnumerable<AdminEventLogDto>?> GetAdminEventsAsync()
    {
        string accessToken = await _identityService.GetAccessTokenAsync();
        string url = $"{_restApi}/admin-events";
        var response = await _identityService.SendHttpRequestAsync(url, HttpMethod.Get, accessToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<AdminEventLogDto>>(content);
    }
}
