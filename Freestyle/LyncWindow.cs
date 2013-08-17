using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Freestyle
{


    class LyncWindow
    {

        public string Title
        {
            get { return Platform.User32.GetWindowText(hWnd); }
        }

        public IntPtr hWnd { get; private set; }

        public LyncWindow(IntPtr hWnd)
        {
            this.hWnd = hWnd;

            var wd = new WWADOMHelper();

            var windows = wd.GetTexts(hWnd);

            foreach (var x in windows)
            {
                Trace.WriteLine("IE Window in Lync: " + x);
            }
        }



    }
}
