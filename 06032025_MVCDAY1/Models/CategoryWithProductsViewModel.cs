namespace _06032025_MVCDAY1.Models
{
    public class CategoryWithProductsViewModel
    {
        public int category_id { get; set; }
        public string name { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
