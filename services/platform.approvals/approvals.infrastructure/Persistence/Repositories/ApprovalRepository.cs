using AutoMapper;
using approvals.application.Interfaces;
using approvals.domain.Entities;
using Microsoft.EntityFrameworkCore;
using approvals.application.DTOs.ApplicationType;
using approvals.application.Common.Exceptions;

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
            var entities = await _context.ApplicationTypes.AsNoTracking().ToListAsync();

            return _mapper.Map<List<GetApplicationTypeDto>>(entities);
        }

        public async Task<GetApplicationTypeDto> GetByIdAsync(Guid id)
        {
            var entity = await _context.ApplicationTypes.FindAsync(id);

            if (entity == null)
                throw new NotFoundException($"ApplicationType with ID {id} not found.");

            return _mapper.Map<GetApplicationTypeDto>(entity);
        }

        public async Task<Guid> CreateAsync(CreateApplicationTypeDto dto)
        {
            var exists = await _context.ApplicationTypes.AnyAsync(x => x.Name == dto.Name); 

            if (exists)
                throw new ConflictException("Application type already exists.");

            var entity = _mapper.Map<ApplicationType>(dto);
            _context.ApplicationTypes.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(UpdateApplicationTypeDto updateApp)
        {
            var entity = await _context.ApplicationTypes.FindAsync(updateApp.Id);
            if (entity == null)
                return false;

            _mapper.Map(updateApp, entity);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false; 
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.ApplicationTypes.FindAsync(id);
            if (entity == null)
                return false;

            _context.ApplicationTypes.Remove(entity);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
