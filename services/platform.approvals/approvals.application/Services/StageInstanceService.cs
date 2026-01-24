using approvals.application.DTOs.StageDefinition;
using approvals.application.DTOs.StageInstance;
using approvals.application.Interfaces;
using approvals.application.Interfaces.Repository;
using approvals.application.Interfaces.Services;
using AutoMapper;

namespace approvals.application.Services
{
    public class StageInstanceService : IStageInstanceService
    {
        private readonly IStageInstanceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StageInstanceService(IStageInstanceRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetStageInstanceDto>> GetAllAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<IEnumerable<GetStageInstanceDto>>(list);
        }

        public async Task AssignApproverAsync(AssignmentDto assignment)
        {
            var stage = await _repository.GetByIdAsync(assignment.StageInstanceId);
            if (stage == null)
                throw new Exception("Stage instance not found");

            stage.AssignApprover(assignment.ApproverId);
            await _repository.UpdateAsync(stage);
            await _unitOfWork.CommitAsync();
        }

        public async Task UnassignApproverAsync(AssignmentDto assignment)
        {
            var stage = await _repository.GetByIdAsync(assignment.StageInstanceId);
            if (stage == null)
                throw new Exception("Stage instance not found");

            stage.UnassignApprover();
            await _repository.UpdateAsync(stage);
            await _unitOfWork.CommitAsync();
        }
    }

}
