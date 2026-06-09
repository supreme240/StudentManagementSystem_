using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.domain.Domain;

namespace Student_management_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        // GET: Roles/Index
        public async Task<IActionResult> Index()
        {
            var roles = await _rolesService.GetAllRolesAsync();
            return View(roles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            var role = new Roles();
            role.RoleName = "Admin";
            return View(role);
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Roles role)
        {
            if (ModelState.IsValid)
            {
                var (success, error) = await _rolesService.AddRoleAsync(role);

                if (!success)
                {
                    TempData["Error"] = error;
                    return View(role);
                }

                TempData["Success"] = "Role added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _rolesService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();
            return View(role);
        }

        // POST: Roles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Roles role)
        {
            if (ModelState.IsValid)
            {
                await _rolesService.UpdateRoleAsync(role);
                TempData["Success"] = "Role updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // POST: Roles/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _rolesService.DeleteRoleAsync(id);
            TempData["Success"] = "Role deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}