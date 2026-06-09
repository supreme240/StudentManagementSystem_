using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationStudentManagement.Interfaces;
using StudentManagement.domain.Domain;

namespace Student_management_system.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly IRolesService _rolesService;

        public StudentController(IRegistrationService registrationService, IRolesService rolesService)
        {
            _registrationService = registrationService;
            _rolesService = rolesService;
        }

        // GET: Student/Index — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var registrations = await _registrationService.GetAllRegistrationsAsync();
            return View(registrations);
        }

        // GET: Student/Details/5 — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var registration = await _registrationService.GetRegistrationByIdAsync(id);
            if (registration == null)
                return NotFound();
            return View(registration);
        }

        // GET: Student/Create — Admin & Student
        [Authorize(Roles = "Admin,student")]
        public async Task<IActionResult> Create()
        {
            var roles = await _rolesService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "RoleName", "RoleName");
            return View();
        }

        // POST: Student/Create — Admin & Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,student")]
        //
        public async Task<IActionResult> Create(Registration registration)
        {
            if (ModelState.IsValid)
            {
                var (success, error) = await _registrationService.AddRegistrationAsync(registration);

                if (!success)
                {
                    TempData["Error"] = error;  // shown via existing alert in your view
                    var roleList = await _rolesService.GetAllRolesAsync();
                    ViewBag.Roles = new SelectList(roleList, "RoleName", "RoleName");
                    return View(registration);
                }

                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Create");
            }

            var roles = await _rolesService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "RoleName", "RoleName");
            return View(registration);
        }

        // GET: Student/Edit/5 — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var registration = await _registrationService.GetRegistrationByIdAsync(id);
            if (registration == null)
                return NotFound();

            // Preselect current role
            var roles = await _rolesService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Role", "Role", registration.Role);
            return View(registration);
        }

        // POST: Student/Edit — Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Registration registration)
        {
            if (ModelState.IsValid)
            {
                await _registrationService.UpdateRegistrationAsync(registration);
                TempData["Success"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Refetch roles if validation fails
            var roles = await _rolesService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Role", "Role", registration.Role);
            return View(registration);
        }

        // POST: Student/Delete — Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _registrationService.DeleteRegistrationAsync(id);
            TempData["Success"] = "Registration deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}