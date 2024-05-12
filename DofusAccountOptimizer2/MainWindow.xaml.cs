﻿using DofusAccountOptimizer2.Tables;
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

        private static IntPtr _hookID = IntPtr.Zero;
        private static IntPtr _hookIDM = IntPtr.Zero;
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
            if (code >= 0 && (wParam == 524))
            {
                HandleHook();
                //SetForegroundWindow(currentIntpr);
            }

            return PInvoke.CallNextHookEx(new HHOOK(_hookIDM), code, wParam, lParam);
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
        private static void HandleHook()
        {
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

                    var primer = accounts.OrderBy(x => x.POSICIO);
                    if (primer.Count() > 0)
                    {
                        try
                        {
                            var p = primer.First();
                            var at = t.FirstOrDefault(x => x.MainWindowTitle.Contains(p.NOM));
                            window = p.POSICIO;
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
                var e = accounts.FirstOrDefault(x => act.MainWindowTitle.Contains(x.NOM));
                var p1 = accounts.OrderBy(x => x.POSICIO).FirstOrDefault(x => x.POSICIO > e.POSICIO);
                if (p1 != null)
                {
                    try
                    {
                        var at = t.FirstOrDefault(x => x.MainWindowTitle.Contains(p1.NOM));
                        FocusWindow(t, p1, at);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    var primer = accounts.OrderBy(x => x.POSICIO);
                    if (primer.Count() > 0)
                    {
                        try
                        {
                            var p = primer.First();
                            var at = t.FirstOrDefault(x => x.MainWindowTitle.Contains(p.NOM));
                            window = p.POSICIO;
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
        private static void FocusWindow(List<Process> t, Account p1, Process at)
        {
            if (at != null)
            {
                PInvoke.ShowWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED);
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: {at.MainWindowTitle}");
                Console.WriteLine(PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle)).Value);
            }
            else
            {
                Account next;
                Process w;
                var pos = p1.POSICIO;
                do
                {

                    next = accounts.OrderBy(x => x.POSICIO).FirstOrDefault(x => x.POSICIO > pos);
                    if (next == null)
                    {
                        break;
                    }
                    w = t.FirstOrDefault(x => x.MainWindowTitle.Contains(next.NOM));
                    if (w != null)
                    {
                        PInvoke.ShowWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED);
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: {w.MainWindowTitle}");
                        Console.WriteLine(PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(w.MainWindowHandle)).Value);
                        break;
                    }
                    pos = next.POSICIO;
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
                accounts = db.accounts.ToList();
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
            var process = Process.GetProcesses().OfType<Process>().FirstOrDefault(x => x.Id == processId);
            if (process != null)
            {
                if (accounts.FirstOrDefault(x => process.MainWindowTitle.Contains(x.NOM) && !process.MainWindowTitle.EndsWith("Dofus")) != null)
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

            await SetDataSource();

        }

        private async Task SetDataSource()
        {
            this.panel.Children.Clear();
            var accounts = await dofusContext.accounts.ToListAsync();
            foreach (var account in accounts)
            {
                Personatge personatge = new Personatge(account);
                personatge.btnRemove.Click += BtnRemove_Click;
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

        private async void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var p=((Personatge)((Grid)btn.Parent).Parent);
            dofusContext.accounts.Remove(p.account);
            await dofusContext.SaveChangesAsync();
            await SetDataSource();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            var clases = await dofusContext.clases.AsNoTracking().ToListAsync();
            Add add = new Add(clases);

            if (add.ShowDialog().GetValueOrDefault())
            {
                Classe classe = (Classe)add.cbxClasse.SelectedItem;
                var last = await dofusContext.accounts.OrderBy(x => x.POSICIO).LastOrDefaultAsync();
                var lastPosition = last != null ? last.POSICIO : 0;
                await dofusContext.accounts.AddAsync(new Tables.Account()
                {
                    NOM = add.tbxName.Text,
                    POSICIO = lastPosition,
                    ID_CLASSE = classe.ID

                });
                await dofusContext.SaveChangesAsync();
                await SetDataSource();
            }
        }

        private void cbxCanviIcones_Checked(object sender, RoutedEventArgs e)
        {
            bool @checked = cbxCanviIcones.IsChecked.GetValueOrDefault();
            StartIconsChecker(@checked);

            var trobat = dofusContext.config.First();
            trobat.UPDATE_ICONS = @checked;
            dofusContext.SaveChanges();

        }

        private void tbxKey_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;

            dofusContext.config.First().KEY = handleKey = (int)key;
            dofusContext.SaveChanges();


            e.Handled = true;

            tbxKey.Text = key.ToString();
            this.Activate();
        }
    }
}
