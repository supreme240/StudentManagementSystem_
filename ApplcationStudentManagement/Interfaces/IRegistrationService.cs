using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IRegistrationService
    {
        Task AddRegistrationAsync(Registration registration);
        Task<List<Registration>> GetAllRegistrationsAsync();

        Task<Registration?> GetRegistrationByIdAsync(int id);

        Task UpdateRegistrationAsync(Registration registration);

        Task DeleteRegistrationAsync(int id);
        Task<Registration> CheckAUthenticationAsync(string email, string password);
        Task<Registration> FindByEmailAndPhoneAsync(string email, string phone);
        Task<List<Registration>> GetByStudentCourseAsync(string studentCourse);
    }
}