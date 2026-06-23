using ApplicationStudentManagement.DTOs;
using ApplicationStudentManagement.Interfaces;
using Microsoft.EntityFrameworkCore.Query.Internal;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo;

namespace ApplicationStudentManagement.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;

        public RegistrationService(IRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task AddRegistrationAsync(RegistrationViewModel registrationViewModel)
        {
            try {
                if (registrationViewModel == null)
                    throw new ArgumentNullException(nameof(registrationViewModel));

                var reg = new Registration();
                reg.FullName = registrationViewModel.FullName;
                reg.DateOfBirth = registrationViewModel.DateOfBirth;
                reg.Gender = registrationViewModel.Gender;
                reg.Address = registrationViewModel.Address;
                reg.Course = registrationViewModel.Course;
                reg.Email = registrationViewModel.Email;
                reg.PhoneNumber = registrationViewModel.PhoneNumber;
                reg.Password = registrationViewModel.Password;
                reg.Role = registrationViewModel.Role;

                await _registrationRepository.AddAsync(reg);
                await _registrationRepository.SaveChangesAsync();
            }
            catch (Exception ex) {
                throw new Exception("An error occurred while adding registration.", ex);
            }
        }

        public async Task<List<RegistrationViewModel>> GetAllRegistrationsAsync()
        {
            try
            {
                var registration = await _registrationRepository.GetAllAsync();

                return registration.Select(r => new RegistrationViewModel
                {
                    FullName = r.FullName,
                    DateOfBirth = r.DateOfBirth,
                    Gender = r.Gender,
                    Address = r.Address,
                    Course = r.Course,
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    Role = r.Role
                }).ToList();
            }
            catch (Exception ex) {
                throw new Exception("An error occured while fetching all registrations.", ex);
            }
        }

        public async Task<RegistrationViewModel> GetRegistrationByIdAsync(int id)
        {
            try
            {
                var registration = await _registrationRepository.GetByIdAsync(id);

                if (registration == null) return null;

                return new RegistrationViewModel
                {
                    FullName = registration.FullName,
                    DateOfBirth = registration.DateOfBirth,
                    Gender = registration.Gender,
                    Address = registration.Address,
                    Course = registration.Course,
                    Email = registration.Email,
                    PhoneNumber = registration.PhoneNumber,
                    Role = registration.Role
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while fetching registration with ID {id}.", ex);
            }
        }

        public async Task UpdateRegistrationAsync(RegistrationViewModel registrationViewModel)
        {
            try
            {
                var reg = await _registrationRepository.GetByIdAsync(registrationViewModel.Id);
                if (reg == null) return;

                reg.FullName = registrationViewModel.FullName;
                reg.DateOfBirth = registrationViewModel.DateOfBirth;
                reg.Gender = registrationViewModel.Gender;
                reg.Address = registrationViewModel.Address;
                reg.Course = registrationViewModel.Course;
                reg.Email = registrationViewModel.Email;
                reg.PhoneNumber = registrationViewModel.PhoneNumber;
                reg.Password = registrationViewModel.Password;
                //reg.Password = BCrypt.Net.BCrypt.HashPassword(registrationViewModel.Password;
                reg.Role = registrationViewModel.Role;

                _registrationRepository.Update(reg);
                await _registrationRepository.SaveChangesAsync();
            }
            catch (Exception ex) {
                throw new Exception("An error occured while updating registration.", ex);
            }
        }

        public async Task DeleteRegistrationAsync(int id)
        {
            try
            {
                await _registrationRepository.DeleteAsync(id);
                await _registrationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while deleting registration with ID {id}.", ex);
            }
        }

        public async Task<List<RegistrationViewModel>> GetByStudentCourseAsync(string studentCourse)
        {
            try
            {
                var registration =  await _registrationRepository.GetByStudentCourseAsync(studentCourse);

                return registration.Select(r => new RegistrationViewModel
                { 
                    FullName = r.FullName,
                    DateOfBirth = r.DateOfBirth,
                    Gender = r.Gender,
                    Address = r.Address,
                    Course = r.Course,
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    Role = r.Role
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while fetching registrations for course '{studentCourse}'.", ex);
            }
        }

        public async Task<RegistrationViewModel> CheckAUthenticationAsync(string email, string password)
        {
            try
            {
                var registrtaion = await _registrationRepository.CheckAuthenticationAsync(email, password);

                if (registrtaion == null) return null;

                return new RegistrationViewModel { 
                    FullName = registrtaion.FullName,
                    DateOfBirth = registrtaion.DateOfBirth,
                    Gender = registrtaion.Gender,
                    Address = registrtaion.Address,
                    Email = registrtaion.Email,
                    PhoneNumber = registrtaion.PhoneNumber,
                    Role = registrtaion.Role
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while checking authentication.", ex);
            }
        }

        public async Task<RegistrationViewModel> FindByEmailAndPhoneAsync(string email, string phone)
        {
            try
            {
                var registrtaion = await _registrationRepository.FindByEmailAndPhoneAsync(email, phone);

                if (registrtaion == null) return null;

                return new RegistrationViewModel
                {
                    FullName = registrtaion.FullName,
                    DateOfBirth = registrtaion.DateOfBirth,
                    Gender = registrtaion.Gender,
                    Address = registrtaion.Address,
                    Email = registrtaion.Email,
                    PhoneNumber = registrtaion.PhoneNumber,
                    Role = registrtaion.Role
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while finding registration by email and phone.", ex);
            }
        }

        public async Task ResetPasswordAsync(int userId, string newPassword) {
            try
            {
                var user = await _registrationRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found.");

                user.Password = newPassword;
                _registrationRepository.Update(user);
                await _registrationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while resetting password.", ex);
            }
        }
    }
}
