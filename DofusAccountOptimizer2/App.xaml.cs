﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Windows.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;

namespace DofusAccountOptimizer2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        NotifyIcon NotifyIcon = new NotifyIcon();
        public App()
        {
            using (DofusContext dofusContext = new DofusContext())
            {
                dofusContext.Database.EnsureCreated();
            }
            NotifyIcon.Icon = new Icon($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\pandawa_ico.ico");
            NotifyIcon.Visible = true;
            //NotifyIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);

            NotifyIcon.MouseClick += NotifyIcon_MouseClick;
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripButton toolStripButton = new ToolStripButton();
            toolStripButton.Text = "Exit";
            toolStripButton.Click += ToolStripButton_Click;
            contextMenuStrip.Items.Add(toolStripButton);
            NotifyIcon.ContextMenuStrip = contextMenuStrip;

        }

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                NotifyIcon.ContextMenuStrip.Show();
            }
            else if(e.Button == MouseButtons.Left)
            {
                this.MainWindow.Show();
                IntPtr windowHandle = new WindowInteropHelper(this.MainWindow).Handle;
                PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(windowHandle));
            }
        }

        private void ToolStripButton_Click(object? sender, EventArgs e)
        {
            this.Shutdown();
        }

        private void NotifyIcon_Click(object? sender, EventArgs e)
        {
            
            
        }
    }
}
