using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();

        void NewProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes, List<VendorStock> vstock);

        // IEnumerable<Category> GetCategories();
        //Products UpdateProduct(Products product);
        //Products DeleteProduct(int id);
    }
}
