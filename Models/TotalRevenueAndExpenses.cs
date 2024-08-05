namespace Agri_Smart.Models
{
    public class TotalRevenueAndExpenses
    {
        public decimal? TotalExpenses { get; set; }
        public CategorisedExpenses CategorisedExpenses { get; set; } = new CategorisedExpenses(); // Initialize the property
        public decimal? TotalRevenue { get; set; }
        public CategorisedRevenue CategorisedRevenues { get; set; } = new CategorisedRevenue(); // Initialize the property
        public decimal? Budget { get; set; }
    }
    public class CategorisedExpenses
    {
        public decimal? Workers { get; set; }
        public decimal? Machinery { get; set; }
        public decimal? OtherExpenses { get; set; }
    }
    public class CategorisedRevenue
    {
        public decimal? RevenueDetails { get; set; }
        public decimal? HarvestedAndSold { get; set; }
    }
}
