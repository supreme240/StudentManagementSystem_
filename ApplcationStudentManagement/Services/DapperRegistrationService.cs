using ApplicationStudentManagement.DTO;
using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.DapperRepositories;

namespace ApplicationStudentManagement.Services
{
    public class DapperRegistrationService : IDapperRegistrationService
    {
        private readonly IDapperRegistrationRepository _repository;

        public DapperRegistrationService(IDapperRegistrationRepository repository)
        {
            _repository = repository;
        }

        // ------------------------------------------------------------
        // GET ONE
        // ------------------------------------------------------------
        public async Task<RegistrationViewModel?> GetByIdAsync(int id)
        {
            // Repository returns a Registration entity (or null)
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return null;

            // Convert before sending up — controller never sees Registration
            return ToViewModel(entity);
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        public async Task<IEnumerable<RegistrationViewModel>> GetAllAsync()
        {
            // Repository returns IEnumerable<Registration>
            var entities = await _repository.GetAllAsync();

            // Convert every entity to a ViewModel before returning the list
            return entities.Select(e => ToViewModel(e));
        }

        // ------------------------------------------------------------
        // Private converter
        // ------------------------------------------------------------

        // Exact same shape as the one in RegistrationService.
        // It is duplicated on purpose — these two services are independent
        // and should not share a converter class.
        private static RegistrationViewModel ToViewModel(Registration e) => new()
        {
            Id = e.Id,
            FullName = e.FullName,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber,
            Address = e.Address,
            DateOfBirth = e.DateOfBirth,
            Gender = e.Gender,
            Course = e.Course,
            UserName = e.UserName,
            Password = e.Password,
            Role = e.Role,
            ConfirmPassword = string.Empty
        };
    }
}