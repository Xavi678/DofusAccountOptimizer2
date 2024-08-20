using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Interaction logic for EditKey.xaml
    /// </summary>
    public partial class EditKey : Window, INotifyPropertyChanged
    {
        private const int WM_KEYDOWN = 0x0100;
        private static IntPtr _hookID;
        private HOOKPROC _prockeyboard = EditKey.HookCallback;

        private static ObservableCollection<int> keyCodes= new ObservableCollection<int>();

        public event PropertyChangedEventHandler? PropertyChanged;


        public static event ListEventHandler ListChanged;

        public delegate void ListEventHandler();

        public ObservableCollection<int> KeyCodes {
            get
            {
                return keyCodes;
            }
            set
            {
                keyCodes = value;

            }
        }
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public EditKey()
        {
            
            InitializeComponent();
            ListChanged += EditKey_ProvaChanged;
            DataContext = this;
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                unsafe
                {
                    fixed (char* modName = curModule.ModuleName)
                    {
                        _hookID = (PInvoke.SetWindowsHookEx(Windows.Win32.UI.WindowsAndMessaging.WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _prockeyboard,
                        PInvoke.GetModuleHandle(modName), 0));
                    }
                }


            }

        }

        private void EditKey_ProvaChanged()
        {
            this.RaisePropertyChanged("KeyCodes");
        }

        private static LRESULT HookCallback(int code, WPARAM wParam, LPARAM lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Console.WriteLine(vkCode);
            if (code >= 0 && ((wParam == WM_KEYDOWN)))
            {
               keyCodes.Add(vkCode);
               ListChanged.Invoke();
            }
            return PInvoke.CallNextHookEx(new HHOOK(_hookID), code, wParam, lParam);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (KeyCodes.Count()<=3)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(Properties.Resources.max_key_codes,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            KeyCodes.Clear();
            this.RaisePropertyChanged("KeyCodes");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            PInvoke.UnhookWindowsHookEx(new HHOOK(_hookID));
        }
    }
}
