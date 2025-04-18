using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IUserRepository
    {
        bool Register(User user);
        bool Login(string email, string password);

         List<Product> GetAllProduct();
        IEnumerable<Product> GetAllProducts();

        //For fetching username with session
        User getSessionData(string email);

        //customer cart

        void AddItemToCart(int userId, int productId, int quantity);
        void RemoveCartItem(int cartId);
        int GetCartItemCount(int userId);

        List<CartItemViewModel> GetCartItemsByUserId(int userId);
        CartItemViewModel GetCartItemById(int cartId, int userId);
        void UpdateCartItemQuantity(int cartId, int quantity);

        //Customer WishList

        void AddToWishlist(int userId, int productId);
        void RemoveFromWishlist(int userId, int productId);
        bool IsInWishlist(int productId, int userID);
        List<Product> GetUserWishlist(int userId);
       // bool AddWishList(CustomerWishList customerWishList);

        //Product side methods

        int AddProduct(Product product);
        bool AddProductImage(ProductImage productImage);
       // List<Product> GetAllProducts();
        List<Product> ProductById(int id);
        List<ProductImage> GetImagesByProductId(int productId);
    }
}
