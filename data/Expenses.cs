using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Expenses
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CategoryId { get; set; } = Guid.Empty;
        public Guid ActivityId { get; set; } = Guid.Empty;
        public string? CategoryName { get; set; } = string.Empty;
        public string? CategoryDate { get; set; } = string.Empty;
        public DateTime? EstimatedHarvestDate { get; set; } = DateTime.MinValue;
        public double? FuelCost { get; set; } = 0.0;
        public double? TotalCost { get; set; } = 0.0;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public Guid? UserID { get; set; } = Guid.Empty;
        public Guid? CreatedBy { get; set; } = Guid.Empty;
    }

}
