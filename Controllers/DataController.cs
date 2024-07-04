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
        [Route("GetSensorData")]
        public async Task<IActionResult> GetSensorData()
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
            //var userInfo = new UserInfo();
            //userInfo.PhoneNumber = request.PhoneNumber;
            //userInfo.Name = request.Name;
            //userInfo.Email = request.Email;
            //userInfo.Country = request.Country;
            //userInfo.State = request.State;
            //userInfo.District = request.District;
            //userInfo.ZipCode = request.ZipCode;
            //userInfo.LandSize = request.LandSize;
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

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
