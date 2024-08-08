using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class sensorsavgdata
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? window_start { get; set; }
        public DateTime? window_end { get; set; }
        public string? macAddress { get; set; }
        public double? avg_temperature { get; set; }
        public double? avg_temperatureF { get; set; }
        public double? avg_humidity { get; set; }
        public double? avg_soilMoistureValue { get; set; }
        public double? avg_soilMoisturePercent { get; set; }
        public double? avg_nitrogen { get; set; }
        public double? avg_phosphorous { get; set; }
        public double? avg_potassium { get; set; }
    }
}
