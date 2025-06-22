using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts(int catid,int subcateid);
        IEnumerable<Product> VendorGetProducts();
        IEnumerable<Product> VendorOwnProducts(int vendorId);
        List<Product> GetOutOfStockProducts(int vendorId);
        IEnumerable<Product> PendingApproval();
        Product GetProductById(int id);
        void NewProduct(int userID,Product product, List<ProductImage> images, List<Prod_Attributes> attributes, List<VendorStock> vstock);
        void UpdateProduct(Product product, List<ProductImage> images, List<Prod_Attributes> attributes);
        int GetStockByProductId(int productId); //for stock check
        IEnumerable<Category> GetCategories();
        IEnumerable<Subcategory> GetSubcategories();
        IEnumerable<Brands> GetBrands();

        int GetCategoryIdByName(string catname);
        int GetSubCategoryIdByName(string subcatname);

        //user homescreen
        List<CategoryWithProductsViewModel> GetHomePageCategoriesWithProducts();
        List<SubCategoryWithProductsViewModel> GetHomePagesubCategoriesWithProducts();
        // IEnumerable<Category> GetCategories();
        //Products UpdateProduct(Products product);
        //Products DeleteProduct(int id);

        //customer reviews
        List<ProductReview> GetAllReviewsbyvendor(int vendorid);

        List<ProductReview> GetReviewsByProductId(int productId);
        IEnumerable<Product> GetRelatedProducts(int subcategoryId, int excludeProductId);

        // FIlter Product feature for Customer


        IEnumerable<Product> GetFilteredProducts(int catid, int subcateid,List<string> brands, decimal minPrice, decimal maxPrice);
        List<string> GetAllBrands(int categoryId, int subcategoryId);
        (decimal, decimal) GetPriceRange(int catid, int subcatid);
        IEnumerable<Product> TopSellingProduct();

        //product perfomance list

        List<ProductPerformanceModel> GetProductPerformance(int vendorId);

    }
}
