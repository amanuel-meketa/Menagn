﻿using approvals.domain.Entities;
using approvals.shared.Repositories;

namespace approvals.application.Interfaces.Repository
{
    public interface IStageInstanceRepository : IGenericRepository<StageInstance> 
    {
        Task ApproveStageAsync(Guid instanceId, Guid approverId, string comment);
    }
}
