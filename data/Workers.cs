using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agri_Smart.data
{
    public class Workers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkerId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ActivityId { get; set; }
        public int? NoOfWorkers { get; set; }
        public double? CostPerWorker { get; set; }
        public double? TotalCost { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UserId { get; set; }
    }
}
