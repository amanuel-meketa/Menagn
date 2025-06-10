using approvals.application.DTOs.StageDefinition;

public interface IStageDefinitionService
{
    Task<Guid> CreateApplicationTypeAsync(CreateStageDefinitionDto dto);
    Task<GetStageDefinitionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<GetStageDefinitionDto>> GetAllAsync();
    Task<Guid> UpdateAsync(Guid id, UpdateStageDefinitionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid> ApproveStageAsync(Guid instanceId, Guid approverId, string comment);
}