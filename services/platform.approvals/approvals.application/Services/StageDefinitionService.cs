using approvals.application.DTOs.ApprovalInstance;
using approvals.application.DTOs.StageDefinition;
using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using AutoMapper;

public class StageDefinitionService : IStageDefinitionService
{
    private readonly IStageDefinitionRepository _repository;
    private readonly IApprovalInstanceService _approvalInstanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StageDefinitionService(IStageDefinitionRepository repository, IApprovalInstanceService approvalInstanceService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
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
        var instance = await _approvalInstanceService.GetByIdAsync(instanceId);
        if (instance == null) throw new Exception("Instance not found");

        var instanceUpdate = _mapper.Map<UpdateApprovaleInstanceDto>(instance);

        var currentStage = instanceUpdate.StageInstances.First(si => si.SequenceOrder == instanceUpdate.CurrentStageOrder);
        currentStage.Complete("Approved", approverId, comment);

        var nextStage = instanceUpdate.StageInstances.FirstOrDefault(si => si.SequenceOrder == currentStage.SequenceOrder + 1);
        if (nextStage != null)
        {
            nextStage.Activate();
            instanceUpdate.CurrentStageOrder = nextStage.SequenceOrder;
        }
        else
        {
            instanceUpdate.OverallStatus = "Approved";
            //instance.CurrentStageOrder = nextStage.SequenceOrder;
            instanceUpdate.CompletedAt = DateTime.UtcNow;
        }

        await _approvalInstanceService.UpdateAsync(instanceUpdate.InstanceId, instanceUpdate);
        await _unitOfWork.CommitAsync();
        return instanceUpdate.InstanceId;
    }

    public async Task<IEnumerable<GetStageDefinitionDto?>> GetStagesByTempIdAsync(Guid tempId)
    {
        var result =  await _repository.GetStagesByTempIdAsync(tempId);
        return _mapper.Map<IEnumerable<GetStageDefinitionDto?>>(result);
    }
}