using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;

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

           var products = _repository.GetAllProducts();

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
            List<Product> products = _repository.ProductById(id);
            foreach (var product in products)
            {
                product.ProductImages = _repository.GetImagesByProductId(product.product_id);
            }

            return View(products);
        }
        public IActionResult ProductFAQ()
        {
            return View();
        }

        public IActionResult Inventory()
        {
            return View();
        }

        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product product, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                int productId = _repository.AddProduct(product);

                if (images != null && images.Count > 0)
                {
                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            string fileName = Path.GetFileName(image.FileName);
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }

                            ProductImage img = new ProductImage
                            {
                                product_id = productId,
                                imgName = "/uploads/" + fileName,
                                imgType = image.ContentType

                            };

                            _repository.AddProductImage(img);
                        }
                    }
                }
                return RedirectToAction("HomeDashBoard", "DashBoard");
            }
            return View(product);
        }


        //Vendor Side Product Controller...

        [HttpGet]
        public IActionResult NewProduct()
        {
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

            return RedirectToAction("GetProduct");
        }

        public IActionResult GetProduct()
        {
            var allproduct = _Prodrepository.GetProducts();
            return View(allproduct);
        }

    }
}
