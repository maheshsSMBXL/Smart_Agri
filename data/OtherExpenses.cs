using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class OtherExpenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpenseId { get; set; } = 0;

        public Guid? CategoryId { get; set; } = Guid.Empty;
        public Guid? ActivityId { get; set; } = Guid.Empty;
        public string? Expense { get; set; } = string.Empty;
        public double? Cost { get; set; } = 0.0;
        public double? TotalCost { get; set; } = 0.0;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public Guid? UserId { get; set; } = Guid.Empty;
    }
}
