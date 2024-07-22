using Agri_Smart.data;
using InfluxDB.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        [Route("MapDevice")]
        public async Task<IActionResult> MapDevice([FromBody] Devices request)
        {
            await _dbcontext.Devices.AddAsync(request);
            _dbcontext.SaveChanges();

            return Ok(new { Status = "Success", Message = "Device mapped with user successfully."});
        }

    }
}
