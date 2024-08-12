using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;

namespace security.business.Contracts
{
    public interface IUserService
    {
        public Task<GetUserDto> CreateUser(CreateUserDto user);
        public Task<IEnumerable<GetUserDto>> GetUsers();
        public Task<GetUserDto> GetUser(string id);
    }
}
