using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agri_Smart.data
{
    public class Workers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkerId { get; set; } = 0;

        public Guid? CategoryId { get; set; } = Guid.Empty;
        public Guid? ActivityId { get; set; } = Guid.Empty;
        public int? NoOfWorkers { get; set; } = 0;
        public double? CostPerWorker { get; set; } = 0.0;
        public double? TotalCost { get; set; } = 0.0;
        public DateTime? CreatedDate { get; set; } = DateTime.MinValue;
        public Guid? UserId { get; set; } = Guid.Empty;
    }

}
