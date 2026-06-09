using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStudentManagement.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _repository;

        public RegistrationService(IRegistrationRepository repository)
        {
            _repository = repository;
        }

        //
        public async Task<(bool success, string? error)> AddRegistrationAsync(Registration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            
            var all = await _repository.GetAllAsync();

            bool emailExists = all.Any(r => r.Email == registration.Email);
            bool phoneExists = all.Any(r => r.PhoneNumber == registration.PhoneNumber);
            bool usernameExists = all.Any(r => r.UserName == registration.UserName);

            if (emailExists)
                return (false, "This email is already registered.");

            if (phoneExists)
                return (false, "This phone number is already registered.");

            if (usernameExists)
                return (false, "This username is already taken.");

            await _repository.AddAsync(registration);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<Registration>> GetAllRegistrationsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Registration?> GetRegistrationByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task UpdateRegistrationAsync(Registration registration)
        {
            await _repository.UpdateAsync(registration);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteRegistrationAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }



        public Registration GetRegistrationInformation()
        {
            return GetAllRegistrationsAsync().Result.FirstOrDefault() ?? new Registration();
        }

        public List<Registration> GetAllRegistrations()
        {
            return GetAllRegistrationsAsync().Result;
        }

        public void AddRegistration(Registration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var result = AddRegistrationAsync(registration).Result;
            if (!result.success)
                throw new InvalidOperationException(result.error);
        }

        public Task<string?> GetALLRegistrationsInformationAsync()
        {
            throw new NotImplementedException();
        }
    }
}