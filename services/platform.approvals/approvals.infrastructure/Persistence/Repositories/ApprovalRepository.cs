using approvals.application.DTOs;
using approvals.application.Interfaces;
using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly AppDbContext _context;

        public ApprovalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApprovalDto>> GetAllAsync()
        {
            return await _context.Approvals
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Status = a.Status
                }).ToListAsync();
        }

        public async Task<ApprovalDto?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Approvals.FindAsync(id);
            return entity == null ? null : new ApprovalDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Status = entity.Status
            };
        }

        public async Task<Guid> CreateAsync(ApprovalDto dto)
        {
            var approval = new Approval
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Status = dto.Status
            };

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();

            return approval.Id;
        }

        public async Task<bool> UpdateAsync(ApprovalDto dto)
        {
            var approval = await _context.Approvals.FindAsync(dto.Id);
            if (approval == null) return false;

            approval.Name = dto.Name;
            approval.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var approval = await _context.Approvals.FindAsync(id);
            if (approval == null) return false;

            _context.Approvals.Remove(approval);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}