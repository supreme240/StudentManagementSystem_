// ============================================================
// WHAT IS THIS FILE?
// This is the SERVICE — it contains all the business logic.
//
// THE GOLDEN RULE OF THIS FILE:
// The domain model (Registration) is used ONLY inside here.
// It never goes up to the controller.
//
// HOW IT WORKS:
//   - When data comes IN  (Add, Update): DTO → domain model (ToEntity)
//   - When data goes OUT (Get):          domain model → DTO  (ToViewModel)
//
// These two private methods (ToEntity, ToViewModel) at the
// bottom of this file do all the conversion work.
// ============================================================

using ApplicationStudentManagement.DTO;           // your DTO lives here
using ApplicationStudentManagement.Interfaces;    // the interface this class implements
using StudentManagement.domain.Domain;            // Registration domain model (used ONLY here)
using StudentManagementSystem.Infrastructure.Repositories; // repository interface

namespace ApplicationStudentManagement.Services
{
    public class RegistrationService : IRegistrationService
    {
        // The repository talks to the database using EF Core.
        // It works with Registration (domain model) internally.
        private readonly IRegistrationRepository _repository;

        // Constructor: ASP.NET injects the repository automatically
        public RegistrationService(IRegistrationRepository repository)
        {
            _repository = repository;
        }

        // ============================================================
        // CREATE
        // ============================================================
        public async Task<(bool success, string? error)> AddRegistrationAsync(
            RegistrationViewModel viewModel)
        {
            // Safety check — should never be null but good practice
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            // Load all existing records to check for duplicates
            // These are domain models but they stay inside this method
            var allEntities = await _repository.GetAllAsync();

            // Check: is this email already registered?
            if (allEntities.Any(r => r.Email == viewModel.Email))
                return (false, "This email is already registered.");

            // Check: is this phone number already registered?
            if (allEntities.Any(r => r.PhoneNumber == viewModel.PhoneNumber))
                return (false, "This phone number is already registered.");

            // Check: is this username already taken?
            if (allEntities.Any(r => r.UserName == viewModel.UserName))
                return (false, "This username is already taken.");

            // Convert DTO → domain model so the repository can save it
            // ToEntity() is a private method at the bottom of this class
            var entity = ToEntity(viewModel);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            // Return success
            return (true, null);
        }

        // ============================================================
        // READ ALL
        // ============================================================
        public async Task<List<RegistrationViewModel>> GetAllRegistrationsAsync()
        {
            // Get all Registration entities from the database
            var entities = await _repository.GetAllAsync();

            // Convert each domain model → DTO before returning
            // The controller will receive List<RegistrationViewModel>
            // and will never see a Registration object
            return entities.Select(entity => ToViewModel(entity)).ToList();
        }

        // ============================================================
        // READ ONE
        // ============================================================
        public async Task<RegistrationViewModel?> GetRegistrationByIdAsync(int id)
        {
            // Get a single Registration entity from the database
            var entity = await _repository.GetByIdAsync(id);

            // If not found, return null (controller will handle the 404)
            if (entity == null)
                return null;

            // Convert domain model → DTO before returning
            return ToViewModel(entity);
        }

        // ============================================================
        // UPDATE
        // ============================================================
        public async Task<(bool success, string? error)> UpdateRegistrationAsync(
            RegistrationViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            // First check if the record we want to update still exists
            var existing = await _repository.GetByIdAsync(viewModel.Id);
            if (existing == null)
                return (false, "Registration not found.");

            // Duplicate checks — but EXCLUDE the current record (its own id)
            // because it's fine for a record to keep its own email/phone/username
            var allEntities = await _repository.GetAllAsync();

            if (allEntities.Any(r => r.Email == viewModel.Email && r.Id != viewModel.Id))
                return (false, "This email is already used by another registration.");

            if (allEntities.Any(r => r.PhoneNumber == viewModel.PhoneNumber && r.Id != viewModel.Id))
                return (false, "This phone number is already used by another registration.");

            if (allEntities.Any(r => r.UserName == viewModel.UserName && r.Id != viewModel.Id))
                return (false, "This username is already taken by another registration.");

            // Apply the new values from the DTO onto the existing entity.
            // We update the EXISTING entity (not create a new one) because
            // EF Core is already tracking it — this prevents concurrency issues.
            existing.FullName = viewModel.FullName;
            existing.Email = viewModel.Email;
            existing.PhoneNumber = viewModel.PhoneNumber;
            existing.Address = viewModel.Address;
            existing.DateOfBirth = viewModel.DateOfBirth;
            existing.Gender = viewModel.Gender;
            existing.Course = viewModel.Course;
            existing.UserName = viewModel.UserName;
            existing.Password = viewModel.Password;
            existing.Role = viewModel.Role;
            // Note: ConfirmPassword is NOT copied — it's a form-only field

            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        // ============================================================
        // DELETE
        // ============================================================
        public async Task DeleteRegistrationAsync(int id)
        {
            // Repository handles finding and removing the entity
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }

        // ============================================================
        // PRIVATE CONVERTERS
        // These two methods are the ONLY place where conversion happens.
        // They are private — nothing outside this class can call them.
        // ============================================================

        // Converts DTO → domain model (used when SAVING to database)
        private static Registration ToEntity(RegistrationViewModel vm)
        {
            return new Registration
            {
                Id = vm.Id,
                FullName = vm.FullName,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Address = vm.Address,
                DateOfBirth = vm.DateOfBirth,
                Gender = vm.Gender,
                Course = vm.Course,
                UserName = vm.UserName,
                Password = vm.Password,
                Role = vm.Role
                // ConfirmPassword is NOT copied — not a database column
            };
        }

        // Converts domain model → DTO (used when READING from database)
        private static RegistrationViewModel ToViewModel(Registration entity)
        {
            return new RegistrationViewModel
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                Course = entity.Course,
                UserName = entity.UserName,
                Password = entity.Password,
                Role = entity.Role,
                // ConfirmPassword is left empty on purpose —
                // we never read it back from the database
                ConfirmPassword = string.Empty
            };
        }
    }
}