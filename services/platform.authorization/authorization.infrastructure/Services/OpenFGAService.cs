using authorization.application.Abstractions;
using authorization.data.Models;
using authorization.data.Models.Enums;
using Microsoft.Extensions.Logging;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Exceptions;
using System.Data;

namespace authorization.infrastructure.Services;

public sealed class OpenFGAService : IOpenFGAService
{
    private readonly OpenFgaClient _fgaClient;
    private readonly ILogger<OpenFGAService> _logger;
    private readonly string ResourceId = "23e5ad76-d474-41d1-9371-ada775ea9899";

    public OpenFGAService(OpenFgaClient fgaClient, ILogger<OpenFGAService> logger)
    {
        _fgaClient = fgaClient ?? throw new ArgumentNullException(nameof(fgaClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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
    public async Task AssignUserToResourceAsync(Guid userId, UserResourceAssignment userResourceAssignment, CancellationToken cancellationToken = default)
    {
        var tupleKeys = userResourceAssignment.Scopes.Select(scope => new ClientTupleKey
        {
            User = $"user:{userId}",
            Relation = scope,
            Object = $"{userResourceAssignment.Resource}:{ResourceId}"
        }).ToList();

        var write = tupleKeys.Select(t => new ClientTupleKey
        {
            User = t.User,
            Relation = t.Relation,
            Object = t.Object
        }).ToList();

        var request = new ClientWriteRequest
        {
            Writes = write
        };

        try
        {
            await _fgaClient.Write(request, null, cancellationToken);
            _logger.LogInformation($"Assigned scopes {userResourceAssignment.Scopes} to user {userId} on approvalInstance {userResourceAssignment.Resource}");
        }
        catch (FgaApiValidationError ex) when (ex.Message.Contains("cannot write a tuple which already exists"))
        {
            _logger.LogWarning($"Attempted to assign existing scopes {userResourceAssignment.Scopes} to user {userId}. This is treated as successful.");
            return;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "OpenFGA API error while assigning scopes for user {UserId} on approvalInstance {ApprovalInstanceId}", userId, userResourceAssignment.Resource);
            throw;
        }
    }
    public async Task UnassignUserFromResourceAsync(Guid userId, UserResourceAssignment userResourceAssignment, CancellationToken cancellationToken = default)
    {
        var tupleKeys = userResourceAssignment.Scopes.Select(scope => new ClientTupleKey
        {
            User = $"user:{userId}",
            Relation = scope,
            Object = $"{userResourceAssignment.Resource}:{ResourceId}"
        }).ToList();

        var deletes = tupleKeys.Select(t => new ClientTupleKeyWithoutCondition
        {
            User = t.User,
            Relation = t.Relation,
            Object = t.Object
        })
        .ToList();

        var request = new ClientWriteRequest
        {
            Deletes = deletes
        };

        try
        {
            await _fgaClient.Write(request, null, cancellationToken);
            _logger.LogInformation("Unassigned scopes {Scopes} from user {UserId} on resource {Resource}",
             string.Join(",", userResourceAssignment.Scopes), userId, "approvalInstance:all"
            );
        }
        catch (FgaApiValidationError ex)
        {
            if (ex.Message?.Contains("tuple to be deleted did not exist", StringComparison.OrdinalIgnoreCase) == true)
            {
                _logger.LogWarning($"Attempted to delete tuples that do not exist. Details: {ex.Message}");
                return;
            }

            _logger.LogError(ex, $"Service validation error while unassigning scopes for user {userId}");
            throw;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, $"Service API error while unassigning scopes for user {userId}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while unassigning scopes for user {userId}");
            throw;
        }
    }
    public async Task AssignRoleToResourceAsync(string roleName, RoleResourceAssignment assignment, CancellationToken cancellationToken = default)
    {
        var tuples = assignment.Scopes.Select(scope => new ClientTupleKey
        {
            User = $"role:{roleName}#assignee",
            Relation = scope,
            Object = $"{assignment.Resource}:{ResourceId}"
        }).ToList();

        var request = new ClientWriteRequest { Writes = tuples };

        try
        {
            await _fgaClient.Write(request, null, cancellationToken);

            _logger.LogInformation($"Assigned scopes {assignment.Scopes} to role {roleName} on resource {assignment.Resource}");
        }
        catch (FgaApiValidationError ex) when (ex.Message.Contains("already existed"))
        {
            _logger.LogWarning($"Role {roleName} already has scopes {assignment.Scopes} on resource {assignment.Resource}. Ignoring duplicate assignment.");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, $"API error while assigning scopes for role {roleName} on resource {assignment.Resource}");
            throw new DataException($"Failed to assign role to resource with scopes {assignment.Scopes} {ex}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while assigning scopes for role {roleName} on resource {assignment.Resource}");
            throw new DataException($"Failed to assign role to resource with scopes {ex}");
        }
    }
    public async Task UnassignRoleFromResourceAsync(string roleName, RoleResourceAssignment assignment, CancellationToken cancellationToken = default)
    {
        var tuples = assignment.Scopes.Select(scope => new ClientTupleKeyWithoutCondition
        {
            User = $"role:{roleName}#assignee",
            Relation = scope,
            Object = $"{assignment.Resource}:{ResourceId}"
        }).ToList();

        var request = new ClientWriteRequest { Deletes = tuples };

        try
        {
            await _fgaClient.Write(request, null, cancellationToken);

            _logger.LogInformation($"Unassigned scopes {assignment.Scopes} from role {roleName} on resource {assignment.Resource}");
        }
        catch (FgaApiValidationError ex) when (ex.Message.Contains("did not exist"))
        {
            _logger.LogWarning($"Attempted to unassign scopes {assignment.Scopes} from role {roleName} on resource {assignment.Resource}, but they did not exist. Ignoring.");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, $"API error while unassigning scopes for role {roleName} on resource {assignment.Resource}");
            throw new DataException($"Failed to unassign role from resource with scopes {assignment.Scopes}: {ex}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while unassigning scopes for role {roleName} on resource {assignment.Resource}");
            throw new DataException($"Failed to unassign role from resource with scopes {assignment.Scopes}: {ex}");
        }
    }
    public async Task<bool> CheckAccessAsync(CheckAccess checkAccess, CancellationToken cancellationToken = default)
    {
        var request = new ClientCheckRequest
        {
            User = $"user:{checkAccess.UserId}",
            Relation = checkAccess.Relation,
            Object = $"{checkAccess.Resource}:{ResourceId}"
        };

        try
        {
            var response = await _fgaClient.Check(request, null, cancellationToken);
            response.Allowed = response.Allowed ?? false;

            if ((bool)response.Allowed)
                _logger.LogInformation($"Access ALLOWED for user {checkAccess.UserId}, relation {checkAccess.Relation}, resource {checkAccess.Resource}");
            else
                _logger.LogWarning($"Access DENIED for user {checkAccess.UserId}, relation {checkAccess.Relation}, resource {checkAccess.Resource}");

            return (bool)response.Allowed;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, $"API error while checking access for user {checkAccess.UserId}, relation {checkAccess.Relation}, resource {checkAccess.Resource}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while checking access for user {checkAccess.UserId}, relation {checkAccess.Relation}, resource {checkAccess.Resource}");
            throw;
        }
    }
    public async Task<IEnumerable<Assignment>> ListAssignmentsAsync(string resource, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _fgaClient.Read(new ClientReadRequest
            {
                Object = $"{resource}:{ResourceId}"
            }, cancellationToken: cancellationToken);

            return response.Tuples.Select(t => new Assignment
            {
                User = t.Key.User ?? string.Empty,
                Relation = t.Key.Relation ?? string.Empty,
                Resource = $"{t.Key.Object}"
            }).ToList();

        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, $"API error while listing assignments for resource {resource}");
            throw new DataException($"API error while listing assignments for resource {resource}: {ex}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while listing assignments for resource {resource}");
            throw new DataException($"Unexpected error while listing assignments for resource {resource}: {ex}");
        }
    }
    public async Task<List<AccessAssignment>> GetAllTuplesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<AccessAssignment>();
        var readRequest = new ClientReadRequest();

        string? continuation = null;
        int pageCount = 0;
        int totalTuples = 0;

        try
        {
            _logger.LogDebug("Starting to retrieve all tuples from OpenFGA");

            do
            {
                pageCount++;
                var options = new ClientReadOptions
                {
                    ContinuationToken = continuation
                };

                _logger.LogDebug("Retrieving page {PageCount} of tuples, ContinuationToken: {ContinuationToken}", pageCount, continuation ?? "null");

                var resp = await _fgaClient.Read(readRequest, options, cancellationToken);

                if (resp?.Tuples != null)
                {
                    _logger.LogDebug("Retrieved {TupleCount} tuples from page {PageCount}",
                        resp.Tuples.Count, pageCount);

                    foreach (var tuple in resp.Tuples)
                    {
                        var assignment = CreateAccessAssignmentFromTuple(tuple);
                        if (assignment is not null)
                        {
                            list.Add(assignment);
                            totalTuples++;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No tuples found in page {PageCount}", pageCount);
                }

                continuation = resp?.ContinuationToken;

                _logger.LogDebug("Page {PageCount} processed. Has more pages: {HasMorePages}", pageCount, !string.IsNullOrEmpty(continuation));

            } while (!string.IsNullOrEmpty(continuation));

            _logger.LogInformation("Successfully retrieved all tuples. Total pages: {PageCount}, Total tuples: {TotalTuples}", pageCount, totalTuples);

            return list;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "OpenFGA API error while retrieving all tuples. Pages processed: {PageCount}, Tuples processed: {TotalTuples}. Details: {Message}", pageCount, totalTuples, ex.Message);
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation canceled while retrieving tuples. Pages processed: {PageCount}, Tuples processed: {TotalTuples}", pageCount, totalTuples);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving all tuples. Pages processed: {PageCount}, Tuples processed: {TotalTuples}", pageCount, totalTuples);
            throw;
        }
    }
    public async Task<List<AccessAssignment>> GetRoleAssignmentsAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var assignments = new List<AccessAssignment>();
        string? continuation = null;

        try
        {
            _logger.LogDebug("Fetching all assignments for RoleId={RoleId}", roleId);

            do
            {
                var readRequest = new ClientReadRequest();
                var options = new ClientReadOptions { ContinuationToken = continuation };
                var resp = await _fgaClient.Read(readRequest, options, cancellationToken);

                if (resp?.Tuples != null)
                {
                    foreach (var tuple in resp.Tuples)
                    {
                        var assignment = CreateAccessAssignmentFromTuple(tuple);
                        if (assignment != null)
                        {
                            if ((assignment.ActorType == ActorType.Role && assignment.ActorId == roleId) ||
                                (assignment.ResourceType == "role" && assignment.ResourceId == roleId && assignment.Relation == "assignee"))
                            {
                                assignments.Add(assignment);
                            }
                        }
                    }
                }

                continuation = resp?.ContinuationToken;
            } while (!string.IsNullOrEmpty(continuation));

            _logger.LogInformation("Found {Count} assignments for RoleId={RoleId}", assignments.Count, roleId);
            return assignments;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "OpenFGA API error while fetching assignments for RoleId={RoleId}", roleId);
            throw new DataException("Failed to list RoleAssignment", ex);
        }
    }
    private AccessAssignment? CreateAccessAssignmentFromTuple(object? tuple)
    {
        if (tuple is null)
        {
            _logger.LogWarning("CreateAccessAssignmentFromTuple was called with null.");
            return null;
        }

        try
        {
            var tupleType = tuple.GetType();
            var key = tupleType.GetProperty("Key")?.GetValue(tuple);

            if (key is null)
            {
                _logger.LogWarning("Tuple of type {TypeName} does not contain a valid 'Key' property.", tupleType.Name);
                return null;
            }

            var timestamp = tupleType.GetProperty("Timestamp")?.GetValue(tuple) as DateTime? ?? DateTime.MinValue;

            var keyType = key.GetType();
            var user = keyType.GetProperty("User")?.GetValue(key) as string ?? string.Empty;
            var relation = keyType.GetProperty("Relation")?.GetValue(key) as string ?? string.Empty;
            var obj = keyType.GetProperty("Object")?.GetValue(key) as string ?? string.Empty;

            var (actorType, actorId) = ExtractActor(user);
            var (resourceType, resourceId) = ExtractResource(obj);

            var assignment = new AccessAssignment
            {
                ActorType = actorType,
                ActorId = actorId,
                Relation = relation,
                ResourceType = resourceType,
                ResourceId = resourceId,
                AssignedAt = timestamp
            };

            _logger.LogDebug($"Mapped tuple to AccessAssignment: {actorType}/{actorId} -> {relation} {resourceType}:{resourceId}");

            return assignment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert tuple of type {TupleType}.", tuple.GetType().Name);
            return null;
        }
    }

    private static (ActorType actorType, string actorId) ExtractActor(string user)
    {
        if (user.StartsWith("user:", StringComparison.OrdinalIgnoreCase))
            return (ActorType.User, user["user:".Length..]);

        if (user.StartsWith("role:", StringComparison.OrdinalIgnoreCase))
        {
            var after = user["role:".Length..];
            var hashIndex = after.IndexOf('#');
            return hashIndex >= 0
                ? (ActorType.Role, after[..hashIndex])
                : (ActorType.Role, after);
        }

        return (ActorType.User, user); // fallback
    }
    private static (string resourceType, string resourceId) ExtractResource(string obj)
    {
        var parts = obj.Split(':', 2);
        return parts.Length == 2 ? (parts[0], parts[1]): (obj, string.Empty);
    }

}
