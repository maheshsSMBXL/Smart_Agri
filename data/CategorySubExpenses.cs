using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class CategorySubExpenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ActivityId { get; set; }
        public TimeSpan? IrrigationDuration { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public string? Units { get; set; }
        public double? Cost { get; set; }
        public string? Observations { get; set; }
        public string? Attachments { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UserId { get; set; }
    }
}
