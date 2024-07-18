namespace Agri_Smart.Models
{
    public class UnstableData
    {
        public string? TenantId { get; set; }
        public UnstableDataModel? Data { get; set; }
    }
    public class UnstableDataModel
    {
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
