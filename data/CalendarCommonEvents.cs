using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class CalendarCommonEvents
    {
        [Key]
        public int Id { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? Title { get; set; }
        public string? ColorPrimary { get; set; }
        public string? ColorSecondary { get; set; }
        public bool? AllDay { get; set; }
        public List<string>? MetaDetails { get; set; }
    }
}
