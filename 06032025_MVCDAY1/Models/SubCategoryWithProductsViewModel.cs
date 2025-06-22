namespace _06032025_MVCDAY1.Models
{
    public class SubCategoryWithProductsViewModel
    {
        public int subcategory_id { get; set; }
        public string subcatname { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
