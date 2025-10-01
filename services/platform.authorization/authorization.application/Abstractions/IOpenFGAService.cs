using authorization.data.Models;

namespace authorization.application.Abstractions
{
    public interface IOpenFGAService
    {
        Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
        Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
        Task UnassignRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
        Task AssignUserToResourceAsync(Guid userId, UserResourceAssignment userResourceAssignment, CancellationToken cancellationToken = default);
        Task UnassignUserFromResourceAsync(Guid userId, UserResourceAssignment userResourceAssignment, CancellationToken cancellationToken = default);
        Task AssignRoleToResourceAsync(string roleName, RoleResourceAssignment assignment, CancellationToken cancellationToken = default);
        Task UnassignRoleFromResourceAsync(string roleName, RoleResourceAssignment assignment, CancellationToken cancellationToken = default);
        Task<bool> CheckAccessAsync(CheckAccess checkAccess, CancellationToken cancellationToken = default);
        Task<IEnumerable<Assignment>> ListAssignmentsAsync(string resource, CancellationToken cancellationToken = default);
        Task<List<AccessAssignment>> GetAllTuplesAsync(CancellationToken cancellationToken = default);
        Task<List<AccessAssignment>> GetRoleAssignmentsAsync(string RoleId, CancellationToken cancellationToken = default);

    }
}
