using Microsoft.AspNetCore.Mvc;
using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Student_management_system.Controllers
{
    public class StudentController : Controller
    {
        private readonly IRegistrationService _registrationService;

        public StudentController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }
        public async Task<IActionResult> GetRegistrationInformation()
        {
            var registration = await _registrationService.GetALLRegistrationsInformationAsync();
            return View(registration);
        }

        // GET: Student/Index
        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            var registrations = _registrationService.GetAllRegistrations();
            return View(registrations);
        }

        // GET: Student/Details/5
        public IActionResult Details(int id)
        {
            _ = id; // mark as used until you implement lookup by id
            var registration = _registrationService.GetRegistrationInformation(); // or create GetById later
            return View(registration);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Registration registration)
        {
            if (ModelState.IsValid)
            {
                _registrationService.AddRegistration(registration);   // ← Fixed
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
            return View(registration);   // This should open Edit.cshtml
        }

        // POST: Save Edited Data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Registration registration)
        {
            if (ModelState.IsValid)
            {
                await _registrationService.UpdateRegistrationAsync(registration);
                TempData["Success"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(registration);   // Return to Edit form if validation fails
        }


        // ✅ Keep Only This (POST Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _registrationService.DeleteRegistrationAsync(id);
            TempData["Success"] = "Registration deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}