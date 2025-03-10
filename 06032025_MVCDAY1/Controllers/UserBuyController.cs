using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{

    [Route("Product")]
    public class UserBuyController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        //[Route("BuyProduct")]

       // [Route("")]
        [Route("BuyProduct")]
        public IActionResult BuyProduct()
        {
            return View();
        }

        [Route("WishList")]
        public IActionResult WishList()
        {
            return View();
        }

        [Route("AddCart")]
        public IActionResult AddCart()
        {
            return View();
        }

        [Route("ManProduct")]
        public IActionResult ManProduct()
        {
            return View();
        }

        //public ActionResult GetPartial()
        //{
        //    return PartialView("BuyProduct");
        //}
    }
}
