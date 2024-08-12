using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Data;

namespace Agri_Smart.data
{
    public interface IApplicationDbContext
    {
        DbSet<Intractions> Interactions { get; set; }
        DbSet<UserInfo> UserInfo { get; set; }
        DbSet<Diseases> Diseases { get; set; }
        DbSet<Devices> Devices { get; set; }
        DbSet<DeviceUnstableData> DeviceUnstableData { get; set; }

        DbSet<Category> Category { get; set; }
        DbSet<Expenses> Expenses { get; set; }
        DbSet<Workers> Workers { get; set; }
        DbSet<Machinery> Machineries { get; set; }
        DbSet<OtherExpenses> OtherExpenses { get; set; }
        DbSet<CategorySubExpenses> CategorySubExpenses { get; set; }
        DbSet<MapCategorySubCategory> MapCategorySubCategory { get; set; }
        DbSet<SubCategory> SubCategory { get; set; }
        DbSet<Revenue> Revenue { get; set; }
        DbSet<CustomerRevenue> CustomerRevenue { get; set; }
        DbSet<CalendarCommonEvents> CalendarCommonEvents { get; set; }
        DbSet<UserCalendarEvents> UserCalendarEvents { get; set; }
        DbSet<sensorsavgdata> sensorsavgdata { get; set; }
        DbSet<DeletedUsers> DeletedUsers { get; set; }

        int SaveChanges();
        DataTable ExecuteReader
        (
            string sql
        );
    }
}
