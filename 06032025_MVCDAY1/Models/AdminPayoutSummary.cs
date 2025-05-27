namespace _06032025_MVCDAY1.Models
{
    public class AdminPayoutSummary
    {
        public List<WeekSalesViewModel> Weekly { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Payout { get; set; }
        public decimal Pending { get; set; }
        public string MonthName { get; set; }
    }
}
