using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class EstimatedYield
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? CoffeeVariant { get; set; }
        public double? Area { get; set; }
        public double? SoilMoisture { get; set; }
        public double? Temperature { get; set; }
        public double? Rainfall { get; set; }
        public double? PestPresence { get; set; }
        public double? FinalEstimatedYield { get; set; }
    }
}
