using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWIN_API
{
    
        public class ClientsDbContext : DbContext
        {
            public ClientsDbContext() : base("DefaultConnection")
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<ClientsDbContext, ContextMigrationConfiguration>(true));
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<ClientInfo>().ToTable("ClientsInfo").HasKey(p => p.Id);
                //modelBuilder.Entity<Bus>().Property(P => P.ServiceDate).IsOptional();
                //modelBuilder.Entity<Bus>().Property(P => P.CycleTimes).IsOptional();
                //modelBuilder.Entity<Person>().ToTable("Person").HasKey(p => p.Id);
                base.OnModelCreating(modelBuilder);
            }

            public DbSet<ClientInfo> ClientsInfo { get; set; }

            //public DbSet<Person> People { get; set; }
        }

        internal sealed class ContextMigrationConfiguration : DbMigrationsConfiguration<ClientsDbContext>
        {
            public ContextMigrationConfiguration()
            {
                AutomaticMigrationsEnabled = true;
                AutomaticMigrationDataLossAllowed = true;
                SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
            }
        }
    
}
