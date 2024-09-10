using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class DeletedUsers
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Reason { get; set; } = string.Empty;
        public DateTime? UserCreatedDate { get; set; } = DateTime.Now;
        public DateTime? UserDeletedDate { get; set; } = DateTime.Now;
    }

}
