using _06032025_MVCDAY1.Repository;
using _06032025_MVCDAY1.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace _06032025_MVCDAY1.Controllers.Supplier
{
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierController(ISupplierRepository Repository)
        {
            _supplierRepository = Repository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Contact1()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public IActionResult Contact1(SupplierContactus contact)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess = _supplierRepository.AddContactMessage(contact);
                TempData["Message"] = isSuccess ? "Message sent successfully!" : "Failed to send message!";
                return RedirectToAction("Contact1");
            }

            return View(contact);
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        // This sends JSON data for dashboard counts (used by JavaScript)
        [HttpGet]
        public IActionResult GetDashboardCounts()
        {
            var dashboardData = _supplierRepository.GetDashboardCounts();
            return Json(dashboardData);
        }

        public IActionResult Order()
        {
            var orders = _supplierRepository.GetAllVendorOrders();
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string newStatus)
        {
            bool result = _supplierRepository.UpdateOrderStatus(orderId, newStatus);
            if (result)
            {
                TempData["Message"] = $"Order #{orderId} updated to '{newStatus}'";
            }
            else
            {
                TempData["Message"] = $"Failed to update Order #{orderId}";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Vendor()
        {
            var vendors = _supplierRepository.GetAll();
            return View(vendors);
        }

        [HttpPost]
        public IActionResult AddVendor([FromBody] SupplierVendor vendor)
        {
            if (ModelState.IsValid)
            {
                bool success = _supplierRepository.AddVendor(vendor);
                if (success)
                {
                    return Json(new { success = true });
                }
            }
            return BadRequest(new { success = false });
        }

        [HttpGet]
        public IActionResult GetAllVendors()
        {
            var vendors = _supplierRepository.GetAll();
            return Json(vendors);
        }
        // ✅ Display all Warehouses (READ)
        [HttpGet]
        [Route("Index2")] // Updated Route
        public IActionResult Index2()
        {
            var warehouses = _supplierRepository.GetAllwarehouse();
            return View(warehouses);
        }

        // ✅ Show the Create Form (GET)
        [HttpGet]
        [Route("Create1")]
        public IActionResult Create1()
        {
            return View("Create1");
        }

        // ✅ Handle Warehouse Creation (POST)
        [HttpPost]
        [Route("Create1")]
        public IActionResult Create1(WareHouse warehouse)
        {
            if (ModelState.IsValid)
            {
                bool success = _supplierRepository.AddWareHouse(warehouse);
                if (success)
                    return RedirectToAction("Index2");
            }
            ModelState.AddModelError("", "Failed to create Warehouse.");
            return View("Create1", warehouse);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var warehouse = _supplierRepository.GetbyId(id);
            if (warehouse == null)
                return NotFound();
            return View(warehouse);
        }

        // ✅ POST: Supplier/Warehouse/Edit
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, WareHouse wareHouse)
        {
            if (ModelState.IsValid)
            {
                wareHouse.warehouse_id = id;
                _supplierRepository.EditWareHouse(wareHouse);
                return RedirectToAction("Index2");
            }
            return View(wareHouse);
        }


        // ✅ Delete a Warehouse
        [HttpGet]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            bool success = _supplierRepository.DeleteWareHouse(id);
            if (!success)
                return NotFound();

            return RedirectToAction("Index2"); // ✅ Fixed Redirect
        }


        [HttpGet]
        public IActionResult WareHouseDetails(int id)
        {
            var details = _supplierRepository.GetWareHouse_details(id);
            if (details == null)
            {
                ViewBag.WarehouseId = id;
                return View("CreateDetails");
            }

            return View(details);
        }
        [HttpPost]
        public IActionResult CreateDetails(int id, WareHouse_details model)
        {
            if (ModelState.IsValid)
            {
                model.warehouse_id = id;
                bool isAdded = _supplierRepository.AddWareHouseDetails(model);
                if (isAdded)
                    return RedirectToAction("WareHouseDetails", new { id });

                ModelState.AddModelError("", "Failed to add warehouse details.");
            }

            ViewBag.WarehouseId = id;
            return View(model);
        }
        // ✅ Handle adding warehouse details (POST)
        [HttpPost]
        [Route("AddWareHouseDetails/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult AddWareHouseDetails(int id, WareHouse_details details)
        {
            if (ModelState.IsValid)
            {
                details.warehouse_id = id; // Ensure warehouse_id is set properly
                bool success = _supplierRepository.AddWareHouseDetails(details);
                if (success)
                {
                    return RedirectToAction("Index2"); // ✅ Redirect to the warehouse list
                }
                ModelState.AddModelError("", "Failed to add warehouse details.");
            }
            return View(details); // Show form again with errors if any
        }
    }
}
