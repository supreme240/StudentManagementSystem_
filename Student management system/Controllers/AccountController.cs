using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.domain.Domain;
using System.Security.Claims;

namespace StudentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogIn _logIn;
        private readonly IForgotPassword _forgotPassword;

        // Inject login and forgot password services via constructor
        public AccountController(ILogIn logIn, IForgotPassword forgotPassword)
        {
            _logIn = logIn;
            _forgotPassword = forgotPassword;
        }

        // GET: /Account/LogIn — renders the login form
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        // POST: /Account/LogIn — validates credentials and signs in the user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogIn model, string? returnUrl)
        {
            // Return the form with validation errors if model is invalid
            if (!ModelState.IsValid)
                return View("LogIn", model);

            try
            {
                // Attempt to validate the user credentials against the database
                var user = await _logIn.ValidateUserAsync(model.UserNameOrEmail, model.Password);

                // If no user found, show an error message and re-render the login form
                if (user == null)
                {
                    ViewBag.ErrorMessage = "Invalid username/email or password.";
                    return View("LogIn", model);
                }

                // Build the list of claims to store in the auth cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),       // Used for User.Identity.Name
                    new Claim(ClaimTypes.Role, user.Role),            // Used for role-based authorization
                    new Claim("FullName", user.FullName),             // Custom claim for display purposes
                };

                // Create the claims identity and principal using cookie authentication
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user — this sets the auth cookie in the browser
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect based on role:
                // Admin → Student list (Index)
                // Student → Registration form (Create)
                return user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                    ? RedirectToAction("Index", "Student")
                    : RedirectToAction("Create", "Student");
            }
            catch (Exception ex)
            {
                // Log the exception (replace with your logger if available e.g. _logger.LogError)
                Console.WriteLine($"[LogIn Error] {ex.Message}");

                // Show a generic error to the user — avoid exposing internal details
                ViewBag.ErrorMessage = "An unexpected error occurred during login. Please try again.";
                return View("LogIn", model);
            }
        }

        // GET: /Account/LoginFailed — shown when authentication middleware rejects the login
        [HttpGet]
        public IActionResult LoginFailed()
        {
            ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
            return View("Login", new LogIn());
        }

        // GET: /Account/ForgotPassword — renders the forgot password form
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPassword());
        }

        // POST: /Account/ForgotPassword — validates identity and resets the user's password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            // Return the form with validation errors if model is invalid
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Step 1: Check if the email and phone number match a real account
                int? userId = await _forgotPassword.ValidateUserAsync(model.Email, model.PhoneNumber);

                // If no matching user found, show an error and stop
                if (userId == null)
                {
                    ViewBag.ErrorMessage = "No account found with that email and phone number.";
                    return View(model);
                }

                // Step 2: Attempt to reset the password for the matched user
                bool success = await _forgotPassword.ResetPasswordAsync(userId.Value, model.Password);

                // If the reset failed (e.g. DB error), notify the user
                if (!success)
                {
                    ViewBag.ErrorMessage = "Something went wrong. Please try again.";
                    return View(model);
                }

                // Password reset succeeded — redirect to the success confirmation page
                return RedirectToAction("ForgotPasswordSuccess");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"[ForgotPassword Error] {ex.Message}");

                // Show a safe generic error message to the user
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                return View(model);
            }
        }

        // GET: /Account/ForgotPasswordSuccess — confirmation page after a successful password reset
        [HttpGet]
        public IActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        // GET: /Account/AccessDenied — shown when a user tries to access a page they don't have permission for
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // POST: /Account/Logout — signs the user out and clears the session
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Remove the auth cookie — this ends the authenticated session
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Clear any server-side session data (e.g. temp data stored in session)
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                // Log the error but still redirect — don't leave the user stuck on the page
                Console.WriteLine($"[Logout Error] {ex.Message}");
            }

            // Always redirect to login page after logout, even if an error occurred
            return RedirectToAction("Login", "Account");
        }
    }
}