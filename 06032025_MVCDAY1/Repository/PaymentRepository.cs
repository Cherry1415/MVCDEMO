using Microsoft.Data.SqlClient;

namespace _06032025_MVCDAY1.Repository
{
    public class PaymentRepository:IPaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void SavePayment(string razorpayOrderId, string razorpayPaymentId, decimal amount, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO customer.Payment (RazorpayOrderId, PaymentId, payment_status, amount, PaidOn) " +
                               "VALUES (@RazorpayOrderId, @RazorpayPaymentId, @Status, @Amount, GETDATE())";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);
                command.Parameters.AddWithValue("@RazorpayPaymentId", razorpayPaymentId);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@Amount", amount);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        
    }
}
