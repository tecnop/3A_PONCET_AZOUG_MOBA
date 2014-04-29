using UnityEngine;
using System.Collections;

public static class FFMStyles
{
	public static GUIStyle defaultText
	{
		get
		{
			GUIStyle style = new GUIStyle();
			style.wordWrap = false;
			style.normal.textColor = Color.white;
			return style;
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

	public static GUIStyle centeredText
	{ // TODO: Create this once and for all instead of re-executing the function every time
		get
		{
			return Text();
		}
	}

	public static GUIStyle centeredText_wrapped
	{
		get
		{
			return Text(wordWrap: true);
		}
	}
}
