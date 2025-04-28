using System.ComponentModel.DataAnnotations;

namespace _06032025_MVCDAY1.Models
{
    public class SupplierContactus
    {
       
        
            [Required(ErrorMessage = "User ID is required")]
            public int user_id { get; set; }

            [Required(ErrorMessage = "Message is required")]
            public string Message { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    }
}
