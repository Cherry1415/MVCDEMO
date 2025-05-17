namespace _06032025_MVCDAY1.Models
{
    public class UserOrder
    {
       
        public int Id { get; set; }
        public int UserId { get; set; }             // Assuming you track logged-in user
       // public int ProductId { get; set; }          // Purchased product
        public string RazorpayOrderId { get; set; } // Razorpay's order ID
      //  public string? PaymentId { get; set; }      // Razorpay's payment ID (set after success)
        public decimal TotalAmount { get; set; }    // In INR
        public string Status { get; set; }          // e.g., "Pending", "Paid", "Failed"
        public DateTime CreatedDate { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // List of purchased items
        public string addressid { get; set; }

    }
}
