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
                HttpContext.Session.SetString("email", email);
                return RedirectToAction("HomeDashBoard","DashBoard");
            }
            else
            {
                TempData["Error"] = "Invalid Cridetial!!";
                //ViewBag.ErrorMessage = "Invalid Cridetial!!";
                return View();
            }
                
        }
    }
}
