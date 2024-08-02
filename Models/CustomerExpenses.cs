using Agri_Smart.data;

namespace Agri_Smart.Models
{
    public class CustomerExpenses
    {
        public Expenses Expenses { get; set; }
        public List<Workers> Workers { get; set; }
        public List<Machinery> Machinery { get; set; }
        public List<OtherExpenses> OtherExpenses { get; set; }
    }
}
