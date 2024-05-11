using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
        public BitmapImage GetImage
        {
            get { return new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\{FOTO}.png")); }
        }
    }
}
