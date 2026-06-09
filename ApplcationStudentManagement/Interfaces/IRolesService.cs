using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IRolesService
    {
        Task<List<Roles>> GetAllRolesAsync();
        Task<Roles?> GetRoleByIdAsync(int id);
        Task<(bool success, string? error)> AddRoleAsync(Roles role);
        Task UpdateRoleAsync(Roles role);
        Task DeleteRoleAsync(int id);
    }
}