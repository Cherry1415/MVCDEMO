namespace _06032025_MVCDAY1.Models
{
    public class PaymentInitiateModel
    {
     

        public int Id { get; set; }
        public int Amount { get; set; }
        public int ProductId { get; set; } // optional, just in case
        public int Quantity { get; set; }
        public List<OrderItem> orderItems { get; set; }
        public int AddressId { get; set; }

    }
}
    