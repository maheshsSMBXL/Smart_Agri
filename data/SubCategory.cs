using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class SubCategory
    {
        [Key]
        public Guid SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
    }
}
