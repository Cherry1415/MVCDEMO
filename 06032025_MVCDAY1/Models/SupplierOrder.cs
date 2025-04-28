using System.ComponentModel.DataAnnotations;

namespace _06032025_MVCDAY1.Models
{
    public class SupplierOrder
    {
        [Key]
        public int order_id { get; set; }

        public int user_id { get; set; }

        public DateOnly order_date { get; set; }

        public DateOnly required_date { get; set; }

        public int? address_id { get; set; }

        public string? status { get; set; }
    }
}
