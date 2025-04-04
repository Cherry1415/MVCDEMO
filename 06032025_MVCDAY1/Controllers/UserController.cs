using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository repository)
        {
            _repo = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserHomePage()
        {
            ViewData["Layout"] = "~/Views/Shared/_HomeLayout.cshtml";
            return View();
        }

        public IActionResult Register()
        {

            return View(new User());
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
           // if(ModelState.IsValid)
            //{
                bool res = _repo.Register(user);
                if(res)
                {
                    return RedirectToAction("LogInPage","Login");
                }
                
           // }
            return View(user);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string email,string password)
        {
            if(_repo.Login(email,password))
            {
                var user = _repo.getSessionData(email);
              
                string fname = user.first_name;
                int role = user.Role_ID;
                int uid = user.user_id;
                var wishitem = _repo.GetUserWishlist(uid);
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetString("first_name", fname);
                HttpContext.Session.SetString("Role_ID", role.ToString());
                HttpContext.Session.SetString("user_id", uid.ToString());
                HttpContext.Session.SetString("wishlist", string.Join(",",wishitem));

                return RedirectToAction("HomeDashBoard","DashBoard");
            }
            else
            {
                TempData["Error"] = "Invalid Cridetial!!";
                //ViewBag.ErrorMessage = "Invalid Cridetial!!";
                return View();
            }
                
        }

        public ActionResult ProductWishList()
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            List<int> wishlist = _repo.GetUserWishlist(uid);

            //foreach (var wish in wishlist)
            //{
            //    wish.ProductImages = _repo.GetImagesByProductId(wish.product_id);
            //}
            return View(wishlist);

        }

        //Customer WishList
        public ActionResult AddToWishlist()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToWishlist(int userid,int productId)

        {
            //int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
          //  int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (userid == 0)
            {
                TempData["Error"] = "Please log in First to add Wishlist!!";
                return RedirectToAction("SignIn", "User");
            }
            bool isInwishlist = _repo.IsInWishlist(productId, userid);

            if(isInwishlist)
            {
                _repo.RemoveFromWishlist(userid, productId);
            }
            else
            {
                _repo.AddToWishlist(userid, productId);
                
            }
            var wishlist = _repo.GetUserWishlist(userid);
            HttpContext.Session.SetString("Wishlist", string.Join(",", wishlist));
            return Content(""); // Redirect back to product listing
        }

        

        [HttpPost]
        public ActionResult togglewishlist(int prodid)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if(uid==0)
            {
                TempData["Error"] = "Please log in First to add Wishlist!!";
            }

            bool isInwishlist = _repo.IsInWishlist(prodid, uid);

            if(isInwishlist)
            {

            }
            else
            {
                _repo.AddToWishlist(uid, prodid);
            }
            return Content("");
        }


        //public List<int> GetUserWishlistIds(int userId)
        //{

        //    return _repo.GetUserWishlist(userId).Select(p => p.product_id).ToList();

        //}
    }
}
