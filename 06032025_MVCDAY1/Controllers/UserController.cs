using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            if (res)
            {
                return RedirectToAction("LogInPage", "Login");
            }

            // }
            return View(user);
        }
        [HttpGet]
        public IActionResult SignIn(int? role)
        {
            if (role != null)
            {
                HttpContext.Session.SetInt32("Temp_Role", role ?? 2);
            }
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            if (_repo.Login(email, password))
            {
                var user = _repo.getSessionData(email);
                int roleFromDB = user.Role_ID;

                // Optional: Validate role from session (if someone used vendor login link)
                int tempRole = HttpContext.Session.GetInt32("Temp_Role") ?? 2;
                if (tempRole != roleFromDB)
                {
                    TempData["Error"] = "You are not allowed to login as this user type.";
                    return View();
                }
                
                var cookieData = new UserCookieModel
                {
                    user_id = user.user_id,
                    name = user.first_name,
                    role = user.Role_ID
                };
                string jsonData = JsonSerializer.Serialize(cookieData);

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // Keep cookie for 30 days
                    HttpOnly = false,  // So JS can access it
                    Secure = false,    // Set true if using https
                    SameSite = SameSiteMode.Lax // This prevents CSRF attacks
                };

                Response.Cookies.Append("UserData", jsonData, options);
                string fname = user.first_name;
                int uid = user.user_id;
                var wishitem = _repo.GetUserWishlist(uid);
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetString("first_name", fname);
                HttpContext.Session.SetString("Role_ID", roleFromDB.ToString());
                HttpContext.Session.SetString("user_id", uid.ToString());
                HttpContext.Session.SetString("wishlist", string.Join(",", wishitem));

                if (roleFromDB == 3)
                    return RedirectToAction("SellerHome", "Seller");
                else if (roleFromDB == 4)
                    return RedirectToAction("Index", "Supplier");

                return RedirectToAction("HomeDashBoard", "DashBoard");
            }
            else
            {
                TempData["Error"] = "Invalid Credentials!!";
                return View();
            }

        }


        public ActionResult ProductWishList()
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            List<Product> wishlist = _repo.GetUserWishlist(uid);

            foreach (var wish in wishlist)
            {
                wish.ProductImages = _repo.GetImagesByProductId(wish.product_id);
            }
            return View(wishlist);

        }

        //Customer WishList
        public ActionResult AddToWishlist()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToWishlist(int userid, int productId)

        {
            //int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            //  int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (userid == 0)
            {
                TempData["Error"] = "Please log in First to add Wishlist!!";
                return RedirectToAction("SignIn", "User");
            }
            bool isInwishlist = _repo.IsInWishlist(productId, userid);

            if (isInwishlist)
            {
                _repo.RemoveFromWishlist(userid, productId);
            }
            else
            {
                _repo.AddToWishlist(userid, productId);
                TempData["Message"] = "Item added to wishlist.";
            }
            var wishlist = _repo.GetUserWishlist(userid);
            //HttpContext.Session.SetString("Wishlist", string.Join(",", wishlist));
            return Content(""); // Redirect back to product listing
        }



        [HttpPost]
        public ActionResult togglewishlist(int prodid)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (uid == 0)
            {
                TempData["Error"] = "Please log in First to add Wishlist!!";
            }

            bool isInwishlist = _repo.IsInWishlist(prodid, uid);

            if (isInwishlist)
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
        //public ActionResult AddToCart(int id)
        //{
        //    var product = _repo.ProductById(id);
        //  //  if (product == null) return HttpNotFound();

        //    List<Cart> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
        //    bool isIncart = _repo.IsInWishlist(productId, userid);
        //    var existingItem = cart.FirstOrDefault(x => x.ProductId == id);
        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity++;
        //    }
        //    else
        //    {
        //        cart.Add(new CartItem
        //        {
        //            ProductId = product.ProductId,
        //            ProductName = product.Name,
        //            ImageUrl = product.ImageUrl,
        //            Price = product.Price,
        //            Quantity = 1
        //        });
        //    }

        //    Session["Cart"] = cart;
        //    return RedirectToAction("Index", "Cart");
        //}
    }
}