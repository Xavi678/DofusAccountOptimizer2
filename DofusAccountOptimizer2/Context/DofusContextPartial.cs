using DofusAccountOptimizer2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Context
{
    public partial class DofusContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            List<Classe> classes = new List<Classe>();
            classes.Add(new Classe() { Id = 2, Foto = "cra", Nom = "CRA" });
            classes.Add(new Classe() { Id = 3, Foto = "ecaflip", Nom = "ECAFLIP" });
            classes.Add(new Classe() { Id = 4, Foto = "eliotrope", Nom = "ELIOTROPE" });
            classes.Add(new Classe() { Id = 5, Foto = "eniripsa", Nom = "ENIRIPSA" });
            classes.Add(new Classe() { Id = 6, Foto = "enutrof", Nom = "ENUTROF" });
            classes.Add(new Classe() { Id = 7, Foto = "feca", Nom = "FECA" });
            classes.Add(new Classe() { Id = 8, Foto = "forgelance", Nom = "FORGELANCE" });
            classes.Add(new Classe() { Id = 9, Foto = "huppermage", Nom = "HUPPERMAGE" });
            classes.Add(new Classe() { Id = 10, Foto = "iop", Nom = "IOP" });
            classes.Add(new Classe() { Id = 11, Foto = "osamodas", Nom = "OSAMODAS" });
            classes.Add(new Classe() { Id = 12, Foto = "ouginak", Nom = "OUGINAK" });
            classes.Add(new Classe() { Id = 13, Foto = "pandawa", Nom = "PANDAWA" });
            classes.Add(new Classe() { Id = 14, Foto = "roublard", Nom = "ROUBLARD" });
            classes.Add(new Classe() { Id = 15, Foto = "sacrieur", Nom = "SACRIEUR" });
            classes.Add(new Classe() { Id = 16, Foto = "sadida", Nom = "SADIDA" });
            classes.Add(new Classe() { Id = 17, Foto = "sram", Nom = "SRAM" });
            classes.Add(new Classe() { Id = 18, Foto = "steamer", Nom = "STEAMER" });
            classes.Add(new Classe() { Id = 19, Foto = "xelor", Nom = "XELOR" });
            classes.Add(new Classe() { Id = 20, Foto = "zobal", Nom = "ZOBAL" });


            modelBuilder.Entity<Classe>().HasData(classes);
            modelBuilder.Entity<Configuracio>().HasData(new Configuracio() { Id = 1, UpdateIcons = 0, KeyCodes = "112|160", OrderWindows = 0,EnableKeyboard=1,EnableMouse=1 });
            modelBuilder.Entity<Composition>().HasData(new Composition() { Id=1, Nom = "DEFAULT" });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var localappdata = Environment.GetEnvironmentVariable("localappdata");
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite($"Data Source={localappdata}\\DofusAccountOptimizer\\Dofus.db");
            }
        }
    }
}
