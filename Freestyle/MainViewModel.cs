using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Freestyle
{
    public class MainViewModel
    {
        private WWAApp _CurrentApp;
        public WWAApp CurrentApp
        {
            get { return _CurrentApp; }
            set
            {
                if (_CurrentApp != value)
                {
                    if (CurrentApp != null)
                    {
                        _CurrentApp.ReleaseResources();
                    }

                    _CurrentApp = value;
                    if (CurrentAppChanging != null) CurrentAppChanging();
                }
            }
        }
        public event Action CurrentAppChanging;

        public MainViewModel()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBox.Show("Error: " + e.ExceptionObject.ToString());
            };
        }

        public void Initialize()
        {
           // Com.Init();

            var mon = new WWAMonitor();
            mon.AppStarted += new Action<WWAMonitor, WWAApp>(mon_AppStarted);
            mon.AppSwitched += new Action<WWAMonitor, WWAApp>(mon_AppSwitched);
            mon.FocusChangedFromApp += new Action<WWAMonitor, string>(mon_FocusChangedFromApp);
            mon.Start();
        }

        void mon_FocusChangedFromApp(WWAMonitor arg1, string arg2)
        {
            Trace.WriteLine(":: " + arg2);

            CurrentApp = null;
            // TODO hide
        }

        void mon_AppSwitched(WWAMonitor mon, WWAApp app)
        {
            // User switched to a running app that we already know about.
            Trace.WriteLine("Switch: " + app);
            CurrentApp = app;
        }

        void mon_AppStarted(WWAMonitor mon, WWAApp app)
        {
            // First time we've seen the app.
            Trace.WriteLine("App Start: " + app);
            CurrentApp = app;

            app.Initialize();
        }
    }
}
