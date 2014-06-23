using UnityEngine;
using System.Collections;

public class HUDSpellSlot : HUDComponent
{
	private PlayerMiscDataScript _misc;
	private PlayerInputScript _input;

	private SpellSlot slot;
	private Rect labelFrame;

	public HUDSpellSlot(Rect frame, HUDContainer parent, SpellSlot slot, Rect labelFrame)
		: base("HUD_spell_slot" + (int)slot, frame, parent: parent)
	{
		this._misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
		this._input = (PlayerInputScript)GameData.activePlayer.GetInputScript();
		this.slot = slot;
		this.labelFrame = labelFrame;
	}

	public override void Render()
	{
		float w = frame.width;
		//float h = frame.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, w, "spell_slot" + (int)slot + "_local");

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

			if (_input.MouseIsInRect(this.GetAbsoluteRect()))
			{
				HUDRenderer.SetDataViewObject(spell);
			}
		}

		if (GUI.Button(localRect, GUIContent.none))
		{ // Display the thing
			HUDRenderer.SetSlot(this.slot);
			HUDRenderer.OpenMenu(HUDMenu.SpellSlot);
		}

		if (name.Length > 0)
		{
			GUI.Label(localRect, name, FFMStyles.centeredText_wrapped);
		}

		string keyName;

		// Display the default key for each slot... can't do better atm
		if (slot == SpellSlot.SLOT_0)
			keyName = "Clic G";
		else if (slot == SpellSlot.SLOT_1)
			keyName = "Clic D";
		else if (slot == SpellSlot.SLOT_2)
			keyName = "Clic M";
		else if (slot == SpellSlot.SLOT_3)
			keyName = "1";
		else if (slot == SpellSlot.SLOT_4)
			keyName = "2";
		else if (slot == SpellSlot.SLOT_5)
			keyName = "3";
		else if (slot == SpellSlot.SLOT_6)
			keyName = "4";
		else if (slot == SpellSlot.SLOT_7)
			keyName = "R";
		else
			keyName = "???";

		GUI.Label(labelFrame, keyName, FFMStyles.centeredText);

		GUI.EndGroup();
	}
}
