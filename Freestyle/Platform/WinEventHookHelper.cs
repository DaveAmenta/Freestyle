using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Freestyle.Platform
{
    class WinEventHookHelper
    {
        public event Action OnEvent;

        private IntPtr _Hook;
        private User32.NativeMethods.SystemEvents evt;
        static private User32.NativeMethods.WinEventDelegate evtProc;

        public WinEventHookHelper(User32.NativeMethods.SystemEvents evt)
        {
            this.evt = evt;
            evtProc = new User32.NativeMethods.WinEventDelegate(EventProc);
        }

        public void TouchEvent()
        {
            Trace.WriteLine(evtProc.ToString());
        }

        public void Start()
        {
           // var t = new Thread(() =>
            //    {

                    _Hook = User32.NativeMethods.SetWinEventHook(
                        (uint)evt, (uint)evt, IntPtr.Zero, evtProc,
                        0, 0, User32.NativeMethods.WINEVENT_OUTOFCONTEXT);
                    // Cheat and use Windows Forms to pump messages
                //    System.Windows.Forms.Application.Run();
                   // GC.KeepAlive(evtProc);
            //    });
            //t.SetApartmentState(ApartmentState.STA);
           // t.Start();
        }

        void EventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            OnEvent();
        }

        public void Stop()
        {
            User32.NativeMethods.UnhookWinEvent(_Hook);
            _Hook = IntPtr.Zero;
            // TODO: Shut down the hook thread.
        }
    }
}
