using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.domain.Domain;

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


        // GET:
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        // POST: /Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LogIn model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _logIn.ValidateUser(
                model.UserNameOrEmail,
                model.Password
            );

            if (user == null)
            {
                //  Redirect back with error via route query
                return RedirectToAction("LoginFailed");
            }

            //  Success - redirect to Student list
            return RedirectToAction("Index", "Student");
        }

        // GET:LoginFailed  / shown when credentials are wrong
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
    }
}