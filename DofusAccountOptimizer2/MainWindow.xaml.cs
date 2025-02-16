﻿using DofusAccountOptimizer2.Models;
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
using System.Windows.Threading;
using System.Net;
using DofusAccountOptimizer2.Classes;
using Windows.Win32.UI.Shell.PropertiesSystem;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Microsoft.Win32;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private static CollectionViewSource personatgeViewSource;
        private static CollectionViewSource compositionsViewSource;
        DofusContext dofusContext = new DofusContext();
        //static private List<Personatge> accounts = new List<Personatge>();
        static DispatcherTimer windowChecker;
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
        internal static HOOKPROC _procKeyBoard = MainWindow.HookCallback;
        static int window = 0;
        bool isOrdered = false;
        public static List<int> keyCodes = new List<int>();
        public static int ItemsCount { get; set; } = -1;
        public string LanguageCode { get; set; } = "en";
        public long lastCompId { get; set; }
        public bool IsMouseEnabled { get; set; } = true;
        public bool IsKeyboardEnabled { get; set; } = true;
        public ObservableCollection<Composition> compositions_;
        public ObservableCollection<Composition> Compositions
        {
            get
            {
                return compositions_;
            }
            set
            {
                compositions_ = value;
                this.RaisePropertyChanged("Compositions");
            }
        }
        private Composition selectedComposition;
        public Composition SelectedComposition
        {
            get
            {
                return selectedComposition;
            }
            set
            {
                selectedComposition = value;
                RaisePropertyChanged("SelectedComposition");
            }
        }

        public bool OrderAutomatically { get; set; } = false;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private bool WindowsAreSeparated = false;
        public bool TaskbarSmallIcons { get; set; } = false;
        private const string APP_IID = "com.dofus.d1elauncher";
        public MainWindow()
        {
            InitializeComponent();
            var trobat = dofusContext.Configuracios.FirstOrDefault();
            personatgeViewSource = (CollectionViewSource)FindResource(nameof(personatgeViewSource));
            compositionsViewSource = (CollectionViewSource)FindResource(nameof(compositionsViewSource));
            lastCompId = (trobat.LastCompositionId.HasValue ? trobat.LastCompositionId.Value : 1);
            dofusContext.Personatges.Load();
            dofusContext.Classes.LoadAsync();
            dofusContext.Compositions.LoadAsync();
            var obsCollection = dofusContext.Personatges.Local.ToObservableCollection();

            ItemsCount = obsCollection.Count;

            // bind to the source
            personatgeViewSource.Source = obsCollection;
            compositions_ = dofusContext.Compositions.Local.ToObservableCollection();
            compositionsViewSource.Source = compositions_;
            //DataContext = this;
            //db.Database.Log = X => { Console.WriteLine(X); };

            var taskBarGlomLevel = RegistryExplorer.GetTaskbarRegistryPropertie();

            if (taskBarGlomLevel != null)
            {
                
                var glom = taskBarGlomLevel.GetValue(RegistryExplorer.TASKBAR_GLOOM_LEVEL);
                if (glom != null)
                {
                    var regKind = taskBarGlomLevel.GetValueKind(RegistryExplorer.TASKBAR_GLOOM_LEVEL);
                    if (regKind == RegistryValueKind.DWord)
                    {
                        int res = Convert.ToInt32(glom);
                        TaskbarSmallIcons = res == 2;
                    }
                }
            }

            if (trobat != null)
            {
                bool updIcons = trobat.GetUpdateIcons();
                bool updTitle = trobat.GetUpdateTitle();
                if (updIcons || updTitle)
                {
                    StartIconsChecker(updIcons, updTitle);
                }
                comboBoxCompositions.SelectedIndex = compositions_.IndexOf(dofusContext.Compositions.Local.First(x => x.Id == lastCompId));


                //comboBoxCompositions.SelectedValue = lastCompId;
                cbxCanviIcones.IsChecked = updIcons;
                cbxCanviTitle.IsChecked = updTitle;
                LanguageCode = trobat.Language;
                IsMouseEnabled = trobat.GetMouseEnabled();
                IsKeyboardEnabled = trobat.GetKeyboardEnabled();
                OrderAutomatically = trobat.GetOrderWindowsOnChangeComp();
            }
            else
            {
                MainWindow.keyCodes = new List<int>() { 112, 160 };
                var c = new Configuracio()
                {
                    Id = 1,
                    KeyCodes = "112|160"
                };
                comboBoxCompositions.SelectedValue = 1;
                c.SetUpdateIcons(false);
                dofusContext.Configuracios.Add(c);
                dofusContext.SaveChanges();
            }
            if (IsMouseEnabled)
            {

                _hookIDM = SetHookM(_proc);
            }
            if (IsKeyboardEnabled)
            {
                _hookID = SetHookKey(_procKeyBoard);
            }
            //GetPersonatges();
        }

        private static IntPtr SetHookM(HOOKPROC procM)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                unsafe
                {
                    fixed (char* modName = curModule.ModuleName)
                    {
                        return (PInvoke.SetWindowsHookEx(Windows.Win32.UI.WindowsAndMessaging.WINDOWS_HOOK_ID.WH_MOUSE_LL, procM, PInvoke.GetModuleHandle(modName), 0));
                    }
                }


            }
        }
        internal static IntPtr SetHookKey(HOOKPROC proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                unsafe
                {
                    fixed (char* modName = curModule.ModuleName)
                    {
                        return (PInvoke.SetWindowsHookEx(Windows.Win32.UI.WindowsAndMessaging.WINDOWS_HOOK_ID.WH_KEYBOARD_LL, proc,
    PInvoke.GetModuleHandle(modName), 0));
                    }
                }


            }
        }
        private static LRESULT HookCallback(int code, WPARAM wParam, LPARAM lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Console.WriteLine(vkCode);
            var persos = ((ObservableCollection<Personatge>)personatgeViewSource.Source).Where(x => x.KeyCodes != null && x.KeyCodes != String.Empty);
            bool keyCharacterClicked = false;
            Personatge? keyPersonatge = null;
            foreach (var perso in persos)
            {
                var keyCodes = perso.KeyCodes;
                
                foreach (var keyCode in KeyCodesExtensions.ConvertKeys( keyCodes))
                {
                    if (vkCode == keyCode)
                    {
                        keyCharacterClicked = true;
                        keyPersonatge = perso;
                        break;
                    }
                }
                if (keyCharacterClicked)
                {
                    break;
                }
            }
            if (code >= 0 && keyCharacterClicked && keyPersonatge != null)
            {
                bool areAllClicked = true;
                foreach (var otherKey in keyPersonatge!.KeyCodes!.Split("|"))
                {
                    var keyState = PInvoke.GetKeyState(Convert.ToInt32(otherKey));
                    if (keyState >= 0)
                    {
                        areAllClicked = false;
                    }
                }
                if (areAllClicked)
                {
                    Debug.WriteLine("Hooked");
                    var list = Process.GetProcesses().ToList();
                    var t = list.Where(x => x.ProcessName == "Dofus").ToList();
                    var at = t.FirstOrDefault(x => x.MainWindowTitle.Contains(keyPersonatge.Nom));
                    if (at != null)
                    {
                        PInvoke.ShowWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED);
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: {at.MainWindowTitle}");
                        Console.WriteLine(PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(at.MainWindowHandle)).Value);
                    }
                    return PInvoke.CallNextHookEx(new HHOOK(_hookID), code, wParam, lParam);
                }
            }

            if (code >= 0 && ((wParam == WM_KEYDOWN && keyCodes.Contains(vkCode))))
            {
                bool areAllClicked = true;
                if (keyCodes.Count > 1)
                {
                    foreach (var keyCode in keyCodes.Where(x => x != vkCode))
                    {
                        var keyState = PInvoke.GetKeyState(keyCode);
                        Debug.WriteLine($"{(System.Windows.Forms.Keys)keyCode}: {keyState}");
                        if (keyState >= 0)
                        {
                            areAllClicked = false;
                        }
                    }
                }

                if (areAllClicked)
                {
                    Debug.WriteLine("Hooked");
                    HandleHook();
                }
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

        public event PropertyChangedEventHandler? PropertyChanged;

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
            var accounts = personatgeViewSource.View.Cast<Personatge>();
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
                var e = accounts.FirstOrDefault(x => act.MainWindowTitle.ContainsCharName(x.Nom));
                if (e == null)
                {

                    MessageBox.Show($"{Properties.Resources.character_not_found} '{act.MainWindowTitle}'", "Error");
                    return;
                }
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
                            MessageBox.Show($"{Properties.Resources.character_not_found} '{p1.Nom}'", "Error");
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
            var accounts = personatgeViewSource.View.Cast<Personatge>();
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
        private static void StartIconsChecker(bool updIcons, bool updTitle)
        {
            if (updIcons || updTitle)
            {

                if (windowChecker == null)
                {
                    windowChecker = new DispatcherTimer();
                }
                windowChecker.Stop();
                windowChecker.Tag = (updIcons, updTitle);
                windowChecker.Interval = TimeSpan.FromMilliseconds(60000);
                windowChecker.Tick += WindowChecker_Elapsed;
                if (Process.GetProcessesByName("Dofus").Count() != 0)
                {
                    SetWindowsIcons(updIcons, updTitle);
                    windowChecker.Start();
                }
                try
                {
                    if (startWatch == null)
                    {
                        startWatch = new ManagementEventWatcher(
           new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName='Dofus.exe'"));
                        startWatch.Options.Context.Add("updIcons", updIcons);
                        startWatch.Options.Context.Add("updTitle", updTitle);
                        startWatch.EventArrived
                                            += StartWatch_EventArrived;

                        startWatch.Start();
                    }
                    else
                    {
                        startWatch.Options.Context.Remove("updIcons");
                        startWatch.Options.Context.Remove("updTitle");

                        startWatch.Options.Context.Add("updIcons", updIcons);
                        startWatch.Options.Context.Add("updTitle", updTitle);
                    }
                }
                catch (ManagementException ex)
                {
                    MessageBox.Show(Properties.Resources.privilege_error);
                    App.Current.Shutdown();
                }
            }
            else
            {
                if (startWatch != null)
                {
                    startWatch.Stop();
                    startWatch.Dispose();
                    startWatch = null;
                }
                if (windowChecker != null)
                {
                    windowChecker.Stop();
                }
            }
        }

        private static void WindowChecker_Elapsed(object? sender, EventArgs e)
        {
            var tag = ((DispatcherTimer?)sender).Tag;
            var tuple = (ValueTuple<bool, bool>)tag;
            SetWindowsIcons(tuple.Item1, tuple.Item2);
        }
        private static void SetWindowsIcons(bool updIcons, bool updTitle)
        {

            var accounts = personatgeViewSource.View.Cast<Personatge>();
            var allProcess = GetAllProcess();
            if (allProcess == null || (allProcess != null && allProcess.Count == 0))
            {
                windowChecker.Stop();
            }
            else
            {
                foreach (var process in allProcess)
                {

                    if (accounts.FirstOrDefault(x => process.MainWindowTitle.ContainsCharName(x.Nom)) != null)
                    {
                        SetSettings(process, updIcons, updTitle);
                    }
                }
            }
        }
        private static void SetSettings(Process process, bool updIcons, bool updTitle)
        {
            var accounts = personatgeViewSource.View.Cast<Personatge>();
            string img = null;
            string title = null;
            //var i=Icon.ExtractAssociatedIcon(process.MainModule.FileName);
            //Console.WriteLine(i.ToString());
            var p = accounts.FirstOrDefault(x => process.MainWindowTitle.ContainsCharName(x.Nom)  /*&& !process.MainWindowTitle.EndsWith("Dofus")*/);
            if (p != null)
            {
                using (DofusContext db = new DofusContext())
                {
                    var c = db.Classes.First(x => x.Id == p.IdClasse);
                    img = c.Foto;
                    title = $"{DofusAccountOptimizer2.Properties.Resources.ResourceManager.GetString($"class_{c.Id}")} - {p.Nom} - Dofus";
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

                    var currentTitle=process.MainWindowTitle;
                    PInvoke.SetWindowText(handle, updTitle ? title : currentTitle);

                    if (updIcons)
                    {
                        Icon icon = new Icon($@"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\\{img}_ico.ico");

                        var res = PInvoke.SendMessage(handle, WM_SETICON, ICON_BIG, icon.Handle);
                       
                        var win32Error = Marshal.GetLastWin32Error();
                        Console.WriteLine(new Win32Exception(win32Error).Message);
                        var lastPinvoke=Marshal.GetLastPInvokeError();
                        Console.WriteLine(lastPinvoke);
                    }
                }
            }
        }

        private static void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine(e.NewEvent.Properties["ProcessName"].Value);
            if (e.NewEvent.Properties["ProcessName"].Value?.ToString() == "Dofus.exe")
            {
                var mew = ((ManagementEventWatcher)sender);
                var f = ((IntPtr)e.NewEvent);
                var procesid = (uint)e.NewEvent.Properties["ProcessID"].Value;
                var updIcons = (bool)mew.Options.Context["updIcons"];
                var updTitle = (bool)mew.Options.Context["updTitle"];
                DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background, Application.Current.Dispatcher);
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += (s, e1) => Timer_Elapsed(s, e1, procesid, updIcons, updTitle);
                timer.Start();
            }
        }
        private static void Timer_Elapsed(object sender, EventArgs e, uint processId, bool updIcons, bool updTitle)
        {
            var accounts = personatgeViewSource.View.Cast<Personatge>();
            var process = Process.GetProcesses().OfType<Process>().FirstOrDefault(x => x.Id == processId);
            if (process != null)
            {
                if (accounts.FirstOrDefault(x => process.MainWindowTitle.ContainsCharName(x.Nom) && !process.MainWindowTitle.EndsWith("Dofus")) != null)
                {
                    windowChecker.Start();
                    ((DispatcherTimer)sender).Stop();
                    SetSettings(process, updIcons, updTitle);
                }
            }
            else
            {
                ((DispatcherTimer)sender).Stop();
            }
        }
        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var mw = sender as MainWindow;
            MainWindow.keyCodes = KeyCodesExtensions.ConvertKeys(dofusContext.Configuracios.FirstOrDefault()!.KeyCodes).ToList();

            tbxKey.Text = KeyCodesExtensions.ConvertToTextBoxString(MainWindow.keyCodes);


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
            var compAct = (Composition)comboBoxCompositions.SelectedItem;
            if ((await dofusContext.Personatges.Where(x => x.IdComposition == compAct.Id).CountAsync()) < 8)
            {
                Add add = new Add(clases);

                if (add.ShowDialog().GetValueOrDefault())
                {
                    Classe classe = (Classe)add.cbxClasse.SelectedItem;
                    var last = await dofusContext.Personatges.Where(x => x.IdComposition == compAct.Id).OrderBy(x => x.Posicio).LastOrDefaultAsync();
                    var lastPosition = last != null ? last.Posicio + 1 : 0;
                    if (dofusContext.Personatges.FirstOrDefault(x => x.Nom == classe.Nom) == null)
                    {
                        await dofusContext.Personatges.AddAsync(new Personatge()
                        {
                            Nom = add.tbxName.Text,
                            Posicio = lastPosition,
                            IdClasse = classe.Id,
                            IsActive = 1,
                            IdComposition = compAct.Id,

                        });
                        await dofusContext.SaveChangesAsync();
                        personatgeViewSource.View.Refresh();
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.error_already_exists, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.error_max_characters, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbxCanviIcones_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void tbxKey_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;

            //dofusContext.Configuracios.First().Key = handleKey = (int)key;
            dofusContext.SaveChanges();


            e.Handled = true;

            tbxKey.Text = key.ToString();
            this.Activate();
        }

        private void cbxCanviIcones_Click(object sender, RoutedEventArgs e)
        {
            bool @checked = cbxCanviIcones.IsChecked.GetValueOrDefault();
            bool updTtitle = cbxCanviTitle.IsChecked.GetValueOrDefault();
            StartIconsChecker(@checked, updTtitle);

            var trobat = dofusContext.Configuracios.First();
            trobat.SetUpdateIcons(@checked);
            trobat.SetUpdateTitle(updTtitle);
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
        /*public static int GetZOrder(Process p)
        {
            IntPtr hWnd = p.MainWindowHandle;
            var z = 0;

            for (var h = hWnd; h != IntPtr.Zero; h =PInvoke.GetWindow(new HWND(h), GET_WINDOW_CMD.GW_HWNDPREV)) z++;
            return z;
        }*/
        private void OrderWindows()
        {
            var accounts = personatgeViewSource.View.Cast<Personatge>();
            var allProcess = GetAllProcess();
            bool areSameOrder = true;
            LPARAM lPARAM = new LPARAM();
            List<Process> llistaOrder = new List<Process>();
            PInvoke.EnumWindows(new WNDENUMPROC((x, y) =>
            {

                var found = allProcess.FirstOrDefault(z => z.MainWindowHandle == x.Value);
                if (found != null)
                {
                    llistaOrder.Add(found);
                }
                Console.WriteLine();
                return true;
            }), lPARAM);
            llistaOrder.Reverse();
            foreach (var item in llistaOrder)
            {
                Personatge? currentAccount = null;
                foreach (var account in accounts)
                {
                    if (item.MainWindowTitle.Contains(account.Nom))
                    {
                        currentAccount = account;
                        break;
                    }
                }

                if (currentAccount != null && currentAccount.Posicio != llistaOrder.IndexOf(item))
                {
                    areSameOrder = false;
                }
            }


            if (!areSameOrder)
            {
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
                        //File.AppendAllText("ordenar finestres.txt", "-------------------------------START------------------------------------");
                        do
                        {
                            i++;
                            resH = PInvoke.ShowWindow(new HWND(pr.MainWindowHandle), SHOW_WINDOW_CMD.SW_HIDE);
                            var winexH = new Win32Exception(Marshal.GetLastWin32Error());
                            errorCodeH = winexH.ErrorCode;
                            resS = PInvoke.ShowWindow(new HWND(pr.MainWindowHandle), SHOW_WINDOW_CMD.SW_SHOW);
                            var winexS = new Win32Exception(Marshal.GetLastWin32Error());
                            errorCodeS = winexS.ErrorCode;

                            //File.AppendAllText("ordenar finestres.txt", $"\n{account.Nom} {resS} {resH} {pr.MainWindowHandle} {pr.MainWindowTitle} {errorCodeH} {errorCodeS} {winexH.Message} {winexS.Message}");
                            isOrdered = true;
                        } while ((resH.Value != 24 || resS.Value != 0) && i < 3);
                        //File.AppendAllText("ordenar finestres.txt", "-------------------------------END------------------------------------");
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        private static List<Process> GetAllProcess()
        {
            return Process.GetProcessesByName("Dofus").ToList();
        }

        private void btnChangeIcons_Click(object sender, RoutedEventArgs e)
        {

            SetWindowsIcons(true, false);
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
            var comp = (Composition)comboBoxCompositions.SelectedItem;
            var personatges = (ObservableCollection<Personatge>)personatgeViewSource.Source;

            var found = personatges.FirstOrDefault(x => x.Posicio == e.PosicioNova && x.IdComposition == comp.Id && x.Nom != actualData.Nom);
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
                trobat.Language = (string)comboBox.SelectedValue;

                dofusContext.SaveChanges();
                MessageBox.Show("Restart the app to apply the language changes");
            }
        }

        private void Character_CharacterRemoved(object sender, string id)
        {
            var personatges = (ObservableCollection<Personatge>)personatgeViewSource.Source;
            var compId = ((Composition)comboBoxCompositions.SelectedItem).Id;
            var found = personatges.FirstOrDefault(x => x.Nom == id);
            if (found != null)
            {
                personatges.Remove(found);
                int i = 0;
                foreach (var personatge in personatges.Where(x => x.IdComposition == compId).OrderBy(x => x.Posicio))
                {
                    personatge.Posicio = i;
                    i++;
                }

                var r = dofusContext.SaveChanges();
                personatgeViewSource.View.Refresh();
            }
        }

        private void Character_CharacterIsActiveChanged(object sender, string id, bool isActive)
        {
            var personatges = (ObservableCollection<Personatge>)personatgeViewSource.Source;
            var found = personatges.FirstOrDefault(x => x.Nom == id);
            if (found != null)
            {
                found.SetActive(isActive);
                dofusContext.SaveChanges();
                personatgeViewSource.View.Refresh();
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }

        private void btnChangekey_Click(object sender, RoutedEventArgs e)
        {
            PInvoke.UnhookWindowsHookEx(new HHOOK(_hookID));
            var trobat = dofusContext.Configuracios.First();

            EditKey editKey = new EditKey();

            editKey.KeyCodes = new ObservableCollection<int>(KeyCodesExtensions.ConvertKeys(trobat.KeyCodes));
            if (editKey.ShowDialog().GetValueOrDefault())
            {
                MainWindow.keyCodes = editKey.KeyCodes.ToList();
                trobat.KeyCodes = String.Join("|", editKey.KeyCodes);


                tbxKey.Text = KeyCodesExtensions.ConvertToTextBoxString(editKey.KeyCodes);
                dofusContext.SaveChanges();
            }
            _hookID = SetHookKey(_procKeyBoard);
        }

        private void btnAddComp_Click(object sender, RoutedEventArgs e)
        {

            AddComposition addComposition = new AddComposition();

            if (addComposition.ShowDialog().GetValueOrDefault())
            {
                var lastid = dofusContext.Compositions.AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefault()?.Id;
                var compo = new Composition()
                {
                    Nom = addComposition.CompositionName,
                    Id = lastid.HasValue ? lastid.Value + 1 : 1,
                };
                dofusContext.Compositions.Add(compo);

                dofusContext.SaveChanges();
                comboBoxCompositions.SelectedItem = compo;
            }
        }

        private void btnRemoveComp_Click(object sender, RoutedEventArgs e)
        {
            var item = (Composition)comboBoxCompositions.SelectedItem;

            dofusContext.Compositions.Remove(item);
            dofusContext.SaveChanges();
            //compositionsViewSource.View.Refresh();
        }

        private void comboBoxCompositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comp = (Composition)comboBoxCompositions.SelectedItem;
            if (comp != null)
            {
                var conf = dofusContext.Configuracios.First();
                if (conf.LastCompositionId != comp.Id)
                {
                    conf.LastCompositionId = comp.Id;
                    dofusContext.SaveChanges();
                }
                personatgeViewSource.View.Filter = x =>
                {
                    var p = (Personatge)x;
                    return p.IdComposition == comp.Id;
                };
                if (OrderAutomatically)
                {
                    OrderWindows();
                }
            }
        }

        private void btnEditComp_Click(object sender, RoutedEventArgs e)
        {
            var comp = (Composition)comboBoxCompositions.SelectedItem;
            AddComposition addComposition = new AddComposition(comp.Nom);
            if (addComposition.ShowDialog().GetValueOrDefault())
            {
                var c = dofusContext.Compositions.Local.First(x => x.Id == comp.Id);
                c.Nom = addComposition.CompositionName;
                comboBoxCompositions.SelectedIndex = dofusContext.Compositions.Local.ToList().IndexOf(c);
                dofusContext.SaveChanges();
                compositionsViewSource.View.Refresh();
            }
        }

        private void cbxKeyboard_Click(object sender, RoutedEventArgs e)
        {
            var conf = dofusContext.Configuracios.First();
            conf.SetKeyboardEnabled(cbxKeyboard.IsChecked.GetValueOrDefault());
            if (cbxKeyboard.IsChecked == true)
            {
                _hookID = SetHookKey(_procKeyBoard);
            }
            else
            {
                PInvoke.UnhookWindowsHookEx(new Windows.Win32.UI.WindowsAndMessaging.HHOOK(_hookID));
            }
            dofusContext.SaveChanges();
        }

        private void cbxMouse_Click(object sender, RoutedEventArgs e)
        {
            var conf = dofusContext.Configuracios.First();
            conf.SetMouseEnabled(cbxMouse.IsChecked.GetValueOrDefault());
            if (cbxMouse.IsChecked == true)
            {
                _hookIDM = SetHookM(_proc);
            }
            else
            {
                PInvoke.UnhookWindowsHookEx(new Windows.Win32.UI.WindowsAndMessaging.HHOOK(_hookIDM));
            }
            dofusContext.SaveChanges();
        }

        private void btnSeparateWindows_Click(object sender, RoutedEventArgs e)
        {
            var processes = GetAllProcess();

            var g = new Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99");
            object ppsTemp;
            int i = 1;
            foreach (var proces in processes)
            {
                var result = PInvoke.SHGetPropertyStoreForWindow(new HWND(proces.MainWindowHandle), g, out ppsTemp);
                var win32exc = new Win32Exception(Marshal.GetLastWin32Error());
                IPropertyStore propertyStore1 = (IPropertyStore)ppsTemp;
                PropertyKey pROPERTYKEY = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
                string appIid = !WindowsAreSeparated ? APP_IID + i : APP_IID;
                PropVariant propVariant = new PropVariant(appIid);
                propertyStore1.SetValue(pROPERTYKEY, propVariant);
                propertyStore1.Commit();
                Console.WriteLine(win32exc.Message);
                i++;
            }

            if (!WindowsAreSeparated)
            {
                btnSeparateWindows.Content = Properties.Resources.group_windows;
                WindowsAreSeparated = true;
            }
            else
            {
                btnSeparateWindows.Content = Properties.Resources.separate_windows;
                WindowsAreSeparated = false;
            }

        }

        private void chbxOrderWindowsOnChange_Click(object sender, RoutedEventArgs e)
        {

            var conf = dofusContext.Configuracios.First();
            if (conf != null)
            {
                conf.SetOrderWindowsOnChangeComp(OrderAutomatically);

                if (OrderAutomatically)
                {
                    OrderWindows();
                }
                dofusContext.SaveChanges();
            }
        }

        //private void Character_KeyEdited(object sender, string id, string newValue)
        //{
        //    var compId = ((Composition)comboBoxCompositions.SelectedValue).Id;
        //    var found = dofusContext.Personatges.FirstOrDefault(x => x.IdComposition == compId && x.Nom == id);
        //    if (found != null)
        //    {
        //        found.KeyCodes = newValue;
        //        dofusContext.SaveChanges();
        //    }
        //}

        private void Character_KeyRemoved(object sender, string id)
        {
            var compId = ((Composition)comboBoxCompositions.SelectedValue).Id;
            var found = dofusContext.Personatges.FirstOrDefault(x => x.IdComposition == compId && x.Nom == id);
            if (found != null)
            {
                found.KeyCodes = null;
                dofusContext.SaveChanges();
                personatgeViewSource.View.Refresh();
            }
        }

        private void Character_KeyEdited(object sender, string id, ICollection<int> newValue, string parsedKeyCodes)
        {
            var compId = ((Composition)comboBoxCompositions.SelectedValue).Id;
            var personatgesCompo = dofusContext.Personatges.Where(x => x.IdComposition == compId);
            bool isDuplicated = false;
            foreach (var personatge in personatgesCompo.Where(x => x.Nom != id))
            {
                var keyCodes = personatge.GetKeyCodes();
                isDuplicated = AlreadyHasKey(newValue, keyCodes);
                if (isDuplicated)
                {
                    MessageBox.Show(Properties.Resources.error_keybinding_duplicated, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }

            isDuplicated = AlreadyHasKey(newValue, keyCodes);

            if (isDuplicated)
            {
                MessageBox.Show(Properties.Resources.error_keybinding_duplicated, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var found = personatgesCompo.FirstOrDefault(x => x.Nom == id);
            if (found != null)
            {
                found.KeyCodes = parsedKeyCodes;
                dofusContext.SaveChanges();
                personatgeViewSource.View.Refresh();
            }
        }

        private static bool AlreadyHasKey(ICollection<int> newValue, IEnumerable<int>? keyCodes)
        {
            bool hasAlready = true;
            if (keyCodes != null && keyCodes.Count() > 0)
            {
                var orderedKeyCodes = keyCodes.OrderBy(x => x);
                var orderedNewKeyCodes = newValue.OrderBy(x => x);
                int i = 0;

                foreach (var keyCode in orderedNewKeyCodes)
                {
                    if (keyCode != orderedKeyCodes.ElementAtOrDefault(i))
                    {
                        hasAlready = false;
                        break;
                    }
                    i++;
                }

            }
            return hasAlready;
        }

        private void btnKillWindows_Click(object sender, RoutedEventArgs e)
        {
            var processes = GetAllProcess();
            foreach (var item in processes)
            {
                item.Kill();
            }
        }

        private void chbTaskbarIcons_Click(object sender, RoutedEventArgs e)
        {
            //TaskbarGlomLevel 2(Hexadecimal)
            var advancedSubKey = RegistryExplorer.GetTaskbarRegistryPropertie();
            if (chbTaskbarIcons.IsChecked.GetValueOrDefault())
            {

                var taskBarGlomLevel = advancedSubKey?.GetValue(RegistryExplorer.TASKBAR_GLOOM_LEVEL);
                

                if (taskBarGlomLevel == null)
                {
                    SetTBGlomLevel(advancedSubKey);
                }
                else
                {
                    var regKind = advancedSubKey.GetValueKind(RegistryExplorer.TASKBAR_GLOOM_LEVEL);
                    if (regKind != RegistryValueKind.DWord)
                    {
                        DeleteTBGlomLevel(advancedSubKey);
                        SetTBGlomLevel(advancedSubKey);
                    }
                }
            }
            else
            {
                DeleteTBGlomLevel(advancedSubKey);
            }
            ResetExplorer();
        }

        private static void DeleteTBGlomLevel(RegistryKey? advancedSubKey)
        {
            advancedSubKey?.DeleteValue(RegistryExplorer.TASKBAR_GLOOM_LEVEL);
        }

        private static void SetTBGlomLevel(RegistryKey? advancedSubKey)
        {
            try
            {
                advancedSubKey?.SetValue(RegistryExplorer.TASKBAR_GLOOM_LEVEL, 2, RegistryValueKind.DWord);
            }
            catch (UnauthorizedAccessException)
            {

                MessageBox.Show(Properties.Resources.privilege_error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetExplorer()
        {
            var explorer = Process.GetProcessesByName("explorer").FirstOrDefault();
            if (explorer != null)
            {
                explorer.Kill();
            }

        }

        private void btnChangeTitle_Click(object sender, RoutedEventArgs e)
        {
            SetWindowsIcons(false, true);
        }
    }
}
