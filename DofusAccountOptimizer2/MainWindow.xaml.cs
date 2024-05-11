using DofusAccountOptimizer2.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Management;
using System.Timers;
using Windows.Win32;
using System.Drawing;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DofusContext dofusContext = new DofusContext();
        static private List<Account> accounts = new List<Account>();
        static System.Timers.Timer windowChecker;
        static Dictionary<string, Process> ProcessList = new Dictionary<string, Process>();
        private static ManagementEventWatcher startWatch;

        private const int WM_SETICON = 0x80;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;

        public MainWindow()
        {
            InitializeComponent();

                //db.Database.Log = X => { Console.WriteLine(X); };
                var trobat = dofusContext.config.FirstOrDefault();
                if (trobat != null)
                {
                    bool updIcons = trobat.UPDATE_ICONS;
                    if (updIcons)
                    {
                        StartIconsChecker(updIcons);
                    }
                    cbxCanviIcones.IsChecked = updIcons;
                
                    tbxKey.Text = ((Key)trobat.KEY).ToString();
                }
                else
                {
                dofusContext.config.Add(new Configuracio()
                    {
                        ID = 1,
                        UPDATE_ICONS = false
                    });
                dofusContext.SaveChanges();
                }
            
        }
        private static void StartIconsChecker(bool @checked)
        {
            if (@checked)
            {
                if (windowChecker == null)
                {
                    windowChecker = new System.Timers.Timer();
                }
                windowChecker.Stop();

                windowChecker.Interval = 60000;
                windowChecker.Elapsed += WindowChecker_Elapsed;
                if (Process.GetProcessesByName("Dofus").Count() != 0)
                {
                    windowChecker.Start();
                }

                startWatch = new ManagementEventWatcher(
   new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName='Dofus.exe'"));
                startWatch.EventArrived
                                    += StartWatch_EventArrived;

                startWatch.Start();
            }
            else
            {
                if (startWatch != null)
                {
                    startWatch.Stop();
                    startWatch.Dispose();
                }
                if (windowChecker != null)
                {
                    windowChecker.Stop();
                    windowChecker.Dispose();
                }
            }
        }

        private static void WindowChecker_Elapsed(object? sender, ElapsedEventArgs e)
        {
            SetWindowsIcons();
        }
        private static void SetWindowsIcons()
        {
            var allProcess = Process.GetProcessesByName("Dofus").ToList();
            if (allProcess == null || (allProcess != null && allProcess.Count == 0))
            {
                windowChecker.Stop();
                windowChecker.Dispose();
            }
            else
            {
                foreach (var process in allProcess)
                {

                    if (accounts.FirstOrDefault(x => process.MainWindowTitle.Contains(x.NOM)) != null)
                    {
                        SetSettings(process);
                    }
                }
            }
        }
        private static void SetSettings(Process process)
        {

            string img = null;
            string title = null;
            //var i=Icon.ExtractAssociatedIcon(process.MainModule.FileName);
            //Console.WriteLine(i.ToString());
            var p = accounts.FirstOrDefault(x => process.MainWindowTitle.Contains(x.NOM)  /*&& !process.MainWindowTitle.EndsWith("Dofus")*/);
            if (p != null)
            {
                using (DofusContext db = new DofusContext())
                {
                    var c = db.clases.First(x => x.ID == p.ID_CLASSE);
                    img = c.FOTO;
                    title = $"{c.NOM} - {p.NOM} Dofus";
                }
                if (img != null)
                {
                    var handle = new Windows.Win32.Foundation.HWND(process.MainWindowHandle);
                    if (!ProcessList.ContainsKey(title))
                    {
                        ProcessList.Add(title, process);
                    }
                    else
                    {
                        ProcessList.Remove(title);
                        ProcessList.Add(title, process);
                    }

                    PInvoke.SetWindowText(handle, title);
                    Icon icon = new Icon($@"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\\{img}_ico.ico");
                    
                    PInvoke.SendMessage(handle, WM_SETICON, ICON_BIG, icon.Handle);
                }
            }
        }
        public void SetPersonatges()
        {
            using (DofusContext db = new DofusContext())
            {
                personatges = db.personatges.ToList();
            }
        }

        private static void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            await SetDataSource();

        }

        private async Task SetDataSource()
        {
            this.panel.Children.Clear();
            var accounts = await dofusContext.accounts.ToListAsync();
            foreach (var account in accounts)
            {
                Personatge personatge = new Personatge();
                var classe = dofusContext.clases.FirstOrDefault(x => x.ID == account.ID_CLASSE);
                if (classe != null)
                {
                    personatge.SetClasse(classe.NOM);
                    personatge.SetFoto(classe.FOTO);
                    personatge.SetNom(account.NOM);
                }
                
                this.panel.Children.Add(personatge);
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            
            var clases=await dofusContext.clases.AsNoTracking().ToListAsync();
            Add add = new Add(clases);

            if (add.ShowDialog().GetValueOrDefault())
            {
                Classe classe=(Classe)add.cbxClasse.SelectedItem;
                var last=await dofusContext.accounts.OrderBy(x => x.POSICIO).LastOrDefaultAsync();
                var lastPosition=last != null ? last.POSICIO : 0;
                await dofusContext.accounts.AddAsync(new Tables.Account()
                {
                    NOM = add.tbxName.Text,
                    POSICIO = lastPosition,
                    ID_CLASSE= classe.ID

                });
                await dofusContext.SaveChangesAsync();
                await SetDataSource();
            }
        }
    }
}
