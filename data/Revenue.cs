using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Revenue
    {
        [Key]
        public Guid RevenueId { get; set; }
        public string? RevenueName { get; set; }
    }
}
