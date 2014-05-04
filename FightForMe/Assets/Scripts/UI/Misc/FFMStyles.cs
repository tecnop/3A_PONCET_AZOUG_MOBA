using UnityEngine;
using System.Collections;

public static class FFMStyles
{
	private static bool _loaded = false;

	private static GUIStyle _defaultText;

	public static GUIStyle defaultText
	{
		get
		{
			return _defaultText;
		}
	}

	private static GUIStyle _centeredText;

	public static GUIStyle centeredText
	{
		get
		{
			return _centeredText;
		}
	}

	private static GUIStyle _centeredText_wrapped;

	public static GUIStyle centeredText_wrapped
	{
		get
		{
			return _centeredText_wrapped;
		}
	}

	public static GUIStyle Text(TextAnchor alignment = TextAnchor.MiddleCenter, int leftPadding = 0, int rightPadding = 0, int topPadding = 0, int bottomPadding = 0, bool wordWrap = false)
	{
		GUIStyle style = new GUIStyle(defaultText);
		style.alignment = alignment;
		style.padding = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
		style.wordWrap = wordWrap;
		return style;
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
	}

}
