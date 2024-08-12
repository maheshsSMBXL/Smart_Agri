using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class DeletedUsers
    {
        [Key]
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Reason { get; set; }
        public DateTime? UserCreatedDate { get; set; }
        public DateTime? UserDeletedDate { get; set; }
    }
}
