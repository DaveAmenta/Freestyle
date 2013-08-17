((function()
{
	document.getElementById('mailReadingPaneBodyFrame').addEventListener('load', function(){
		var iframe = document.getElementById('mailReadingPaneBodyFrame');
		var doc = iframe.contentWindow.document;
		var style_node = doc.createElement("style");
		style_node.setAttribute("type", "text/css");
		style_node.setAttribute("media", "screen"); 
		style_node.appendChild(doc.createTextNode("* { font-size: 30px !important; line-height: 1.2 !important; word-wrap:break-word !important; }"));
		doc.getElementsByTagName("head")[0].appendChild(style_node);
	}, false);
})())