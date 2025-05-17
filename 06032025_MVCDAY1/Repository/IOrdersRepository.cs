using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IOrdersRepository
    {
        IEnumerable<VendorStock> manageInventoty(int vendorId);
        UserOrder GetUserOrders(int userId);
        VendorStock GetVendorStockById(int id);
        IEnumerable<VendorStock> GetShortageItems(int threshold);
        void UpdateVendorStock(VendorStock vs);
        UserOrder CreateOrder(int userId,decimal totalAmount, string razorpayOrderId, List<OrderItem> items,int addressid);
        void UpdateOrderStatus(string razorpayOrderId,string status);
        void ClearCart(int userId);
        List<UserOrder> GetUserOrdersWithItemsAndImages(int userId);

    }
}
