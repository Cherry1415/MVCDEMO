using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;
//using Razorpay.Api;

namespace _06032025_MVCDAY1.Controllers
{
    public class ProductController : Controller
    {

        private readonly IUserRepository _repository;
        private readonly IProductRepository _Prodrepository;

        public ProductController(IUserRepository repository, IProductRepository prodrepository)
        {
            _repository = repository;
            _Prodrepository = prodrepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Product()
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));

           var products = _Prodrepository.GetProducts();

            List<int> wishlistid = _repository.GetUserWishlist(uid).Select(w => w.product_id)
                                  .ToList();
            //if(uid == 0)
            //{
            //   products = _repository.GetAllProducts();
            //}
            //else
            //{
            //    products = _repository.GetUserWishlist(uid);
            //}
            //return View(products);
            foreach (var product in products)
            {
                product.ProductImages = _repository.GetImagesByProductId(product.product_id);
                product.IsInWishlist = wishlistid.Contains(product.product_id);
            }

            return View(products);
        }

        //Mobile
        public IActionResult ProductFilter()
        {
            return View();
        }
        public IActionResult ClothFilter()
        {
            return View();
        }
        public IActionResult ElectronicFilter()
        {
            return View();
        }

        public IActionResult ElectronicProduct()
        {
            return View();
        }

        public IActionResult WomenProduct()
        {
            return View();
        }

        
        public IActionResult DetailProduct(int id)
        {
            var products = _Prodrepository.GetProductById(id);
           
            return View(new List<Product> { products });
        }
        public IActionResult ProductFAQ()
        {
            return View();
        }
        //Vendor Side Product Controller...

        [HttpGet]
        public IActionResult NewProduct()
        {
            ViewBag.Categories = _Prodrepository.GetCategories();
            ViewBag.Subcategories = _Prodrepository.GetSubcategories();
            ViewBag.Brands = _Prodrepository.GetBrands();
            return View();
        }


        [HttpPost]
        public IActionResult NewProduct(Product product, List<IFormFile> images, List<Prod_Attributes> attributes, List<VendorStock> vstock)
        {
            if (images == null || images.Count == 0)
            {
                ModelState.AddModelError("images", "Please upload at least one image.");
                return View();
            }

            List<ProductImage> productImages = new List<ProductImage>();

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in images)
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    productImages.Add(new ProductImage
                    {
                        imgName = fileName,
                        imgType = Path.GetExtension(file.FileName).Replace(".", "")
                        // product_id will be set later
                    });
                }
            }

            // Call repository method
            _Prodrepository.NewProduct(product, productImages, attributes, vstock);
            ViewBag.Categories = _Prodrepository.GetCategories();
            ViewBag.Subcategories = _Prodrepository.GetSubcategories();
            ViewBag.Brands = _Prodrepository.GetBrands();
            return RedirectToAction("GetProduct");
        }

        public IActionResult GetProduct()
        {
            var allproduct = _Prodrepository.GetProducts();
            return View(allproduct);
        }

        [HttpGet]
        public JsonResult GetProductById(int id)
        {
            var product = _Prodrepository.GetProductById(id); // Ensure this fetches complete product data including attributes and images
            return Json(product);

        }


        [HttpPost]
        public IActionResult UpdateProduct(Product product, List<IFormFile> images, List<Prod_Attributes> attributes)
        {

            List<ProductImage> productImages = new List<ProductImage>();

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in images)
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    productImages.Add(new ProductImage
                    {
                        imgName = fileName,
                        imgType = Path.GetExtension(file.FileName).Replace(".", "")
                        // product_id will be set later
                    });
                }
            }

            // Call repository method
            _Prodrepository.UpdateProduct(product, productImages, attributes);
            //ViewBag.Categories = _repository.GetCategories();
            //ViewBag.Subcategories = _repository.GetSubcategories();
            //ViewBag.Brands = _repository.GetBrands();

            return RedirectToAction("GetProduct");

        }

    }
}
