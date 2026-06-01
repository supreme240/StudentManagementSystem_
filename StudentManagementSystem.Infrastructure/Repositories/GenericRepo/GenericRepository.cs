using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Infrastructure.Data;

namespace StudentManagementSystem.Infrastructure.Repositories.GenericRepo {
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) 
            => await _dbSet.FindAsync(id);

        public async Task<List<T>> GetAllAsync() 
            => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity) 
            => await _dbSet.AddAsync(entity);

        public void Update(T entity) 
            => _dbSet.Update(entity);

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }
        public async Task SaveChangesAsync()       
            => await _dbContext.SaveChangesAsync();
    }
}