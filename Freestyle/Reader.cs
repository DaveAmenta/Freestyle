using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSHTML;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace Freestyle
{
    class Reader
    {
        internal static void EnableReaderMode(WWAApp app, HTMLDocument frame)
        {
            var f = app.Frames.Values.First();
            var bodyx = f.GetProperty("body") as HTMLBody;
            var content = bodyx.innerHTML;

            int height = app.AppDocument.documentElement.offsetHeight;
            int width = app.AppDocument.documentElement.offsetWidth;

            height -= 128;
            width -= 96;

            var css = app.AppDocument.createStyleSheet();
            css.cssText = @"
#AccReaderControl
{
	position: absolute;
	top: 80px; left: 32px; 
	display: block; 
	background: white; 
	color: black;
    padding: 12px;
    font-size: 48px;
    font-family: Segoe UI;
	border-style: solid;
    border-width: 1px;
	font-size: 30px; 
    overflow: scroll;
	width: " + width + @"px; height: " + height + @"px; 
	z-index: 9999;
}

#AccReaderControl *
{
	font-size: 30px; 
}

.AccReaderButton
{
	position: absolute;
	top: 20px; right: 0px; 
	display: block; 
	background: black; 
	color: white;
    padding: 12px;
    font-size: 48px;
    font-family: Verdana;
    text-align: center;
    vertical-align: middle;
	border-style: solid;
    border-width: 1px;
	font-size: 30px; 
	width: 80px; height: 30px; 
	z-index: 9999;
}
";

            var div = app.AppDocument.createElement("div");
            div.innerHTML = @"<div id='AccReaderControl'>Loading Content...</div>
                            <div class='AccReaderButton' id='AccReaderCloseButton'>Close</div>
                            <div class='AccReaderButton' style='right: 100px' id='AccReaderPlusButton'>[ + ]</div>
                            <div class='AccReaderButton' style='right: 200px' id='AccReaderMinButton'>[ - ]</div>

";
            var body = app.AppDocument.GetProperty("body") as HTMLBody;

            body.appendChild(div as IHTMLDOMNode);

            Trace.WriteLine("Injecting default JS...");
            var script = app.AppDocument.createElement("script") as HTMLScriptElement;
            script.type = "text/javascript";
            script.innerText = @"

function getCssRule(m)
{
    for (var i=0; i<document.styleSheets.length; i++)
    {
        var sheet = document.styleSheets[i];
        var r = 0;
        var rule = false;
        do
        {
            rule = sheet.rules[r];
            if (rule && rule.selectorText.toLowerCase()==m.toLowerCase())
            {
                return rule;
            }
        r++;
        } while(rule)
    }
    return null;
}

function getCssRules(m, cb)
{
    for (var i=0; i<document.styleSheets.length; i++)
    {
        var sheet = document.styleSheets[i];
        var r = 0;
        var rule = false;
        do
        {
            rule = sheet.rules[r];
            if (rule && rule.selectorText.toLowerCase()==m.toLowerCase())
            {
                cb(rule);
            }
        r++;
        } while(rule)
    }
}

function allCssRules(cb)
{
    for (var i=0; i<document.styleSheets.length; i++)
    {
        var sheet = document.styleSheets[i];
        var r = 0;
        var rule = false;
        do
        {
            rule = sheet.rules[r];
            if (rule)
            {
                cb(rule);
            }
        r++;
        } while(rule)
    }
}

function getFontSizeType(fontSize)
{
    if (fontSize.indexOf('px') != -1)
    {
        return 'px';
    }
    if (fontSize.indexOf('pt') != -1)
    {
        return 'pt';
    }
    if (fontSize.indexOf('em') != -1)
    {
        return 'em';
    }
    if (fontSize.indexOf('ex') != -1)
    {
        return 'ex';
    }
    if (fontSize.indexOf('%') != -1)
    {
        return '%';
    }
    return 'px';
}

function getIncForfontSizeType(ty)
{
    switch(ty)
    {
        case 'px':
            return 2;
        case 'px':
            return 2;
        case 'pt':
            return 1;
        case 'em':
            return 0.2;
        case 'ex':
            return 0.4;
        case '%':
            return 10;
    } 
    return 1;
}

function addProperty(prop, multip)
{
    var ty = getFontSizeType(prop);
    var inc = getIncForfontSizeType(ty);
    return (parseFloat(prop) + (inc * multip)) + ty; 
}

function addFontSizeByRule(r, s)
{
    var rule = getCssRule(r);
    if (rule != null)
    { 
        rule.style.fontSize = addProperty(rule.style.fontSize, s);
        rule.style.lineHeight = addProperty(rule.style.lineHeight, s);
    }
}



document.getElementById('AccReaderCloseButton').addEventListener('click', function(){
    element = document.getElementById('AccReaderControl');
    element.parentNode.removeChild(element);

    element = document.getElementById('AccReaderPlusButton');
    element.parentNode.removeChild(element);
    element = document.getElementById('AccReaderMinButton');
    element.parentNode.removeChild(element);
    element = document.getElementById('AccReaderCloseButton');
    element.parentNode.removeChild(element);
}, false);

document.getElementById('AccReaderPlusButton').addEventListener('click', function(){
    addFontSizeByRule('#AccReaderControl', 1);
    addFontSizeByRule('#AccReaderControl *', 1);
}, false);

document.getElementById('AccReaderMinButton').addEventListener('click', function(){
    addFontSizeByRule('#AccReaderControl', -1);
    addFontSizeByRule('#AccReaderControl *', -1);
}, false);

function b64_to_utf8( str ) {
    return decodeURIComponent(escape(window.atob( str )));
}


document.getElementById('AccReaderControl').innerHTML = b64_to_utf8('" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content)) + @"');";
            (app.AppDocument.getElementsByTagName("head").item(0, 0) as HTMLHeadElement).appendChild(script as IHTMLDOMNode);
            //(MainDoc.body as HTMLBody).appendChild(script as IHTMLDOMNode);
            Trace.WriteLine(app.AppDocument.documentElement.innerHTML);
        }
    }
}
