using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class DeviceUnstableData
    {
        [Key]
        public Guid Id { get; set; }
        public string? MacId { get; set; }
        public string? TenantId { get; set; }
        public double? SoilMoistureP { get; set; }
        public double? SoilMoistureF { get; set; }
        public double? TemperatureC { get; set; }
        public double? TemperatureF { get; set; }
        public double? Humidity { get; set; }
        public double? Nitrogen { get; set; }
        public double? Potassium { get; set; }
        public double? Phosphorus { get; set; }
    }
}
