using DofusAccountOptimizer2.Tables;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2
{
    public class DofusContext : DbContext
    {
        public DofusContext()
        {

        }
        public DofusContext(DbContextOptions dbContextOptions) :
        base(dbContextOptions)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=C:\\Users\\ASUS\\Documents\\sqlite-tools-win32-x86-3430000\\DofusDb.db");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Classe> clases { get; set; }
        public DbSet<Account> accounts { get; set; }
        public DbSet<Configuracio> config { get; set; }
    }
}
