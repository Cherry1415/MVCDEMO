using _06032025_MVCDAY1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace _06032025_MVCDAY1.Controllers
{
    
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
           // TempData.Keep("email");
            return View();
        }


        public IActionResult HomeDashBoard()

        {

          /*  string json = Request.Cookies["UserData"];
            Console.WriteLine("Cookie Data: " + json);
            if (json != null)
            {
                var userData = JsonSerializer.Deserialize<UserCookieModel>(json);
                Console.WriteLine("User Data from Cookie: " + userData.name);
                int uid = userData.user_id;
                string name = userData.name;
                int role = userData.role;

                ViewBag.UserName = name;
                ViewBag.Role = role;

                // Aap yahan se user-specific data fetch kar sakte ho
                // var orders = _repo.GetUserOrders(uid);
                // return View(orders);
            }
            else
            {
                // Cookie nahi mili, to redirect to login
                return RedirectToAction("SignIn", "User");
            }*/

            return View();
        }
    }
}
