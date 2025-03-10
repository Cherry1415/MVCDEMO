using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult LogInPage()
        {
            return View();
        }

        //This code for tampdata
        [HttpPost]
        public IActionResult LogInPage(string umail)
        {
            TempData["Usermail"] = umail;
           // TempData.Keep("mail");
            return RedirectToAction("HomeDashBoard", "DashBoard");
            //return View();
        }
    }
}
