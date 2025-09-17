using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Models.Authorization;
using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MicroserviceBootstrapper.Initializers;

public sealed class AuthorizationInitializer(Logger logger, IOptions<AuthorizationConfig> config)
    : BaseServiceInitializer(logger)
{
    private readonly AuthorizationConfig _config = config.Value;

    public override async Task InitializeAsync()
    {
        _logger.Info($"Initializing authorization store '{_config.StoreName}'...");

        using var client = new HttpClient { BaseAddress = new Uri(_config.BaseUrl) };

        if (!string.IsNullOrWhiteSpace(_config.ApiToken))client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config.ApiToken);

        // Check existing stores
        var storeList = await client.GetFromJsonAsync<StoreList>("stores")
                         ?? throw new InvalidOperationException("Failed to parse stores response");

        var existing = storeList.Stores.FirstOrDefault(s => s.Name == _config.StoreName);

        if (existing is not null)
        {
            _logger.Info($"Store '{_config.StoreName}' already exists with id: {existing.Id}");
            return;
        }

        // Create if not exists
        var storePayload = new { name = _config.StoreName };
        var createResponse = await client.PostAsJsonAsync("stores", storePayload);

        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            throw new Exception($"OpenFGA store creation failed: {createResponse.StatusCode} - {error}");
        }

        var created = await createResponse.Content.ReadFromJsonAsync<Store>()
                       ?? throw new InvalidOperationException("Failed to parse created store response");

        _logger.Info($"Store '{_config.StoreName}' created with id: {created.Id}");
    }
}
