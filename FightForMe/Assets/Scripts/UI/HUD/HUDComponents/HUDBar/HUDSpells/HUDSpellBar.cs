using UnityEngine;
using System.Collections;

public class HUDSpellBar : HUDContainer
{
	public HUDSpellBar(Rect frame, HUDContainer parent)
		: base("HUD_spell_bar", frame, parent:parent)
	{
		float w = frame.width;
		float h = frame.height;

		float size = 64.0f;		// Size of an icon
		float minOffset = 4.0f;	// Minimum space between two icons

		// I don't think this is the right place to do this actually
		//int iconPerLine = Mathf.FloorToInt(w / (size+minOffset));

		float x = 0;
		float y = (h - size) / 2;

		for (SpellSlot i = SpellSlot.SLOT_0; i < SpellSlot.NUM_SLOTS; i++)
		{
			new HUDSpellSlot(new Rect(x, y, size, size), this, i);
			x += size+minOffset;
		}
	}
}
