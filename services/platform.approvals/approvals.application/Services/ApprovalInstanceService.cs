using approvals.application.DTOs.ApprovalInstance;
using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using AutoMapper;

public class ApprovalInstanceService : IApprovalInstanceService
{
    private readonly IApprovalInstanceRepository _approvalInstanceRepository;
    private readonly IApprovalTemplateService _approvalTemplateService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApprovalInstanceService(
        IApprovalInstanceRepository approvalInstanceRepository,
        IApprovalTemplateService approvalTemplateService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _approvalInstanceRepository = approvalInstanceRepository;
        _approvalTemplateService = approvalTemplateService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GetApprovalInstanceDto>> GetAllAsync()
    {
        var list = await _approvalInstanceRepository.ListAsync();
        return _mapper.Map<IEnumerable<GetApprovalInstanceDto>>(list);
    }

    public async Task<GetApprovalInstanceDto?> GetByIdAsync(Guid id)
    {
        var entity = await _approvalInstanceRepository.GetByIdAsync(id);
        return _mapper.Map<GetApprovalInstanceDto>(entity);
    }

    public async Task<Guid> UpdateAsync(Guid id, UpdateApprovaleInstanceDto dto)
    {
        var entity = await _approvalInstanceRepository.GetByIdAsync(id);
        if (entity == null) return Guid.Empty;

        _mapper.Map(dto, entity);

        await _approvalInstanceRepository.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
        return id;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _approvalInstanceRepository.GetByIdAsync(id);
        if (entity == null) return false;

        await _approvalInstanceRepository.DeleteAsync(entity);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<Guid> StartAppInstanceAsync(Guid templateId, Guid createdBy)
    {
        // 1. Get the template with stage definitions - with AsNoTracking already done in repo
        var templateDto = await _approvalTemplateService.GetByIdAsync(templateId);
        var template = _mapper.Map<ApprovalTemplate>(templateDto);

        // 2. Create new ApprovalInstance with StageInstances referencing StageDefinitions by Id only
        var instance = new ApprovalInstance
        {
            TemplateId = templateId,
            CreatedBy = createdBy,
            CurrentStageOrder = 1,
            StageInstances = template.StageDefinitions.Select(sd => new StageInstance
            {
                StageDefId = sd.StageDefId,
                StageName = sd.StageName,
                SequenceOrder = sd.SequenceOrder,
                Status = "Pending"
            }).ToList()
        };

        // 3. Activate first stage
        var firstStage = instance.StageInstances.FirstOrDefault(s => s.SequenceOrder == 1);
        if (firstStage == null)
            throw new Exception("No first stage found in the template.");

        firstStage.Activate();

        // 4. Add instance
        await _approvalInstanceRepository.AddAsync(instance);
        await _unitOfWork.CommitAsync();

        return instance.InstanceId;
    }

    Task<IEnumerable<ApprovalInstance?>> IApprovalInstanceService.GetByTemplateIdAsync(Guid templateId)
    {
        return _approvalInstanceRepository.GetByTemplateIdAsync(templateId);
    }

    public async Task<IEnumerable<GetApprovalInstanceDto?>> GetMyAppInstances(Guid userId)
    {
        var result = await _approvalInstanceRepository.GetMyAppInstances(userId);
        return _mapper.Map<IEnumerable<GetApprovalInstanceDto>>(result);
    }
}