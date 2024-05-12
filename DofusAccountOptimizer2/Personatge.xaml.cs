using DofusAccountOptimizer2.Tables;
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
using static System.Net.Mime.MediaTypeNames;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Lógica de interacción para Personatge.xaml
    /// </summary>
    public partial class Personatge : UserControl
    {
        public Account account { get; private set; }
        public Personatge(Account account)
        {
            InitializeComponent();
            this.account = account;
            tbPos.Text = $"{account.POSICIO}";
        }
        public void SetNom(string name)
        {
            tbName.Text = name;
            var w = lblNom.Width / 2;
            var tW = this.Width / 2;
            //new Thickness(0,tbName.Margin.Top, 0, tbName.Margin.Bottom);
            //lblNom.Margin = new Point(tW - w, lblNom.Location.Y);
        }
        public void SetClasse(string classe)
        {
            tbClasse.Text = classe;
        }
        public void SetFoto(string foto)
        {

            this.image.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\{foto}.png"));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
