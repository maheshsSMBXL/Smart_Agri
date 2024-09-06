using Newtonsoft.Json;

namespace Agri_Smart.Services
{
    public class JsonSerializerSettingsService
    {
        public JsonSerializerSettings JsonSettings { get; }

        public JsonSerializerSettingsService()
        {
            JsonSettings = new JsonSerializerSettings
            {
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
                FloatParseHandling = FloatParseHandling.Double,
                Formatting = Formatting.Indented,
                //NullValueHandling = NullValueHandling.Include,
                //DefaultValueHandling = DefaultValueHandling.Populate
            };
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSettings);
        }
    }
}
