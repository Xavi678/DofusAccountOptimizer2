using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Tables
{
    [Table(name: "PERSONATGE")]
    public class Account
    {
        [Key]
        public string NOM { get; set; }
        public int POSICIO { get; set; }
        public int ID_CLASSE { get; set; }
        public bool IS_ACTIVE { get; set; }

    }
}
