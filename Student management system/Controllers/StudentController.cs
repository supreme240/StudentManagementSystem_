// ============================================================
// WHAT IS THIS FILE?
// The controller receives HTTP requests and sends back responses.
//
// KEY POINT FOR THIS FILE:
// There is NO "using StudentManagement.domain.Domain" here.
// The controller has absolutely no knowledge of the Registration
// domain model. It only knows about RegistrationViewModel (DTO).
//
// The flow for every action is:
//   GET  actions: call service → get DTO → pass to View
//   POST actions: receive DTO from form → call service → redirect or show errors
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationStudentManagement.Interfaces;  // service interface
using ApplicationStudentManagement.DTO;         // only the DTO — NO domain model

namespace Student_management_system.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        // _registrationService  → handles all student CRUD via DTO
        // _rolesService         → provides the roles dropdown list
        private readonly IRegistrationService _registrationService;
        private readonly IRolesService _rolesService;

        // Constructor: ASP.NET automatically provides these services
        public StudentController(
            IRegistrationService registrationService,
            IRolesService rolesService)
        {
            _registrationService = registrationService;
            _rolesService = rolesService;
        }

        // ============================================================
        // INDEX — show all registrations (Admin only)
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Service returns List<RegistrationViewModel>
                // No domain model involved here at all
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
                // finally always runs — guarantees the page title is set
                ViewBag.Title = "Student Registrations";
            }
        }

        // ============================================================
        // DETAILS — show one registration (Admin only)
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Service returns RegistrationViewModel? (null if not found)
                var viewModel = await _registrationService.GetRegistrationByIdAsync(id);

                // If null, throw so the catch block returns a clean 404
                if (viewModel == null)
                    throw new KeyNotFoundException($"No registration found with id {id}.");

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                // Clean 404 page — not a crash
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

        // ============================================================
        // GET CREATE — show the empty registration form
        // ============================================================
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Create()
        {
            // Load the roles dropdown before showing the form
            await LoadRolesIntoViewBag();

            // Pass an empty DTO to the view so asp-for tag helpers work
            return View(new RegistrationViewModel());
        }

        // ============================================================
        // POST CREATE — receive the filled form and save it
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Create(RegistrationViewModel viewModel)
        {
            // If validation annotations on RegistrationViewModel fail
            // (e.g. Required, EmailAddress) return the form with errors shown
            if (!ModelState.IsValid)
            {
                await LoadRolesIntoViewBag();
                return View(viewModel);
            }

            try
            {
                // Pass the DTO straight to the service — no conversion here
                var (success, error) = await _registrationService.AddRegistrationAsync(viewModel);

                if (!success)
                {
                    // Service returned a business rule failure (e.g. duplicate email)
                    // Show the error message on the form
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
                // Reload the roles dropdown so the form always has it
                await LoadRolesIntoViewBag();
            }
        }

        // ============================================================
        // GET EDIT — load existing data into the edit form
        // ============================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Service returns the existing record as a DTO
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

        // ============================================================
        // POST EDIT — receive updated form and save changes
        // ============================================================
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
                // Pass the updated DTO to the service
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

        // ============================================================
        // POST DELETE — remove a registration
        // ============================================================
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

            // Always go back to the list whether it succeeded or failed
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // PRIVATE HELPER — reused by Create and Edit actions
        // Loads roles into ViewBag so the dropdown always works
        // ============================================================
        private async Task LoadRolesIntoViewBag()
        {
            try
            {
                var roles = await _rolesService.GetAllRolesAsync();
                ViewBag.Roles = new SelectList(roles, "RoleName", "RoleName");
            }
            catch (Exception)
            {
                // If roles fail to load, give an empty dropdown
                // rather than crashing the whole page
                ViewBag.Roles = new SelectList(Enumerable.Empty<string>());
            }
        }
    }
}