using _06032025_MVCDAY1.Models;

namespace _06032025_MVCDAY1.Repository
{
    public interface IAdminRepository
    {

        //AuditLogs
        void Log(AdminAuditLog log);
        List<AdminAuditLog> GetAllLogs();

        //dashboard method
        DashboardViewModel GetDashboardData();

        //all order show status wise
        List<AdminOrderViewModel> GetAllOrders(string status);

        //all product show
        List<AdminProductViewModel> GetAllProducts();

        //allcategory 
        List<Category> GetAll();
        //category by id
        Category GetById(int id);

        //add category
        void Add(Category category);

        //update category
        void Update(Category category);

        //delete category
        void Delete(int id);

        //all subcategory

        List<Subcategory> GetAllSubCategory();
        Subcategory subcategoryGetById(int id);
        void Savesubcategory(Subcategory subcategory);
        void Deletesubcategory(int id);

        List<Product> GetVendorProductApproval();
        void ApproveProduct(int productId);
        void RejectProduct(int productId);

        //Payments methods
        List<WeekSalesViewModel> GetWeeklySales(int month, int year);
        AdminPayoutSummary GetPayoutSummary(int month, int year);
        List<AdminPaymentViewModel> GetAllPaymentsWithOrders();

        List<OrderItem> GetOrderItemsByOrderId(int orderId);

        List<UserOrder> GetFilteredOrders(DateTime? fromDate, DateTime? toDate, string orderStatus);


        
    }
}
