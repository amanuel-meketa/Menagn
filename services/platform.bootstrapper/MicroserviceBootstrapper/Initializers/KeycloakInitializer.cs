using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MicroserviceBootstrapper.Initializers
{
    public class KeycloakInitializer : BaseServiceInitializer
    {
        private readonly KeycloakConfig _config;
        private string? _accessToken;

        public KeycloakInitializer(IOptions<KeycloakConfig> configOptions, Logger logger) : base(logger)
        {
            _config = configOptions.Value;
        }

        public override async Task InitializeAsync()
        {
            _logger.Info("Initializing Keycloak...");

            using var client = new HttpClient { BaseAddress = new Uri(_config.BaseUrl) };

            // 1. Authenticate as Keycloak admin
            _accessToken = await AuthenticateAdminAsync(client);

            if (string.IsNullOrEmpty(_accessToken))
            {
                _logger.Error("Failed to authenticate with Keycloak.");
                return;
            }

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _accessToken);

            // 2. Create Realm
            await CreateRealmAsync(client);

            // 3. Create Client
            await CreateClientAsync(client);

            // 4. Create Role
            await CreateRoleAsync(client, "app_user");

            // 5. Create User
            var userId = await CreateUserAsync(client, "demo.user", "Demo", "User", "demo@company.com", "P@ssword123");

            if (!string.IsNullOrEmpty(userId))
            {
                await AssignRoleToUserAsync(client, userId, "app_user");
            }

            // 6. Create Group
            var groupId = await CreateGroupAsync(client, "demo-group");

            if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(userId))
            {
                await AddUserToGroupAsync(client, userId, groupId);
            }

            // 7. Example: Add Resource & Scope
            await CreateResourceAsync(client, "my-api-resource", new[] { "read", "write" });

            _logger.Info("Keycloak initialization completed successfully.");
        }

        private async Task<string?> AuthenticateAdminAsync(HttpClient client)
        {
            var form = new Dictionary<string, string>
            {
                { "client_id", "admin-cli" },
                { "username", _config.AdminUser },
                { "password", _config.AdminPassword },
                { "grant_type", "password" }
            };

            var response = await client.PostAsync("/realms/master/protocol/openid-connect/token", new FormUrlEncodedContent(form));
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(result);
            return json.RootElement.GetProperty("access_token").GetString();
        }

        private async Task CreateRealmAsync(HttpClient client)
        {
            var payload = new { realm = _config.Realm, enabled = true };
            await PostIfNotExistsAsync(client, $"/admin/realms", payload, $"Realm '{_config.Realm}'");
        }

        private async Task CreateClientAsync(HttpClient client)
        {
            var payload = new
            {
                clientId = _config.ClientId,
                enabled = true,
                publicClient = false,
                secret = _config.ClientSecret,
                serviceAccountsEnabled = true,
                authorizationServicesEnabled = true
            };

            await PostIfNotExistsAsync(client, $"/admin/realms/{_config.Realm}/clients", payload, $"Client '{_config.ClientId}'");
        }

        private async Task CreateRoleAsync(HttpClient client, string roleName)
        {
            var payload = new { name = roleName };
            await PostIfNotExistsAsync(client, $"/admin/realms/{_config.Realm}/roles", payload, $"Role '{roleName}'");
        }

        private async Task<string?> CreateUserAsync(HttpClient client, string username, string firstName, string lastName, string email, string password)
        {
            var payload = new
            {
                username,
                firstName,
                lastName,
                email,
                enabled = true,
                credentials = new[]
                {
                    new { type = "password", value = password, temporary = false }
                }
            };

            var response = await client.PostAsync($"/admin/realms/{_config.Realm}/users",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                _logger.Info($"User '{username}' created.");
                var location = response.Headers.Location?.ToString();
                return location?.Split('/').Last();
            }

            _logger.Warn($"Failed to create user '{username}': {response.StatusCode}");
            return null;
        }

        private async Task AssignRoleToUserAsync(HttpClient client, string userId, string roleName)
        {
            var roleResponse = await client.GetAsync($"/admin/realms/{_config.Realm}/roles/{roleName}");
            if (!roleResponse.IsSuccessStatusCode) return;

            var roleJson = await roleResponse.Content.ReadAsStringAsync();
            var role = JsonSerializer.Deserialize<JsonElement>(roleJson);

            var payload = new[]
            {
                new { id = role.GetProperty("id").GetString(), name = roleName }
            };

            var response = await client.PostAsync($"/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                _logger.Info($"Role '{roleName}' assigned to user '{userId}'.");
        }

        private async Task<string?> CreateGroupAsync(HttpClient client, string groupName)
        {
            var payload = new { name = groupName };
            var response = await client.PostAsync($"/admin/realms/{_config.Realm}/groups",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                _logger.Info($"Group '{groupName}' created.");
                var location = response.Headers.Location?.ToString();
                return location?.Split('/').Last();
            }

            _logger.Warn($"Failed to create group '{groupName}': {response.StatusCode}");
            return null;
        }

        private async Task AddUserToGroupAsync(HttpClient client, string userId, string groupId)
        {
            var response = await client.PutAsync($"/admin/realms/{_config.Realm}/users/{userId}/groups/{groupId}", null);

            if (response.IsSuccessStatusCode)
                _logger.Info($"User '{userId}' added to group '{groupId}'.");
        }

        private async Task CreateResourceAsync(HttpClient client, string resourceName, string[] scopes)
        {
            var payload = new
            {
                name = resourceName,
                type = "urn:my-api:resource",
                uris = new[] { $"/{resourceName}/*" },
                scopes = scopes.Select(s => new { name = s })
            };

            var clients = await client.GetAsync($"/admin/realms/{_config.Realm}/clients?clientId={_config.ClientId}");
            var clientJson = await clients.Content.ReadAsStringAsync();
            var clientId = JsonDocument.Parse(clientJson).RootElement[0].GetProperty("id").GetString();

            var response = await client.PostAsync($"/admin/realms/{_config.Realm}/clients/{clientId}/authz/resource-server/resource",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                _logger.Info($"Resource '{resourceName}' with scopes created.");
        }

        private async Task PostIfNotExistsAsync(HttpClient client, string endpoint, object payload, string entityName)
        {
            var response = await client.PostAsync(endpoint,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                _logger.Info($"{entityName} created.");
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                _logger.Warn($"{entityName} already exists.");
            else
                _logger.Warn($"Failed to create {entityName}: {response.StatusCode}");
        }
    }
}
