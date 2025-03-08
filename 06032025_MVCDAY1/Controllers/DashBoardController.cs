using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult HomeDashBoard()
        {
            return View();
        }
    }
}
