using ApplicationStudentManagement.DTO;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IRegistrationService
    {
        // CREATE
        Task<(bool success, string? error)> AddRegistrationAsync(RegistrationViewModel viewModel);

        // READ
        Task<List<RegistrationViewModel>> GetAllRegistrationsAsync();
        Task<RegistrationViewModel?> GetRegistrationByIdAsync(int id);

        // UPDATE
        Task<(bool success, string? error)> UpdateRegistrationAsync(RegistrationViewModel viewModel);

        // DELETE
        Task DeleteRegistrationAsync(int id);
    }
}