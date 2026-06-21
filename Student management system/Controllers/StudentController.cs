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

        // CHANGED: Added Dapper service for read operations (Index, Details)
        // Reads are now handled by Dapper (raw SQL) instead of EF Core
        private readonly IDapperRegistrationService _dapperRegistrationService;

        // CHANGED: Constructor now accepts IDapperRegistrationService as a third parameter
        // IRegistrationService and IRolesService are unchanged
        public StudentController(
            IRegistrationService registrationService,   // unchanged — handles Create, Edit, Delete
            IRolesService rolesService,                 // unchanged — handles role dropdown
            IDapperRegistrationService dapperRegistrationService) // NEW — handles Index, Details
        {
            _registrationService = registrationService;
            _rolesService = rolesService;
            _dapperRegistrationService = dapperRegistrationService; // NEW
        }

        // GET: Student/Index — Admin only
        // CHANGED: Was _registrationService.GetAllRegistrationsAsync() (EF Core)
        // Now uses _dapperRegistrationService.GetAllAsync() (Dapper/raw SQL)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var registrations = await _dapperRegistrationService.GetAllAsync();
            return View(registrations);
        }

        // GET: Student/Details/5 — Admin only
        // CHANGED: Was _registrationService.GetRegistrationByIdAsync(id) (EF Core)
        // Now uses _dapperRegistrationService.GetByIdAsync(id) (Dapper/raw SQL)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var registration = await _dapperRegistrationService.GetByIdAsync(id);
            if (registration == null)
                return NotFound();
            return View(registration);
        }

        // GET: Student/Create — Admin & Student
        // UNCHANGED
        [Authorize(Roles = "Admin,student")]
        public async Task<IActionResult> Create()
        {
            var roles = await _rolesService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "RoleName", "RoleName");
            return View();
        }

        // POST: Student/Create — Admin & Student
        // UNCHANGED — writes still go through EF Core (IRegistrationService)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,student")]
        public async Task<IActionResult> Create(Registration registration)
        {
            if (ModelState.IsValid)
            {
                var (success, error) = await _registrationService.AddRegistrationAsync(registration);

                if (!success)
                {
                    TempData["Error"] = error;
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
        // UNCHANGED — still uses EF Core to fetch before editing
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var registration = await _registrationService.GetRegistrationByIdAsync(id);
            if (registration == null)
                return NotFound();

            var roles = await _rolesService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Role", "Role", registration.Role);
            return View(registration);
        }

        // POST: Student/Edit — Admin only
        // UNCHANGED — writes still go through EF Core
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

            var roles = await _rolesService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Role", "Role", registration.Role);
            return View(registration);
        }

        // POST: Student/Delete — Admin only
        // UNCHANGED — deletes still go through EF Core
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