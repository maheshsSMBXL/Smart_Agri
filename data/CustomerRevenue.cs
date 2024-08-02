using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class CustomerRevenue
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RevenueCategoryId { get; set; }
        public string? RevenueCategoryName { get; set; }
        public Guid ActivityId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }   
        public string? PriceUnits { get; set; }
        public decimal? Quantity { get; set; }
        public string? QuantityUnits { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Total { get; set; }
        public decimal? ActivityTotal { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UserID { get; set; }
        public Guid? CreatedBy { get; set; }

    }
}
