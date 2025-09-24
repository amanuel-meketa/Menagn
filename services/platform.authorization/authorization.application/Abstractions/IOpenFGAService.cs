using authorization.data.Models;

namespace authorization.application.Abstractions
{
    public interface IOpenFGAService
    {
        Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
        Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
        Task UnassignRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
        Task AssignUserToResourceAsync(UserResourceAssignment userResourceAssignment, CancellationToken cancellationToken = default);
        Task UnassignUserFromResourceAsync(UserResourceAssignment userResourceAssignment, CancellationToken cancellationToken = default);
        Task AssignRoleToResourceAsync(RoleResourceAssignment assignment, CancellationToken cancellationToken = default);
        Task UnassignRoleFromResourceAsync(RoleResourceAssignment assignment, CancellationToken cancellationToken = default);
        Task<bool> CheckAccessAsync(CheckAccessAsync checkAccess, CancellationToken cancellationToken = default);

    }
}
