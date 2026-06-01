using Microsoft.AspNetCore.Mvc;
using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;

namespace Student_management_system.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        // GET: Student/Index
        public async Task<IActionResult> Index()
        {
            var registrations = await _registrationService.GetAllRegistrationsAsync();
            return View(registrations);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Registration registration)
        {
            if (ModelState.IsValid)
            {
                await _registrationService.AddRegistrationAsync(registration);   
                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Index");
            }

            return View(registration);
        }

        // GET: Student/Edit/5
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

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _registrationService.DeleteRegistrationAsync(id);
            TempData["Success"] = "Registration deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}