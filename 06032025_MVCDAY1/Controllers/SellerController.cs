using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    
    public class SellerController : Controller
    {
        private readonly IProductRepository _Prodrepository;

        public SellerController(IProductRepository prodrepository)
        {
            
            _Prodrepository = prodrepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SellerHome()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var perform = _Prodrepository.GetProductPerformance(userId);
            return View(perform);
            
        }
    }
}
