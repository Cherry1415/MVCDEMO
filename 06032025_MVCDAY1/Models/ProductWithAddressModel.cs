namespace _06032025_MVCDAY1.Models
{
    public class ProductWithAddressModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }
}
