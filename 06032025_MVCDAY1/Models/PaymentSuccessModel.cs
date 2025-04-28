namespace _06032025_MVCDAY1.Models
{
    public class PaymentSuccessModel
    {
        public string razorpay_payment_id { get; set; }
        public string razorpay_order_id { get; set; }
        public string razorpay_signature { get; set; }
        public decimal amount { get; set; }
    }
}
