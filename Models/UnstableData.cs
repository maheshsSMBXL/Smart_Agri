namespace Agri_Smart.Models
{
    public class UnstableData
    {
        public string? tenant_id { get; set; }
        public UnstableDataModel? unstable_data { get; set; }
    }
    public class UnstableDataModel
    {
        public double? soil_moisture_p { get; set; }
        public double? soil_moisture_f { get; set; }
        public double? temperature_c { get; set; }
        public double? temperature_f { get; set; }
        public double? humidity { get; set; }
        public double? nitrogen { get; set; }
        public double? potassium { get; set; }
        public double? phosphorus { get; set; }
    }
}
