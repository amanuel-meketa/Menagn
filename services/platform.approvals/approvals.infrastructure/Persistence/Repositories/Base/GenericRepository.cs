using approvals.application.Interfaces;
using approvals.shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace approvals.infrastructure.Persistence.Repositories.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> ListAsync() => await _dbSet.ToListAsync();

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
