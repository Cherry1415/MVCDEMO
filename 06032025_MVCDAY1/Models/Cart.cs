namespace _06032025_MVCDAY1.Models
{
    public class Cart
    {
        public int cart_item_id { get; set; }
        public int user_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
