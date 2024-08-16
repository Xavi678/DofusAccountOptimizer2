using System;
using System.Collections.Generic;

namespace DofusAccountOptimizer2.Models
{
    public partial class Classe
    {
        public Classe()
        {
            Personatges = new HashSet<Personatge>();
        }

        public long Id { get; set; }
        public string Foto { get; set; } = null!;
        public string Nom { get; set; } = null!;

        public virtual ICollection<Personatge> Personatges { get; set; }
    }
}
