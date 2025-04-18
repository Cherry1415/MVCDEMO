namespace _06032025_MVCDAY1.Models
{
    public class CartItemViewModel
    {
        public int cart_item_id { get; set; }
        public int user_id { get; set; }
        public int product_id { get; set; }

        public string product_name { get; set; }
        public decimal price { get; set; }

        public string imgName { get; set; }
      //  public string ImagePath { get; set; }
        public int quantity { get; set; }
        public decimal TotalPrice => price * quantity;
    }
}
