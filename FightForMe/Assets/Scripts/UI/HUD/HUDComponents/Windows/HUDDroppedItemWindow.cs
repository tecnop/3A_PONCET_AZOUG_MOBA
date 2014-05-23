using UnityEngine;
using System.Collections;

public class HUDDroppedItemWindow : HUDContainer
{
	public HUDDroppedItemWindow(Rect frame, HUDContainer parent)
		: base("HUD_dropped_item_window", frame, null, false, true, parent)
	{
		new HUDWindowGrabber(SRect.Make(0, 0, frame.width, 30, "window_grabber_item"), this);
		new HUDDroppedItemDisplay(SRect.Make(0, 30, frame.width, frame.height - 30, "dropped_item"), this);
	}
}
