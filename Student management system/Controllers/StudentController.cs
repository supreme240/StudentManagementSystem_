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
            IDapperRegistrationService dapperRegistrationService) // handles Index, Details
        {
            _registrationService = registrationService;
            _rolesService = rolesService;
            _dapperRegistrationService = dapperRegistrationService;
        }

        // ------------------------------------------------------------------
        // GET: Student/Index — Admin only
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: ask the Dapper service for every registration row.
        // 2. catch: if the database/connection throws, instead of letting
        //    the page crash with a raw 500 error, store a friendly message
        //    in TempData and show the shared Error view.
        // 3. finally: runs whether step 1 succeeded or step 2 caught an
        //    error — used here to guarantee ViewBag.Title is always set
        //    so the layout never renders with a blank page title.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var registrations = await _dapperRegistrationService.GetAllAsync();
                return View(registrations);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load the student list. Please try again later.";
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Student Registrations";
            }
        }

        // ------------------------------------------------------------------
        // GET: Student/Details/5 — Admin only
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: fetch a single registration by id from the Dapper service.
        // 2. If it doesn't exist, explicitly THROW a KeyNotFoundException
        //    instead of just returning null — this makes "not found" a real,
        //    catchable exception instead of a silent edge case.
        // 3. catch (KeyNotFoundException): translates that specific
        //    exception into an HTTP 404 response.
        // 4. catch (Exception): catches anything else (DB/connection errors)
        //    and handles it separately from the "not found" case.
        // 5. finally: always sets the page title regardless of outcome.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var registration = await _dapperRegistrationService.GetByIdAsync(id);

                if (registration == null)
                {
                    // Explicitly throw instead of silently returning NotFound()
                    throw new KeyNotFoundException($"Registration with id {id} was not found.");
                }

                return View(registration);
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

        // ------------------------------------------------------------------
        // GET: Student/Create — Admin & Student
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: load roles for the dropdown and render the empty Create form.
        // 2. catch: if the roles service fails, still render a usable form
        //    (with an empty dropdown) rather than blocking the page entirely.
        // 3. finally: ensures ViewBag.Roles always has a value, even on the
        //    success path, by being the single place that assigns it last.
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Create()
        {
            List<string> roleNames = new();

            try
            {
                var roles = await _rolesService.GetAllRolesAsync();
                roleNames = roles.Select(r => r.RoleName).ToList();
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load the registration form right now. Please try again later.";
            }
            finally
            {
                ViewBag.Roles = new SelectList(roleNames);
            }

            return View();
        }

        // ------------------------------------------------------------------
        // POST: Student/Create — Admin & Student
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: if the model is valid, attempt to add the registration
        //    through the EF Core write service.
        // 2. If the service reports failure (success == false), THROW an
        //    InvalidOperationException carrying the service's error message
        //    — this turns a "soft" failure into a real exception so it flows
        //    through the same catch path as everything else.
        // 3. catch (InvalidOperationException): handles the "soft" failure
        //    from step 2 and shows the specific error back to the user.
        // 4. catch (Exception): handles anything unexpected (DB down, etc.)
        //    and shows a generic error instead of crashing.
        // 5. finally: always reloads the roles dropdown so the view never
        //    renders without it, whether we returned early, succeeded, or failed.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Create(Registration registration)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(registration);
                }

                var (success, error) = await _registrationService.AddRegistrationAsync(registration);

                if (!success)
                {
                    // Convert the service-level failure into a thrown exception
                    throw new InvalidOperationException(error ?? "Registration could not be completed.");
                }

                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Create");
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(registration);
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while submitting your registration.";
                return View(registration);
            }
            finally
            {
                // Runs no matter which return path above was taken, ensuring
                // ViewBag.Roles is always populated before the view renders.
                try
                {
                    var roles = await _rolesService.GetAllRolesAsync();
                    ViewBag.Roles = new SelectList(roles, "RoleName", "RoleName");
                }
                catch (Exception)
                {
                    // A failure here shouldn't override the original result,
                    // so it's swallowed rather than thrown again.
                    ViewBag.Roles = new SelectList(Enumerable.Empty<string>());
                }
            }
        }

        // ------------------------------------------------------------------
        // GET: Student/Edit/5 — Admin only
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: fetch the existing registration via EF Core.
        // 2. If not found, THROW a KeyNotFoundException (same pattern as Details()).
        // 3. catch (KeyNotFoundException): returns 404.
        // 4. catch (Exception): handles any other failure (DB, roles service, etc.).
        // 5. finally: sets the page title regardless of outcome.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var registration = await _registrationService.GetRegistrationByIdAsync(id);

                if (registration == null)
                {
                    throw new KeyNotFoundException($"Registration with id {id} was not found.");
                }

                var roles = await _rolesService.GetAllRolesAsync();
                ViewBag.Roles = new SelectList(roles, "Role", "Role", registration.Role);
                return View(registration);
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
            }
        }

        // ------------------------------------------------------------------
        // POST: Student/Edit — Admin only
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: if the model is valid, update the registration via EF Core.
        // 2. catch: any DB/concurrency/unexpected failure is shown to the
        //    user, and the form is re-rendered instead of crashing.
        // 3. finally: always reloads the roles dropdown so the view is safe
        //    to render on every code path (success, validation failure, or error).
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Registration registration)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(registration);
                }

                await _registrationService.UpdateRegistrationAsync(registration);
                TempData["Success"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while updating this registration.";
                return View(registration);
            }
            finally
            {
                try
                {
                    var roles = await _rolesService.GetAllRolesAsync();
                    ViewBag.Roles = new SelectList(roles, "Role", "Role", registration?.Role);
                }
                catch (Exception)
                {
                    ViewBag.Roles = new SelectList(Enumerable.Empty<string>());
                }
            }
        }

        // ------------------------------------------------------------------
        // POST: Student/Delete — Admin only
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: delete the registration via EF Core.
        // 2. catch: catches any failure (e.g. record already deleted, FK
        //    constraint, DB unavailable) and informs the user via TempData
        //    instead of letting an unhandled exception bubble up as a 500 error.
        // 3. finally: always redirects back to Index, success or not — this
        //    keeps the post-action navigation consistent regardless of outcome.
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
            finally
            {
                // No-op cleanup placeholder; kept to show the delete attempt
                // always reaches a defined completion point before redirecting.
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
