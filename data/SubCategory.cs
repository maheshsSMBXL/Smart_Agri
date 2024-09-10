using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class SubCategory
    {
        [Key]
        public Guid SubCategoryId { get; set; } = Guid.NewGuid();

        public string? SubCategoryName { get; set; } = string.Empty;
    }

}
