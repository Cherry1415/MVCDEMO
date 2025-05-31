namespace _06032025_MVCDAY1.Models
{
    public class ProductFilterModel
    {
        public List<string> AvailableBrands { get; set; }
        public List<string> SelectedBrands { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPriceRange { get; set; }
        public decimal MaxPriceRange { get; set; }
    }
}
