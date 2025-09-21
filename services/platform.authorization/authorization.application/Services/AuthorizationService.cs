using authorization.application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Data;

namespace authorization.application.Services
{
    public class AuthorizationService
    {
        private readonly IOpenFGAService _openFgaService;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(IOpenFGAService openFgaService, ILogger<AuthorizationService> logger)
        {
            _openFgaService = openFgaService;
            _logger = logger;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                var roles = await _openFgaService.GetUserRolesAsync(userId);
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user roles");
                throw new DataException("Failed to get user roles", ex);
            }
        }
    }
}
