using System;
using System.Collections.Generic;

namespace DofusAccountOptimizer2.Models
{
    public partial class Personatge
    {
        public string Nom { get; set; } = null!;
        public long Posicio { get; set; }
        public long IdClasse { get; set; }
        public long IsActive { get; set; }

        public virtual Classe IdClasseNavigation { get; set; } = null!;
    }
}
