using ApplicationStudentManagement.DTOs;
using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.domain.Domain;
using System.Threading.Tasks;

namespace Student_management_system.Controllers {
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller {
        private readonly IRoleService _roleService;
            
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        // GET: RoleController
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }

        // GET: RoleController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: RoleController/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: RoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid) { 
                await _roleService.AddRolesAsync(roleViewModel);
            }

            TempData["SuccessMessage"] = "Role added successfully!";
            return RedirectToAction("Index", "Role");
        }

        // GET: RoleController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleService.GetRolesByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: RoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleViewModel roleViewModel)
        {
            if(id != roleViewModel.Id) 
                return BadRequest();

            if (ModelState.IsValid) { 
                await _roleService.UpdateRolesAsync(roleViewModel);
                TempData["Success"] = "Role updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(roleViewModel);
        }

        // GET: RoleController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleService.GetRolesByIdAsync(id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        // POST: RoleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roleService.DeleteRolesAsync(id);
            TempData["Success"] = "Role deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
