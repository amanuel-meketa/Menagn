using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Models.Authorization;
using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MicroserviceBootstrapper.Initializers;

public sealed class AuthorizationInitializer(Logger logger, IOptions<AuthorizationConfig> config)
    : BaseServiceInitializer(logger)
{
    private readonly AuthorizationConfig _config = config.Value;

    // Define all resources & scopes in one place
    private readonly string[] _resources = { "approval_template" };

    public override async Task InitializeAsync()
    {
        _logger.Info($"Initializing authorization store '{_config.StoreName}'...");

        using var client = new HttpClient { BaseAddress = new Uri(_config.BaseUrl) };
        if (!string.IsNullOrWhiteSpace(_config.ApiToken))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config.ApiToken);

        // Ensure store exists
        var storeList = await client.GetFromJsonAsync<StoreList>("stores")
                         ?? throw new InvalidOperationException("Failed to parse stores response");

        var existing = storeList.Stores.FirstOrDefault(s => s.Name == _config.StoreName);
        string storeId = existing?.Id ?? "";

        if (string.IsNullOrEmpty(storeId))
        {
            var storePayload = new { name = _config.StoreName };
            var createResponse = await client.PostAsJsonAsync("stores", storePayload);

            createResponse.EnsureSuccessStatusCode();

            var created = await createResponse.Content.ReadFromJsonAsync<Store>()
                           ?? throw new InvalidOperationException("Failed to parse created store response");

            _logger.Info($"Store '{_config.StoreName}' created with id: {created.Id}");
            storeId = created.Id;
        }
        else
        {
            _logger.Info($"Store '{_config.StoreName}' already exists with id: {storeId}");
        }

        // --- Collect new type definitions ---
        var typeDefs = new List<object> { new { type = "user", relations = new { } } };

        foreach (var resource in _resources)
        {
            typeDefs.Add(new
            {
                type = resource,
                relations = new { },       // empty relations, no users assigned
                metadata = new { relations = new { } } // optional metadata
            });
        }

        // Push new authorization model version
        var authModel = new
        {
            schema_version = "1.1",
            type_definitions = typeDefs
        };

        var pushResp = await client.PostAsJsonAsync($"stores/{storeId}/authorization-models", authModel);

        if (!pushResp.IsSuccessStatusCode)
        {
            var err = await pushResp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create authorization model: {pushResp.StatusCode} - {err}");
        }

        _logger.Info($"Authorization model updated. Added: {string.Join(", ", _resources)}");
    }
}
