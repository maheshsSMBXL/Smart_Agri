using Agri_Smart.data;

namespace Agri_Smart.Models
{
    public class CategorySubCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }        
        public List<SubCategory> SubCategory { get; set; }
        public string Icon { get; set; }
    }
}
