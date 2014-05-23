using UnityEngine;
using System.Collections;

public class HUDSpellWindow : HUDContainer
{
	public HUDSpellWindow(Rect frame, HUDContainer parent)
		: base("HUD_spell_window", frame, null, false, true, parent)
	{
		new HUDWindowGrabber(SRect.Make(0, 0, frame.width, 30, "window_grabber_spells"), this);
		new HUDSpellList(SRect.Make(0, 30, frame.width, frame.height - 30, "spell_list"), this);
	}
}
