using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        public string? CategoryName { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
    }

}
