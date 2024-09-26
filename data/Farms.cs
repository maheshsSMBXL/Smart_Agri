using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Farms
    {
        [Key]
        public Guid FarmId { get; set; } = Guid.NewGuid();
        public Guid? EstateId { get; set; } = Guid.NewGuid();
        public string? FarmName { get; set; } = string.Empty;
        public double? LandSize { get; set; } = 0.0;
        public DateTime? CropGrowingStartDate { get; set; } = DateTime.MinValue;
        public string? SoilType { get; set; } = string.Empty;
        public string? CropType { get; set; } = string.Empty;
        public string? CropVariety { get; set; } = string.Empty;
        public double? Latitude { get; set; } = 0.0;
        public double? Longitude { get; set; } = 0.0;
        public string? State { get; set; } = string.Empty;
        public string? ZipCode { get; set; } = string.Empty;
        public string? FarmAddress { get; set; } = string.Empty;
        public double? BudgetAmount { get; set; } = 0.0;
        public bool? DeviceStatus { get; set; } = false;
        //public string? FireBaseToken { get; set; } = string.Empty;

    }
}
