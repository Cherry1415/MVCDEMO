using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;

namespace _06032025_MVCDAY1.Controllers
{
    public class ProductController : Controller
    {

        private readonly IUserRepository _repository;

        public ProductController(IUserRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Product()
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));

            List<Product> products= _repository.GetAllProducts();

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


        public IActionResult DetailProduct()
        {
            return View();
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
    }
}
