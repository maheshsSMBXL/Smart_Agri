using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class sensorsavgdata
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime? window_start { get; set; } = DateTime.MinValue;
        public DateTime? window_end { get; set; } = DateTime.MinValue;
        public string? macAddress { get; set; } = string.Empty;
        public double? avg_temperature { get; set; } = 0.0;
        public double? avg_temperatureF { get; set; } = 0.0;
        public double? avg_humidity { get; set; } = 0.0;
        public double? avg_soilMoistureValue { get; set; } = 0.0;
        public double? avg_soilMoisturePercent { get; set; } = 0.0;
        public double? avg_nitrogen { get; set; } = 0.0;
        public double? avg_phosphorous { get; set; } = 0.0;
        public double? avg_potassium { get; set; } = 0.0;
    }

}
