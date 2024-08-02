using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class OtherExpenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpenseId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ActivityId { get; set; }
        public string? Expense { get; set; }
        public double? Cost { get; set; }
        public double? TotalCost { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UserId { get; set; }
    }
}
