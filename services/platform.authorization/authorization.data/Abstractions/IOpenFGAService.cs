
namespace authorization.application.Abstractions
{
    public interface IOpenFGAService
    {
        Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    }
}
