using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class CategorySubExpenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = 0;

        public Guid? CategoryId { get; set; } = Guid.Empty;
        public Guid? ActivityId { get; set; } = Guid.Empty;
        public TimeSpan? IrrigationDuration { get; set; } = TimeSpan.Zero;
        public string? Name { get; set; } = string.Empty;
        public int? Quantity { get; set; } = 0;
        public string? Units { get; set; } = string.Empty;
        public double? Cost { get; set; } = 0.0;
        public string? Observations { get; set; } = string.Empty;
        public string? Attachments { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public Guid? UserId { get; set; } = Guid.Empty;
    }

}
