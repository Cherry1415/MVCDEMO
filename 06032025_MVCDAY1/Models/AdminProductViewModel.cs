namespace _06032025_MVCDAY1.Models
{
    public class AdminProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public string subcategory { get; set; }
        public decimal Price { get; set; }

        public string SellerName { get; set; }
        public bool IsActive { get; set; }
    }
}
