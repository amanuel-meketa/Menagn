using approvals.application.DTOs.ApplicationType;
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

    public async Task<Guid> StartAppInstance(Guid templateId, Guid createdBy)
    {
        // 1. Get the template with stage definitions - with AsNoTracking already done in repo
        var templateDto = await _approvalTemplateService.GetByIdAsync(templateId);

        // Map to domain, no tracked StageDefinitions
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

}
