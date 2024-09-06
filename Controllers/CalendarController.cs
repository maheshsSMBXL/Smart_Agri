using Agri_Smart.data;
using Agri_Smart.Models;
using Agri_Smart.Services;
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
        private readonly JsonSerializerSettingsService _jsonSerializerSettingsService;

        public CalendarController(UserManager<IdentityUser> context, InfluxDBClient influxDBClient,
            IConfiguration configuration, IApplicationDbContext dbcontext, JsonSerializerSettingsService jsonSerializerSettingsService)
        {
            _context = context;
            _influxDBClient = influxDBClient;
            _bucket = configuration["ConnectionStrings:InfluxDBBucket"];
            _org = configuration["ConnectionStrings:InfluxDBOrg"];
            _dbcontext = dbcontext;
            _jsonSerializerSettingsService = jsonSerializerSettingsService;
        }

        [HttpPost]
        [Route("SaveUserCalendarEvents")]
        public async Task<IActionResult> SaveUserCalendarEvents([FromBody] UserCalendarEvents userCalendarEvents)
        {
            var mobileNumber = User?.Claims?.FirstOrDefault(c => c.Type == "MobileNumber")?.Value;
            var userInfo = await _dbcontext.UserInfo.FirstOrDefaultAsync(a => a.PhoneNumber == mobileNumber);

            if (userCalendarEvents.EventID == null || userCalendarEvents.EventID == Guid.NewGuid())
            {
                var EventID = Guid.NewGuid();
                userCalendarEvents.UserID = userInfo.Id;
                userCalendarEvents.EventID = EventID;
                userCalendarEvents.CreatedDate = DateTime.UtcNow;

                await _dbcontext.UserCalendarEvents.AddAsync(userCalendarEvents);
                _dbcontext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Data saved successfully.", EventID = EventID.ToString() });
            }
            else 
            {
                var calendarEvents = await _dbcontext.UserCalendarEvents.FirstOrDefaultAsync(a => a.EventID == userCalendarEvents.EventID);

                _dbcontext.UserCalendarEvents.RemoveRange(calendarEvents);

                userCalendarEvents.UserID = userInfo.Id;
                userCalendarEvents.CreatedDate = DateTime.UtcNow;

                await _dbcontext.UserCalendarEvents.AddAsync(userCalendarEvents);
                _dbcontext.SaveChanges();

                return Ok(new { Status = "Success", Message = "Data Updated Successfully.", EventID = userCalendarEvents.EventID.ToString() });

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

            var jsonResponse = _jsonSerializerSettingsService.SerializeObject(calendarEvents);

            return Ok(jsonResponse);
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
