using StudentManagement.domain.Domain;

namespace StudentManagementSystem.Infrastructure.Repositories
{
    public interface IRegistrationRepository
    {
        Task AddAsync(Registration registration);
        Task<List<Registration>> GetAllAsync();
        Task<Registration?> GetByIdAsync(int id);
        Task UpdateAsync(Registration registration);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}