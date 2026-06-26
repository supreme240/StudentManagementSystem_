using ApplicationStudentManagement.DTO;
using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Student_management_system.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly IRolesService _rolesService;

        public StudentController(
            IRegistrationService registrationService,
            IRolesService rolesService)
        {
            _registrationService = registrationService;
            _rolesService = rolesService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModels = await _registrationService.GetAllRegistrationsAsync();
                return View(viewModels);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load the student list. Please try again later.";
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Student Registrations";
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var viewModel = await _registrationService.GetRegistrationByIdAsync(id);

                if (viewModel == null)
                    throw new KeyNotFoundException($"No registration found with id {id}.");

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load this student's details. Please try again later.";
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Registration Details";
            }
        }

        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Create()
        {
            await LoadRolesIntoViewBag();
            return View(new RegistrationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Create(RegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesIntoViewBag();
                return View(viewModel);
            }

            try
            {
                var (success, error) = await _registrationService.AddRegistrationAsync(viewModel);

                if (!success)
                {
                    TempData["Error"] = error;
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return View(viewModel);
            }
            finally
            {
                await LoadRolesIntoViewBag();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var viewModel = await _registrationService.GetRegistrationByIdAsync(id);

                if (viewModel == null)
                    throw new KeyNotFoundException($"No registration found with id {id}.");

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load this registration for editing. Please try again later.";
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Edit Registration";
                await LoadRolesIntoViewBag();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(RegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesIntoViewBag();
                return View(viewModel);
            }

            try
            {
                var (success, error) = await _registrationService.UpdateRegistrationAsync(viewModel);

                if (!success)
                {
                    TempData["Error"] = error;
                    return View(viewModel);
                }

                TempData["Success"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while updating. Please try again.";
                return View(viewModel);
            }
            finally
            {
                await LoadRolesIntoViewBag();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _registrationService.DeleteRegistrationAsync(id);
                TempData["Success"] = "Registration deleted successfully!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to delete this registration. Please try again later.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRolesIntoViewBag()
        {
            try
            {
                var roles = await _rolesService.GetAllRolesAsync();
                ViewBag.Roles = new SelectList(roles, "RoleName", "RoleName");
            }
            catch (Exception)
            {
                ViewBag.Roles = new SelectList(Enumerable.Empty<string>());
            }
        }
    }
}