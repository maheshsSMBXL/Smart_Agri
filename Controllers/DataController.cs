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
        [Route("GetSensorData/{tenantId}/{timeRangeStart}/{timeRangeStop}")]
        public async Task<IActionResult> GetSensorData(string tenantId, string timeRangeStart,string timeRangeStop)
        {

            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
            var macId = await _dbcontext.Devices.Where(a => a.TenantId == tenantId).Select(a => a.MacId).FirstOrDefaultAsync();

            //var timeRangeStart = "2024-01-01T06:02:00.000Z";
            //var timeRangeStop = "2090-07-01T06:02:00.000Z";
            string flux = $"from(bucket: \"{_bucket}\") " +
                $"|> range(start: {timeRangeStart}, stop: {timeRangeStop}) " +
                $"|> filter(fn: (r) => r[\"_measurement\"] == \"treesandrays_data\") " +
                $"|> filter(fn: (r) => r[\"tenant_id\"] == \"{macId}\") ";
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
                        .Tag("tenant_id", sensorData.tenant_id)
                        .Field("soil_moisture_p", sensorData.unstable_data.soil_moisture_p.HasValue ? sensorData.unstable_data.soil_moisture_p.Value : double.NaN)
                        .Field("soil_moisture_f", sensorData.unstable_data.soil_moisture_f.HasValue ? sensorData.unstable_data.soil_moisture_f.Value : double.NaN)
                        .Field("temperature_c", sensorData.unstable_data.temperature_c.HasValue ? sensorData.unstable_data.temperature_c.Value : double.NaN)
                        .Field("temperature_f", sensorData.unstable_data.temperature_f.HasValue ? sensorData.unstable_data.temperature_f.Value : double.NaN)
                        .Field("humidity", sensorData.unstable_data.humidity.HasValue ? sensorData.unstable_data.humidity.Value : double.NaN)
                        .Field("nitrogen", sensorData.unstable_data.nitrogen.HasValue ? sensorData.unstable_data.nitrogen.Value : double.NaN)
                        .Field("potassium", sensorData.unstable_data.potassium.HasValue ? sensorData.unstable_data.potassium.Value : double.NaN)
                        .Field("phosphorus", sensorData.unstable_data.phosphorus.HasValue ? sensorData.unstable_data.phosphorus.Value : double.NaN)
                        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                    // Write the point to InfluxDB
                    writeApi.WritePoint(point, _bucket, _org);
                }
            }

            return Ok(new { Status = "Success", Message = "Data Saved Successfully." });
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("SaveDeviceUnstableData")]
        public async Task<IActionResult> SaveDeviceUnstableData([FromBody] List<UnstableData> deviceDataList)
        {
            foreach (var deviceData in deviceDataList)
            {
                var deviceUnstableData = new DeviceUnstableData();
                deviceUnstableData.MacId = deviceData?.tenant_id;
                deviceUnstableData.SoilMoistureP = deviceData?.unstable_data?.soil_moisture_p;
                deviceUnstableData.SoilMoistureF = deviceData?.unstable_data?.soil_moisture_f;
                deviceUnstableData.TemperatureC = deviceData?.unstable_data?.temperature_c;
                deviceUnstableData.TemperatureF = deviceData?.unstable_data?.temperature_f;
                deviceUnstableData.Humidity = deviceData?.unstable_data?.humidity;
                deviceUnstableData.Nitrogen = deviceData?.unstable_data?.nitrogen;
                deviceUnstableData.Potassium = deviceData?.unstable_data?.potassium;
                deviceUnstableData.Phosphorus = deviceData?.unstable_data?.phosphorus;
                await _dbcontext.DeviceUnstableData.AddAsync(deviceUnstableData);
                _dbcontext.SaveChanges();
            }

            return Ok(new { Status = "Success", Message = "Data Saved Successfully." });
        }


        [HttpGet]
        [Route("GetSensorLatestData/{tenantId}")]
        public async Task<IActionResult> GetSensorLatestData(string tenantId)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var macId = await _dbcontext.Devices.Where(a => a.TenantId == tenantId).Select(a => a.MacId).FirstOrDefaultAsync();
            //var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var timeRangeStart = "2024-01-01T06:02:00.000Z";
            var timeRangeStop = "2090-07-01T06:02:00.000Z";
            string flux = $"from(bucket: \"{_bucket}\") " +
                $"|> range(start: {timeRangeStart}, stop: {timeRangeStop}) " +
                $"|> filter(fn: (r) => r[\"_measurement\"] == \"treesandrays_data\") " +
                $"|> filter(fn: (r) => r[\"tenant_id\"] == \"{macId}\") "; 
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
                TemperatureCelsius = latestValues.ContainsKey("temperature_celsius") ? FormatDouble(latestValues["temperature_celsius"]["_value"]) : null,
                TemperatureFahrenheit = latestValues.ContainsKey("temperature_fahrenheit") ? FormatDouble(latestValues["temperature_fahrenheit"]["_value"]) : null,
                HumidityPercentage = latestValues.ContainsKey("humidity_percentage") ? FormatDouble(latestValues["humidity_percentage"]["_value"]) : null,
                Moisture = latestValues.ContainsKey("moisture") ? FormatDouble(latestValues["moisture"]["_value"]) : null,
                MoisturePercentage = latestValues.ContainsKey("moisture_percentage") ? FormatDouble(latestValues["moisture_percentage"]["_value"]) : null,
                Nitrogen = latestValues.ContainsKey("nitrogen") ? FormatDouble(latestValues["nitrogen"]["_value"]) : null,
                Phosphorus = latestValues.ContainsKey("phosphorus") ? FormatDouble(latestValues["phosphorus"]["_value"]) : null,
                Potassium = latestValues.ContainsKey("potassium") ? FormatDouble(latestValues["potassium"]["_value"]) : null
            };

            return Ok(result);
        }
        private string FormatDouble(object value)
        {
            double result;
            if (value is double)
            {
                result = (double)value;
            }
            else if (double.TryParse(value.ToString(), out result))
            {
                // No further action needed, as 'result' has already been assigned
            }
            else
            {
                return value?.ToString();
            }
            return result.ToString("F1");
        }

        [HttpGet]
        [Route("GetSensorData1/{tenantId}")]
        public async Task<IActionResult> GetSensorData1(string tenantId)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var macId = await _dbcontext.Devices.Where(a => a.TenantId == tenantId).Select(a => a.MacId).FirstOrDefaultAsync();
            //var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var timeRangeStart = "2024-07-21T06:02:00.000Z";
            var timeRangeStop = "2024-07-23T06:02:00.000Z";
            string flux = $"from(bucket: \"{_bucket}\") " +
                $"|> range(start: {timeRangeStart}, stop: {timeRangeStop}) " +
                $"|> filter(fn: (r) => r[\"_measurement\"] == \"treesandrays_data\") " +
                $"|> filter(fn: (r) => r[\"tenant_id\"] == \"{macId}\")";

            var fluxTables = await _influxDBClient.GetQueryApi().QueryAsync(flux, _org);

            var allRecords = new List<Dictionary<string, object>>();

            var Result = new List<Dictionary<string, object>>();

            foreach (FluxTable table in fluxTables)
            {
                foreach (FluxRecord record in table.Records)
                {
                    var field = record.Values["_field"].ToString();
                    var recordValues = new Dictionary<string, object>
                    {
                        { "_start", record.Values.ContainsKey("_start") ? record.Values["_start"].ToString() : null },
                        { "_stop", record.Values.ContainsKey("_stop") ? record.Values["_stop"].ToString() : null },
                        { "_time", record.Values.ContainsKey("_time") ? record.Values["_time"].ToString() : null },
                        { "_value", record.Values["_value"] },
                        { "_field", record.Values["_field"] },
                        { "_measurement", record.Values["_measurement"] },
                        { "tenant_id", record.Values["tenant_id"] }
                    };
                    allRecords.Add(recordValues);

                    var resultValues = new Dictionary<string, object>
                    {
                        { "tenantId", record.Values["tenant_id"] },
                        { "field", record.Values["_field"] },
                        { "value", record.Values["_value"] }                        
                    };
                    Result.Add(resultValues);

                    //var deviceUnstableData = new DeviceUnstableData();
                    //switch (field)
                    //{
                    //    case "temperature_celsius":
                    //        deviceUnstableData.TemperatureC = (double?)record.Values["_value"];
                    //        break;
                    //    case "temperature_fahrenheit":
                    //        deviceUnstableData.TemperatureF = (double?)record.Values["_value"];
                    //        break;
                    //    case "humidity_percentage":
                    //        deviceUnstableData.Humidity = (double?)record.Values["_value"];
                    //        break;
                    //    case "moisture":
                    //        deviceUnstableData.SoilMoistureP = (double?)record.Values["_value"];
                    //        break;
                    //    case "moisture_percentage":
                    //        deviceUnstableData.SoilMoistureF = (double?)record.Values["_value"];
                    //        break;
                    //    case "nitrogen":
                    //        deviceUnstableData.Nitrogen = (double?)record.Values["_value"];
                    //        break;
                    //    case "phosphorus":
                    //        deviceUnstableData.Phosphorus = (double?)record.Values["_value"];
                    //        break;
                    //    case "potassium":
                    //        deviceUnstableData.Potassium = (double?)record.Values["_value"];
                    //        break;
                    //}
                    //Result.Add(deviceUnstableData);
                }
            }

            return Ok(Result.ToList());
        }

        [HttpGet]
        [Route("GetSensorAverageData")]
        public async Task<IActionResult> GetSensorAverageData()
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var UserInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);
            var macId = await _dbcontext.Devices.Where(a => a.TenantId == UserInfo.TenantId).Select(a => a.MacId).FirstOrDefaultAsync();

            var SensorAgerageData = await _dbcontext.sensorsavgdata.ToListAsync();
            return Ok(SensorAgerageData);
        }

        [HttpPost]
        [Route("SaveOnBoardData")]
        public async Task<IActionResult> SaveOnBoardData([FromBody] UserInfo request)
        {
            request.UserCreatedDate = DateTime.Now;
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
            var tenants = await _dbcontext.Devices.Where(a => a.PhoneNumber == mobileNumber).Select(a => a.TenantId).ToListAsync();

            var userInfoOutPut = new UserInfoOutPut();
            userInfoOutPut.UserInfo = UserInfo;
            userInfoOutPut.Tenants = tenants;

            //UserInfo.TenantId = string.Join(",", tenants);
            return Ok(userInfoOutPut);
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
