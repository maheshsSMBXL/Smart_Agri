using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class EstimatedYield
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } = Guid.Empty;
        public string? CoffeeVariant { get; set; } = string.Empty;
        public double? Area { get; set; } = 0.0;
        public double? SoilMoisture { get; set; } = 0.0;
        public double? Temperature { get; set; } = 0.0;
        public double? Rainfall { get; set; } = 0.0;
        public double? PestPresence { get; set; } = 0.0;
        public double? FinalEstimatedYield { get; set; } = 0.0;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }

}
