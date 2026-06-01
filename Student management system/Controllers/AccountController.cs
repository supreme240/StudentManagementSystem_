using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.domain.Domain;

namespace StudentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogIn _logIn;

        public AccountController (ILogIn logIn)
        {
            _logIn = logIn;
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
        public IActionResult LogIn(LogIn model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _logIn.ValidateUser(
                model.UserNameOrEmail,
                model.Password
            );

            if (user == null)
            {
                ViewBag.ErrorMessage = "login failed";
                return View(model);
            }

            // ✅ Success → redirect to Student list
            return RedirectToAction("Index", "Student");
        }

        // GET: /Account/LoginFailed  ← shown when credentials are wrong
        [HttpGet]
        public IActionResult LoginFailed()
        {
            ViewBag.ErrorMessage = "Login failed. Incorrect username/email or password.";
            return View("Login", new LogIn());
        }
    }
}