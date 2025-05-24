using security.sharedUtils.Dtos.Role.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;

namespace security.business.Contracts
{
    public interface IRoleService
    {
        public Task<RoleDto> CreateRole(CreateRoleDto role);
        public Task<IEnumerable<RoleDto>> GetRoles();
        public Task<RoleDto> GetRole(string id);
        public Task<RoleDto> UpdateRole(string id, UpdateRoleDto role);
        public Task DeleteRole(string id);
        Task<IEnumerable<GetUserDto>> GetRoleUsers(string roleName);
    }
}
