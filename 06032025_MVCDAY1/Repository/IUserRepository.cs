using _06032025_MVCDAY1.Models;


namespace _06032025_MVCDAY1.Repository
{
    public interface IUserRepository
    {

        //registration proccess

        void SaveOTP(string email, string otp);
        bool ValidateOTP(string email, string otp);

        void updatepassword(string Newpassword,string email);
        bool Register(User user);

        IEnumerable<Role> GetRoles();
        bool Login(string email, string password);

        //For fetching username with session
        User getSessionData(string email);

        //customer cart
        void AddToCart(int productId, int quantity, decimal price, int userId);
        void AddItemToCart(int userId, int productId, int quantity);
        void RemoveCartItem(int cartId);
        int GetCartItemCount(int userId);

        List<CartItemViewModel> GetCartItemsByUserId(int userId);
        CartItemViewModel GetCartItemById(int cartId, int userId);
        void UpdateCartItemQuantity(int cartId, int quantity);

        //customer Addresses 

        List<AddressViewModel> GetAddressesByUserId(int userId);
        void AddAddress(int userId, AddressViewModel model);

        //Customer WishList

        void AddToWishlist(int userId, int productId);
        void RemoveFromWishlist(int userId, int productId);
        bool IsInWishlist(int productId, int userID);
        List<Product> GetUserWishlist(int userId);
       // bool AddWishList(CustomerWishList customerWishList);

        //Product side methods

        
        
       // List<Product> GetAllProducts();
        
        List<ProductImage> GetImagesByProductId(int productId);

        //supplier contanctus

        //product review
        void SubmitReview(ProductReview review);

        //methods for user bill

        UserOrder GetOrderById(int orderId);
        List<OrderItem> GetOrderItemsByOrderId(int orderId);

        User GetUserById(int userId);

        AddressViewModel GetAddressById(int addressId);
        UserOrder GetOrderTrackingDetails(int orderId, int userId);
    }
}
