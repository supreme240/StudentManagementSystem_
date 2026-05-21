using ApplicationStudentManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Student_management_system.Models;
using System.Diagnostics;

namespace Student_management_system.Controllers
{
    public class HomeController : Controller
    { 
        private readonly IStudentInterface studentInterface;

        public HomeController(IStudentInterface studentInterface)
        {


            this.studentInterface = studentInterface;
        }
        public IActionResult Index()
        {
            var data= studentInterface.GetStudentInformation();
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Student()
        {
            var data = studentInterface.GetStudentInformation();


            return View(data);
        }
    }
}
