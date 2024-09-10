using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class CalendarCommonEvents
    {
        [Key]
        public int Id { get; set; } = 0;

        public DateTime? Start { get; set; } = DateTime.Now;
        public DateTime? End { get; set; } = DateTime.Now;
        public string? Title { get; set; } = string.Empty;
        public string? ColorPrimary { get; set; } = string.Empty;
        public string? ColorSecondary { get; set; } = string.Empty;
        public bool? AllDay { get; set; } = false;
        public List<string>? MetaDetails { get; set; } = new List<string>();
    }

}
