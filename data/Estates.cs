using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Estates
    {
        [Key]
        public Guid EstateId { get; set; } = Guid.NewGuid();
        public string? EstateName { get; set; } = string.Empty;
        public Guid? EstateManagerId { get; set; } = Guid.NewGuid();
    }
}
