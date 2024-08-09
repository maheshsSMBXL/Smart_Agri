using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Devices
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MacId { get; set; }
        public string? TenantId { get; set; }
        public string? DeviceName { get; set; }
    }
}
