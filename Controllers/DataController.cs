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

namespace Agri_Smart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _context;
        private readonly InfluxDBClient _influxDBClient;
        private readonly string _bucket;
        private readonly string _org;

        public DataController(UserManager<IdentityUser> context, InfluxDBClient influxDBClient, IConfiguration configuration)
        {
            _context = context;
            _influxDBClient = influxDBClient;
            _bucket = configuration["ConnectionStrings:InfluxDBBucket"];
            _org = configuration["ConnectionStrings:InfluxDBOrg"];
        }

        [HttpPost]
        [Route("GetSensorData")]
        public async Task<IActionResult> GetSensorData([FromBody]InfluxBucketRange bucketRange)
        {
            string flux = $"from(bucket: \"{_bucket}\") |> range(start: {bucketRange.start}, stop: {bucketRange.stop}) |> filter(fn: (r) => r[\"_measurement\"] == \"sensor_data\") |> filter(fn: (r) => r[\"tenant_id\"] == \"{bucketRange.tenantId}\")";
            var fluxTables = await _influxDBClient.GetQueryApi().QueryAsync(flux, _org);

            var result = new List<Dictionary<string, object>>();
            foreach (FluxTable table in fluxTables)
            {
                foreach (FluxRecord record in table.Records)
                {
                    result.Add(record.Values);
                }
            }
            return Ok(result);
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
