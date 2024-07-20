using DofusAccountOptimizer2.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Lógica de interacción para Personatge.xaml
    /// </summary>
    public partial class Personatge : UserControl
    {
        public static DependencyProperty CustomTextProperty =
             DependencyProperty.Register("CustomText", typeof(string),
             typeof(Personatge), typeMetadata: new FrameworkPropertyMetadata("",
          flags: FrameworkPropertyMetadataOptions.AffectsRender,
          propertyChangedCallback: new PropertyChangedCallback(OnCustomTextChanged)));

        public static DependencyProperty FotoProperty =
             DependencyProperty.Register("Foto", typeof(string),
             typeof(Personatge), typeMetadata: new FrameworkPropertyMetadata("",
          flags: FrameworkPropertyMetadataOptions.AffectsRender,
          propertyChangedCallback: new PropertyChangedCallback(OnFotoChanged)));

        public static DependencyProperty ClasseProperty =
     DependencyProperty.Register("Classe", typeof(string),
     typeof(Personatge), typeMetadata: new FrameworkPropertyMetadata("",
  flags: FrameworkPropertyMetadataOptions.AffectsRender,
  propertyChangedCallback: new PropertyChangedCallback(OnClasseChanged)));
        public static DependencyProperty PositionProperty =
DependencyProperty.Register("Position", typeof(string),
typeof(Personatge), typeMetadata: new FrameworkPropertyMetadata("",
flags: FrameworkPropertyMetadataOptions.AffectsRender,
propertyChangedCallback: new PropertyChangedCallback(OnPositionChanged)));

        public static DependencyProperty IsActiveProperty =
DependencyProperty.Register("IsActive", typeof(string),
typeof(Personatge), typeMetadata: new FrameworkPropertyMetadata("",
flags: FrameworkPropertyMetadataOptions.AffectsRender,
propertyChangedCallback: new PropertyChangedCallback(OnIsActiveChanged)));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Personatge;
            p.cbxIsActive.IsChecked=(bool)e.NewValue;
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Personatge;
            p.tbPos.Text = $"{e.NewValue}";
        }

        private static void OnCustomTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Personatge;
            p.tbName.Text = (string)e.NewValue;
        }
        private static void OnFotoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Personatge;
            p.image.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\{e.NewValue}.png"));

        }
        private static void OnClasseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Personatge;
            p.tbClasse.Text = (string)e.NewValue;

        }

        public string CustomText
        {
            get
            {
                return (string)GetValue(CustomTextProperty);
            }
            set
            {
                SetValue(CustomTextProperty, value);
            }
        }
        public string Foto
        {
            get
            {
                return (string)GetValue(FotoProperty);
            }
            set
            {
                SetValue(FotoProperty, value);
            }
        }
        public string Classe
        {
            get
            {
                return (string)GetValue(ClasseProperty);
            }
            set
            {
                SetValue(ClasseProperty, value);
            }
        }
        public string Position
        {
            get
            {
                return (string)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }
        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }
        public Account account { get; private set; }
        public Personatge()
        {
            InitializeComponent();
        }

        public Personatge(Account account) : this()
        {

            this.account = account;
            tbPos.Text = $"{account.POSICIO}";
            cbxIsActive.IsChecked = account.IS_ACTIVE;
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
