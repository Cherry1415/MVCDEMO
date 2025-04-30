
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("category", Schema = "admin")]
public class category
{
    [Key]
    public int category_Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public DateTime? Created_At { get; set; }
    public DateTime? Updated_At { get; set; }
}


public class Comment
{
    [Key]
    public int comment_id { get; set; }
    public int? user_id { get; set; }
    public int? product_id { get; set; }
    public string comment { get; set; }
    public int? rating { get; set; }
    public DateTime comment_date { get; set; }
    public string status { get; set; }

    // Optional: Enriched fields for display purposes (if joined from user/product tables)
    public string user_Name { get; set; }
    public string product_Name { get; set; }
}

