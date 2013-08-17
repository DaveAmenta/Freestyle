using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Freestyle
{

    /* EXTERN_C const IID DIID_DispHTMLDocument;


    MIDL_INTERFACE("3050f55f-98b5-11cf-bb82-00aa00bdce0b")
    DispHTMLDocument : public IDispatch
    {
    };
     */

    [ComImport]
    [Guid("3050f55f-98b5-11cf-bb82-00aa00bdce0b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface DispHTMLDocument
    {

    }



    /*
    class Com
    {
        private static Dispatcher comDispatcher = null;

        public static void Init()
        {
            var comThread = new Thread(() =>
                {
                    Trace.WriteLine("Running COM thread");
                    comDispatcher = Dispatcher.CurrentDispatcher;
                    Dispatcher.Run();
                });
            comThread.SetApartmentState(ApartmentState.STA);
            comThread.Start();
        }

        public static void Run(Action COMThreadAction)
        {
            comDispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                try
                {
                    COMThreadAction.Invoke();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("*** Error on COM thread: " + ex);
                }
            }));
        }
    } */
}
