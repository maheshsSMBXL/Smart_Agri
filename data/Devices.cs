using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Devices
    {
        [Key]
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TenantId { get; set; }
    }
}
