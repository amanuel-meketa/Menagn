using IdentityModel.Client;
using security.sharedUtils.Dtos.Account;

namespace security.business.Contracts
{
    public interface IAccountService
    {
        public Task<TokenResponseDto> LogIn(LoginCredentialsDto credential);
    }
}
