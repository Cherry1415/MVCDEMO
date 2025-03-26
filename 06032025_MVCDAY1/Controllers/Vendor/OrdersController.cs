using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers.Vendor
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Orders()
        {
            return View();
        }
        public IActionResult Inventory()
        {
            return View();
        }
    }
}
