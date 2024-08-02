using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class MapCategorySubCategory
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
    }
}
