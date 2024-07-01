using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Data;

namespace Agri_Smart.data
{
    public interface IApplicationDbContext
    {
        DbSet<Intractions> Interactions { get; set; }

        int SaveChanges();
        DataTable ExecuteReader
        (
            string sql
        );
    }
}
