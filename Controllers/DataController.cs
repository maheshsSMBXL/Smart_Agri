using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Agri_Smart.Models;
using Agri_Smart.data;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using InfluxDB.Client.Writes;

namespace Agri_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _context;
        private readonly InfluxDBClient _influxDBClient;
        private readonly string _bucket;
        private readonly string _org;
        private readonly IApplicationDbContext _dbcontext;

        public DataController(UserManager<IdentityUser> context, InfluxDBClient influxDBClient,
            IConfiguration configuration, IApplicationDbContext dbcontext)
        {
            _context = context;
            _influxDBClient = influxDBClient;
            _bucket = configuration["ConnectionStrings:InfluxDBBucket"];
            _org = configuration["ConnectionStrings:InfluxDBOrg"];
            _dbcontext = dbcontext;
        }

        [HttpGet]
        [Route("GetSensorData/{timeRangeStart}/{timeRangeStop}")]
        public async Task<IActionResult> GetSensorData(string timeRangeStart,string timeRangeStop)
        {

            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            //var timeRangeStart = "2024-01-01T06:02:00.000Z";
            //var timeRangeStop = "2090-07-01T06:02:00.000Z";
            string flux = $"from(bucket: \"{_bucket}\") " +
                $"|> range(start: {timeRangeStart}, stop: {timeRangeStop}) " +
                $"|> filter(fn: (r) => r[\"_measurement\"] == \"treesandrays_data\") " +
                $"|> filter(fn: (r) => r[\"tenant_id\"] == \"CC:7B:5C:35:32:9C\") ";
                //$"|> filter(fn: (r) => r[\"_field\"] == \"humidity_percentage\" or r[\"_field\"] == \"moisture\" or r[\"_field\"] == \"moisture_percentage\" or r[\"_field\"] == \"nitrogen\" or r[\"_field\"] == \"phosphorus\")";

            var fluxTables = await _influxDBClient.GetQueryApi().QueryAsync(flux, _org);

            var temperatureCelsius = new List<Dictionary<string, object>>();
            var temperatureFahrenheit = new List<Dictionary<string, object>>();
            var humidityPercentageValues = new List<Dictionary<string, object>>();
            var moistureValues = new List<Dictionary<string, object>>();
            var moisturePercentageValues = new List<Dictionary<string, object>>();
            var nitrogenValues = new List<Dictionary<string, object>>();
            var phosphorusValues = new List<Dictionary<string, object>>();
            var potassiumValues = new List<Dictionary<string, object>>();

            foreach (FluxTable table in fluxTables)
            {
                foreach (FluxRecord record in table.Records)
                {
                    // Log the raw record values for debugging
                    Console.WriteLine($"Raw Record Values: {string.Join(", ", record.Values.Select(kv => $"{kv.Key}: {kv.Value}"))}");

                    var field = record.Values["_field"].ToString();
                    var recordValues = new Dictionary<string, object>
                    {
                        { "_start", record.Values.ContainsKey("_start") ? record.Values["_start"].ToString() : null },
                        { "_stop", record.Values.ContainsKey("_stop") ? record.Values["_stop"].ToString() : null },
                        { "_time", record.Values.ContainsKey("_time") ? record.Values["_time"].ToString() : null }, // Extract _time as string
                        { "_value", record.Values["_value"] },
                        { "_field", record.Values["_field"] },
                        { "_measurement", record.Values["_measurement"] },
                        { "tenant_id", record.Values["tenant_id"] }
                    };

                    switch (field)
                    {
                        case "temperature_celsius":
                            temperatureCelsius.Add(recordValues);
                            break;
                        case "temperature_fahrenheit":
                            temperatureFahrenheit.Add(recordValues);
                            break;
                        case "humidity_percentage":
                            humidityPercentageValues.Add(recordValues);
                            break;
                        case "moisture":
                            moistureValues.Add(recordValues);
                            break;
                        case "moisture_percentage":
                            moisturePercentageValues.Add(recordValues);
                            break;
                        case "nitrogen":
                            nitrogenValues.Add(recordValues);
                            break;
                        case "phosphorus":
                            phosphorusValues.Add(recordValues);
                            break;
                        case "potassium":
                            potassiumValues.Add(recordValues);
                            break;
                    }
                }
            }

            var result = new
            {
                TemperatureCelsiusValues = temperatureCelsius,
                TemperatureFahrenheitValues = temperatureFahrenheit,
                HumidityPercentageValues = humidityPercentageValues,
                MoistureValues = moistureValues,
                MoisturePercentageValues = moisturePercentageValues,
                NitrogenValues = nitrogenValues,
                PhosphorusValues = phosphorusValues,
                PotassiumValues = potassiumValues,
            };

            return Ok(result);

        }
        [AllowAnonymous]
        [HttpPost]
        [Route("InsertUnstableData")]
        public async Task<IActionResult> InsertUnstableData([FromBody] List<UnstableData> sensorDataList)
        {
            // Ensure the sensorDataList is not null or empty
            if (sensorDataList == null || !sensorDataList.Any())
            {
                return BadRequest(new { message = "No data provided." });
            }

            // Get the write API
            using (var writeApi = _influxDBClient.GetWriteApi())
            {
                foreach (var sensorData in sensorDataList)
                {
                    // Create a point for each data item
                    var point = PointData
                        .Measurement("unstable_data_new")
                        .Tag("tenant_id", sensorData.TenantId)
                        .Field("soil_moisture_p", sensorData.Data.SoilMoistureP.HasValue ? sensorData.Data.SoilMoistureP.Value : double.NaN)
                        .Field("soil_moisture_f", sensorData.Data.SoilMoistureF.HasValue ? sensorData.Data.SoilMoistureF.Value : double.NaN)
                        .Field("temperature_c", sensorData.Data.TemperatureC.HasValue ? sensorData.Data.TemperatureC.Value : double.NaN)
                        .Field("temperature_f", sensorData.Data.TemperatureF.HasValue ? sensorData.Data.TemperatureF.Value : double.NaN)
                        .Field("humidity", sensorData.Data.Humidity.HasValue ? sensorData.Data.Humidity.Value : double.NaN)
                        .Field("nitrogen", sensorData.Data.Nitrogen.HasValue ? sensorData.Data.Nitrogen.Value : double.NaN)
                        .Field("potassium", sensorData.Data.Potassium.HasValue ? sensorData.Data.Potassium.Value : double.NaN)
                        .Field("phosphorus", sensorData.Data.Phosphorus.HasValue ? sensorData.Data.Phosphorus.Value : double.NaN)
                        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                    // Write the point to InfluxDB
                    writeApi.WritePoint(point, _bucket, _org);
                }
            }

            return Ok(new { message = "Data inserted successfully." });
        }


        [HttpGet]
        [Route("GetSensorLatestData")]
        public async Task<IActionResult> GetSensorLatestData()
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var timeRangeStart = "2024-01-01T06:02:00.000Z";
            var timeRangeStop = "2090-07-01T06:02:00.000Z";
            string flux = $"from(bucket: \"{_bucket}\") " +
                $"|> range(start: {timeRangeStart}, stop: {timeRangeStop}) " +
                $"|> filter(fn: (r) => r[\"_measurement\"] == \"treesandrays_data\") " +
                $"|> filter(fn: (r) => r[\"tenant_id\"] == \"CC:7B:5C:35:32:9C\") "; 
                //$"|> filter(fn: (r) => r[\"_field\"] == \"humidity_percentage\" or r[\"_field\"] == \"moisture\" or r[\"_field\"] == \"moisture_percentage\" or r[\"_field\"] == \"nitrogen\" or r[\"_field\"] == \"phosphorus\")";

            var fluxTables = await _influxDBClient.GetQueryApi().QueryAsync(flux, _org);

            var latestValues = new Dictionary<string, Dictionary<string, object>>();

            foreach (FluxTable table in fluxTables)
            {
                foreach (FluxRecord record in table.Records)
                {
                    // Log the raw record values for debugging
                    Console.WriteLine($"Raw Record Values: {string.Join(", ", record.Values.Select(kv => $"{kv.Key}: {kv.Value}"))}");

                    var field = record.Values["_field"].ToString();
                    var recordValues = new Dictionary<string, object>
            {
                { "_start", record.Values.ContainsKey("_start") ? record.Values["_start"].ToString() : null },
                { "_stop", record.Values.ContainsKey("_stop") ? record.Values["_stop"].ToString() : null },
                { "_time", record.Values.ContainsKey("_time") ? record.Values["_time"].ToString() : null }, // Extract _time as string
                { "_value", record.Values["_value"] },
                { "_field", record.Values["_field"] },
                { "_measurement", record.Values["_measurement"] },
                { "tenant_id", record.Values["tenant_id"] }
            };

                    if (latestValues.ContainsKey(field))
                    {
                        var existingTime = DateTime.Parse(latestValues[field]["_time"].ToString());
                        var newTime = DateTime.Parse(recordValues["_time"].ToString());
                        if (newTime > existingTime)
                        {
                            latestValues[field] = recordValues;
                        }
                    }
                    else
                    {
                        latestValues[field] = recordValues;
                    }
                }
            }

            var result = new
            {
                TemperatureCelsius = latestValues.ContainsKey("temperature_celsius") ? latestValues["temperature_celsius"]["_value"] : null,
                TemperatureFahrenheit = latestValues.ContainsKey("temperature_fahrenheit") ? latestValues["temperature_fahrenheit"]["_value"] : null,
                HumidityPercentage = latestValues.ContainsKey("humidity_percentage") ? latestValues["humidity_percentage"]["_value"] : null,
                Moisture = latestValues.ContainsKey("moisture") ? latestValues["moisture"]["_value"] : null,
                MoisturePercentage = latestValues.ContainsKey("moisture_percentage") ? latestValues["moisture_percentage"]["_value"] : null,
                Nitrogen = latestValues.ContainsKey("nitrogen") ? latestValues["nitrogen"]["_value"] : null,
                Phosphorus = latestValues.ContainsKey("phosphorus") ? latestValues["phosphorus"]["_value"] : null,
                Potassium = latestValues.ContainsKey("potassium") ? latestValues["potassium"]["_value"] : null
            };

            return Ok(result);
        }

        [HttpPost]
        [Route("SaveOnBoardData")]
        public async Task<IActionResult> SaveOnBoardData([FromBody] UserInfo request)
        {
            await _dbcontext.UserInfo.AddAsync(request);
            _dbcontext.SaveChanges();

            return Ok(new { Status = "Success", Message = "User OnBoard data saved successfully.", OnBoardStatus = request?.OnBoardingStatus });
        }
        [HttpGet]
        [Route("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
            return Ok(UserInfo);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SaveDiseases")]
        public async Task<IActionResult> SaveDiseases([FromBody] Diseases diseases)
        {
            await _dbcontext.Diseases.AddAsync(diseases);
            _dbcontext.SaveChanges();

            return Ok(new { Status = "Success", Message = "Data saved successfully." });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetDiseases")]
        public async Task<IActionResult> GetDiseases()
        {
            var Diseases = await _dbcontext.Diseases.ToListAsync();
            return Ok(Diseases);
        }
        [HttpPost]
        [Route("UpdateFireBaseToken/{fireBaseToken}")]
        public async Task<IActionResult> UpdateFireBaseToken(string fireBaseToken)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            if (userInfo != null) 
            {
                userInfo.FireBaseToken = fireBaseToken;
                _dbcontext.SaveChanges();
                return Ok(new { Status = "Success", Message = "Data saved successfully." });
            }

            return Ok();
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
