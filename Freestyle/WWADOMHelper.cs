using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSHTML;
using System.Runtime.InteropServices;
using Accessibility;
using System.Diagnostics;

namespace Freestyle
{
    class WWADOMHelper
    {
        #region Win32 Interop
        private delegate int EnumProc(IntPtr hWnd, ref IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern int EnumChildWindows(IntPtr hWndParent, EnumProc lpEnumFunc, ref  IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            int fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);
        [DllImport("OLEACC.dll")]
        private static extern int ObjectFromLresult(UIntPtr lResult, ref Guid riid, int wParam, out IHTMLDocument2 ppvObject);
        private const int SMTO_ABORTIFHUNG = 0x2;
        private static Guid IID_IHTMLDocument = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");
        #endregion

        public static IHTMLDocument2 GetDocumentForWindow(IntPtr hWnd)
        {
            IHTMLDocument2 document;
            EnumProc proc = new EnumProc(EnumWindows);
            IntPtr hWndChild = IntPtr.Zero;
            EnumChildWindows(hWnd, proc, ref hWndChild);
            if (!hWndChild.Equals(IntPtr.Zero))
            {
                uint lngMsg = RegisterWindowMessage("WM_HTML_GETOBJECT");
                if (lngMsg != 0)
                {
                    UIntPtr lRes;
                    SendMessageTimeout(hWndChild, lngMsg, (UIntPtr)0, (IntPtr)0, SMTO_ABORTIFHUNG, 1000, out lRes);
                    if (lRes != UIntPtr.Zero)
                    {
                        int hr = ObjectFromLresult(lRes, ref IID_IHTMLDocument, 0, out document);
                        if (hr == 0) // S_OK;
                        {
                            return document;
                        }
                    }
                }
            }
            return null;
        }

        private static int EnumWindows(IntPtr hWnd, ref IntPtr lParam)
        {
            if (Platform.User32.GetClassName(hWnd) == "Internet Explorer_Server")
            {
                lParam = hWnd;
                return 0;
            }
            else
            {
                return 1;
            }
        }

        static public IHTMLDocument2 GetDocumentForFrame(object element)
        {
            var Acc = (element as Platform.IServiceProvider).QueryService<IAccessible>();
            var Win = (Acc as Platform.IServiceProvider).QueryService<IHTMLWindow2>();
            return Win.document;
        }

        private List<IntPtr> Docs = new List<IntPtr>();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private string GetText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = 4096;
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }


        public List<string> GetTexts(IntPtr hwnd)
        {
 EnumProc proc = new EnumProc(EnumWindowProc);
            IntPtr hWndChild = IntPtr.Zero;
            EnumChildWindows(hwnd, proc, ref hWndChild);

            List<string> RetDocs = new List<string>();

            foreach (var hw in Docs)
            {
                RetDocs.Add(GetText(hw));
            }
            return RetDocs;
        }

        public List<IHTMLDocument2> GetDocumentsForWindow(IntPtr hWnd)
        {
            IHTMLDocument2 document;
            EnumProc proc = new EnumProc(EnumWindowProc);
            IntPtr hWndChild = IntPtr.Zero;
            EnumChildWindows(hWnd, proc, ref hWndChild);

            List<IHTMLDocument2> RetDocs = new List<IHTMLDocument2>();

            foreach (var hw in Docs)
            {
                if (!hw.Equals(IntPtr.Zero))
                {
                    uint lngMsg = RegisterWindowMessage("WM_HTML_GETOBJECT");
                    if (lngMsg != 0)
                    {
                        UIntPtr lRes;
                        SendMessageTimeout(hWndChild, lngMsg, (UIntPtr)0, (IntPtr)0, SMTO_ABORTIFHUNG, 1000, out lRes);
                        if (lRes != UIntPtr.Zero)
                        {
                            int hr = ObjectFromLresult(lRes, ref IID_IHTMLDocument, 0, out document);
                            if (hr == 0) // S_OK;
                            {
                                RetDocs.Add(document);
                            }
                        }
                    }
                }
            }
            return RetDocs;
        }

        private int EnumWindowProc(IntPtr hWnd, ref IntPtr lParam)
        {
            var cls = Platform.User32.GetClassName(hWnd);
            Trace.WriteLine("ENF: " + cls);
            if (cls == "RICHEDIT60W")
            {
                Docs.Add(hWnd);
                //lParam = hWnd;
                return 1;
            }
            else
            {
                return 1;
            }
        }
    }
}
