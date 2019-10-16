using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Fork.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ForkHelper.Init();

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = Fork.Gui.Properties.Resources.fork_512;
            //notifyIcon.Icon = new System.Drawing.Icon(System.Drawing.SystemIcons.Shield, 40, 40);
            notifyIcon.Visible = true;

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();

            var showItem = notifyIcon.ContextMenuStrip.Items.Add("Show");
            showItem.Click += NotifyIcon_DoubleClick;
            showItem.Font = new System.Drawing.Font(showItem.Font, System.Drawing.FontStyle.Bold);

            notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (o, x) => Shutdown();

            

        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
                MainWindow?.Show();
            }
            else
            {
                var wnd = MainWindow;
                if (wnd != null)
                {
                    if (wnd.WindowState != WindowState.Normal)
                        wnd.WindowState = WindowState.Normal;

                    wnd.Activate();
                }
            }
            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
