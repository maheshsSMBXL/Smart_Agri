using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Revenue
    {
        [Key]
        public Guid RevenueId { get; set; } = Guid.NewGuid();

        public string? RevenueName { get; set; } = string.Empty;
    }

}
