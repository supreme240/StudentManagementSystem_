using ApplicationStudentManagement.Interfaces;
using StudentManagementSystem.Infrastructure.DapperRepositories;
using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Services
{
    public class DapperRegistrationService : IDapperRegistrationService
    {
        // Injected via constructor — depends on the interface, not the
        // concrete DapperRegistrationRepository class
        private readonly IDapperRegistrationRepository _repository;

        public DapperRegistrationService(IDapperRegistrationRepository repository)
        {
            _repository = repository;
        }

        // Simply passes the call through to the repository
        public Task<Registration?> GetByIdAsync(int id)
            => _repository.GetByIdAsync(id);

        // Same here — no extra logic, just delegation
        public Task<IEnumerable<Registration>> GetAllAsync()
            => _repository.GetAllAsync();

        public Task<(bool success, string error)> AddRegistrationAsync(Registration registration)
            => _repository.AddRegistrationAsync(registration);
    }
}