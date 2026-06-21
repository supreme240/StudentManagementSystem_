using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.domain.Domain;
using System.Security.Claims;

namespace StudentManagementSystem.Controllers
{
    public class LogInController : Controller
    {
        private readonly ILogIn _logIn;
        private readonly IForgotPassword _forgotPassword;

        public LogInController(ILogIn logIn, IForgotPassword forgotPassword)
        {
            _logIn = logIn;
            _forgotPassword = forgotPassword;
        }

        // ------------------------------------------------------------------
        // GET: /LogIn/LogIn
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: simply render the empty login form.
        // 2. catch: if rendering somehow fails (rare, but kept for
        //    consistency), fall back to a generic error view.
        // 3. finally: sets the page title regardless of outcome.
        [HttpGet]
        public IActionResult LogIn()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Log In";
            }
        }

        // ------------------------------------------------------------------
        // POST: /LogIn/Login
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: validate the model, then ask _logIn to validate the
        //    submitted credentials.
        // 2. If no matching user is found, explicitly THROW an
        //    UnauthorizedAccessException instead of just checking for null
        //    — this turns "bad credentials" into a real, catchable exception.
        // 3. Build claims, sign the user in via cookie auth, and redirect
        //    based on role.
        // 4. catch (UnauthorizedAccessException): handles the bad-credentials
        //    case from step 2 and re-shows the login form with an error.
        // 5. catch (Exception): handles anything unexpected (auth service
        //    down, sign-in failure, etc.) and shows a generic error instead
        //    of crashing.
        // 6. finally: runs regardless of outcome — used here to guarantee
        //    ViewBag.Title is always set before any view renders.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogIn model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("LogIn", model);
                }

                var user = await _logIn.ValidateUserAsync(model.UserNameOrEmail, model.Password);

                if (user == null)
                {
                    // Explicitly throw instead of silently branching on null
                    throw new UnauthorizedAccessException("Login failed. Incorrect username/email or password.");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                    ? RedirectToAction("Index", "Student")
                    : RedirectToAction("Create", "Student");
            }
            catch (UnauthorizedAccessException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("LogIn", model);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Something went wrong while logging you in. Please try again.";
                return View("LogIn", model);
            }
            finally
            {
                ViewBag.Title = "Log In";
            }
        }

        // ------------------------------------------------------------------
        // GET: /LogIn/LoginFailed
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: set the error message and render the login view.
        // 2. catch: kept for consistency in case the view fails to render.
        // 3. finally: sets the page title regardless of outcome.
        [HttpGet]
        public IActionResult LoginFailed()
        {
            try
            {
                ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
                return View("Login", new LogIn());
            }
            catch (Exception)
            {
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Log In";
            }
        }

        // ------------------------------------------------------------------
        // GET: /LogIn/ForgotPassword
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: render an empty ForgotPassword form.
        // 2. catch: kept for consistency in case the view fails to render.
        // 3. finally: sets the page title regardless of outcome.
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            try
            {
                return View(new ForgotPassword());
            }
            catch (Exception)
            {
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Forgot Password";
            }
        }

        // ------------------------------------------------------------------
        // POST: /LogIn/ForgotPassword
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: validate the model, then look up the account by email and
        //    phone number.
        // 2. If no matching account is found, THROW a KeyNotFoundException
        //    instead of just checking for null — makes "no account found"
        //    a real, catchable exception.
        // 3. Attempt to reset the password. If the service reports failure,
        //    THROW an InvalidOperationException so that "soft" failure also
        //    flows through the catch path below.
        // 4. catch (KeyNotFoundException): handles step 2 — shows the
        //    "no account found" message.
        // 5. catch (InvalidOperationException): handles step 3 — shows the
        //    "something went wrong" message.
        // 6. catch (Exception): handles anything else unexpected.
        // 7. finally: sets the page title regardless of which path was taken.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                int? userId = await _forgotPassword.ValidateUserAsync(model.Email, model.PhoneNumber);

                if (userId == null)
                {
                    throw new KeyNotFoundException("No account found with that email and phone number.");
                }

                bool success = await _forgotPassword.ResetPasswordAsync(userId.Value, model.Password);

                if (!success)
                {
                    throw new InvalidOperationException("Something went wrong. Please try again.");
                }

                return RedirectToAction("ForgotPasswordSuccess");
            }
            catch (KeyNotFoundException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again.";
                return View(model);
            }
            finally
            {
                ViewBag.Title = "Forgot Password";
            }
        }

        // ------------------------------------------------------------------
        // GET: /LogIn/ForgotPasswordSuccess
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: render the success confirmation view.
        // 2. catch: kept for consistency in case the view fails to render.
        // 3. finally: sets the page title regardless of outcome.
        [HttpGet]
        public IActionResult ForgotPasswordSuccess()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Password Reset Successful";
            }
        }

        // ------------------------------------------------------------------
        // GET: /LogIn/AccessDenied
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: render the access-denied view.
        // 2. catch: kept for consistency in case the view fails to render.
        // 3. finally: sets the page title regardless of outcome.
        [HttpGet]
        public IActionResult AccessDenied()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
            finally
            {
                ViewBag.Title = "Access Denied";
            }
        }

        // ------------------------------------------------------------------
        // POST: /LogIn/Logout
        // ------------------------------------------------------------------
        // WORKFLOW:
        // 1. try: sign the user out of cookie auth and clear the session.
        // 2. catch: if sign-out or session clearing throws (e.g. session
        //    provider unavailable), the error is swallowed here so the user
        //    is still redirected to the login page rather than seeing a crash.
        // 3. finally: always redirects to LogIn regardless of whether sign-out
        //    succeeded or failed, keeping logout behavior predictable.
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
            }
            catch (Exception)
            {
                // Intentionally swallowed: even if sign-out/session clearing
                // fails, the user should still be sent back to the login page.
            }
            finally
            {
                // No-op cleanup placeholder; kept to show the logout attempt
                // always reaches a defined completion point before redirecting.
            }

            return RedirectToAction("LogIn", "LogIn");
        }
    }
}
