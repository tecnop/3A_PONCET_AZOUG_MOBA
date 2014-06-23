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
		float maxSize = Mathf.Min(h - 20.0f, w / (int)SpellSlot.NUM_SLOTS); // Maximum size of an icon

		float size;
		float offset;

		if (maxSize > 64.0f)
		{ // Eh, don't need this much
			offset = maxSize - 64.0f;
			size = 64.0f;
		}
		else if (maxSize < 32.0f)
		{ // TODO: See if we can make a second line instead of crushing those icons until they're a line of pixels.
			offset = (w / (int)SpellSlot.NUM_SLOTS) - maxSize;
			size = maxSize;
		}
		else
		{ // FIXME: Offset isn't perfect
			offset = (w / (int)SpellSlot.NUM_SLOTS) - maxSize;
			size = maxSize;
		}

		float x = 0;
		for (SpellSlot i = SpellSlot.SLOT_0; i < SpellSlot.NUM_SLOTS; i++)
		{
			new HUDSpellSlot(SRect.Make(x, 3.0f, size, h), this, i, SRect.Make(0.0f, size, size, h-size));
			x += size + offset;
		}
	}
}
