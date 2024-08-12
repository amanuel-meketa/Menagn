using security.business.Dtos.Incoming;

namespace security.business.Contracts
{
    public interface IUserService
    {
        public Task<IEnumerable<GetUserDto>> GetUsers();
        public Task<GetUserDto> GetUser(string id);
    }
}
