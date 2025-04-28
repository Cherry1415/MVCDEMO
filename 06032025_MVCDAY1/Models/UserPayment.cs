namespace _06032025_MVCDAY1.Models
{
    public class UserPayment
    {
        public int Id { get; set; }
        public string PaymentId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
