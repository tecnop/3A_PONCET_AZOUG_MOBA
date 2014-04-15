using UnityEngine;
using System.Collections;

public class HUDDroppedItemWindow : HUDContainer
{
	public HUDDroppedItemWindow(Rect frame, HUDContainer parent)
		: base("HUD_dropped_item_window", frame, null, false, true, parent)
	{
		new HUDWindowGrabber(new Rect(0, 0, frame.width, 30), this);
		new HUDDroppedItemDisplay(new Rect(0, 30, frame.width, frame.height - 30), this);
	}
}
