using System.ComponentModel.DataAnnotations.Schema;

namespace _06032025_MVCDAY1.Models
{
    public class Product
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string category_name { get; set; }

        public string brand_name { get; set; }
        public string subcat_name { get; set; }
        public int brand_id { get; set; }
        public int category_id { get; set; }
        public int vendor_id { get; set; }
        public decimal price { get; set; }
       
        public int sub_category_id { get; set; }
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public List<Prod_Attributes> Prod_Attributes { get; set; } = new List<Prod_Attributes>();
        public List<VendorStock> VendorStock { get; set; } = new List<VendorStock>();

        [NotMapped]
        public bool IsInWishlist { get; set; }
      //  public List<AddressViewModel> addressViewModels { get; set; } = new List<AddressViewModel>();
    }
}
