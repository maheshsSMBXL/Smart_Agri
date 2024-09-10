using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Diseases
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public string? Details { get; set; } = string.Empty;
        public List<string>? Symptoms { get; set; } = new List<string>();
        public List<string>? Remedies { get; set; } = new List<string>();
        public List<string>? PreventiveMeasures { get; set; } = new List<string>();
        public string? Cause { get; set; } = string.Empty;
    }

}
