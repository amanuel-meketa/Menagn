using authorization.application.Abstractions;
using Microsoft.Extensions.Logging;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Configuration;
using OpenFga.Sdk.Exceptions;
using OpenFga.Sdk.Model;

namespace authorization.infrastructure.Services
{
    public class OpenFGAService : IOpenFGAService
    {
        private readonly OpenFgaClient _fgaClient;
        private readonly ILogger<OpenFGAService> _logger;

        public OpenFGAService(OpenFgaClient fgaClient, ILogger<OpenFGAService> logger)
        {
            _fgaClient = fgaClient;
            _logger = logger;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var request = new ClientListObjectsRequest
            {
                User = $"user:{userId}",
                Relation = "assignee",
                Type = "role"  
            };

            var response = await _fgaClient.ListObjects(request);
            return response.Objects.Select(o => o.Replace("role:", "")).ToList();
        }

    }
}
