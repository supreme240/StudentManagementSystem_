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

        // Keep old methods if you are still using them
        Registration GetRegistrationInformation();
        List<Registration> GetAllRegistrations();
        Task<string?> GetALLRegistrationsInformationAsync();
        void AddRegistration(Registration registration);
    }
}