using _06032025_MVCDAY1.Models;
using Microsoft.Data.SqlClient;

namespace _06032025_MVCDAY1.Repository
{
    public interface ISupplierRepository
    {
        SupplierDashboard GetDashboardCounts();
        
        bool AddContactMessage(SupplierContactus contact);
        List<SupplierOrder> GetAllVendorOrders();
        bool UpdateOrderStatus(int orderId, string newStatus);
        IEnumerable<SupplierVendor> GetAll();
        bool AddVendor(SupplierVendor vendor);
        SupplierVendor GetVendor(int id);
        IEnumerable<WareHouse> GetAllwarehouse();
        WareHouse GetbyId(int Id);
        bool AddWareHouse(WareHouse wareHouse);
        bool EditWareHouse(WareHouse wareHouse); // ✅ Updated from `void` to `bool`
        bool DeleteWareHouse(int id);

        WareHouse_details GetWareHouse_details(int warehouse_id);

        bool AddWareHouseDetails(WareHouse_details warehouseDetails);

        //Add by my side

        List<UserOrder> GetOrdersAssignedToSupplier(int supplierId);

        int GetAssignedOrdersCount(int supplierId);
        int GetDeliveredOrdersCount(int supplierId);
        int GetInTransitOrdersCount(int supplierId);
        int GetRejectedOrdersCount(int supplierId);
        List<UserOrder> GetRecentOrders(int supplierId);
        List<OrderChartData> GetOrdersLast7Days(int supplierId); //for chart
        List<UserOrder> GetTodayDeliveries(int supplierId);

        bool CancelSupplierOrder(int orderId, string reason, string comment);
        bool MarkRetryDelivery(int orderId);
    }
}
