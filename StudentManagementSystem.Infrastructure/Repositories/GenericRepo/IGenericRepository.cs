using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentManagementSystem.Infrastructure.Repositories.GenericRepo {
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}