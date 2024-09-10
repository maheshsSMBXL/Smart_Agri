using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agri_Smart.data
{
    public class UserCalendarEvents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = 0;

        public DateTime? Start { get; set; } = DateTime.MinValue;
        public DateTime? End { get; set; } = DateTime.MinValue;
        public string? Title { get; set; } = string.Empty;
        public List<string>? MetaDetails { get; set; } = new List<string>();
        public Guid? UserID { get; set; } = Guid.Empty;
        public Guid? EventID { get; set; } = Guid.Empty;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }

}
