using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Diseases
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Details { get; set; }
        public List<string>? Symptoms { get; set; }
        public List<string>? Remedies { get; set; }
        public List<string>? PreventiveMeasures { get; set; }
        public string? Cause { get; set; }
    }
}
