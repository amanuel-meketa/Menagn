using authorization.application.Abstractions;
using authorization.data.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace authorization.application.Services
{
    public class AuthorizationService
    {
        private readonly IOpenFGAService _openFgaService;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(IOpenFGAService openFgaService, ILogger<AuthorizationService> logger)
        {
            _openFgaService = openFgaService;
            _logger = logger;
        }

        public async Task<IReadOnlyList<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                return await _openFgaService.GetUserRolesAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user roles");
                throw new DataException("Failed to get user roles", ex);
            }
        }

        public async Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                await _openFgaService.AssignRoleToUserAsync(userId, roleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign roles");
                throw new DataException("Failed to assign roles", ex);
            }
        }

        public async Task UnassignRoleFromUserAsync(string userId, string roleName)
        {
            try
            {
                await _openFgaService.UnassignRoleFromUserAsync(userId, roleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unassign roles");
                throw new DataException("Failed to unassign roles", ex);
            }
        }

        public async Task AssignUserToResourceAsync(Guid userId, UserResourceAssignment userResourceAssignment)
        {
            try
            {
                await _openFgaService.AssignUserToResourceAsync(userId, userResourceAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign user to resource with scopes");
                throw new DataException("Failed to assign user to resource with scopes", ex);
            }
        }

        public async Task UnassignUserFromResourceAsync(Guid userId, UserResourceAssignment userResourceAssignment)
        {
            try
            {
                await _openFgaService.UnassignUserFromResourceAsync(userId, userResourceAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unassign user to resource with scopes");
                throw new DataException("Failed to unassign user to resource with scopes", ex);
            }
        }

        public async Task AssignRoleToResourceAsync(string roleNam, RoleResourceAssignment assignment)
        {
            try
            {
                await _openFgaService.AssignRoleToResourceAsync(roleNam, assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign role to resource with scopes");
                throw new DataException("Failed to assign role to resource with scopes", ex);
            }
        }

        public async Task UnassignRoleFromResourceAsync(string roleNam, RoleResourceAssignment assignment)
        {
            try
            {
                await _openFgaService.UnassignRoleFromResourceAsync(roleNam, assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unassign role to resource with scopes");
                throw new DataException("Failed to unassign role to resource with scopes", ex);
            }
        }

        public async Task<bool> CheckAccessAsync(CheckAccess checkAccess)
        {
            try
            {
                return await _openFgaService.CheckAccessAsync(checkAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check access ");
                throw new DataException("Failed to check access", ex);
            }
        }

        public async Task<IEnumerable<Assignment>> ListAssignmentsAsync(string resource)
        {
            try
            {
                return await _openFgaService.ListAssignmentsAsync(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list assignmentsAsync access");
                throw new DataException("Failed to list assignmentsAsync access", ex);
            }
        }

        public async Task<IEnumerable<AccessAssignment>> GetAllTuplesAsync()
        {
            try
            {
                return await _openFgaService.GetAllTuplesAsync(); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list all tuples");
                throw new DataException("Failed to list all tuples", ex);
            }
        }

        public async Task<IEnumerable<AccessAssignment>> GetRoleAssignmentsAsync(string roleId)
        {
            try
            {
                return await _openFgaService.GetRoleAssignmentsAsync(roleId); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list RoleAssignment");
                throw new DataException("Failed to list RoleAssignment", ex);
            }
        }
    }
}
