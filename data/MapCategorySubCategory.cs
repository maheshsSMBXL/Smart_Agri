using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class MapCategorySubCategory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CategoryId { get; set; } = Guid.Empty;
        public Guid SubCategoryId { get; set; } = Guid.Empty;
    }

}
