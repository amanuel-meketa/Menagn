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

       
    }
}
