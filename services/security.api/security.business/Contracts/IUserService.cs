using security.sharedUtils.Dtos.User.Incoming;
using security.sharedUtils.Dtos.User.Outgoing;

namespace security.business.Contracts
{
    public interface IUserService
    {
        public Task<GetUserDto> Create(CreateUserDto user);
        public Task<IEnumerable<GetUserDto>> GetAll();
        public Task<GetUserDto> Get(Guid id);
        public Task<GetUserDto> Update(Guid id, UpdateUserDto user);
        public Task Delete(Guid id);
    }
}
