namespace _06032025_MVCDAY1.Models
{
    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
