using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class AgronomicPractice
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<string>? Description { get; set; }  
                
        public ICollection<AgronomicDetail>? AgronomicDetails { get; set; }

    }
}
