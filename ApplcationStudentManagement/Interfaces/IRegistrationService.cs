using ApplicationStudentManagement.DTOs;
using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IRegistrationService
    {
        Task AddRegistrationAsync(RegistrationViewModel registrationViewModel);
        Task<List<RegistrationViewModel>> GetAllRegistrationsAsync();

        Task<RegistrationViewModel?> GetRegistrationByIdAsync(int id);

        Task UpdateRegistrationAsync(RegistrationViewModel registrationViewModel);

        Task DeleteRegistrationAsync(int id);
        Task<RegistrationViewModel> CheckAUthenticationAsync(string email, string password);
        Task<RegistrationViewModel> FindByEmailAndPhoneAsync(string email, string phone);
        Task ResetPasswordAsync(int userId, string newPassword);
        Task<List<RegistrationViewModel>> GetByStudentCourseAsync(string studentCourse);
    }
}