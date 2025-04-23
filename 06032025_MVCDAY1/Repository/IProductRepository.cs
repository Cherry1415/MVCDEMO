using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetProductById(int id);
        void NewProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes, List<VendorStock> vstock);
        void UpdateProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes);
        IEnumerable<Category> GetCategories();
        IEnumerable<Subcategory> GetSubcategories();
        IEnumerable<Brands> GetBrands();

        // IEnumerable<Category> GetCategories();
        //Products UpdateProduct(Products product);
        //Products DeleteProduct(int id);
    }
}
