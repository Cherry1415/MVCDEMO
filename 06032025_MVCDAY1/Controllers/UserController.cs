using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserHomePage()
        {
            ViewData["Layout"] = "~/Views/Shared/_HomeLayout.cshtml";
            return View();
        }
    }
}
