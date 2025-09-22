using authorization.application.Abstractions;
using authorization.data.Models;
using Microsoft.Extensions.Logging;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Exceptions;
using OpenFga.Sdk.Model;

namespace authorization.infrastructure.Services;

public sealed class OpenFGAService : IOpenFGAService
{
    private readonly OpenFgaClient _fgaClient;
    private readonly ILogger<OpenFGAService> _logger;

    public OpenFGAService(OpenFgaClient fgaClient, ILogger<OpenFGAService> logger)
    {
        _fgaClient = fgaClient ?? throw new ArgumentNullException(nameof(fgaClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Fetch roles assigned to a given user from OpenFGA.
    /// </summary>
    /// <param name="userId">User identifier (without prefix).</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>List of role names (without "role:" prefix).</returns>
    public async Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId must not be empty", nameof(userId));

        var request = new ClientListObjectsRequest
        {
            User = $"user:{userId}",
            Relation = "assignee",
            Type = "role"
        };

        try
        {
            _logger.LogDebug("Fetching roles for UserId={UserId}", userId);

            var response = await _fgaClient.ListObjects(request, null, cancellationToken);

            var roles = response.Objects
                .Where(o => o.StartsWith("role:", StringComparison.OrdinalIgnoreCase))
                .Select(o => o["role:".Length..])
                .ToList();

            if (roles.Count > 0)
                _logger.LogInformation("User {UserId} has {Count} roles: {Roles}", userId, roles.Count, string.Join(",", roles));
            else
                _logger.LogInformation("User {UserId} has no roles", userId);

            return roles;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error while fetching roles for UserId={UserId}. Details={Message}", userId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching roles for UserId={UserId}", userId);
            throw;
        }
    }
    public async Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        var tupleKey = new ClientTupleKey
        {
            User = $"user:{userId}",
            Relation = "assignee",
            Object = $"role:{roleName}"
        };

        var request = new ClientWriteRequest
        {
          
            Writes = new List<ClientTupleKey> { tupleKey }
        };

        try
        {
            _logger.LogDebug("Assigning role {Role} to UserId={UserId}", roleName, userId);

            await _fgaClient.Write(request, null, cancellationToken);

            _logger.LogInformation("Successfully assigned role {Role} to UserId={UserId}", roleName, userId);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "OpenFGA API error while assigning role {Role} to UserId={UserId}. Details={Message}",
                roleName, userId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while assigning role {Role} to UserId={UserId}", roleName, userId);
            throw;
        }
    }
    public async Task UnassignRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        var tuple = new ClientTupleKeyWithoutCondition
        {
            User = $"user:{userId}",
            Relation = "assignee",
            Object = $"role:{roleName}"
        };

        var request = new ClientWriteRequest
        {
            Deletes = new List<ClientTupleKeyWithoutCondition> { tuple }
        };

        try
        {
            _logger.LogDebug("Unassigning role {Role} from UserId={UserId}", userId, roleName);

            await _fgaClient.Write(request, null, cancellationToken);

            _logger.LogInformation("Successfully unassigned role {Role} from UserId={UserId}", userId, roleName);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "OpenFGA API error while unassigning role {Role} from UserId={UserId}. Details={Message}",
                roleName, userId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while unassigning role {Role} from UserId={UserId}", userId, roleName);
            throw;
        }
    }
    public async Task AssignUserToResourceAsync(ResourceAssignment resourceAssignment, CancellationToken cancellationToken = default)
    {
        var tuples = resourceAssignment.Scopes.Select(scope => new ClientTupleKey
        {
            User = $"user:{resourceAssignment.UserId}",
            Relation = scope,
            Object = "approvalInstance:all"
        }).ToList();


        var request = new ClientWriteRequest
        {
            Writes = tuples
        };

        try
        {
            await _fgaClient.Write(request, null, cancellationToken);
            _logger.LogInformation("Assigned scopes {Scopes} to user {UserId} on approvalInstance {ApprovalInstanceId}",
                string.Join(",", resourceAssignment.Scopes), resourceAssignment.UserId, resourceAssignment.Resource);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "OpenFGA API error while assigning scopes for user {UserId} on approvalInstance {ApprovalInstanceId}",
                resourceAssignment.UserId, resourceAssignment.Resource);
            throw;
        }
    }
}
