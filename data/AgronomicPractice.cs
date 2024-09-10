using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class AgronomicPractice
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } = string.Empty;
        public List<string>? Description { get; set; } = new List<string>();
        public string? Image { get; set; } = string.Empty;
        public ICollection<AgronomicDetail>? AgronomicDetails { get; set; } = new List<AgronomicDetail>();
    }

}
