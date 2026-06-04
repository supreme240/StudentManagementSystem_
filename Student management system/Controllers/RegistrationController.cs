using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _service;

        public RegistrationController(IRegistrationService service)
        {
            _service = service;
        }

        // List All Registrations
        public async Task<IActionResult> Index()
        {
            var registrations = await _service.GetAllRegistrationsAsync();
            return View(registrations);
        }

        // Create Form
        public IActionResult Create()
        {
            return View();
        }

        // Save New Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Registration registration)
        {
            if (!ModelState.IsValid)
                return View(registration);

            // Extra safety check
            if (registration.Role != "Student" && registration.Role != "Admin")
            {
                ModelState.AddModelError("Role", "Invalid role selected.");
                return View(registration);
            }

            await _service.AddRegistrationAsync(registration);
            TempData["Success"] = "Registration added successfully!";

            return RedirectToAction(nameof(Index));
        }
        // Edit Form
        public async Task<IActionResult> Edit(int id)
        {
            var reg = await _service.GetRegistrationByIdAsync(id);
            if (reg == null) return NotFound();
            return View(reg);
        }

        // Update Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Registration registration)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateRegistrationAsync(registration);
                TempData["Success"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(registration);
        }

        // Delete Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteRegistrationAsync(id);
            TempData["Success"] = "Registration deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}