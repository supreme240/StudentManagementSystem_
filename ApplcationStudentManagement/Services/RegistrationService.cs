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

        public async Task<(bool success, string error)> AddRegistrationAsync(Registration registration)
        {
            try
            {
                if (registration == null)
                    throw new ArgumentNullException(nameof(registration));

                await _repository.AddAsync(registration);
                await _repository.SaveChangesAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
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

            AddRegistrationAsync(registration).Wait();
        }

        public Task<string?> GetALLRegistrationsInformationAsync()
        {
            throw new NotImplementedException();
        }
    }
}