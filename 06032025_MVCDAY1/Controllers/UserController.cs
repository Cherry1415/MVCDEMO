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
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetString("first_name", fname);
                HttpContext.Session.SetString("Role_ID", role.ToString());
                HttpContext.Session.SetString("user_id)", uid.ToString());
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
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id")); // Replace this with your actual user ID retrieval method

            var wishlistItems = _repo.GetUserWishlist(uid); // Fetch wishlist items

            if (wishlistItems == null)
            {
                wishlistItems = new List<Product>(); // Ensure it's never null
            }

            return View(wishlistItems);
            
        }

        //Customer WishList

        [HttpPost]
        public ActionResult AddToWishlist(int productId)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            
            _repo.AddToWishlist(uid, productId);
            return View(); // Redirect back to product listing
        }

        public ActionResult Wishlist()
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            List<Product> wishlist = _repo.GetUserWishlist(uid);

            foreach (var wish in wishlist)
            {
                wish.ProductImages = _repo.GetImagesByProductId(wish.product_id);
            }
            return View(wishlist);
        }
    }
}
