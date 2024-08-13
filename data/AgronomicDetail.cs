using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class AgronomicDetail
    {
        [Key]
        public Guid Id { get; set; }
        public string? DetailType { get; set; } 
        public List<string>? Description { get; set; } 

        // Foreign Key
        public Guid? AgronomicPracticeId { get; set; }
        public AgronomicPractice? AgronomicPractice { get; set; }

    }
}
