namespace Agri_Smart.Models
{
    public class SensorDataOutPut
    {
        public string TenantId { get; set; }
        public string DeviceName { get; set; }
        public string TemperatureCelsius { get; set; }
        public string TemperatureFahrenheit { get; set; }
        public string HumidityPercentage { get; set; }
        public string Moisture { get; set; }
        public string MoisturePercentage { get; set; }
        public string Nitrogen { get; set; }
        public string Phosphorus { get; set; }
        public string Potassium { get; set; }
    }
}
