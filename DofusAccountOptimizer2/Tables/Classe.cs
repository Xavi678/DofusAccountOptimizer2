using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Tables
{
    [Table(name: "Classe")]
    public class Classe
    {
        [Column("ID")]
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string FOTO { get; set; }
        public string NOM { get; set; }

    }
}
