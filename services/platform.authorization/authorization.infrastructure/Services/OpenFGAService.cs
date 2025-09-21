using authorization.application.Abstractions;
using Microsoft.Extensions.Logging;
using OpenFga.Sdk.Client;

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
                Relation = "has_role",
                Type = "role"
            };

            var response = await _fgaClient.ListObjects((OpenFga.Sdk.Client.Model.IClientListObjectsRequest)request);
            return response.Objects.Select(o => o.Replace("role:", "")).ToList();
        }
    }

    internal class ClientListObjectsRequest
    {
        public string User { get; set; }
        public string Relation { get; set; }
        public string Type { get; set; }
    }
}
