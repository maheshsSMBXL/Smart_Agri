using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class CustomerRevenue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RevenueCategoryId { get; set; } = Guid.Empty;
        public string? RevenueCategoryName { get; set; } = string.Empty;
        public Guid ActivityId { get; set; } = Guid.Empty;
        public string? Name { get; set; } = string.Empty;
        public decimal? Price { get; set; } = 0.0m;
        public string? PriceUnits { get; set; } = string.Empty;
        public decimal? Quantity { get; set; } = 0.0m;
        public string? QuantityUnits { get; set; } = string.Empty;
        public DateTime? Date { get; set; } = DateTime.Now;
        public decimal? Total { get; set; } = 0.0m;
        public decimal? ActivityTotal { get; set; } = 0.0m;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public Guid? UserID { get; set; } = Guid.Empty;
        public Guid? CreatedBy { get; set; } = Guid.Empty;
    }

}
