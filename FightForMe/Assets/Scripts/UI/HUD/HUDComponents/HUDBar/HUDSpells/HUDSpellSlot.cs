﻿using UnityEngine;
using System.Collections;

public class HUDSpellSlot : HUDComponent
{
	private PlayerMiscDataScript _misc;

	private SpellSlot slot;

	public HUDSpellSlot(Rect frame, HUDContainer parent, SpellSlot slot)
		: base("HUD_spell_slot"+(int)slot, frame, parent:parent)
	{
		this._misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
		this.slot = slot;
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(frame);

		uint boundSpell = _misc.GetSpellForSlot(slot);
		string name;

		if (HUDRenderer.GetSlot() == this.slot)
		{
			name = "^";
		}
		else if (boundSpell == 0)
		{ // No spell attached
			if (GameData.activePlayer.GetStatsScript().GetKnownSpells().Count > (int)slot)
			{ // There's an available spell to equip!
				name = "Cliquez pour équiper un sort";
			}
			else
			{ // Nothing, just display a black square
				name = "";
			}
		}
		else
		{ // We have a spell attached
			Spell spell = DataTables.GetSpell(boundSpell);

			name = spell.GetName();
		}

		if (GUI.Button(localRect, GUIContent.none))
		{ // Display the thing
			HUDRenderer.SetSlot(this.slot);
			HUDRenderer.OpenMenu(HUDMenu.SpellSlot);
		}

		GUIStyle style = FFMStyles.centeredText;
		style.wordWrap = true;

		if (name != "")
		{
			GUI.Label(localRect, name, style);
		}

		GUI.EndGroup();
	}
}