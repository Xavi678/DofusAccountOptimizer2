using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DofusAccountOptimizer2.Models
{
    public partial class Classe
    {
        public BitmapImage GetImage
        {
            get { return new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\{Foto}.png")); }
        }
    }
}
