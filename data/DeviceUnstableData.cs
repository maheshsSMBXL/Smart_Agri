using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class DeviceUnstableData
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? MacId { get; set; } = string.Empty;
        public string? TenantId { get; set; } = string.Empty;
        public double? SoilMoistureP { get; set; } = 0.0;
        public double? SoilMoistureF { get; set; } = 0.0;
        public double? TemperatureC { get; set; } = 0.0;
        public double? TemperatureF { get; set; } = 0.0;
        public double? Humidity { get; set; } = 0.0;
        public double? Nitrogen { get; set; } = 0.0;
        public double? Potassium { get; set; } = 0.0;
        public double? Phosphorus { get; set; } = 0.0;
    }

}
