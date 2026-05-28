using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repository;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        Task AddAsync(Registration registration);
        Task<List<Registration>> GetAllAsync();
        Task<Registration?> GetByIdAsync(int id);
        Task UpdateAsync(Registration registration);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
} //