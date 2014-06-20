using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSpellList : HUDComponent
{
	private CharacterStatsScript _stats;
	private PlayerMiscDataScript _misc;

	public HUDSpellList(Rect frame, HUDContainer parent)
		: base("HUD_spell_list", frame, parent: parent)
	{
		this._stats = GameData.activePlayer.GetStatsScript();
		this._misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "spell_list_local");

		GUI.BeginGroup(frame);

		List<uint> spells = _stats.GetKnownSpells();

		int rows = 1;
		float maxSize = Mathf.Min(w / spells.Count, h); // Maximum size of an icon

		float size;

		if (maxSize > 64.0f)
		{ // Eh, don't need this much
			size = 64.0f;
		}
		else if (maxSize < 32.0f && h >= 32.0f)
		{ // TODO: See if we can make a second line instead of crushing those icons until they're a line of pixels.
			do
			{
				rows++;
				maxSize = rows * w / spells.Count; // Should be changed a bit but the only way it would matter is if the window is too small for another row soooo
			} while (maxSize < 32.0f);
			size = maxSize;

			if (size * rows > h)
			{
				Debug.LogWarning("WARNING: Too many spells to fit in the window!");
			}
		}
		else
		{
			size = maxSize;
		}

		float x = 0;
		float y = 0;

		GUIStyle style = FFMStyles.centeredText;
		style.wordWrap = true;

		// First block
		Rect firstRect = SRect.Make(x, y, size, size);
		if (GUI.Button(firstRect, GUIContent.none))
		{ // Clear it
			_misc.AssignSpellToSlot(0, HUDRenderer.GetSlot());
			HUDRenderer.SetSlot(SpellSlot.NUM_SLOTS);
			HUDRenderer.OpenMenu(HUDMenu.None);
		}

		x += size;
		if (x + size > localRect.width)
		{ // Unlikely
			x = 0;
			y += size;
		}

		foreach (uint spellID in _stats.GetKnownSpells())
		{
			Rect rect = SRect.Make(x, y, size, size);
			Spell spell = DataTables.GetSpell(spellID);

			if (GUI.Button(rect, GUIContent.none))
			{ // Equip it!
				_misc.AssignSpellToSlot(spellID, HUDRenderer.GetSlot());
				HUDRenderer.SetSlot(SpellSlot.NUM_SLOTS);
				HUDRenderer.OpenMenu(HUDMenu.None);
			}

			GUI.Label(rect, spell.GetName(), style);

			x += size;

			if (x + size > localRect.width)
			{
				x = 0;
				y += size;
			}
		}

		GUI.EndGroup();
	}
}
