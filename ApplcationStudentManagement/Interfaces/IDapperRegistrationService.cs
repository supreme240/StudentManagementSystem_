using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IDapperRegistrationService
    {
        // Declares what select operations the service exposes.
        // The controller depends only on this interface, never on the
        // concrete DapperRegistrationService class directly.


        // Get a single registration by Id, or null if not found
        Task<Registration?> GetByIdAsync(int id);

        // Get every registration in the table
        Task<IEnumerable<Registration>> GetAllAsync();

    }
}
