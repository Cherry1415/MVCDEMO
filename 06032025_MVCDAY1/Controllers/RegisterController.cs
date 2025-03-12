using _06032025_MVCDAY1.Models;
using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class RegisterController : Controller
    {
        private static List<Employee> _emp = new List<Employee>();
        public IActionResult Index()
        {
            return View(_emp);
        }
        public IActionResult RegUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegUser(Employee emp)
        {
            if (ModelState.IsValid)
            {
                emp.uid = _emp.Count + 1;
                _emp.Add(emp);
                return RedirectToAction("Index");
            }
            return View(emp);
        }
        public IActionResult EditUser(int id)
        {
            var employee = _emp.FirstOrDefault(e => e.uid == id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // Edit: POST
        [HttpPost]
        public IActionResult EditUser(Employee emp)
        {
            var employee = _emp.FirstOrDefault(e => e.uid == emp.uid);
            if (employee == null) return NotFound();

            if (ModelState.IsValid)
            {
                employee.fname = emp.fname;
                employee.lname = emp.lname;
                employee.email = emp.email;
                employee.phone = emp.phone;
                employee.password = emp.password;
                return RedirectToAction("Index");
            }
            return View(emp);
        }

        public IActionResult DeleteUser(int id)
        {
            var employee = _emp.FirstOrDefault(e => e.uid == id);
            if (employee == null) return NotFound();
            return View(employee);
        }

   

        // Delete: POST (Confirmed)
        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _emp.FirstOrDefault(e => e.uid == id);
            if (employee == null) return NotFound();

            _emp.Remove(employee);
            return RedirectToAction("Index");
        }
    }
}
