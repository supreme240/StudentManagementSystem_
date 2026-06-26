using ApplicationStudentManagement.Interfaces;
using ApplicationStudentManagement.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationStudentManagement.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _service;

        public RegistrationController(IRegistrationService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var viewModels = await _service.GetAllRegistrationsAsync();
            return View(viewModels);
        }

        public IActionResult Create()
        {
            return View(new RegistrationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var (success, error) = await _service.AddRegistrationAsync(vm);

            if (!success)
            {
                TempData["Error"] = error;
                return View(vm);
            }

            TempData["Success"] = "Registration added successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetRegistrationByIdAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegistrationViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var (success, error) = await _service.UpdateRegistrationAsync(vm);

            if (!success)
            {
                TempData["Error"] = error;
                return View(vm);
            }

            TempData["Success"] = "Registration updated successfully!";
            return RedirectToAction(nameof(Index));
        }

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