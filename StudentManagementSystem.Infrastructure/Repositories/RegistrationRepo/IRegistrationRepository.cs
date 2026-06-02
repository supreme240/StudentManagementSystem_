using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repositories.GenericRepo;

namespace StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo {
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        Task<Registration> CheckAuthenticationAsync(string email, string password);
        Task<Registration> FindByEmailAndPhoneAsync(string email, string number);
        Task<List<Registration>> GetByStudentCourseAsync(string course);
    }
}