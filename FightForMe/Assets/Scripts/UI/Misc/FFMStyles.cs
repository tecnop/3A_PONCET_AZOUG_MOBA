using UnityEngine;
using System.Collections;

public static class FFMStyles
{
	private static bool _loaded = false;

	private static GUIStyle _defaultText;
	public static GUIStyle defaultText { get { return _defaultText; } }

	private static GUIStyle _centeredText;
	public static GUIStyle centeredText { get { return _centeredText; } }

	private static GUIStyle _centeredText_wrapped;
	public static GUIStyle centeredText_wrapped { get { return _centeredText_wrapped; } }

	private static GUIStyle _title;
	public static GUIStyle title { get { return _title; } }

	private static GUIStyle _bigTitle;
	public static GUIStyle bigTitle { get { return _bigTitle; } }

	private static GUIStyle _textBlock;
	public static GUIStyle textBlock { get { return _textBlock; } }

	private static GUIStyle _loreBlock;
	public static GUIStyle loreBlock { get { return _loreBlock; } }

	private static GUIStyle _positive;
	public static GUIStyle positive { get { return _positive; } }

	private static GUIStyle _negative;
	public static GUIStyle negative { get { return _negative; } }

	public static GUIStyle Text(TextAnchor alignment = TextAnchor.MiddleCenter, int leftPadding = 0, int rightPadding = 0, int topPadding = 0, int bottomPadding = 0, bool wordWrap = false)
	{
		GUIStyle style = new GUIStyle(defaultText);
		style.alignment = alignment;
		style.padding = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
		style.wordWrap = wordWrap;
		return style;
	}

	private static GUIStyle junkTitle, commonTitle, rareTitle, epicTitle, uniqueTitle;
	private static GUIStyle junkTitle_large, commonTitle_large, rareTitle_large, epicTitle_large, uniqueTitle_large;
	public static GUIStyle StyleForQuality(Quality quality, bool large)
	{ // Title styles
		if (quality == Quality.JUNK)
		{
			return large ? junkTitle_large : junkTitle;
		}
		else if (quality == Quality.COMMON)
		{
			return large ? commonTitle_large : commonTitle;
		}
		else if (quality == Quality.RARE)
		{
			return large ? rareTitle_large : rareTitle;
		}
		else if (quality == Quality.EPIC)
		{
			return large ? epicTitle_large : epicTitle;
		}
		else if (quality == Quality.UNIQUE)
		{
			return large ? uniqueTitle_large : uniqueTitle;
		}
		return large ? _title : _bigTitle;
	}

	public static void Load()
	{
		if (_loaded)
		{
			return;
		}

		_loaded = true;

		_defaultText = new GUIStyle();
		_defaultText.wordWrap = false;
		_defaultText.normal.textColor = Color.white;

		_centeredText = new GUIStyle(_defaultText);
		_centeredText.alignment = TextAnchor.MiddleCenter;

		_centeredText_wrapped = new GUIStyle(_centeredText);
		_centeredText_wrapped.wordWrap = true;

		_textBlock = new GUIStyle(_centeredText_wrapped);
		//_textBlock.normal.background = GUI.skin.box.normal.background;

		_loreBlock = new GUIStyle(_textBlock);
		_loreBlock.fontStyle = FontStyle.Italic;

		_title = new GUIStyle(_centeredText_wrapped);
		_title.fontStyle = FontStyle.Bold;

		_bigTitle = new GUIStyle(_title);
		_bigTitle.fontSize = 42;

		_positive = new GUIStyle();
		_positive.wordWrap = true;
		_positive.padding = new RectOffset(2, 0, 0, 0);
		_positive.normal.textColor = Color.green;

		_negative = new GUIStyle(_positive);
		_negative.normal.textColor = Color.red;

		// Quality styles
		junkTitle = new GUIStyle(_title);
		junkTitle.normal.textColor = Color.gray;
		
		commonTitle = new GUIStyle(_title);
		
		rareTitle = new GUIStyle(_title);
		rareTitle.normal.textColor = Color.magenta;

		epicTitle = new GUIStyle(_title);
		epicTitle.normal.textColor = new Color(1.0f, 0.5f, 0.0f);

		uniqueTitle = new GUIStyle(_title); // TODO: Special font
		uniqueTitle.normal.textColor = Color.black;

		junkTitle_large = new GUIStyle(junkTitle);
		junkTitle_large.fontSize = 42;

		commonTitle_large = new GUIStyle(commonTitle);
		commonTitle_large.fontSize = 42;

		rareTitle_large = new GUIStyle(rareTitle);
		rareTitle_large.fontSize = 42;

		epicTitle_large = new GUIStyle(epicTitle);
		epicTitle_large.fontSize = 42;

		uniqueTitle_large = new GUIStyle(uniqueTitle);
		uniqueTitle_large.fontSize = 42;
	}

}
