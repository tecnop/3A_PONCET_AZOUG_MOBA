﻿using UnityEngine;
using System.Collections;

public class HUDWindowGrabber : HUDComponent
{
	public HUDWindowGrabber(Rect frame, HUDContainer window)
		: base("HUD_window_grabber", frame, parent:window)
	{

	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "window_grabber");

		GUI.BeginGroup(frame);

		if (GUI.RepeatButton(localRect, /*this.GetParent().GetName()*/GUIContent.none))
		{
			Vector3 pos = Input.mousePosition;
			this.GetParent().SetPos(pos.x - w / 2, Screen.height - pos.y - h / 2);
		}

		GUI.EndGroup();
	}
}