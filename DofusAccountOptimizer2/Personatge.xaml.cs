using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Lógica de interacción para Personatge.xaml
    /// </summary>
    public partial class Personatge : UserControl
    {
        public Personatge()
        {
            InitializeComponent();
        }
        public void SetNom(string name)
        {
            lblNom.Content = name;
        }
        public void SetClasse(string classe)
        {
            lblClasse.Content = classe;
        }
        public void SetFoto(string foto)
        {

            this.image.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\{foto}.png"));
        }
    }
}
