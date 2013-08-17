using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Freestyle
{
    class WWAMonitor
    {
        static int PIDSelf = Process.GetCurrentProcess().Id;

        public event Action<WWAMonitor, WWAApp> AppStarted;
        public event Action<WWAMonitor, WWAApp> AppSwitched;
        public event Action<WWAMonitor, string> FocusChangedFromApp;

        List<WWAApp> KnownWWAs = new List<WWAApp>();

        public void Start()
        {
            var hook = new Platform.WinEventHookHelper(Platform.User32.NativeMethods.SystemEvents.EVENT_SYSTEM_FOREGROUND);
            hook.OnEvent += () =>
            {
                ForegroundChanged(Platform.User32.GetForegroundWindow());
            };
            hook.Start();
        }

        void ForegroundChanged(IntPtr hWnd)
        {
            string cls = Platform.User32.GetClassName(hWnd);
            Trace.WriteLine("FG: " + hWnd + " " + cls);
            if (cls == "Windows.UI.Core.CoreWindow") // Metro WWA App
            {
                var proc = Process.GetProcessById(Platform.User32.GetWindowThreadProcessId(hWnd)).
                    ProcessName.ToLower();
                if (proc == "wwahost" || proc == "authhost")
                {
                    FoundWWA(hWnd);
                }
                else
                {
                    // TEMP: lets just *try* with everything.
                    FoundWWA(hWnd);
                    Trace.WriteLine("Attempting process: " + proc);
                }
                // else == iexplore for IE

                // Known:
                // wwahost
                // authhost (hosts Windows Live ID)
                // iexplore

            }
            else if (cls == "ImmersiveSplashScreenWindowClass") // Metro App starting up
            {
                // Wait for splash screen
                new Timer(_ => ForegroundChanged(hWnd), null, 250, Timeout.Infinite);
            }
            else if (cls == "")
            {
                // The Window is gone (possibly during splash screen transition), so try again.
                new Timer(_ => ForegroundChanged(Platform.User32.GetForegroundWindow()), null, 250, Timeout.Infinite);
            }
            else if (cls == "LyncConversationWindowClass")
            {
                FoundLyncWindow(hWnd);
            }
            else
            {
                int pid = Platform.User32.GetWindowThreadProcessId(hWnd);
                if (pid == PIDSelf)
                {
                    // We are exceptional
                    // TODO: also make magnifier exceptional
                    Trace.WriteLine("Focused change to Freestyle");
                }
                else
                {
                    FocusChangedFromApp(this, cls);
                }
            }
        }

        private void FoundLyncWindow(IntPtr hWnd)
        {
            new LyncWindow(hWnd);
        }

        void FoundWWA(IntPtr hWnd)
        {
            var wwa = KnownWWAs.ToList().FirstOrDefault(w => w.hWnd == hWnd);
            if (wwa == null)
            {
                wwa = new WWAApp(hWnd);
                KnownWWAs.Add(wwa);
                AppStarted(this, wwa);
            }
            else
            {
                AppSwitched(this, wwa);
            }
        }

        public void AppInteractionFailed(WWAApp app)
        {
            // If there is an error with the app, remove it.
            // We'll retry next time it jumps to the foreground.
            if (KnownWWAs.Contains(app)) KnownWWAs.Remove(app);
        }
    }
}
