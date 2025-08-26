using approvals.application.DTOs.ApprovalInstance;
using approvals.application.DTOs.StageDefinition;

public interface IStageDefinitionService
{
    Task<bool> CreateApplicationTypeAsync(CreateStageDefinitionDto[] dto);
    Task<GetStageDefinitionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GetStageDefinitionDto>> GetAllAsync();
    Task<Guid> UpdateAsync(Guid id, UpdateStageDefinitionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid> ApproveStageAsync(Guid instanceId, Guid approverId, string comment);
    Task<IEnumerable<GetStageDefinitionDto?>> GetStagesByTempIdAsync(Guid templateId);
    Task<IEnumerable<GetStageDefinitionDto>> GetAssignedTasksAsync(Guid userId);
}