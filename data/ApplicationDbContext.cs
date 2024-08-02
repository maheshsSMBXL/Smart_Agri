using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Data;

namespace Agri_Smart.data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Intractions> Interactions { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Diseases> Diseases { get; set; }
        public DbSet<Devices> Devices { get; set; }
        public DbSet<DeviceUnstableData> DeviceUnstableData { get; set; }

        public DbSet<Category> Category { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Workers> Workers { get; set; }
        public DbSet<Machinery> Machineries { get; set; }
        public DbSet<OtherExpenses> OtherExpenses { get; set; }
        public DbSet<CategorySubExpenses> CategorySubExpenses { get; set; }
        public DbSet<MapCategorySubCategory> MapCategorySubCategory { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<Revenue> Revenue { get; set; }
        public DbSet<CustomerRevenue> CustomerRevenue { get; set; }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public DataTable ExecuteReader
        (
        string sql
        )
        {
            IDbConnection connection = Database.GetDbConnection();
            IDbCommand command = connection.CreateCommand();
            try
            {
                connection.Open();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
                //result = Convert<TType>(reader);
            }
            finally
            {
                connection.Close();
            }
            return null;
        }
    }
}
