using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class CartController : Controller
    {
        private readonly IUserRepository _cartrepository;
        public CartController(IUserRepository repository)
        {
            _cartrepository = repository;
        }
        public IActionResult Index()
        {
            List<Product> products;
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (userId == 0)
            {
                return RedirectToAction("SignIn", "User");
            }

            products = _cartrepository.GetUserWishlist(userId);
            var cartItems = _cartrepository.GetCartItemsByUserId(userId);
            foreach (var product in products)
            {
                product.ProductImages = _cartrepository.GetImagesByProductId(product.product_id);
            }
            //ViewBag.CartItemCount = _cartrepository.GetCartItemCount(userId);
            //TempData["CartItemCount"] = _cartrepository.GetCartItemCount(userId);
            return View(cartItems);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (userId == 0)
            {
                return RedirectToAction("SignIn", "User");
            }

            _cartrepository.AddItemToCart(userId, productId, quantity);
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartId, string operation)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (userId == 0)
            {
                return RedirectToAction("SignIn", "User");
            }

            var cartItem = _cartrepository.GetCartItemById(cartId, userId);
            if (cartItem != null)
            {
                if (operation == "increase")
                    cartItem.quantity += 1;
                else if (operation == "decrease" && cartItem.quantity > 1)
                    cartItem.quantity -= 1;

                _cartrepository.UpdateCartItemQuantity(cartItem.cart_item_id, cartItem.quantity);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Remove(int cartId)
        {
            _cartrepository.RemoveCartItem(cartId);
            return RedirectToAction("Index");
        }
        public JsonResult GetCartQuantity()
        {
            int totalQuantity=0;

            int? userId = Convert.ToInt32(HttpContext.Session.GetString("user_id")); // or however you're tracking user

           if(userId.HasValue)
           
                 totalQuantity = _cartrepository.GetCartItemCount(userId.Value); // assume returns List<CartItem>
                
            

            return Json(totalQuantity);
        }
    }
}
