using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DofusContext dofusContext = new DofusContext();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var accounts=await dofusContext.accounts.ToListAsync();
            foreach (var account in accounts)
            {
                Personatge personatge = new Personatge();
                //personatge.SetClasse(item.ID_CLASSE);
                var classe = dofusContext.clases.FirstOrDefault(x => x.ID == account.ID_CLASSE);
                personatge.SetClasse(classe.NOM);
                personatge.SetFoto(classe.FOTO);
                personatge.SetNom(account.NOM);
                this.panel.Children.Add(personatge);
            }

        }
    }
}
