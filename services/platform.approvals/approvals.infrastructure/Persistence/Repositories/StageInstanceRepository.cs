using approvals.application.DTOs.ApprovalInstance;
using approvals.application.Interfaces.Repository;
using approvals.domain.Entities;
using approvals.infrastructure.Persistence.Repositories.Base;
using AutoMapper;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class StageInstanceRepository : GenericRepository<StageInstance>, IStageInstanceRepository
    {
        private readonly IApprovalInstanceService _instanceService;
        private readonly IMapper _mapper;
        public StageInstanceRepository(AppDbContext dbContext, IApprovalInstanceService instanceService, IMapper mapper) : base(dbContext)
        {
            _instanceService = instanceService;
            _mapper =  mapper;
        }

        public async Task ApproveStageAsync(Guid instanceId, Guid approverId, string comment)
        {
            var instance = await _instanceService.GetByIdAsync(instanceId);
            if (instance == null) throw new Exception("Instance not found");

            var updateInstance = _mapper.Map<UpdateApprovaleInstanceDto>(instance);

            var currentStage = instance.StageInstances.First(si => si.SequenceOrder == instance.CurrentStageOrder);
            currentStage.Complete("Approved", approverId, comment);

            var nextStage = instance.StageInstances.FirstOrDefault(si => si.SequenceOrder == currentStage.SequenceOrder + 1);
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

            await _instanceService.UpdateAsync(instance.InstanceId, updateInstance);
        }
    }
}
