using ApplicationStudentManagement.DTOs;
using ApplicationStudentManagement.Interfaces;
using ApplicationStudentManagement.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Student_management_system.Controllers {
    public class LoginController : Controller {
        private readonly IRegistrationService _registrationService;

        public LoginController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        // GET: LoginController
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginViewModel, string? returnUrl) {

            var user = await _registrationService.CheckAUthenticationAsync(loginViewModel.Email, loginViewModel.Password);

            if (user == null) {
                ViewBag.ErrorMessage = "Invalid email or password";
                return View(loginViewModel);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
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
                ? RedirectToAction("", "Registration")
                : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await _registrationService.FindByEmailAndPhoneAsync(model.Email, model.Number); 

            if (user == null)
            {
                model.ErrorMessage = "Invalid email or phone number.";
                model.ShowReset = false;
                return View(model);
            }

            model.ShowReset = true;
            model.UserId = user.Id;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgotPasswordViewModel model) {
            await _registrationService.ResetPasswordAsync(model.UserId, model.NewPassword);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Index");

        }

        // GET: LoginController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: LoginController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoginController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Delete/5
        public  IActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoginController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
