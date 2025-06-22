using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Razorpay.Api;
using System.Text.Json;

namespace _06032025_MVCDAY1.Controllers
{
    
    public class DashBoardController : Controller
    {
        private readonly IProductRepository _repo;
        public DashBoardController(IProductRepository repository)
        {
            _repo = repository;
        }
        public IActionResult Index()
        {
           // TempData.Keep("email");
            return View();
        }


        public IActionResult HomeDashBoard()

        {
            var model = _repo.GetHomePagesubCategoriesWithProducts();
            var prod = _repo.TopSellingProduct().ToList();
            var tup = System.Tuple.Create(model,prod);
            return View(tup);
        }
    }
}
