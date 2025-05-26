using AutoMapper;
using approvals.application.Interfaces;
using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;
using approvals.application.DTOs.ApplicationType;

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

        public async Task<IEnumerable<GetApplicationTypeDto>> GetAllAsync()
        {
            var approvals = await _context.applicationType.ToListAsync();
            return _mapper.Map<List<GetApplicationTypeDto>>(approvals);
        }

        public async Task<GetApplicationTypeDto?> GetByIdAsync(Guid id)
        {
            var approval = await _context.applicationType.FindAsync(id);
            return approval == null ? null : _mapper.Map<GetApplicationTypeDto>(approval);
        }

        public async Task<Guid> CreateAsync(CreateApplicationTypeDto dto)
        {
            var entity = _mapper.Map<ApplicationType>(dto);
            _context.applicationType.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(CreateApplicationTypeDto dto)
        {
            //var entity = await _context.Approvals.FindAsync(dto.Id);
            //if (entity == null) return false;

            //_mapper.Map(dto, entity); // Map only changed properties
            //await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.applicationType.FindAsync(id);
            if (entity == null) return false;

            _context.applicationType.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
