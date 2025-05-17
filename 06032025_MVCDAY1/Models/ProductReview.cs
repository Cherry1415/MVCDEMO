namespace _06032025_MVCDAY1.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }         // Product being reviewed
        public int UserId { get; set; }            // User who is giving the review
        public int Rating { get; set; }            // Rating from 1 to 5
        public string Review { get; set; }         // User review text
        public DateTime CreatedDate { get; set; }  // When the review was created

        public string product_name { get; set; }
        public string username { get; set; }
        //public string ImgName { get; set; }
    }
}
