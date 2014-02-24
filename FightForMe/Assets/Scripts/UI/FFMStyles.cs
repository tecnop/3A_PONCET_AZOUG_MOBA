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

	public static GUIStyle Text(TextAnchor alignment = TextAnchor.MiddleCenter, int leftPadding = 0, int rightPadding = 0, int topPadding = 0, int bottomPadding = 0)
	{
		GUIStyle style = new GUIStyle(defaultText);
		style.alignment = alignment;
		style.padding = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
		return style;
	}

	public static GUIStyle centeredText
	{
		get
		{
			return Text();
		}
	}
}
