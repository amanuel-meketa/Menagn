using approvals.application.DTOs.StageDefinition;
using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using AutoMapper;

public class StageDefinitionService : IStageDefinitionService
{
    private readonly IStageDefinitionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StageDefinitionService(IStageDefinitionRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateApplicationTypeAsync(CreateStageDefinitionDto dto)
    {
        StageDefinition entity = _mapper.Map<StageDefinition>(dto);

        await _repository.AddAsync(entity);
        await _unitOfWork.CommitAsync();
        dto.Description = entity.Description;
        return entity.Id;
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
}