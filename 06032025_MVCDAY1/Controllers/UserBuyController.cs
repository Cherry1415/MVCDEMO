using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class order
    {
        public int oid;
        public string o_status;
        public string c_name;
        public string p_name;
    }
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

        [Route("orders")]
        public IActionResult orders()
        {

            List<order> ord = new List<order>();

            ord.Add(new order { oid = 1, o_status = "processing", c_name = "cherry", p_name = "leptop" });
            ord.Add(new order { oid = 2, o_status = "accepted", c_name = "nik", p_name = "pencil" });

            ViewBag.details = ord;

            ViewBag.Title = "Order Details";
            return View();
        }

        //public ActionResult GetPartial()
        //{
        //    return PartialView("BuyProduct");
        //}
    }
}
