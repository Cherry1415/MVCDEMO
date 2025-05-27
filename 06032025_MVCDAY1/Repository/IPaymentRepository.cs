namespace _06032025_MVCDAY1.Repository
{
    public interface IPaymentRepository
    {
        void SavePayment(string razorpayOrderId, string razorpayPaymentId, decimal amount, string status);
        
    }
}
