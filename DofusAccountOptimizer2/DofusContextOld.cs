/*using DofusAccountOptimizer2.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2
{
    public class DofusContext : DbContext
    {
        private string DbPath { get; set; }
        public DofusContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var di=new DirectoryInfo($"{path}\\DofusAccountOptimizer");
            if (!di.Exists)
            {
                di.Create();
            }
            DbPath = System.IO.Path.Join(di.FullName, "Dofus.db");
        }
        public DofusContext(DbContextOptions dbContextOptions) :
        base(dbContextOptions)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"DataSource={DbPath}");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Classe> classes = new List<Classe>();
            classes.Add(new Classe() { ID = 2, FOTO = "cra", NOM = "CRA" });
            classes.Add(new Classe() { ID = 3, FOTO = "ecaflip", NOM = "ECAFLIP" });
            classes.Add(new Classe() { ID = 4, FOTO = "eliotrope", NOM = "ELIOTROPE" });
            classes.Add(new Classe() { ID = 5, FOTO = "eniripsa", NOM = "ENIRIPSA" });
            classes.Add(new Classe() { ID = 6, FOTO = "enutrof", NOM = "ENUTROF" });
            classes.Add(new Classe() { ID = 7, FOTO = "feca", NOM = "FECA" });
            classes.Add(new Classe() { ID = 8, FOTO = "forgelance", NOM = "FORGELANCE" });
            classes.Add(new Classe() { ID = 9, FOTO = "huppermage", NOM = "HUPPERMAGE" });
            classes.Add(new Classe() { ID = 10, FOTO = "iop", NOM = "IOP" });
            classes.Add(new Classe() { ID = 11, FOTO = "osamodas", NOM = "OSAMODAS" });
            classes.Add(new Classe() { ID = 12, FOTO = "ouginak", NOM = "OUGINAK" });
            classes.Add(new Classe() { ID = 13, FOTO = "pandawa", NOM = "PANDAWA" });
            classes.Add(new Classe() { ID = 14, FOTO = "roublard", NOM = "ROUBLARD" });
            classes.Add(new Classe() { ID = 15, FOTO = "sacrieur", NOM = "SACRIEUR" });
            classes.Add(new Classe() { ID = 16, FOTO = "sadida", NOM = "SADIDA" });
            classes.Add(new Classe() { ID = 17, FOTO = "sram", NOM = "SRAM" });
            classes.Add(new Classe() { ID = 18, FOTO = "steamer", NOM = "STEAMER" });
            classes.Add(new Classe() { ID = 19, FOTO = "xelor", NOM = "XELOR" });
            classes.Add(new Classe() { ID = 20, FOTO = "zobal", NOM = "ZOBAL" });


            modelBuilder.Entity<Classe>().HasData(classes);
            modelBuilder.Entity<Configuracio>().HasData(new Configuracio() {ID=1,UPDATE_ICONS=false,KEY=90,ORDER_WINDOWS=false });
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Classe> clases { get; set; }
        public DbSet<Account> accounts { get; set; }
        public DbSet<Configuracio> config { get; set; }
    }
}
*/