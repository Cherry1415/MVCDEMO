namespace _06032025_MVCDAY1.Models
{
    public class Product
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int brand_id { get; set; }
        public int category_id { get; set; }
        public int vendor_id { get; set; }
        public decimal price { get; set; }
        public int product_desc_id { get; set; }
        public int prod_img_id { get; set; }

        

        public int sub_category_id { get; set; }
    }
}
