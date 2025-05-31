namespace _06032025_MVCDAY1.Models
{
    public class DashboardViewModel
    {
        // Models/DashboardViewModel.cs
       
            public int TotalOrders { get; set; }
            public int PendingCount { get; set; }
            public int DeliveredCount { get; set; }
            public int CompletedCount { get; set; }
            public int RejectCount { get; set; }

        public List<TopProduct> TopProducts { get; set; } = new List<TopProduct>();
        public int WeekNumber { get; set; }
        public decimal TotalSales { get; set; }
        //public class TopProduct
        //{
        //    public string Name { get; set; }
        //    public int Count { get; set; }
        //}

    }
}
