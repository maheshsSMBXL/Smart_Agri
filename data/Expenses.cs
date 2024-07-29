using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Expenses
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ActivityId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDate { get; set; }
        public DateTime? EstimatedHarvestDate { get; set;}
        public double? FuelCost { get; set; }
        public string? TotalCost { get; set; }
        public DateTime? CreatedDate { get; set;}
        public Guid? UserID { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
