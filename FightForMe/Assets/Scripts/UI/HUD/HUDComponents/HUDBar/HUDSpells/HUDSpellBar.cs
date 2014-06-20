using UnityEngine;
using System.Collections;

public class HUDSpellBar : HUDContainer
{
	public HUDSpellBar(Rect frame, HUDContainer parent)
		: base("HUD_spell_bar", frame, parent: parent)
	{
		float w = frame.width;
		float h = frame.height;

		//float minOffset = 4.0f;	// Minimum space between two icons
		float maxSize = w / (int)SpellSlot.NUM_SLOTS; // Maximum size of an icon

		float size;
		float offset;

		if (maxSize > 64.0f)
		{ // Eh, don't need this much
			offset = maxSize - 64.0f;
			size = 64.0f;
		}
		else if (maxSize < 32.0f)
		{ // TODO: See if we can make a second line instead of crushing those icons until they're a line of pixels.
			offset = 0.0f;
			size = maxSize;
		}
		else
		{
			offset = 0.0f;
			size = maxSize;
		}

		float x = 0;
		float y = (h - size) / 2;

		for (SpellSlot i = SpellSlot.SLOT_0; i < SpellSlot.NUM_SLOTS; i++)
		{
			new HUDSpellSlot(SRect.Make(x, y, size, size), this, i);
			x += size + offset;
		}
	}
}
