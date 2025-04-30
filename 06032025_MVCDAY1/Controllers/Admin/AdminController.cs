using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository<category> _categoryRepo;
        public AdminController(IAdminRepository<category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public IActionResult GetAllCategories()
        {
            var categories = _categoryRepo.GetAllData();
            return Ok(categories);
        }


        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryRepo.GetDataById(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        public IActionResult AddCategory([FromBody] category category)
        {
            if (category == null)
            {
                return BadRequest("Category data is null.");
            }
            _categoryRepo.Add(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.category_Id }, category); // Fixed property name to `Category_Id`
        }

        public IActionResult UpdateCategory(int id, category category)
        {
            if (category == null || category.category_Id != id) // Corrected property name to `Category_Id`
            {
                return BadRequest("Category data is invalid.");
            }
            var existingCategory = _categoryRepo.GetDataById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            _categoryRepo.Update(category);
            return Ok(category);
        }

        public IActionResult DeleteCategory(int id)
        {
            var existingCategory = _categoryRepo.GetDataById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            _categoryRepo.Delete(id);
            return NoContent();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
