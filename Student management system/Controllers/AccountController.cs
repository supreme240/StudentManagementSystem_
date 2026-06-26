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
    
    public class AccountController : Controller
    {
        private readonly ILogIn _logIn;
        private readonly IForgotPassword _forgotPassword;

        public AccountController(ILogIn logIn, IForgotPassword forgotPassword)
        {
            _logIn = logIn;
            _forgotPassword = forgotPassword;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View("LogIn", model);

            try
            {
                var user = await _logIn.ValidateUserAsync(model.UserNameOrEmail, model.Password);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "Invalid username/email or password.";
                    return View("LogIn", model);
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
            catch (Exception ex)
            {
                Console.WriteLine($"[LogIn Error] {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred during login. Please try again.";
                return View("LogIn", model);
            }
        }

        [HttpGet]
        public IActionResult LoginFailed()
        {
            ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
            return View("LogIn", new LogInViewModel());
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPassword());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                int? userId = await _forgotPassword.ValidateUserAsync(model.Email, model.PhoneNumber);

                if (userId == null)
                {
                    ViewBag.ErrorMessage = "No account found with that email and phone number.";
                    return View(model);
                }

                bool success = await _forgotPassword.ResetPasswordAsync(userId.Value, model.Password);

                if (!success)
                {
                    ViewBag.ErrorMessage = "Something went wrong. Please try again.";
                    return View(model);
                }

                return RedirectToAction("ForgotPasswordSuccess");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ForgotPassword Error] {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Logout Error] {ex.Message}");
            }

            return RedirectToAction("LogIn", "Account");
        }
    }
}