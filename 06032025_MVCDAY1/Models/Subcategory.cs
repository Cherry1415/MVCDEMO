namespace _06032025_MVCDAY1.Models
{
    public class Subcategory
    {
        public int sub_category_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int category_id { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }


    }
}
