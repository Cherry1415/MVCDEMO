using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts(int catid,int subcateid);
        IEnumerable<Product> VendorGetProducts();
        IEnumerable<Product> PendingApproval();
        Product GetProductById(int id);
        void NewProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes, List<VendorStock> vstock);
        void UpdateProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes);
        IEnumerable<Category> GetCategories();
        IEnumerable<Subcategory> GetSubcategories();
        IEnumerable<Brands> GetBrands();

        int GetCategoryIdByName(string catname);
        int GetSubCategoryIdByName(string subcatname);

        //user homescreen
        List<CategoryWithProductsViewModel> GetHomePageCategoriesWithProducts();
        // IEnumerable<Category> GetCategories();
        //Products UpdateProduct(Products product);
        //Products DeleteProduct(int id);

        //customer reviews
        List<ProductReview> GetAllReviewsbyvendor(int vendorid);

        List<ProductReview> GetReviewsByProductId(int productId);
    }
}
