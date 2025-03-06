using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PaymentProduct()
        {
            return View();
        }
    }
}
