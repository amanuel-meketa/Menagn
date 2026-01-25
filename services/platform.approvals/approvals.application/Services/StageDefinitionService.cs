using approvals.application.DTOs.EnumDtos;
using approvals.application.DTOs.StageDefinition;
using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.domain.Enums;
using AutoMapper;

public class StageDefinitionService : IStageDefinitionService
{
    private readonly IStageDefinitionRepository _repository;
    private readonly IApprovalInstanceRepository _instanceRepo;
    private readonly IApprovalInstanceService _approvalInstanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StageDefinitionService(IStageDefinitionRepository repository, IApprovalInstanceRepository instanceRepo, IApprovalInstanceService approvalInstanceService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _instanceRepo = instanceRepo;
        _approvalInstanceService = approvalInstanceService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> CreateApplicationTypeAsync(CreateStageDefinitionDto[] dto)
    {
        StageDefinition[] entity = _mapper.Map<StageDefinition[]>(dto);

        foreach (var stageDto in entity)
        {
            await _repository.AddAsync(stageDto);
            await _unitOfWork.CommitAsync();
        }

        return true;
    }

    public async Task<IEnumerable<GetStageDefinitionDto>> GetAllAsync()
    {
        var list = await _repository.ListAsync();
        return _mapper.Map<IEnumerable<GetStageDefinitionDto>>(list);
    }

    public async Task<GetStageDefinitionDto?> GetByIdAsync(Guid id)
    {
        StageDefinition? entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<GetStageDefinitionDto>(entity);
    }

    public async Task<Guid> UpdateAsync(Guid id, UpdateStageDefinitionDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Guid.Empty;

        _mapper.Map(dto, entity);

        await _repository.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
        return id;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<Guid> ActOnStageAsync(Guid instanceId, Guid approverId, StageInstanceStatus action, string comment)
    {
        // 1️⃣ Load approval instance
        var instance = await _instanceRepo.GetByIdAsync(instanceId);
        if (instance == null)
            throw new Exception("Approval instance not found");

        if (instance.StageInstances == null || !instance.StageInstances.Any())
            throw new Exception("Approval instance has no stages");

        // 2️⃣ Find active stage (NOT by sequence blindly)
        var currentStage = instance.StageInstances.FirstOrDefault(s => s.Status == StageInstanceStatus.Active);

        if (currentStage == null)
            throw new Exception("No active stage found");

        // 3️⃣ Apply the action to current stage
        currentStage.Complete(action, approverId, comment);

        switch (action)
        {
            case StageInstanceStatus.Approved:
                // ✅ Move to next stage if exists
                var nextStage = instance.StageInstances
                    .OrderBy(s => s.SequenceOrder)
                    .FirstOrDefault(s => s.Status == StageInstanceStatus.Pending);

                if (nextStage != null)
                {
                    nextStage.Activate();
                    instance.CurrentStageOrder = nextStage.SequenceOrder;
                    instance.OverallStatus = ApprovalInstanceStatus.InProgress;
                }
                else
                {
                    instance.OverallStatus = ApprovalInstanceStatus.Approved;
                    instance.CompletedAt = DateTime.UtcNow;
                }
                break;

            case StageInstanceStatus.Rejected:
                // ❌ Stop the process immediately
                instance.OverallStatus = ApprovalInstanceStatus.Rejected;
                instance.CompletedAt = DateTime.UtcNow;
                break;

            case StageInstanceStatus.Cancelled:
                // ❌ Stop the process immediately
                instance.OverallStatus = ApprovalInstanceStatus.Cancelled;
                instance.CompletedAt = DateTime.UtcNow;
                break;

            case StageInstanceStatus.Skipped:
                // ⚡ Skip this stage and move to next
                var nextPendingStage = instance.StageInstances
                    .OrderBy(s => s.SequenceOrder)
                    .FirstOrDefault(s => s.Status == StageInstanceStatus.Pending);

                if (nextPendingStage != null)
                {
                    nextPendingStage.Activate();
                    instance.CurrentStageOrder = nextPendingStage.SequenceOrder;
                    instance.OverallStatus = ApprovalInstanceStatus.InProgress;
                }
                else
                {
                    instance.OverallStatus = ApprovalInstanceStatus.Approved;
                    instance.CompletedAt = DateTime.UtcNow;
                }
                break;

            case StageInstanceStatus.Pending:
            case StageInstanceStatus.Active:
            default:
                // ⚠ Usually shouldn't happen, ignore or throw
                break;
        }

        // 4️⃣ Persist changes
        await _instanceRepo.UpdateAsync(instance);
        await _unitOfWork.CommitAsync();

        return instance.InstanceId;
    }

    public async Task<IEnumerable<GetStageDefinitionDto?>> GetStagesByTempIdAsync(Guid tempId)
    {
        var result = await _repository.GetStagesByTempIdAsync(tempId);
        return _mapper.Map<IEnumerable<GetStageDefinitionDto?>>(result);
    }

    public async Task<IEnumerable<GetStageDefinitionDto>> GetAssignedTasksAsync(Guid userId)
    {
        var result = await _repository.GetAssignedTasksAsync(userId);
        return _mapper.Map<IEnumerable<GetStageDefinitionDto>>(result);
    }

}