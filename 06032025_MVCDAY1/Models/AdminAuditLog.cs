namespace _06032025_MVCDAY1.Models
{
    public class AdminAuditLog
    {
        public int Id { get; set; }
        public int userid { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
