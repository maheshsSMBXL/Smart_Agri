namespace Agri_Smart.Models
{
    public class YieldInput
    {
        public string? CoffeeVariant { get; set; }
        public double? Area { get; set; } // in acres
        public double? SoilMoisture { get; set; } // in percentage
        public double? Temperature { get; set; } // in Celsius
        public double? Rainfall { get; set; } // in mm
        public double? PestPresence { get; set; } // in percentage
        public double? EstimatedYield { get; set; }
    }
}
