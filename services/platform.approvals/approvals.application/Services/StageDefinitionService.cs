using approvals.application.DTOs.ApprovalInstance;
using approvals.application.DTOs.StageDefinition;
using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
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

    public async Task<Guid> ApproveStageAsync(Guid instanceId, Guid approverId, string comment)
    {
        // Load the domain entity (including stage instances)
        var instance = await _instanceRepo.GetByIdAsync(instanceId);
        if (instance == null) throw new Exception("Instance not found");

        if (instance.StageInstances == null || !instance.StageInstances.Any())
            throw new Exception("Instance has no stage instances");

        // Get current stage domain object
        var currentStage = instance.StageInstances
            .FirstOrDefault(si => si.SequenceOrder == instance.CurrentStageOrder);

        if (currentStage == null)
            throw new Exception($"Current stage {instance.CurrentStageOrder} not found");

        // Domain operations (these should be methods on StageInstance)
        currentStage.Complete("Approved", approverId, comment);

        // Move to next stage if exists
        var nextStage = instance.StageInstances
            .OrderBy(si => si.SequenceOrder)
            .FirstOrDefault(si => si.SequenceOrder > currentStage.SequenceOrder);

        if (nextStage != null)
        {
            nextStage.Activate();
            instance.CurrentStageOrder = nextStage.SequenceOrder;
        }
        else
        {
            instance.OverallStatus = "Approved";
            instance.CompletedAt = DateTime.UtcNow;
        }

        // Persist and commit: repository only persists the domain entity
        await _instanceRepo.UpdateAsync(instance);
        await _unitOfWork.CommitAsync();

        return instance.InstanceId;
    }

    public async Task<IEnumerable<GetStageDefinitionDto?>> GetStagesByTempIdAsync(Guid tempId)
    {
        var result =  await _repository.GetStagesByTempIdAsync(tempId);
        return _mapper.Map<IEnumerable<GetStageDefinitionDto?>>(result);
    }

    public async Task<IEnumerable<GetStageDefinitionDto>> GetAssignedTasksAsync(Guid userId)
    {
        var result = await _repository.GetAssignedTasksAsync(userId);
        return _mapper.Map<IEnumerable<GetStageDefinitionDto>>(result);
    }
}