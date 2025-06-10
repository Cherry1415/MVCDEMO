namespace _06032025_MVCDAY1.Models
{
    public class SupplierDashboard
    {
        public int TotalOrders { get; set; }

        public int DeliveredOrders { get; set; }
        public int PendingOrders { get; set; }
        public int TodaysDeliveries { get; set; }
        public int VendorCount { get; set; }
        public int WarehouseCount { get; set; }

        //new updates from my side

        public int TotalAssigned { get; set; }
        public int Delivered { get; set; }
        public int InTransit { get; set; }
        public int Rejected { get; set; }
        public List<UserOrder> RecentOrders { get; set; }

        //for chart

        public List<OrderChartData> ChartData { get; set; }
    }
}
