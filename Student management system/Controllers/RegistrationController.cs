// ============================================================
// CHANGES MADE TO THIS FILE:
//
// BEFORE:
//   using StudentManagement.domain.Domain;  - domain model exposed
//   Create(Registration registration)       - domain model in parameter
//   Edit(Registration registration)         - domain model in parameter
//   View(registrations)                     - domain model list sent to view
//
// AFTER:
//   using ApplicationStudentManagement.DTO  - only DTO used
//   Create(RegistrationViewModel vm)        - DTO in parameter
//   Edit(RegistrationViewModel vm)          - DTO in parameter
//   View(viewModels)                        - DTO list sent to view
//
// The controller no longer knows Registration exists at all.
// ============================================================

using ApplicationStudentManagement.Interfaces;
using ApplicationStudentManagement.DTO;          // DTO namespace
using Microsoft.AspNetCore.Mvc;
//  REMOVED: using StudentManagement.domain.Domain;
//    This line is gone. Controller has zero knowledge of domain model.

namespace ApplicationStudentManagement.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _service;

        public RegistrationController(IRegistrationService service)
        {
            _service = service;
        }

        // ============================================================
        // INDEX — show all registrations
        // ============================================================
        // BEFORE: returned List<Registration> (domain model) to view
        // AFTER:  returns List<RegistrationViewModel> (DTO) to view
        // The service does the conversion internally — controller does nothing extra
        // ============================================================
        public async Task<IActionResult> Index()
        {
            // _service.GetAllRegistrationsAsync() now returns List<RegistrationViewModel>
            // because we updated the interface in the previous steps
            var viewModels = await _service.GetAllRegistrationsAsync();

            // Passing DTO list to the view — domain model never touched here
            return View(viewModels);
        }

        // ============================================================
        // GET CREATE — show empty form
        // ============================================================
        // BEFORE: return View() — no model passed, view had no @model type
        // AFTER:  pass empty RegistrationViewModel() so asp-for works properly
        // ============================================================
        public IActionResult Create()
        {
            // Pass an empty DTO so the view's asp-for tag helpers
            // have a model to bind against
            return View(new RegistrationViewModel());
        }

        // ============================================================
        // POST CREATE — save new registration
        // ============================================================
        // BEFORE: Create(Registration registration) — domain model came from form
        //         await _service.AddRegistrationAsync(registration) — old signature
        //
        // AFTER:  Create(RegistrationViewModel vm) — DTO comes from form
        //         service returns (bool success, string? error) — we handle failures
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationViewModel vm)
        {
            // Check if all [Required], [EmailAddress] etc. annotations passed
            if (!ModelState.IsValid)
            {
                // Return the same DTO back so the form keeps the user's input
                return View(vm);
            }

            // Pass DTO to service — service converts it to domain model internally
            var (success, error) = await _service.AddRegistrationAsync(vm);

            if (!success)
            {
                // Business rule failed (e.g. duplicate email, duplicate username)
                // Show the error message on the form without losing the user's input
                TempData["Error"] = error;
                return View(vm);
            }

            TempData["Success"] = "Registration added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // GET EDIT — load existing record into edit form
        // ============================================================
        // BEFORE: returned Registration (domain model) directly to view
        // AFTER:  service returns RegistrationViewModel (DTO) — no domain model here
        // ============================================================
        public async Task<IActionResult> Edit(int id)
        {
            // Service fetches the Registration from DB and converts it to DTO
            // Controller only receives RegistrationViewModel
            var vm = await _service.GetRegistrationByIdAsync(id);

            if (vm == null)
                return NotFound();

            // Pass DTO to the edit view
            return View(vm);
        }

        // ============================================================
        // POST EDIT — save changes to existing registration
        // ============================================================
        // BEFORE: Edit(Registration registration) — domain model from form
        //         await _service.UpdateRegistrationAsync(registration) — no error handling
        //
        // AFTER:  Edit(RegistrationViewModel vm) — DTO from form
        //         service returns (bool, string?) — errors shown on form
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegistrationViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Return DTO back to form — user sees their input + validation errors
                return View(vm);
            }

            // vm.Id tells the service WHICH record to update
            // (Id comes from a hidden field in the edit form)
            var (success, error) = await _service.UpdateRegistrationAsync(vm);

            if (!success)
            {
                // e.g. "Registration not found" or "Email already taken"
                TempData["Error"] = error;
                return View(vm);
            }

            TempData["Success"] = "Registration updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // POST DELETE — remove a registration
        // ============================================================
        // No change needed here — Delete only uses int id, no model involved
        // ============================================================
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