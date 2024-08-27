using security.sharedUtils.Dtos.Account.Incoming;
using security.sharedUtils.Dtos.Account.Outgoing;
using System.Security.Claims;

namespace security.business.Contracts
{
    public interface IAccountService
    {
        public Task<TokenResponseDto> LogIn(LoginCredentialsDto credential);
        public Task LogOut(string refreshToken);
    }
}
