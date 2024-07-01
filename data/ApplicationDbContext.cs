﻿using Microsoft.AspNetCore.Identity;
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
