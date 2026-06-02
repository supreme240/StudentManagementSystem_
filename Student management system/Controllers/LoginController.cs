using ApplicationStudentManagement.Interfaces;
using ApplicationStudentManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Student_management_system.Models;
using StudentManagementSystem.Infrastructure.Repositories.RegistrationRepo;
using System.Threading.Tasks;

namespace Student_management_system.Controllers {
    public class LoginController : Controller {
        private readonly IRegistrationService _registrationService;

        public LoginController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        // GET: LoginController
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginViewModel) {

            var user = await _registrationService.CheckAUthenticationAsync(loginViewModel.Email, loginViewModel.Password);

            if (user == null) {
                ViewBag.ErrorMessage = "Invalid email or password";
                return View(loginViewModel);
            }
            return RedirectToAction("Index", "Registration");
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
            var user = await _registrationService.GetRegistrationByIdAsync(model.UserId); 

            if (user == null)
                return NotFound();

            user.Password = model.NewPassword;

            await _registrationService.UpdateRegistrationAsync(user);
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
