namespace Agri_Smart.Models
{
    public class CustomerRevenues
    {
        public RevenueDetails RevenueDetails { get; set; }
        public HarvestedAndSold HarvestedAndSold { get; set; }
    }
    public class RevenueDetails 
    {
        public string? Name { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Total { get; set; }
    }
    public class HarvestedAndSold
    {             
        public decimal? Quantity { get; set; }
        public string? QuantityUnits { get; set; }
        public decimal? Price { get; set; }
        public string? PriceUnits { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Total { get; set; }
    }
}
