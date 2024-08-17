using DofusAccountOptimizer2.Classes;
using DofusAccountOptimizer2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Lógica de interacción para Character.xaml
    /// </summary>
    public partial class Character : UserControl
    {
        public static DependencyProperty CustomTextProperty =
             DependencyProperty.Register("CustomText", typeof(string),
             typeof(Character), typeMetadata: new FrameworkPropertyMetadata("",
          flags: FrameworkPropertyMetadataOptions.AffectsRender,
          propertyChangedCallback: new PropertyChangedCallback(OnCustomTextChanged)));

        public static DependencyProperty FotoProperty =
             DependencyProperty.Register("Foto", typeof(string),
             typeof(Character), typeMetadata: new FrameworkPropertyMetadata("",
          flags: FrameworkPropertyMetadataOptions.AffectsRender,
          propertyChangedCallback: new PropertyChangedCallback(OnFotoChanged)));

        public static DependencyProperty ClasseProperty =
     DependencyProperty.Register("Classe", typeof(string),
     typeof(Character), typeMetadata: new FrameworkPropertyMetadata("",
  flags: FrameworkPropertyMetadataOptions.AffectsRender,
  propertyChangedCallback: new PropertyChangedCallback(OnClasseChanged)));
        public static DependencyProperty PositionProperty =
DependencyProperty.Register("Position", typeof(string),
typeof(Character), typeMetadata: new FrameworkPropertyMetadata("-1",
flags: FrameworkPropertyMetadataOptions.AffectsRender,
propertyChangedCallback: new PropertyChangedCallback(OnPositionChanged), new CoerceValueCallback(CoerceCallback)));

        public static DependencyProperty IsActiveProperty =
DependencyProperty.Register("IsActive", typeof(string),
typeof(Character), typeMetadata: new FrameworkPropertyMetadata("",
flags: FrameworkPropertyMetadataOptions.AffectsRender,
propertyChangedCallback: new PropertyChangedCallback(OnIsActiveChanged)));

        public static DependencyProperty ItemsLengthProperty =
DependencyProperty.Register("ItemsLength", typeof(int),
typeof(Character), typeMetadata: new FrameworkPropertyMetadata(0,
flags: FrameworkPropertyMetadataOptions.AffectsRender));

        private static object CoerceCallback(DependencyObject d, object baseValue)
        {
            long res;
            var c = d as Character;
            if (long.TryParse((string)baseValue, out res) && ((c.ItemsLength==0) || (res >= 0 && res < c.ItemsLength)))
            {
                return baseValue;
            }
            return c.Position;
        }
        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Character;
            p.cbxIsActive.IsChecked = Convert.ToBoolean(e.NewValue);
        }

        private static void OnItemsLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Character;
            Console.WriteLine(e.NewValue);
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Character;
            p.tbPos.Text = $"{e.NewValue}";
            if (p.PositionChanged != null)
            {
                PosicioEventArgs posicioEventArgs = new PosicioEventArgs();
                long nouValor;
                long vellValor;

                if (long.TryParse((string)e.NewValue, out nouValor) && long.TryParse((string)e.OldValue, out vellValor) && nouValor >= 0 && nouValor < p.ItemsLength)
                {
                    posicioEventArgs.PosicioNova = nouValor;
                    posicioEventArgs.PosicioAntiga = vellValor;
                    p.PositionChanged.Invoke(p, posicioEventArgs);
                }
                else
                {
                    MessageBox.Show($"El valor ha de ser un numero entre 0 i {p.ItemsLength}");
                }
            }
        }

        private static void OnCustomTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Character;
            p.tbName.Text = (string)e.NewValue;

        }
        private static void OnFotoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Character;
            p.image.Source = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\{e.NewValue}.png"));

        }
        private static void OnClasseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as Character;
            p.tbClasse.Text = (string)e.NewValue;


        }

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event CustomEventHandler PositionChanged;

        public delegate void CustomEventHandler(object? sender, PosicioEventArgs e);

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
        public MainWindow Parent_ { get; set; }
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
        public int ItemsLength
        {
            get
            {
                return (int)GetValue(ItemsLengthProperty);
            }
            set
            {
                SetValue(ItemsLengthProperty, value);
            }
        }
        public Personatge account { get; private set; }
        public Character()
        {
            InitializeComponent();
        }

        public Character(Personatge account) : this()
        {

            this.account = account;
            tbPos.Text = $"{account.Posicio}";
            cbxIsActive.IsChecked = account.GetActive;
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

        private void tbPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Position = tbPos.Text;
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Position);
        }

        private void tbPos_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(tbPos), null);
                Keyboard.ClearFocus();
            }
        }

        private void btnEsquerra_Click(object sender, RoutedEventArgs e)
        {
            var pos = Convert.ToInt64(Position);

            if (pos - 1 >= 0)
            {
                this.Position = $"{pos - 1}";
            }
        }

        private void btnDreta_Click(object sender, RoutedEventArgs e)
        {
            var pos = Convert.ToInt64(Position);

            if (pos + 1 < ItemsLength)
            {
                this.Position = $"{pos + 1}";
            }
        }
    }
}
