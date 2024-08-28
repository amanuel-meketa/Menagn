using security.sharedUtils.Dtos.Role.Incoming;

namespace security.business.Contracts
{
    public interface IRoleService
    {
        public Task<RoleDto> Create(CreateRoleDto role);
    }
}
