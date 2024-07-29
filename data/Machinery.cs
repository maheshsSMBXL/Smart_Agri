using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Machinery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MachineryId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ActivityId { get; set; }
        public int? NoOfMachines { get; set; }
        public double? CostPerMachine { get; set; }
        public double? TotalCost { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
