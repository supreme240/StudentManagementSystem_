using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IRegistrationService
    {
        Task<(bool success, string? error)> AddRegistrationAsync(Registration registration);  //
        Task<List<Registration>> GetAllRegistrationsAsync();
        Task<Registration?> GetRegistrationByIdAsync(int id);
        Task UpdateRegistrationAsync(Registration registration);
        Task DeleteRegistrationAsync(int id);

        // Legacy methods
        Registration GetRegistrationInformation();
        List<Registration> GetAllRegistrations();
        Task<string?> GetALLRegistrationsInformationAsync();
        void AddRegistration(Registration registration);
    }
}