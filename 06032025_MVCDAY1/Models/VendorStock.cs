namespace _06032025_MVCDAY1.Models
{
    public class VendorStock
    {
        public int stock_id { get; set; }
        public int product_id { get; set; }
        public int quantity_available { get; set; }
        public DateTime stock_in_date { get; set; }
        public DateTime restock_date { get; set; }
        public string stock_status { get; set; }
        public string product_name { get; set; }
    }
}
