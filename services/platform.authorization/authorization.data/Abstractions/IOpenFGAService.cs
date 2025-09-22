
using authorization.data.Models;

namespace authorization.application.Abstractions
{
    public interface IOpenFGAService
    {
        Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
        Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
        Task UnassignRoleFromUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
        Task AssignUserToResourceAsync(ResourceAssignment resourceAssignment, CancellationToken cancellationToken = default);
    }
}
