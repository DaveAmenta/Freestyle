using System;
using MSHTML;

namespace Freestyle
{
    class HtmlHelper
    {
        public static IHTMLStyleSheet GetSheet(HTMLDocument doc, string sheetName)
        {
            /*
            var styleSheets = doc.GetProperty("styleSheets") as HTMLStyleSheetsCollection;
            foreach (IHTMLStyleSheet sheet in styleSheets)
            {
                if (sheet.title == sheetName) return sheet;
            } */
            var newSheet = doc.createStyleSheet();
            newSheet.title = sheetName;
            return newSheet;
        }
    }
}
