using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSHTML;
using System.IO;
using System.Diagnostics;

namespace Freestyle
{
    public class Profile
    {
        public static IEnumerable<Profile> GetProfiles(string url)
        {
            List<Profile> SupportedProfiles = new List<Profile>();

            foreach (var profileName in Directory.GetDirectories("profiles"))
            {
                var p = new Profile(Path.GetFileName(profileName));

                foreach(var file in Directory.GetFiles(profileName))
                {
                    switch(Path.GetFileName(file))
                    {
                        case "default.css":
                            p.DefaultCss = File.ReadAllText(file);
                            break;
                        case "default.js":
                            p.DefaultJs = File.ReadAllText(file);
                            break;
                        case "default.htm":
                            p.DefaultHtml = File.ReadAllText(file);
                            break;
                        case "urlscope.txt":
                            p.UrlScope = File.ReadAllText(file);
                            break;
                        case "action.css":
                            p.ActionCss = File.ReadAllText(file);
                            break;
                        case "action.js":
                            p.ActionJs = File.ReadAllText(file);
                            break;

                        default:
                            Trace.WriteLine("Unknown profile file: " + file);
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(p.UrlScope))
                {
                    Trace.WriteLine(string.Format("{0} Is invalid (missing UrlScope)",p.Name));
                }
                else
                {
                    if (url.StartsWith(p.UrlScope) || p.UrlScope == "*")
                    {
                        SupportedProfiles.Add(p);
                    }
                }
            }

            return SupportedProfiles;
        }

        public string DefaultCss { get; private set; }
        public string DefaultJs { get; private set; }
        public string DefaultHtml { get; private set; }

        public string ActionCss { get; private set; }
        public string ActionJs { get; set; }

        public string UrlScope { get; private set; }

        public int Count { get; set; }
        public string Name { get; private set; }

        public bool HasAction
        {
            get { return !string.IsNullOrWhiteSpace(ActionCss) ||
                         !string.IsNullOrWhiteSpace(ActionJs);
            }
        }

        public bool HasActionValue
        {
            get { return ActionCss != null &&
                    ActionCss.Contains("[VALUE]"); }
        }

        static int counter = 0;
        public Profile(string name)
        {
            this.Name = name;
            this.Count = ++counter;
        }

        public void DoDefaultInjection(WWAApp app)
        {
            if (!string.IsNullOrWhiteSpace(DefaultCss))
            {
                Trace.WriteLine("Injecting default CSS...");
                var css = app.AppDocument.createStyleSheet();
                css.title = "_AccDefaultSheet" + counter;
                css.cssText = DefaultCss;
            }

            if (!string.IsNullOrWhiteSpace(DefaultHtml))
            {
                Trace.WriteLine("Injecting default HTML...");
                var div = app.AppDocument.createElement("div");
                div.innerHTML = DefaultHtml;
                var body = (app.AppDocument.body as HTMLBody);
                body.appendChild(div as IHTMLDOMNode);
            }

            if (!string.IsNullOrWhiteSpace(DefaultJs))
            {
                Trace.WriteLine("Injecting default JS...");
                var script = app.AppDocument.createElement("script") as HTMLScriptElement;
                script.type = "text/javascript";
                script.innerText = DefaultJs;
                var heads = app.AppDocument.getElementsByTagName("head");
                var head =  (heads.item(0, 0) as HTMLHeadElement);
                head.appendChild(script as IHTMLDOMNode);
            }
        }

        public void ApplyAction(WWAApp app, HTMLDocument Doc, string value)
        {
            if (!string.IsNullOrWhiteSpace(ActionCss))
            {
                var sheet = HtmlHelper.GetSheet(Doc, "_AccActionSheet" + counter);
                sheet.cssText = ActionCss.Replace("[VALUE]", value);
            }

            if (!string.IsNullOrWhiteSpace(ActionJs))
            {
                Trace.WriteLine("Injecting action JS...");
                var script = Doc.createElement("script") as HTMLScriptElement;
                script.type = "text/javascript";
                //script.innerText = ActionJs;
                script.SetProperty("innerText", ActionJs);
                var heads = Doc.getElementsByTagName("head");
                var head = (heads.item(0, 0) as HTMLHeadElement);
                head.appendChild(script as IHTMLDOMNode);
            }
        }

    }
}
