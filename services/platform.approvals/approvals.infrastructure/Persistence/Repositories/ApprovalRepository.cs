using AutoMapper;
using approvals.application.DTOs;
using approvals.application.Interfaces;
using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ApprovalRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApprovalDto>> GetAllAsync()
        {
            var approvals = await _context.Approvals.ToListAsync();
            return _mapper.Map<List<ApprovalDto>>(approvals);
        }

        public async Task<ApprovalDto?> GetByIdAsync(Guid id)
        {
            var approval = await _context.Approvals.FindAsync(id);
            return approval == null ? null : _mapper.Map<ApprovalDto>(approval);
        }

        public async Task<Guid> CreateAsync(ApprovalDto dto)
        {
            var entity = _mapper.Map<Approval>(dto);
            _context.Approvals.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(ApprovalDto dto)
        {
            var entity = await _context.Approvals.FindAsync(dto.Id);
            if (entity == null) return false;

            _mapper.Map(dto, entity); // Map only changed properties
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Approvals.FindAsync(id);
            if (entity == null) return false;

            _context.Approvals.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
