using System;
using System.Collections.Generic;

namespace DofusAccountOptimizer2.Models
{
    public partial class Composition
    {
        public Composition()
        {
            Personatges = new HashSet<Personatge>();
        }

        public long Id { get; set; }
        public string? Nom { get; set; }

        public virtual ICollection<Personatge> Personatges { get; set; }
    }
}
