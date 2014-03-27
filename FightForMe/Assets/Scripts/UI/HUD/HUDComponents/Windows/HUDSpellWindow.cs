using UnityEngine;
using System.Collections;

public class HUDSpellWindow : HUDContainer
{
	public HUDSpellWindow(Rect frame, HUDContainer parent)
		: base("HUD_spell_window", frame, null, false, true, parent)
	{
		new HUDWindowGrabber(new Rect(0, 0, frame.width, 30), this);
		new HUDSpellList(new Rect(0, 30, frame.width, frame.height - 30), this);
	}
}
