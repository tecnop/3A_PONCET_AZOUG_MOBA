﻿using UnityEngine;
using System.Collections;

public class HUDMenuButtons : HUDComponent
{
	public HUDMenuButtons(Rect frame)
		: base("HUD_menu_buttons", frame)
	{

	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;

		GUI.BeginGroup(frame);

		Rect inventoryRect = new Rect(0.0f, 0.0f, w, 0.5f * h);
		Rect skillsRect = new Rect(0.0f, 0.5f * h, w, 0.5f * h);

		if (GUI.Button(inventoryRect, "Inventaire"))
		{
			HUDRenderer.SetMenu(HUDMenu.Inventory);
		}

		if (GUI.Button(skillsRect, "Compétences"))
		{
			HUDRenderer.SetMenu(HUDMenu.Skill);
		}


		ArrayList sets = GameData.activePlayer.GetInventoryScript().GetCompletedSets();
		if (sets.Count > 0)
		{
			GUIStyle bottom = FFMStyles.Text(TextAnchor.LowerCenter, bottomPadding: 2);
			GUI.Label(inventoryRect, "Panoplie complète:" + ((ArmorSet)sets[0]).GetName(), bottom);
		}

		uint skillPoints = ((PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript()).GetSkillPoints();
		if (skillPoints > 0)
		{
			GUIStyle bottom = FFMStyles.Text(TextAnchor.LowerCenter, bottomPadding: 2);
			GUI.Label(skillsRect, skillPoints + " point(s) à attribuer", bottom);
		}

		GUI.EndGroup();
	}
}