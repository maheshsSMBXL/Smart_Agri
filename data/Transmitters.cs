using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Transmitters
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? TransmitterMacId { get; set; } = string.Empty;
        public string? ReceiverMacId { get; set; } = string.Empty;
        public DateTime? MappedDate { get; set; } = DateTime.MinValue;
    }

}
