namespace Agri_Smart.Models
{
    public class SensorDataOutPut
    {
        public string TenantId { get; set; }
        public string DeviceName { get; set; }
        public double TemperatureCelsius { get; set; }
        public double TemperatureFahrenheit { get; set; }
        public double HumidityPercentage { get; set; }
        public double Moisture { get; set; }
        public double MoisturePercentage { get; set; }
        public double Nitrogen { get; set; }
        public double Phosphorus { get; set; }
        public double Potassium { get; set; }
    }
}
