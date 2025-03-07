using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogInPage()
        {
            return View();
        }
    }
}
