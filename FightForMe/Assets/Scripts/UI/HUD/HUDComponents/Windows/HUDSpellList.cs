using UnityEngine;
using System.Collections;

public class HUDSpellList : HUDComponent
{
	private CharacterStatsScript _stats;
	private PlayerMiscDataScript _misc;

	public HUDSpellList(Rect frame, HUDContainer parent)
		: base("HUD_spell_list", frame, parent:parent)
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

		float x = 0;
		float y = 0;
		float size = 64;

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
		{
			x = 0;
			y += size;
			if (y + size > localRect.width)
			{ // That shouldn't happen here...
				Debug.LogWarning("WARNING: Too many spells to fit in the window!");
			}
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
				if (y + size > localRect.width)
				{
					Debug.LogWarning("WARNING: Too many spells to fit in the window!");
				}
			}
		}

		GUI.EndGroup();
	}
}
