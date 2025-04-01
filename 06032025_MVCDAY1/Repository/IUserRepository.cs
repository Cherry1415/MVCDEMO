using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IUserRepository
    {
        bool Register(User user);
        bool Login(string email, string password);

        List<Product> GetAllProduct();

        //For fetching username with session
        User getSessionData(string email);

        //Product side methods

        int AddProduct(Product product);
        bool AddProductImage(ProductImage productImage);
        List<Product> GetAllProducts();
        List<Product> ProductById(int id);
        List<ProductImage> GetImagesByProductId(int productId);
    }
}
