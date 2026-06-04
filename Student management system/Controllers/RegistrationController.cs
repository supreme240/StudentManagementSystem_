using Microsoft.AspNetCore.Mvc;
using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using Student_management_system.Models;
using Microsoft.AspNetCore.Authorization;

namespace Student_management_system.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _registrationService;

        //lazy initialization
        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        // GET: Registration/Index
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var registrations = await _registrationService.GetAllRegistrationsAsync();

            var vm = registrations.Select(r => new RegistrationViewModel {
                Id = r.Id,
                FullName = r.FullName,
                Address = r.Address,
                PhoneNumber = r.PhoneNumber,
                Gender = r.Gender,
                Course = r.Course,
                DateOfBirth = r.DateOfBirth
            });
            return View(vm);
        }

        // GET: Registration/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Registration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationViewModel registrationViewModel)
        {
            if (registrationViewModel.Password != registrationViewModel.ConfirmPassword) {
                ModelState.AddModelError("ConfirmPassword", "Password doesn't match!");
                return View(registrationViewModel);
            }
            if (ModelState.IsValid)
            {
                var registration = new Registration { 
                    FullName = registrationViewModel.FullName,
                    Email = registrationViewModel.Email,
                    PhoneNumber = registrationViewModel.PhoneNumber,
                    Address = registrationViewModel.Address,
                    DateOfBirth = registrationViewModel.DateOfBirth,
                    Gender = registrationViewModel.Gender,
                    Course = registrationViewModel.Course,
                    Password = registrationViewModel.Password,
                    Role = registrationViewModel.Role
                };  
                await _registrationService.AddRegistrationAsync(registration);

                TempData["SuccessMessage"] = "Registrtaion Successfull!";
                return RedirectToAction("Index","Login");
            }
            return View(registrationViewModel);
        }

        // GET: Registration/Edit/5
        // GET: Open Edit Form
        public async Task<IActionResult> Edit(int id)
        {
            var registration = await _registrationService.GetRegistrationByIdAsync(id);
            if (registration == null)
            {
                return NotFound();
            }
            return View(registration);   
        }

        // POST: Save Edited Data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Registration registration)
        {
            if (id != registration.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _registrationService.UpdateRegistrationAsync(registration);
                TempData["Success"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(registration);   
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var registration = await _registrationService.GetRegistrationByIdAsync(id);
            if (registration == null)
                return NotFound();

            return View(registration);
        }

        // POST: Registration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _registrationService.DeleteRegistrationAsync(id);
            TempData["Success"] = "Registration deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout() {
            return RedirectToAction("Create");
        }
    }
}