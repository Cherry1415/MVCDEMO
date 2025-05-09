
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace _06032025_MVCDAY1.Models
{
   // [Table("Warehouse", Schema = "Supplier")]
    public class WareHouse
    {
        [Key]
        public int warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        
        public int capacity { get; set; }

        public string phone { get; set; }
    }
}
