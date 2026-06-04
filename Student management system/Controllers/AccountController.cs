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

        public AccountController (ILogIn logIn, IForgotPassword forgotPassword)
        {
            _logIn = logIn;
            _forgotPassword = forgotPassword;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogIn model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _logIn.ValidateUser(
                model.UserNameOrEmail,
                model.Password
            );
            var password = user.Password;
            if (user == null)
            {
                ViewBag.ErrorMessage = "login failed";
                return View(model);
            }
            var claims = new List<Claim> {
             new Claim(ClaimTypes.Name, user.FullName),
             new Claim(ClaimTypes.Role, user.Role),
             new Claim("Course", user.Course)
             };
            var claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties { IsPersistent = true });
           
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return user.Role == "Admin"
            ? RedirectToAction("Index", "Student")
            : RedirectToAction("Index", "Home");
        }

        // GET: /Account/LoginFailed  ← shown when credentials are wrong
        [HttpGet]
        public IActionResult LoginFailed()
        {
            ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
            return View("Login", new LogIn());
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPassword());
        }

        // POST: /LogIn/ForgetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int? userId = _forgotPassword.ValidateUser(model.Email, model.PhoneNumber);

            if (userId == null)
            {
                ViewBag.ErrorMessage = "No account found with that email and phone number.";
                return View(model);
            }

            bool success = _forgotPassword.ResetPassword(userId.Value, model.Password);

            if (!success)
            {
                ViewBag.ErrorMessage = "Something went wrong. Please try again.";
                return View(model);
            }

            return RedirectToAction("ForgotPasswordSuccess");
        }

        // GET: /LogIn/ForgetPasswordSuccess
        [HttpGet]
        public IActionResult ForgotPasswordSuccess()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login","Account"); 
        }

    }
}