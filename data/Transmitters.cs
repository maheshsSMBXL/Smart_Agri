using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Transmitters
    {
        [Key]
        public Guid Id { get; set; }
        public string? TransmitterMacId { get; set; }
        public string? ReceiverMacId { get; set; }
        public DateTime? MappedDate { get; set; }
    }
}
