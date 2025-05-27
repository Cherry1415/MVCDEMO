namespace _06032025_MVCDAY1.Models
{
    public class OrderReportViewModel
    {
        // Filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string OrderStatus { get; set; }

        // Results
        public List<UserOrder> Orders { get; set; } = new List<UserOrder>();
    }
}
