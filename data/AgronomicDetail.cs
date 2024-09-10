using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class AgronomicDetail
    {
        [Key]
        public Guid Id { get; set; }

        public string? DetailType { get; set; } = string.Empty;
        public string? CoffeeType { get; set; } = string.Empty;
        public string? PlantingPhase { get; set; } = string.Empty;
        public List<string>? Description { get; set; } = new List<string>();

        // Foreign Key
        public Guid? AgronomicPracticeId { get; set; } = Guid.Empty;
        public AgronomicPractice? AgronomicPractice { get; set; }
    }

}
