namespace _06032025_MVCDAY1.Models
{
    public class CustomerWishList
    {
        public int wl_id { get; set; }
        public int user_id { get; set; }
        public int product_id { get; set; }
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
