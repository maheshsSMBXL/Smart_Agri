using Agri_Smart.data;
using Agri_Smart.Models;
using InfluxDB.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Agri_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            if (userCalendarEvents.EventID == null || userCalendarEvents.EventID == Guid.NewGuid())
            {
                userCalendarEvents.UserID = userInfo.Id;
                userCalendarEvents.EventID = Guid.NewGuid();
                userCalendarEvents.CreatedDate = DateTime.UtcNow;

                await _dbcontext.UserCalendarEvents.AddAsync(userCalendarEvents);
                _dbcontext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Data saved successfully." });
            }
            else 
            {
                var calendarEvents = await _dbcontext.UserCalendarEvents.FirstOrDefaultAsync(a => a.EventID == userCalendarEvents.EventID);

                _dbcontext.UserCalendarEvents.RemoveRange(calendarEvents);

                userCalendarEvents.UserID = userInfo.Id;
                userCalendarEvents.CreatedDate = DateTime.UtcNow;

                await _dbcontext.UserCalendarEvents.AddAsync(userCalendarEvents);
                _dbcontext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Data Updated Successfully." });

            }
            return Ok();
        }
        [HttpGet]
        [Route("GetCalendarEvents")]
        public async Task<IActionResult> GetCalendarEvents()
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var CropStartDate = userInfo.CropGrowingStartDate;

            var calendarEvents = new CalendarEvents();

            var calendarCommonEvents = await _dbcontext.CalendarCommonEvents.ToListAsync();

            var userCalendarEvents = await _dbcontext.UserCalendarEvents.Where(a => a.UserID == userInfo.Id).ToListAsync();

            foreach (var calendarEvent in calendarCommonEvents) 
            {               
                // Adjust the event's StartDate
                calendarEvent.Start = calendarEvent.Start
                    .Value.AddMonths(CropStartDate.Value.Month - 1)
                    .AddDays(CropStartDate.Value.Day);
                calendarEvent.End = calendarEvent.End
                    .Value.AddMonths(CropStartDate.Value.Month - 1)
                    .AddDays(CropStartDate.Value.Day);
            }

            calendarEvents.CalendarCommonEvents = calendarCommonEvents;
            calendarEvents.UserCalendarEvents = userCalendarEvents;

            return Ok(calendarEvents);
        }
        [HttpDelete("DeleteCalendarEvent/{EventId}")]
        public async Task<IActionResult> DeleteCalendarEvent(Guid EventId)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            var calendarEvent = await _dbcontext.UserCalendarEvents.FirstOrDefaultAsync(a => a.EventID == EventId);
            if (calendarEvent != null)
            {
                _dbcontext.UserCalendarEvents.Remove(calendarEvent);
                _dbcontext.SaveChanges();
            }
            return Ok(new { Status = "Success", message = "Event deleted successfully" });            
        }
    }
}
