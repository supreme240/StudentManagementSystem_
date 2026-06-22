using ApplicationStudentManagement.DTO;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IDapperRegistrationService
    {
        // Returns null if no record matches the id
        Task<RegistrationViewModel?> GetByIdAsync(int id);

        // Returns every row in the registrations table as ViewModels
        Task<IEnumerable<RegistrationViewModel>> GetAllAsync();
    }
}