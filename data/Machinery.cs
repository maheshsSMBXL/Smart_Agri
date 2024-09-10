using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Machinery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MachineryId { get; set; } = 0;

        public Guid? CategoryId { get; set; } = Guid.Empty;
        public Guid? ActivityId { get; set; } = Guid.Empty;
        public int? NoOfMachines { get; set; } = 0;
        public double? CostPerMachine { get; set; } = 0.0;
        public double? TotalCost { get; set; } = 0.0;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public Guid? UserId { get; set; } = Guid.Empty;
    }

}
