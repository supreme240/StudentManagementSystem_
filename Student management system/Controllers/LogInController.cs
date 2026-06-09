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

        // GET: /LogIn/LogIn
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        // POST: /LogIn/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogIn model)
        {
            // ✅ Return to correct view name on invalid
            if (!ModelState.IsValid)
                return View("LogIn", model); // ← "LogIn" not "Login"

            var user = await _logIn.ValidateUserAsync(model.UserNameOrEmail, model.Password);

            if (user == null)
            {
                // ✅ Don't redirect — stay on page with error
                ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
                return View("LogIn", model); // ← show error on same page
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

        // GET: /LogIn/LoginFailed
        [HttpGet]
        public IActionResult LoginFailed()
        {
            ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
            return View("Login", new LogIn());
        }

        // GET: /LogIn/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPassword());
        }

        // POST: /LogIn/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

        // GET: /LogIn/ForgotPasswordSuccess
        [HttpGet]
        public IActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        // GET: /LogIn/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // POST: /LogIn/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "LogIn");
        }
    }
}