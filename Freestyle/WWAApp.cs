using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSHTML;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;

namespace Freestyle
{
    public class WWAApp
    {
        public IEnumerable<Profile> Profiles { get; private set; }
        public IntPtr hWnd { get; private set; }
        public HTMLDocument AppDocument { get; private set; }

        public string Title
        {
            get { return Platform.User32.GetWindowText(hWnd); }
        }

        public string Url
        {
            get
            {
                while (true)
                {
                    var readyState = AppDocument.GetProperty("readyState") as string;
                    if (readyState == "complete")
                    {
                        break;
                    }
                    else
                    {
                        Trace.WriteLine("ReadyState: " + readyState);
                    }
                    Thread.Sleep(100);
                }
                Trace.Write("App URL: " + AppDocument.GetProperty("URL"));
                return AppDocument.GetProperty("URL") as string;
            }
        }

        public WWAApp(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            Profiles = new List<Profile>();
            AppDocument = WWADOMHelper.GetDocumentForWindow(hWnd) as HTMLDocument;
        }

        public Dictionary<string, HTMLDocument> Frames
        {
            get
            {
                Dictionary<string, HTMLDocument> Docs = new Dictionary<string, HTMLDocument>();

                int i = 0;
                var iframes = AppDocument.getElementsByTagName("iframe");
                foreach (IHTMLElement iframe in iframes)
                {
                    var name = iframe.id;
                    if (string.IsNullOrWhiteSpace(name)) name = iframe.className;
                    if (string.IsNullOrWhiteSpace(name)) name = "Frame " + (++i);
                    var unsecureFrame = WWADOMHelper.GetDocumentForFrame(iframe);
                    Docs.Add(name, unsecureFrame as HTMLDocument);
                }
                return Docs;
            }
        }

        public void Initialize()
        {
            Profiles = Profile.GetProfiles(Url);
            foreach (var p in Profiles)
            {
                p.DoDefaultInjection(this);
            }
        }

        public override string ToString()
        {
            return string.Format("[WWA: {0} {1} {2}]", hWnd, Title, Url);
        }

        internal void ReleaseResources()
        {
            // throw new NotImplementedException();
        }
    }
}
