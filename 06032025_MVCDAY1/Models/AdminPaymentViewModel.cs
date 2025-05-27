namespace _06032025_MVCDAY1.Models
{
    public class AdminPaymentViewModel
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public int ModeId { get; set; }
        public DateTime PaidOn { get; set; }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }

        public string RazorpayOrderId { get; set; }

        // Details
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
