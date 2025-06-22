using approvals.application.DTOs.ApplicationType;
using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using AutoMapper;

public class ApprovalTemplateService : IApprovalTemplateService
{
    private readonly IApprovalRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApprovalTemplateService(IApprovalRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateApplicationTypeAsync(CreateApprovalTemplateDto dto)
    {
        ApprovalTemplate entity = _mapper.Map<ApprovalTemplate>(dto);

        await _repository.AddAsync(entity);
        await _unitOfWork.CommitAsync();
        return entity.TemplateId;
    }

    public async Task<IEnumerable<GetAppTemplateDto>> GetAllAsync()
    {
        var list = await _repository.ListAsync();
        return _mapper.Map<IEnumerable<GetAppTemplateDto>>(list);
    }

    public async Task<GetAppTemplateDto?> GetByIdAsync(Guid id)
    {
        ApprovalTemplate? entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<GetAppTemplateDto>(entity);
    }

    public async Task<Guid> UpdateAsync(Guid id, UpdatAppemplateDto dto)
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
}