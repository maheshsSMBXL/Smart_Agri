namespace Agri_Smart.Models
{
    public class CustomerRevenues
    {
        public Guid? ActivityId { get; set; }
        public List<RevenueDetails>? RevenueDetails { get; set; }
        public List<HarvestedAndSold>? HarvestedAndSold { get; set; }
    }
    public class RevenueDetails 
    {
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? RevenueName { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Total { get; set; }
    }
    public class HarvestedAndSold
    {
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal? Quantity { get; set; }
        public string? QuantityUnits { get; set; }
        public decimal? Price { get; set; }
        public string? PriceUnits { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Total { get; set; }
    }
}
