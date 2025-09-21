
namespace authorization.application.Abstractions
{
    public interface IOpenFGAService
    {
        Task<List<string>> GetUserRolesAsync(string userId);
    }
}
