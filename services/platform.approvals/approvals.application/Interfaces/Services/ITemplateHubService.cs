using approvals.application.DTOs.TemplateHub;
using approvals.domain.Entities;

namespace approvals.application.Interfaces.Services
{
    public interface ITemplateHubService
    {
        Task<List<TemplateIndexItemDtos>> ListTemplatesAsync();
        Task<ApprovalTemplate> GetTemplateDetailsAsync(string key);
    }
}
