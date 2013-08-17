using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using MSHTML;

namespace Freestyle
{
    class ExportPage
    {
        internal static void ExportAll(string pageName, MSHTML.HTMLDocument Doc)
        {
            try
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (!Directory.Exists(Path.Combine(desktop, "Freestyle")))
                {
                    Directory.CreateDirectory(Path.Combine(desktop, "Freestyle"));
                }

                var safeName = SafeFileName(Doc.url);
                if (!Directory.Exists(Path.Combine(desktop, "Freestyle", safeName)))
                {
                    Directory.CreateDirectory(Path.Combine(desktop, "Freestyle", safeName));
                }

                File.WriteAllText(Path.Combine(desktop, "Freestyle", safeName, SafeFileName(pageName) + ".txt"), Doc.documentElement.innerHTML);
                
                try
                {
                    int i = 0;
                    foreach (IHTMLStyleSheet sheet in Doc.styleSheets)
                    {
                        File.WriteAllText(Path.Combine(desktop, "Freestyle", safeName, SafeFileName(pageName) + "_css_ " + ++i + ".txt"), sheet.cssText);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error getting stylesheets: " + ex);
                }

                int s = 0;
                foreach (IHTMLElement script in Doc.getElementsByTagName("script"))
                {
                    File.WriteAllText(Path.Combine(desktop, "Freestyle", safeName, SafeFileName(pageName) + "_js_ " + ++s + ".txt"), script.innerText);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public static string SafeFileName(string unsafeFileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                unsafeFileName = unsafeFileName.Replace(c.ToString(), "");
            foreach (char c in Path.GetInvalidPathChars())
                unsafeFileName = unsafeFileName.Replace(c.ToString(), "");
            return unsafeFileName;
        }
    }
}
