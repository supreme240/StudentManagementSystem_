using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStudentManagement.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;

        public RegistrationService(IRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task AddRegistrationAsync(Registration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            await _registrationRepository.AddAsync(registration);
            await _registrationRepository.SaveChangesAsync();
        }

        public async Task<List<Registration>> GetAllRegistrationsAsync()
        {
            return await _registrationRepository.GetAllAsync();
        }

        public async Task<Registration?> GetRegistrationByIdAsync(int id)
        {
            return await _registrationRepository.GetByIdAsync(id);
        }

        public async Task UpdateRegistrationAsync(Registration registration)
        {
            _registrationRepository.Update(registration);
            await _registrationRepository.SaveChangesAsync();
        }

        public async Task DeleteRegistrationAsync(int id)
        {
            await _registrationRepository.DeleteAsync(id);
            await _registrationRepository.SaveChangesAsync();
        }

        public async Task<List<Registration>> GetByStudentCourseAsync(string studentCourse)
        {
            return await _registrationRepository.GetByStudentCourseAsync(studentCourse);
        }
    }
}
