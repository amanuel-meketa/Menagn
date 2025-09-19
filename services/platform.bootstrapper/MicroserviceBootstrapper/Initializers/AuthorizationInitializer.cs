using MicroserviceBootstrapper.Configs;
using MicroserviceBootstrapper.Utils;
using Microsoft.Extensions.Options;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Configuration;
using OpenFga.Sdk.Exceptions;
using OpenFga.Sdk.Model;
using System.Diagnostics;

namespace MicroserviceBootstrapper.Initializers;

public sealed class AuthorizationInitializer : BaseServiceInitializer
{
    private readonly AuthorizationConfig _config;

    public AuthorizationInitializer(IOptions<AuthorizationConfig> configOptions, Logger logger) : base(logger)
    {
        _config = configOptions.Value;
    }

    // Define your desired authorization model
    private readonly TypeDefinition _userType = new()
    {
        Type = "user",
        Relations = new Dictionary<string, Userset>()
    };

    private readonly TypeDefinition _approvalInstanceType = new()
    {
        Type = "approvalInstance",
        Relations = new Dictionary<string, Userset>
        {
            ["can_delete"] = new Userset { This = new object() },
            ["can_read"] = new Userset { This = new object() },
            ["can_edit"] = new Userset { This = new object() },
            ["can_write"] = new Userset { This = new object() },
            ["can_start"] = new Userset { This = new object() }
        },
        Metadata = new Metadata
        {
            Relations = new Dictionary<string, RelationMetadata>
            {
                ["can_delete"] = new RelationMetadata
                {
                    DirectlyRelatedUserTypes = new List<RelationReference>
                    {
                        new() { Type = "user" }
                    }
                },
                ["can_read"] = new RelationMetadata
                {
                    DirectlyRelatedUserTypes = new List<RelationReference>
                    {
                        new() { Type = "user" }
                    }
                },
                ["can_edit"] = new RelationMetadata
                {
                    DirectlyRelatedUserTypes = new List<RelationReference>
                    {
                        new() { Type = "user" }
                    }
                },
                ["can_write"] = new RelationMetadata
                {
                    DirectlyRelatedUserTypes = new List<RelationReference>
                    {
                        new() { Type = "user" }
                    }
                },
                ["can_start"] = new RelationMetadata
                {
                    DirectlyRelatedUserTypes = new List<RelationReference>
                    {
                        new() { Type = "user" }
                    }
                }
            }
        }
    };

    public override async Task InitializeAsync()
    {
        using var activity = Telemetry.ActivitySource.StartActivity("OpenFGAInitialization");
        _logger.Info($"Initializing authorization store '{_config.StoreName}'...");

        // Validate configuration
        ValidateConfig();

        var configuration = new ClientConfiguration()
        {
            ApiUrl = _config.BaseUrl,
            Credentials = new Credentials()
            {
                Method = CredentialsMethod.ApiToken,
                Config = new CredentialsConfig()
                {
                    ApiToken = _config.ApiToken
                }
            }
        };

        using var fgaClient = new OpenFgaClient(configuration);

        try
        {
            string storeId = await GetOrCreateStoreAsync(fgaClient);
            fgaClient.StoreId = storeId;

            // Check current authorization model and update if needed
            await ValidateAndUpdateAuthorizationModelAsync(fgaClient);

            _logger.Info("OpenFGA initialization completed successfully");
        }
        catch (ApiException ex)
        {
            _logger.Error($"OpenFGA API error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error($"Unexpected error during OpenFGA initialization: {ex.Message}");
            throw;
        }
    }

    private void ValidateConfig()
    {
        if (string.IsNullOrEmpty(_config.BaseUrl))
            throw new ArgumentException("OpenFGA BaseUrl is not configured");

        if (string.IsNullOrEmpty(_config.StoreName))
            throw new ArgumentException("OpenFGA StoreName is not configured");

        _logger.Debug("OpenFGA configuration validated successfully");
    }

    private async Task<string> GetOrCreateStoreAsync(OpenFgaClient fgaClient)
    {
        var stores = await fgaClient.ListStores();
        var existingStore = stores.Stores?.FirstOrDefault(s => s.Name == _config.StoreName);

        if (existingStore != null)
        {
            _logger.Info($"Store '{_config.StoreName}' already exists with id: {existingStore.Id}");
            return existingStore.Id;
        }

        var newStore = await fgaClient.CreateStore(new ClientCreateStoreRequest
        {
            Name = _config.StoreName
        });

        _logger.Info($"Created store '{_config.StoreName}' with id {newStore.Id}");
        return newStore.Id;
    }

    private async Task ValidateAndUpdateAuthorizationModelAsync(OpenFgaClient fgaClient)
    {
        AuthorizationModel? currentModel = null;

        try
        {
            // Try to get the latest authorization model
            var currentModelResponse = await fgaClient.ReadLatestAuthorizationModel();
            currentModel = currentModelResponse?.AuthorizationModel;

            if (currentModel != null)
            {
                _logger.Info($"Found existing authorization model: {currentModel.Id}");

                // Check if the model matches our desired state
                if (IsModelComplete(currentModel))
                {
                    _logger.Info("Authorization model is complete. No updates needed.");
                    return;
                }

                _logger.Info("Current model is missing some elements. Creating updated model...");
            }
            else
            {
                _logger.Info("No authorization model found. Creating new model...");
            }
        }
        catch (ApiException ex)
        {
            // Check for 404 using the exception message, since StatusCode is not available
            if (ex.Message.Contains("404"))
            {
                _logger.Info("No authorization model found (404). Creating new model...");
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.Warn($"Error reading current model: {ex.Message}. Attempting to create new model...");
        }

        // Create the complete authorization model
        await CreateAuthorizationModelAsync(fgaClient);
    }

    private bool IsModelComplete(AuthorizationModel model)
    {
        if (model?.TypeDefinitions == null)
        {
            _logger.Info("Model or type definitions are null.");
            return false;
        }

        var typeDefinitions = model.TypeDefinitions.ToList();

        // Check if user type exists
        var userType = typeDefinitions.FirstOrDefault(t => t.Type == _userType.Type);
        if (userType == null)
        {
            _logger.Info("User type is missing from current model.");
            return false;
        }

        // Check if approvalInstance type exists
        var approvalInstanceType = typeDefinitions.FirstOrDefault(t => t.Type == _approvalInstanceType.Type);
        if (approvalInstanceType == null)
        {
            _logger.Info("ApprovalInstance type is missing from current model.");
            return false;
        }

        // Check if all required relations exist
        foreach (var requiredRelation in _approvalInstanceType.Relations.Keys)
        {
            if (!approvalInstanceType.Relations.ContainsKey(requiredRelation))
            {
                _logger.Info($"Relation '{requiredRelation}' is missing from approvalInstance type.");
                return false;
            }
        }

        // Check metadata for directly related user types
        if (approvalInstanceType.Metadata?.Relations == null)
        {
            _logger.Info("ApprovalInstance type metadata is missing or incomplete.");
            return false;
        }

        foreach (var requiredRelation in _approvalInstanceType.Metadata.Relations.Keys)
        {
            if (!approvalInstanceType.Metadata.Relations.ContainsKey(requiredRelation))
            {
                _logger.Info($"Metadata for relation '{requiredRelation}' is missing.");
                return false;
            }

            var currentRelationMetadata = approvalInstanceType.Metadata.Relations[requiredRelation];

            if (currentRelationMetadata.DirectlyRelatedUserTypes == null ||
                !currentRelationMetadata.DirectlyRelatedUserTypes.Any(drut => drut.Type == "user"))
            {
                _logger.Info($"DirectlyRelatedUserTypes for relation '{requiredRelation}' is incomplete.");
                return false;
            }
        }

        return true;
    }

    private async Task CreateAuthorizationModelAsync(OpenFgaClient fgaClient)
    {
        var request = new ClientWriteAuthorizationModelRequest
        {
            SchemaVersion = "1.1",
            TypeDefinitions = new List<TypeDefinition> { _userType, _approvalInstanceType }
        };

        var response = await fgaClient.WriteAuthorizationModel(request);
        _logger.Info($"Authorization model created/updated successfully with ID: {response.AuthorizationModelId}");

        // Log the details of what was created
        _logger.Info($"Created model includes: User type with {_userType.Relations.Count} relations, " +
                    $"ApprovalInstance type with {_approvalInstanceType.Relations.Count} relations: " +
                    $"{string.Join(", ", _approvalInstanceType.Relations.Keys)}");
    }
}

// Telemetry helper for ASP.NET 9 metrics
public static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new("MicroserviceBootstrapper.Authorization");
}