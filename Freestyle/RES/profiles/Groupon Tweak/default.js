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

document.getElementById('AccSupport_Plus').addEventListener('click', function(){
	addFontSizeByRule('p', 1);
}, false);
document.getElementById('AccSupport_Minus').addEventListener('click', function(){
	addFontSizeByRule('p', -1);
}, false);

document.getElementById('AccSupport_Color').addEventListener('click', function(){
	getCssRules('.splitpage', function(rule)
	{
		rule.style.backgroundImage = '';
		rule.style.background = 'black';
	});
}, false);