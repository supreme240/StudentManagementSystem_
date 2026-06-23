using ApplicationStudentManagement.DTOs;
using ApplicationStudentManagement.Interfaces;
using ApplicationStudentManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Student_management_system.Models;
using StudentManagement.domain.Domain;

namespace Student_management_system.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;

        //lazy initialization
        public RegistrationController(IRegistrationService registrationService, IRoleService roleService)
        {
            _registrationService = registrationService;
            _roleService = roleService;
        }

        private async Task LoadRoles() { 
            var roles = await _roleService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles ?? new List<RoleViewModel>(), "EachRole", "EachRole");
        }

        // GET: Registration/Index
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var registrations = await _registrationService.GetAllRegistrationsAsync();

            var vm = new RegistrationViewModel();
            return View(vm);
        }

        // GET: Registration/Create
        public async Task<IActionResult> Create()
        {
            await LoadRoles();
            return View();
        }

        // POST: Registration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationViewModel registrationViewModel)
        {
            if (registrationViewModel.Password != registrationViewModel.ConfirmPassword) {
                ModelState.AddModelError("ConfirmPassword", "Password doesn't match!");
                await LoadRoles();
                return View(registrationViewModel);
            }
            if (ModelState.IsValid)
            {
                await _registrationService.AddRegistrationAsync(registrationViewModel);

                TempData["SuccessMessage"] = "Registrtaion Successfull!";
                return RedirectToAction("Index","Login");
            }
            await LoadRoles();
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
        public async Task<IActionResult> Edit(int id, RegistrationViewModel registrationViewModel)
        {
            if (id != registrationViewModel.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _registrationService.UpdateRegistrationAsync(registrationViewModel);
                TempData["Success"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(registrationViewModel);   
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