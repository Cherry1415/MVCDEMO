using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace _06032025_MVCDAY1.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _Repo;
        public AdminController(IAdminRepository categoryRepo)
        {
            _Repo = categoryRepo;
        }

        
        public IActionResult Index()
        {
            var data = _Repo.GetDashboardData();
            return View(data);
        }
        public IActionResult Orders()
        {
            return View();
            
        }
        [HttpGet]
        public IActionResult filterorder(string status)
        {
            var orders = _Repo.GetAllOrders(status);
            return Json(orders);
        }

        public IActionResult Products()
        {
            var products = _Repo.GetAllProducts();
            return View(products);
        }

        //category show
        public IActionResult GetAllcategory()
        {
            return View();
        }
        [HttpGet]
        
        public IActionResult GetAllcategory1()
        {
            var cate = _Repo.GetAll();
            return Json(cate);
        }
        [HttpGet]
        public IActionResult GetCategoryById(int id)
        {
            var category = _Repo.GetById(id);
            return Json(category);
        }

        [HttpPost]
        public IActionResult SaveCategory(Category model)
        {
            if (model.category_id == 0)
                _Repo.Add(model);
            else
                _Repo.Update(model);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {
            try
            {
                _Repo.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //subcategory show

        public IActionResult GetAllsubcategory()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetAllSubCategory1()
        {
            var data = _Repo.GetAllSubCategory();
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetSubCategoryById(int id)
        {
            var data = _Repo.subcategoryGetById(id);
            return Json(data);
        }
        [HttpPost]
        public JsonResult SaveSubCategory(Subcategory subcategory)
        {
            _Repo.Savesubcategory(subcategory);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteSubCategory(int id)
        {
            _Repo.Deletesubcategory(id);
            return Json(new { success = true });
        }
    }
}
