using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }

        //Mobile
        public IActionResult ProductFilter()
        {
            return View();
        }
        public IActionResult ClothFilter()
        {
            return View();
        }
        public IActionResult ElectronicFilter()
        {
            return View();
        }

        public IActionResult ElectronicProduct()
        {
            return View();
        }

        public IActionResult WomenProduct()
        {
            return View();
        }


        public IActionResult DetailProduct()
        {
            return View();
        }
        public IActionResult ProductFAQ()
        {
            return View();
        }

        public IActionResult Inventory()
        {
            return View();
        }
    }
}
