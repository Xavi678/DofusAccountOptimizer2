using DofusAccountOptimizer2.Models;
using DofusAccountOptimizer2.Context;
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
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Key = System.Windows.Input.Key;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Markup;
using System.Globalization;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static CollectionViewSource personatgeViewSource;
        DofusContext dofusContext = new DofusContext();
        //static private List<Personatge> accounts = new List<Personatge>();
        static System.Timers.Timer windowChecker;
        static Dictionary<string, Process> ProcessList = new Dictionary<string, Process>();
        private static ManagementEventWatcher startWatch;

        public static IntPtr _hookID = IntPtr.Zero;
        public static IntPtr _hookIDM = IntPtr.Zero;
        private static int handleKey = 113;
        private const int WM_SETICON = 0x80;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;
        private const int WM_KEYDOWN = 0x0100;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;
        //private static MouseHookHandler _proc = MainWindow.HookCallbackM;
        //private static KeyBoardHookHandler _procKeyBoard = MainWindow.HookCallback;
        public delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr KeyBoardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        private static HOOKPROC _proc = MainWindow.HookCallbackM;
        private static HOOKPROC _procKeyBoard = MainWindow.HookCallback;
        static int window = 0;
        bool isOrdered = false;

        public static int ItemsCount { get; set; } = -1;
        public string LanguageCode { get; set; } = "en";
        public MainWindow()
        {
            InitializeComponent();
            
            personatgeViewSource = (CollectionViewSource)FindResource(nameof(personatgeViewSource));
            //DataContext = this;
            //db.Database.Log = X => { Console.WriteLine(X); };
            var trobat = dofusContext.Configuracios.FirstOrDefault();
            if (trobat != null)
            {
                bool updIcons = trobat.GetUpdateIcons();
                if (updIcons)
                {
                    StartIconsChecker(updIcons);
                }
                cbxCanviIcones.IsChecked = updIcons;
                LanguageCode = trobat.Language;
                tbxKey.Text = ((Key)trobat.Key).ToString();
            }
            else
            {
                var c = new Configuracio()
                {
                    Id = 1,

                };
                c.SetUpdateIcons(false);
                dofusContext.Configuracios.Add(c);
                dofusContext.SaveChanges();
            }

            (_hookID, _hookIDM) = SetHook(_procKeyBoard, _proc);
            //GetPersonatges();
        }

        private static (IntPtr, IntPtr) SetHook(HOOKPROC proc, HOOKPROC procM)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                unsafe
                {
                    fixed (char* modName = curModule.ModuleName)
                    {
                        return (PInvoke.SetWindowsHookEx(Windows.Win32.UI.WindowsAndMessaging.WINDOWS_HOOK_ID.WH_KEYBOARD_LL, proc,
    PInvoke.GetModuleHandle(modName), 0),
    PInvoke.SetWindowsHookEx(Windows.Win32.UI.WindowsAndMessaging.WINDOWS_HOOK_ID.WH_MOUSE_LL, procM,
    PInvoke.GetModuleHandle(modName), 0));
                    }
                }


            }
        }
        private static LRESULT HookCallback(int code, WPARAM wParam, LPARAM lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            if (code >= 0 && ((wParam == WM_KEYDOWN && vkCode == handleKey)))
            {
                HandleHook();
                //SetForegroundWindow(currentIntpr);
            }
            return PInvoke.CallNextHookEx(new HHOOK(_hookID), code, wParam, lParam);
        }
        private static LRESULT HookCallbackM(int code, WPARAM wParam, LPARAM lParam)
        {
            if (code >= 0 && (wParam == WM_XBUTTONUP))
            {
                MSLLHOOKSTRUCT mSLLHOOKSTRUCT = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam.Value);
                //Debug.WriteLine(t.dwExtraInfo);
                //var f=Marshal.ReadInt32(t.wHitTestCode);
                Debug.WriteLine(String.Format("{0:X}", mSLLHOOKSTRUCT.mouseData));
                HandleHook(mSLLHOOKSTRUCT.mouseData == VM_XBUTTON1 ? true : false);
                //SetForegroundWindow(currentIntpr);
            }

            return PInvoke.CallNextHookEx(new HHOOK(_hookIDM), code, wParam, lParam);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEHOOKSTRUCTEX
        {
            public IntPtr mouseData;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public Point point { get; set; }
            public IntPtr mouseData { get; set; }
            public IntPtr flags { get; set; }
            public IntPtr time { get; set; }
            public IntPtr dwExtraInfo { get; set; }
        }
        private const int WM_XBUTTONDOWN = 0x020B;
        private const nint VM_XBUTTON1 = 0x020000;
        private const nint VM_XBUTTON2 = 0x010000;
        private const int WM_XBUTTONUP = 0x020C;
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        //public void GetPersonatges()
        //{
        //    using (DofusContext db = new DofusContext())
        //    {
        //        accounts = db.accounts.ToList();
        //        foreach (var personatge in accounts)
        //        {
        //            Personatge per = new Personatge();
        //            per.SetPersonatge(personatge);
        //            per.SetNom(personatge.NOM);
        //            var classe = db.clases.FirstOrDefault(x => x.ID == personatge.ID_CLASSE);
        //            per.SetClasse(classe);
        //            per.SetFoto(classe.FOTO);
        //            TableLayoutPanelCellPosition tblPos = new TableLayoutPanelCellPosition();
        //            tblPos.Column = personatge.POSICIO;
        //            tblPos.Row = 0;
        //            tableLayoutPanel1.Controls.Add(per);
        //            tableLayoutPanel1.SetCellPosition(per, tblPos);

        //        };
        //    }
        //}
        private static void HandleHook(bool forward = true)
        {
            var accounts=(ObservableCollection<Personatge>)personatgeViewSource.Source;
            var list = Process.GetProcesses().ToList();
            var actual = PInvoke.GetForegroundWindow();
            var t = list.Where(x => x.ProcessName == "Dofus").ToList();
            var act = t.FirstOrDefault(x => x.MainWindowHandle == actual);
            // Simulate a key press
            PInvoke.keybd_event(0x0E, 0x45, Windows.Win32.UI.Input.KeyboardAndMouse.KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY, 0);

            //SetForegroundWindow(mainWindowHandle);

            // Simulate a key release
            PInvoke.keybd_event(0x0E, 0x45, Windows.Win32.UI.Input.KeyboardAndMouse.KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY | Windows.Win32.UI.Input.KeyboardAndMouse.KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP, 0);
            if (act == null)
            {
                if (t != null && t.Count != 0)
                {

                    IOrderedEnumerable<Personatge> primer;
                    if (forward)
                    {
                        primer = accounts.Where(x => x.GetActive).OrderBy(x => x.Posicio);
                    }
                    else
                    {
                        primer = accounts.Where(x => x.GetActive).OrderByDescending(x => x.Posicio);
                    }
                    if (primer.Count() > 0)
                    {
                        try
                        {
                            var p = primer.First();
                            var at = t.FirstOrDefault(x => x.MainWindowTitle.Contains(p.Nom));
                            window = (int)p.Posicio;
                            FocusWindow(t, p, at);
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                }
            }
            else
            {
                var e = accounts.FirstOrDefault(x => act.MainWindowTitle.Contains(x.Nom));
                Personatge? p1;
                if (forward)
                {
                    p1 = accounts.Where(x => x.GetActive).OrderBy(x => x.Posicio).FirstOrDefault(x => x.Posicio > e.Posicio);
                }
                else
                {
                    p1 = accounts.Where(x => x.GetActive).OrderByDescending(x => x.Posicio).FirstOrDefault(x => x.Posicio < e.Posicio);
                }
                if (p1 != null)
                {
                    try
                    {
                        var at = t.FirstOrDefault(x => x.MainWindowTitle.Contains(p1.Nom));
                        if (at != null)
                        {
                            FocusWindow(t, p1, at);
                        }
                        else
                        {
                            MessageBox.Show($"El Personatge no existeix '{p1.Nom}'", "Error");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    IOrderedEnumerable<Personatge> primer;
                    if (forward)
                    {
                        primer = accounts.Where(x => x.GetActive).OrderBy(x => x.Posicio);
                    }
                    else
                    {
                        primer = accounts.Where(x => x.GetActive).OrderByDescending(x => x.Posicio);
                    }


                    if (primer.Count() > 0)
                    {
                        try
                        {
                            var p = primer.First();
                            var at = t.FirstOrDefault(x => x.MainWindowTitle.Contains(p.Nom));
                            window = (int)p.Posicio;
                            FocusWindow(t, p, at);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            var currentIntpr = Process.GetCurrentProcess().MainWindowHandle;
        }
        private static void FocusWindow(List<Process> t, Personatge p1, Process at)
        {
            var accounts = (ObservableCollection<Personatge>)personatgeViewSource.Source;
            if (at != null)
            {
                PInvoke.ShowWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED);
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: {at.MainWindowTitle}");
                Console.WriteLine(PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle)).Value);
            }
            else
            {
                Personatge next;
                Process w;
                var pos = p1.Posicio;
                do
                {

                    next = accounts.Where(x => x.GetActive).OrderBy(x => x.Posicio).FirstOrDefault(x => x.Posicio > pos);
                    if (next == null)
                    {
                        break;
                    }
                    w = t.FirstOrDefault(x => x.MainWindowTitle.Contains(next.Nom));
                    if (w != null)
                    {
                        PInvoke.ShowWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED);
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: {w.MainWindowTitle}");
                        Console.WriteLine(PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(w.MainWindowHandle)).Value);
                        break;
                    }
                    pos = next.Posicio;
                } while (w == null);
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
            var accounts = (ObservableCollection<Personatge>)personatgeViewSource.Source;
            var allProcess = GetAllProcess();
            if (allProcess == null || (allProcess != null && allProcess.Count == 0))
            {
                windowChecker.Stop();
                windowChecker.Dispose();
            }
            else
            {
                foreach (var process in allProcess)
                {

                    if (accounts.FirstOrDefault(x => process.MainWindowTitle.Contains(x.Nom)) != null)
                    {
                        SetSettings(process);
                    }
                }
            }
        }
        private static void SetSettings(Process process)
        {
            var accounts = (ObservableCollection<Personatge>)personatgeViewSource.Source;
            string img = null;
            string title = null;
            //var i=Icon.ExtractAssociatedIcon(process.MainModule.FileName);
            //Console.WriteLine(i.ToString());
            var p = accounts.FirstOrDefault(x => process.MainWindowTitle.Contains(x.Nom)  /*&& !process.MainWindowTitle.EndsWith("Dofus")*/);
            if (p != null)
            {
                using (DofusContext db = new DofusContext())
                {
                    var c = db.Classes.First(x => x.Id == p.IdClasse);
                    img = c.Foto;
                    title = $"{c.Nom} - {p.Nom} Dofus";
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

        private static void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine(e.NewEvent.Properties["ProcessName"].Value);
            if (e.NewEvent.Properties["ProcessName"].Value?.ToString() == "Dofus.exe")
            {
                var f = ((IntPtr)e.NewEvent);
                var procesid = (uint)e.NewEvent.Properties["ProcessID"].Value;
                System.Timers.Timer timer = new System.Timers.Timer(5000);
                timer.Start();
                timer.Elapsed += (s, e1) => Timer_Elapsed(s, e1, procesid);


            }
        }
        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e, uint processId)
        {
            var accounts = (ObservableCollection<Personatge>)personatgeViewSource.Source;
            var process = Process.GetProcesses().OfType<Process>().FirstOrDefault(x => x.Id == processId);
            if (process != null)
            {
                if (accounts.FirstOrDefault(x => process.MainWindowTitle.Contains(x.Nom) && !process.MainWindowTitle.EndsWith("Dofus")) != null)
                {
                    windowChecker.Start();
                    ((System.Timers.Timer)sender).Stop();
                    SetSettings(process);
                }
            }
            else
            {
                ((System.Timers.Timer)sender).Stop();
            }
        }
        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var mw = sender as MainWindow;
            await dofusContext.Personatges.LoadAsync();
            await dofusContext.Classes.LoadAsync();
            var obsCollection = dofusContext.Personatges.Local.ToObservableCollection();
            ItemsCount = obsCollection.Count;
            // bind to the source
            personatgeViewSource.Source = obsCollection;



        }

        //private async Task SetDataSource()
        //{

        //    //this.panel.Children.Clear();
        //    var allProcess = GetAllProcess();
        //    accounts = await dofusContext.Personatges.OrderBy(x => x.Posicio).ToListAsync();
        //    foreach (var account in accounts)
        //    {
        //        if (cbxOrder.IsChecked.GetValueOrDefault() && !isOrdered)
        //        {
        //            var pr = allProcess.FirstOrDefault(x => x.MainWindowTitle.Contains(account.Nom));
        //            if (pr != null)
        //            {
        //                PInvoke.ShowWindow(new HWND(pr.MainWindowHandle), SHOW_WINDOW_CMD.SW_HIDE);
        //                PInvoke.ShowWindow(new HWND(pr.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOW);
        //                isOrdered = true;
        //            }
        //        }
        //        Character personatge = new Character(account);
        //        personatge.btnRemove.Click += BtnRemove_Click;
        //        personatge.btnDreta.Click += BtnDreta_Click;
        //        personatge.btnEsquerra.Click += BtnEsquerra_Click;
        //        personatge.tbPos.TextChanged += TbPos_TextChanged;
        //        personatge.cbxIsActive.Click += CbxIsActive_Click;
        //        var classe = dofusContext.Classes.FirstOrDefault(x => x.Id == account.IdClasse);
        //        if (classe != null)
        //        {
        //            //personatge.SetClasse(classe.Nom);
        //            //personatge.SetFoto(classe.Foto);
        //            //personatge.SetNom(account.Nom);
        //        }

        //        //this.panel.Children.Add(personatge);
        //    }
        //}

        private async void CbxIsActive_Click(object sender, RoutedEventArgs e)
        {
            var cbox = ((CheckBox)sender);
            var @checked = cbox.IsChecked.GetValueOrDefault();
            var p = ((Character)((Grid)cbox.Parent).Parent);
            var account = await dofusContext.Personatges.FirstOrDefaultAsync(x => x.Nom == p.account.Nom);
            if (account != null)
            {
                account.SetActive(@checked);
                await dofusContext.SaveChangesAsync();
                personatgeViewSource.View.Refresh();
                //await SetDataSource();
            }
        }





        private async void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var p = ((Character)((Grid)btn.Parent).Parent);
            dofusContext.Personatges.Remove(p.account);
            await dofusContext.SaveChangesAsync();
            //await SetDataSource();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            var clases = await dofusContext.Classes.AsNoTracking().ToListAsync();
            Add add = new Add(clases);

            if (add.ShowDialog().GetValueOrDefault())
            {
                Classe classe = (Classe)add.cbxClasse.SelectedItem;
                var last = await dofusContext.Personatges.OrderBy(x => x.Posicio).LastOrDefaultAsync();
                var lastPosition = last != null ? last.Posicio + 1 : 0;
                await dofusContext.Personatges.AddAsync(new Personatge()
                {
                    Nom = add.tbxName.Text,
                    Posicio = lastPosition,
                    IdClasse = classe.Id

                });
                await dofusContext.SaveChangesAsync();
                personatgeViewSource.View.Refresh();
            }
        }

        private void cbxCanviIcones_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void tbxKey_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;

            dofusContext.Configuracios.First().Key = handleKey = (int)key;
            dofusContext.SaveChanges();


            e.Handled = true;

            tbxKey.Text = key.ToString();
            this.Activate();
        }

        private void cbxCanviIcones_Click(object sender, RoutedEventArgs e)
        {
            bool @checked = cbxCanviIcones.IsChecked.GetValueOrDefault();
            StartIconsChecker(@checked);

            var trobat = dofusContext.Configuracios.First();
            trobat.SetUpdateIcons(@checked);
            dofusContext.SaveChanges();
            e.Handled = true;
        }

        private async void cbxOrder_Click(object sender, RoutedEventArgs e)
        {
            bool @checked = cbxOrder.IsChecked.GetValueOrDefault();
            if (@checked && !isOrdered)
            {
                OrderWindows();
            }

            var found = await dofusContext.Configuracios.FirstOrDefaultAsync();
            if (found != null)
            {
                found.SetOrderWindows(@checked);
                await dofusContext.SaveChangesAsync();
            }
        }

        private void OrderWindows()
        {
            var accounts = (ObservableCollection<Personatge>)personatgeViewSource.Source;
            var allProcess = GetAllProcess();
            foreach (var account in accounts.OrderBy(x => x.Posicio))
            {

                var pr = allProcess.FirstOrDefault(x => x.MainWindowTitle.Contains(account.Nom));
                if (pr != null)
                {
                    int i = 0;
                    BOOL resH;
                    BOOL resS;
                    int errorCodeH;
                    int errorCodeS;
                    File.AppendAllText("ordenar finestres.txt", "-------------------------------START------------------------------------");
                    do
                    {
                        i++;
                        resH = PInvoke.ShowWindow(new HWND(pr.MainWindowHandle), SHOW_WINDOW_CMD.SW_HIDE);
                        var winexH = new Win32Exception(Marshal.GetLastWin32Error());
                        errorCodeH = winexH.ErrorCode;
                        resS = PInvoke.ShowWindow(new HWND(pr.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOW);
                        var winexS = new Win32Exception(Marshal.GetLastWin32Error());
                        errorCodeS = winexS.ErrorCode;
                        
                        File.AppendAllText("ordenar finestres.txt",$"\n{account.Nom} {resS} {resH} {pr.MainWindowHandle} {pr.MainWindowTitle} {errorCodeH} {errorCodeS} {winexH.Message} {winexS.Message}");
                        isOrdered = true;
                    } while ((resH.Value != 24 || resS.Value != 0) && i < 3);
                    File.AppendAllText("ordenar finestres.txt", "-------------------------------END------------------------------------");
                }

                Thread.Sleep(1000);
            }
        }

        private static List<Process> GetAllProcess()
        {
            return Process.GetProcessesByName("Dofus").ToList();
        }

        private void btnChangeIcons_Click(object sender, RoutedEventArgs e)
        {
            SetWindowsIcons();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void btnChangeOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderWindows();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var r = dofusContext.SaveChanges();
        }

        private void Character_PositionChanged(object sender, Classes.PosicioEventArgs e)
        {
            var caracter = ((Character)sender);
            var actualData = (Personatge)caracter.DataContext;
            var personatges = (ObservableCollection<Personatge>)personatgeViewSource.Source;

            var found = personatges.FirstOrDefault(x => x.Posicio == e.PosicioNova && x.Nom != actualData.Nom);
            if (found != null)
            {
                found.Posicio = e.PosicioAntiga;
            }
            dofusContext.SaveChanges();
            personatgeViewSource.View.Refresh();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var trobat = dofusContext.Configuracios.FirstOrDefault();
            if (trobat != null && trobat.Language != (string)comboBox.SelectedValue)
            {
                trobat.Language= (string)comboBox.SelectedValue;
                
                dofusContext.SaveChanges();
                MessageBox.Show("Restart the app to apply the language changes");
            }
        }
    }
}
