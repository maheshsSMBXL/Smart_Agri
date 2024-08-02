using Agri_Smart.data;
using Agri_Smart.Models;
using InfluxDB.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Smart.Controllers
{
    public class CalendarController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _context;
        private readonly InfluxDBClient _influxDBClient;
        private readonly string _bucket;
        private readonly string _org;
        private readonly IApplicationDbContext _dbcontext;

        public CalendarController(UserManager<IdentityUser> context, InfluxDBClient influxDBClient,
            IConfiguration configuration, IApplicationDbContext dbcontext)
        {
            _context = context;
            _influxDBClient = influxDBClient;
            _bucket = configuration["ConnectionStrings:InfluxDBBucket"];
            _org = configuration["ConnectionStrings:InfluxDBOrg"];
            _dbcontext = dbcontext;
        }

        [HttpPost]
        [Route("SaveUserCalendarEvents")]
        public async Task<IActionResult> SaveUserCalendarEvents([FromBody] UserCalendarEvents userCalendarEvents)
        {
            await _dbcontext.UserCalendarEvents.AddAsync(userCalendarEvents);
            _dbcontext.SaveChanges();

            return Ok(new { Status = "Success", Message = "Data saved successfully." });
        }        
    }
}
