using security.sharedUtils.Dtos.Role.Incoming;
using security.sharedUtils.Dtos.Role.Outgoing;
using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;
using System.Data;

namespace security.business.Contracts
{
    public interface IUserService
    {
        public Task<GetUserDto> Create(CreateUserDto user);
        public Task<IEnumerable<GetUserDto>> GetAll();
        public Task<GetUserDto> Get(string id);
        public Task<GetUserDto> Update(string id, UpdateUserDto user);
        public Task Delete(string id);
        public Task<IEnumerable<UserSessionDto>> AllSessions(string id);
        public Task RemoveAllSessions(string id);
        public Task ResetPassword(string id, string newPassword);
        public Task<IEnumerable<GetRoleDto>> AssignedRoles(string id);
        public Task<IEnumerable<GetRoleDto>> UnAssignedRoles(string id);
        public Task AssignRoles(string id, RoleDto[] roles);
        public Task RemoveRoles(string id, RoleDto[] roles);
    }
}
