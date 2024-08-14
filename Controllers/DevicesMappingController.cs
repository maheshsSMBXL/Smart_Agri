using Agri_Smart.data;
using Agri_Smart.Models;
using InfluxDB.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesMappingController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _context;
        private readonly InfluxDBClient _influxDBClient;
        private readonly string _bucket;
        private readonly string _org;
        private readonly IApplicationDbContext _dbcontext;

        public DevicesMappingController(UserManager<IdentityUser> context, InfluxDBClient influxDBClient,
            IConfiguration configuration, IApplicationDbContext dbcontext)
        {
            _context = context;
            _influxDBClient = influxDBClient;
            _bucket = configuration["ConnectionStrings:InfluxDBBucket"];
            _org = configuration["ConnectionStrings:InfluxDBOrg"];
            _dbcontext = dbcontext;
        }
        [HttpPost]
        [Route("MapTransmitterWithCustomer")]
        public async Task<IActionResult> MapTransmitterWithCustomer([FromBody] Receiver receiver)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = _dbcontext.UserInfo.FirstOrDefault(a => a.PhoneNumber == mobileNumber);

            var TransmitterMacIDs = await _dbcontext.Transmitters.Where(a => a.ReceiverMacId == receiver.ReceiverMacId).ToListAsync();
            
            var device = new Devices();

            foreach (var macId in TransmitterMacIDs)
            {
                var tenantId = Guid.NewGuid().ToString();

                device.PhoneNumber = mobileNumber;
                device.MacId = macId.TransmitterMacId;
                device.TenantId = tenantId;

                await _dbcontext.Devices.AddAsync(device);
                _dbcontext.SaveChanges();
            }            

            return Ok(new { Status = "Success", Message = "Device mapped with user successfully." });
        }
    }
}
