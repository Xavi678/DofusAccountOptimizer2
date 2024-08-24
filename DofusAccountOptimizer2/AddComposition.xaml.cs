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
using System.Windows.Shapes;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Interaction logic for AddComposition.xaml
    /// </summary>
    public partial class AddComposition : Window
    {
        public string CompositionName { get; set; }
        public AddComposition()
        {
            InitializeComponent();
            this.DataContext= this;
        }
        public AddComposition(string CompositionName) : this()
        {
            this.CompositionName= CompositionName;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //CompositionName = textBox.Text;
            this.DialogResult = true;
        }
    }
}
