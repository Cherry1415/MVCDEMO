using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers.Vendor
{
    public class OrdersController : Controller
    {
        private readonly IOrdersRepository _repository;
        public OrdersController(IOrdersRepository repository)
        {
            _repository = repository;
        }
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
            int vendorId = 1; // Or get from session/auth
            var stockList = _repository.manageInventoty(vendorId);

            if (stockList == null || !stockList.Any())
            {
                ViewBag.Message = "No inventory found for vendor.";
            }

            return View(stockList);
            //int vendorId = Convert.ToInt32(HttpContext.Session.GetString("VendorId"));
        }
        public JsonResult GetStockById(int id)
        {
            var stock = _repository.GetVendorStockById(id);
            return Json(stock);
        }

        [HttpPost]
        public ActionResult UpdateStock(VendorStock vs)
        {
            _repository.UpdateVendorStock(vs);
            return Json(new { status = 200, message = "Stock updated successfully" });
        }

        public JsonResult GetLowStockNotifications(int threshold)
        {
            var lowStockList = _repository.GetShortageItems(threshold);
            return Json(lowStockList);
        }

    }
}
