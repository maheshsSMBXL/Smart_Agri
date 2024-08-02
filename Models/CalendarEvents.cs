using Agri_Smart.data;

namespace Agri_Smart.Models
{
    public class CalendarEvents
    {        
        public List<CalendarCommonEvents>? CalendarCommonEvents { get; set; }
        public List<UserCalendarEvents>? UserCalendarEvents { get; set; }
    }
}
