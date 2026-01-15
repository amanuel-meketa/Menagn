using approvals.application.DTOs.TemplateHub;
using approvals.domain.Entities;

namespace approvals.application.Interfaces.Repository
{
    public interface ITemplateHubRepository
    {
        Task<List<TemplateIndexItemDtos>> GetTemplateIndexAsync();
        Task<ApprovalTemplate?> GetTemplateAsync(string key);
    }
}
