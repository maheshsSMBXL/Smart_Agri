using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Agri_Smart.data
{
    public interface IApplicationDbContext
    {
        DbSet<Intractions> Interactions { get; set; }
    }
}
