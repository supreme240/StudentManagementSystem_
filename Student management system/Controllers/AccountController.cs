using ApplicationStudentManagement.DTO;
using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.domain.Domain;
using System.Security.Claims;

namespace StudentManagementSystem.Controllers
{


    [AllowAnonymous]
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
        public async Task<IActionResult> LogIn(LogInViewModel model, string? returnUrl)
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
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName),
                };

                // Create the claims identity and principal using cookie authentication
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user — this sets the auth cookie in the browser
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect based on role
                return user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                    ? RedirectToAction("Index", "Student")
                    : RedirectToAction("Create", "Student");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LogIn Error] {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred during login. Please try again.";
                return View("LogIn", model);
            }
        }

        // GET: /Account/LoginFailed — shown when authentication middleware rejects the login
        [HttpGet]
        public IActionResult LoginFailed()
        {
            ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
            return View("LogIn", new LogInViewModel());
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

                // If the reset failed notify the user
                if (!success)
                {
                    ViewBag.ErrorMessage = "Something went wrong. Please try again.";
                    return View(model);
                }

                // Password reset succeeded — redirect to success page
                return RedirectToAction("ForgotPasswordSuccess");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ForgotPassword Error] {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                return View(model);
            }
        }

        // GET: /Account/ForgotPasswordSuccess
        [HttpGet]
        public IActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        // GET: /Account/AccessDenied
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
                // Remove the auth cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Clear session data
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Logout Error] {ex.Message}");
            }

            // Fixed — LogIn not Login
            return RedirectToAction("LogIn", "Account");
        }


    }
}