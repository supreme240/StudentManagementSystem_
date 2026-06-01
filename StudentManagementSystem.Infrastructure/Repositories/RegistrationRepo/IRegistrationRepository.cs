using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repositories.GenericRepo;

namespace StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo {
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        Task<List<Registration>> GetByStudentCourseAsync(string course);
    }
}