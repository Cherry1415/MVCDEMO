using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IOrdersRepository
    {
        IEnumerable<VendorStock> manageInventoty(int vendorId);
        VendorStock GetVendorStockById(int id);
        void UpdateVendorStock(VendorStock vs);
        UserOrder CreateOrder(int userId, decimal totalAmount, string razorpayOrderId, List<OrderItem> items);
        void UpdateOrderStatus(string razorpayOrderId,string status);
        void ClearCart(int userId);
    }
}
