using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Tables
{
    [Table(name: "CONFIGURACIO")]
    public class Configuracio
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public bool UPDATE_ICONS { get; set; }
        public int KEY { get; set; }

    }
}
