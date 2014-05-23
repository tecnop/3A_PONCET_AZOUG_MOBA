using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SRect
{ // Merge this with FFMStyle maybe? It could use the same system
	public static Dictionary<string, Rect> stored = new Dictionary<string, Rect>();

	public static Rect Make(float x, float y, float width, float height, string name = null, bool update = false)
	{
		if (false && !update && name != null && stored.ContainsKey(name))
		{
			return stored[name];
		}
		else
		{
			Rect rect = new Rect(x, y, width, height);
			if (name != null)
			{
				stored[name] = rect;
			}
			return rect;
		}
	}

	public static Rect Get(string name)
	{
		if (stored.ContainsKey(name))
		{
			return stored[name];
		}
		return screen;
	}

	public static Rect screen
	{
		get
		{
			return Make(0.0f, 0.0f, Screen.width, Screen.height, "screen");
		}
	}
}
