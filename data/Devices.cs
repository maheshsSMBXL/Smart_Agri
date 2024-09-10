using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Devices
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } = Guid.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? MacId { get; set; } = string.Empty;
        public string? TenantId { get; set; } = string.Empty;
        public string? DeviceName { get; set; } = string.Empty;
    }

}
