using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;
//using Razorpay.Api;
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
        public IActionResult SellerHome()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var perform = _Prodrepository.GetProductPerformance(userId);
            return View(perform);

        }
        public IActionResult SearchProduct(string query)
        {
            List<Product> matchedProducts;
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (string.IsNullOrWhiteSpace(query))
            {
                // Show all products if query is empty
                matchedProducts = _Prodrepository.VendorGetProducts().ToList();
            }
            else
            {
                string lowerQuery = query.ToLower();

                // Filter products based on query
                matchedProducts = _Prodrepository.VendorGetProducts()
                    .Where(p => p.product_name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            List<int> wishlistid = _repository.GetUserWishlist(uid).Select(w => w.product_id)
                                 .ToList();
            foreach (var product in matchedProducts)
            {
                
                product.IsInWishlist = wishlistid.Contains(product.product_id);
            }
            return PartialView("_ProductCardsPartial", matchedProducts);
        }
        public IActionResult SearchVendorProduct(string query)
        {
            List<Product> matchProducts;
            
            if (string.IsNullOrWhiteSpace(query))
            {
                // Show all products if query is empty
                matchProducts = _Prodrepository.VendorGetProducts().ToList();
            }
            else
            {
                // Filter products based on query
                matchProducts = _Prodrepository.VendorGetProducts()
                    .Where(p => p.product_name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            
            return PartialView("_VendorProductPartial", matchProducts);
        }
        public IActionResult Product(string categoryname, string subcategoryname)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));

            int catid = _Prodrepository.GetCategoryIdByName(categoryname);
            int subcateid = _Prodrepository.GetSubCategoryIdByName(subcategoryname);
            //var (minPriceRange, maxPriceRange) = _Prodrepository.GetPriceRange(catid, subcateid);
            //var filterModel = new ProductFilterModel
            //{
            //    AvailableBrands = _Prodrepository.GetAllBrands(catid, subcateid),
            //    MinPriceRange = minPriceRange,
            //    MaxPriceRange = maxPriceRange,
            //    MinPrice = minPriceRange,
            //    MaxPrice = maxPriceRange
            //};

            //ViewData["FilterModel"] = filterModel;

            var products = _Prodrepository.GetProducts(catid,subcateid);

            List<int> wishlistid = _repository.GetUserWishlist(uid).Select(w => w.product_id)
                                  .ToList();
            
            foreach (var product in products)
            {
                product.ProductImages = _repository.GetImagesByProductId(product.product_id);
                product.IsInWishlist = wishlistid.Contains(product.product_id);
                product.AvailableQuantity = _Prodrepository.GetStockByProductId(product.product_id); // <- NEW LINE
                product.IsAvailable = product.AvailableQuantity > 0; // <- NEW LINE
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
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var product = _Prodrepository.GetProductById(id); // single product
            var addresses = _repository.GetAddressesByUserId(userId); // list
            var reviews = _Prodrepository.GetReviewsByProductId(id);
            product.AvailableQuantity = _Prodrepository.GetStockByProductId(product.product_id); // <- NEW LINE
            product.IsAvailable = product.AvailableQuantity > 0; // <- NEW LINE

            // ✅ Wrap product in a List to match expected model
            var productList = new List<Product> { product };

            var model = Tuple.Create(productList.AsEnumerable(), addresses,reviews);

            return View(model);
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
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
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
            _Prodrepository.NewProduct(userId,product, productImages, attributes, vstock);
            ViewBag.Categories = _Prodrepository.GetCategories();
            ViewBag.Subcategories = _Prodrepository.GetSubcategories();
            ViewBag.Brands = _Prodrepository.GetBrands();
            TempData["ProductAdded"] = "true";
            return RedirectToAction("NewProduct");
        }

        
        public IActionResult GetProduct()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var allproduct = _Prodrepository.VendorOwnProducts(userId);
            return View(allproduct);
        }

        public IActionResult OutOfStockNotifications()
        {
            int vendorId = Convert.ToInt32(HttpContext.Session.GetString("vendor_id"));
            var outOfStockProducts = _Prodrepository.GetOutOfStockProducts(vendorId);
            return View(outOfStockProducts);
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

        public IActionResult PendingApprovals()
        {
            var products = _Prodrepository.PendingApproval();
            return View(products);
        }

        public IActionResult VendorPerformance()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var perform = _Prodrepository.GetProductPerformance(userId);
            return View(perform);
        }
    }
}
