using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;

namespace _06032025_MVCDAY1.Controllers.Vendor
{
    public class VendorController : Controller
    {
        private readonly IProductRepository _Prodrepository;
        private static List<Products> product = new List<Products>();

        public VendorController(IProductRepository repository)
        {
            _Prodrepository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegisterVendor()
        {
            return View();
        }
        public IActionResult LoginView()
        {
            return View();
        }
        //public IActionResult AddProduct()
        //{
        //    AddPro ap = new AddPro()
        //    {
        //        pid = 1,
        //        pname = "T-Shirt"
        //    };

        //    ViewBag.prodlist = ap;
        //    return View();
        //}
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Products p)
        {
            if (ModelState.IsValid)
            {
                p.pid = product.Count + 1;
                product.Add(p);
                return RedirectToAction("GetProduct");
            }
            return View(p);
        }
        public IActionResult GetProduct()
        {
            return View(product);
        }
        //GET
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var prod = product.FirstOrDefault(e => e.pid == id);
            if (prod == null) return NotFound();
            return View(prod);
        }
        // Edit: POST
        [HttpPost]
        public IActionResult EditProduct(Products p)
        {
            var prod = product.FirstOrDefault(e => e.pid == p.pid);
            if (prod == null) return NotFound();

            if (ModelState.IsValid)
            {
                prod.pname = p.pname;
                return RedirectToAction("GetProduct");
            }
            return View(p);
        }
        // Delete: GET (Confirmation Page)
        public IActionResult DeleteProduct(int id)
        {
            var employee = product.FirstOrDefault(e => e.pid == id);
            if (employee == null) return NotFound();
            return View(employee);
        }


        // Delete: POST (Confirmed)
        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            var prod = product.FirstOrDefault(e => e.pid == id);
            if (prod == null) return NotFound();

            product.Remove(prod);
            return RedirectToAction("GetProduct");
        }


        //public class AddPro
        // {
        //     public int pid;
        //     public string pname;


        // }

        public IActionResult review()
        {
            var vendorid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var ratings = _Prodrepository.GetAllReviewsbyvendor(vendorid);
            return View(ratings);
        }

        //customer review

        public IActionResult Reviews()
        {
            var vendorid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var ratings = _Prodrepository.GetAllReviewsbyvendor(vendorid);
            return View(ratings);
        }
    }
}
