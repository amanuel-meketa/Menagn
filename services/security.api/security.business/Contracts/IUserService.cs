using security.business.Dtos;

namespace security.business.Contracts
{
    public interface IUserService
    {
        public Task<IEnumerable<GetUserDto>> GetUsers();
    }
}
