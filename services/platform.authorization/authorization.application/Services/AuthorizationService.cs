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

        public async Task AssignUserToResourceAsync(UserResourceAssignment userResourceAssignment)
        {
            try
            {
                 await _openFgaService.AssignUserToResourceAsync(userResourceAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign user to resource with scopes");
                throw new DataException("Failed to assign user to resource with scopes", ex);
            }
        }

        public async Task UnassignUserFromResourceAsync(UserResourceAssignment userResourceAssignment)
        {
            try
            {
                 await _openFgaService.UnassignUserFromResourceAsync(userResourceAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unassign user to resource with scopes");
                throw new DataException("Failed to unassign user to resource with scopes", ex);
            }
        }

        public async Task AssignRoleToResourceAsync(RoleResourceAssignment assignment)
        {
            try
            {
                await _openFgaService.AssignRoleToResourceAsync(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign role to resource with scopes");
                throw new DataException("Failed to assign role to resource with scopes", ex);
            }
        }
        public async Task UnassignRoleFromResourceAsync(RoleResourceAssignment assignment)
        {
            try
            {
                await _openFgaService.UnassignRoleFromResourceAsync(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unassign role to resource with scopes");
                throw new DataException("Failed to unassign role to resource with scopes", ex);
            }
        }

    }
}
