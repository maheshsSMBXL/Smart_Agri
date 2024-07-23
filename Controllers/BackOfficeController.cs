using Agri_Smart.data;
using Agri_Smart.Models;
using InfluxDB.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Smart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackOfficeController : ControllerBase
    {
        private readonly IApplicationDbContext _dbcontext;

        public BackOfficeController(IApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpPost]
        [Route("RegisterUspDevice")]
        public async Task<IActionResult> RegisterUspDevice([FromBody] RegisterDeviceInput request)
        {
            if (request.OldMacId == null)
            {
                var tenantId = Guid.NewGuid().ToString();
                var device = new Devices();
                device.PhoneNumber = request.PhoneNumber;
                device.MacId = request.NewMacId;
                device.TenantId = tenantId;
                await _dbcontext.Devices.AddAsync(device);
                _dbcontext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Device mapped with user successfully." });
            }
            else
            {
                var device = await _dbcontext.Devices.FirstOrDefaultAsync(a => a.MacId == request.OldMacId && a.PhoneNumber == request.PhoneNumber);
                if (device != null) 
                {
                    device.MacId = request.NewMacId;
                    _dbcontext.SaveChanges();

                    return Ok(new { Status = "Success", Message = "Old device replaced with new device successfully." });
                }
            }
            return Ok(new { Status = "Failed"});

        }

    }
}
