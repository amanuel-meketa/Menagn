using approvals.application.DTOs.StageDefinition;
using approvals.application.DTOs.StageInstance;

namespace approvals.application.Interfaces.Services
{
    public interface IStageInstanceService
    {
        Task<IEnumerable<GetStageInstanceDto>> GetAllAsync();
        Task AssignApproverAsync(AssignmentDto assignment);
        Task UnassignApproverAsync(AssignmentDto assignment);
    }
}
