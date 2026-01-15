using approvals.application.DTOs.TemplateHub;
using approvals.application.Interfaces.Repository;
using approvals.application.Interfaces.Services;
using approvals.domain.Entities;

namespace approvals.application.Services
{
    public class TemplateHubService : ITemplateHubService
    {
        private readonly ITemplateHubRepository _repo;

        public TemplateHubService(ITemplateHubRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<TemplateIndexItemDtos>> ListTemplatesAsync()
        {
            return await _repo.GetTemplateIndexAsync();
        }

        public async Task<ApprovalTemplate?> GetTemplateDetailsAsync(string key)
        {
            return await _repo.GetTemplateAsync(key);
        }
    }
}
