﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDInventory : HUDComponent
{
	PlayerInputScript _input;

	public HUDInventory(Rect frame, HUDContainer parent)
		: base("HUD_inventory", frame, false, parent)
	{
		_input = (PlayerInputScript)GameData.activePlayer.GetInputScript();
	}

	public override void Render()
	{ // Temporary version
		float w = frame.width;
		float h = frame.height;

		GUI.Box(frame, GUIContent.none);

		GUI.BeginGroup(frame);

		List<Item> items = GameData.activePlayer.GetInventoryScript().GetItems();
		uint i = 0;
		foreach (Item item in items)
		{
			Rect itemRect = SRect.Make(0.0f, 40.0f * i, w, 40.0f, "inventory_item" + i);
			Vector2 absPos = this.GetAbsolutePos() + new Vector2(itemRect.x, itemRect.y);
			Rect absItemRect = SRect.Make(absPos.x, absPos.y, itemRect.width, itemRect.height, "inventory_item" + i + "_abs");

			if (GUI.Button(itemRect, item.GetName()))
			{
				GameData.activePlayer.GetInventoryScript().DropItem(i);
			}
			if (_input.MouseIsInRect(absItemRect))
			{
				HUDRenderer.SetDataViewObject(item);
			}
			i++;
		}

		if (GUI.Button(SRect.Make(0.25f * w, 0.9f * h, 0.5f * w, 0.1f * h, "inventory_close"), "Fermer"))
		{ // Exit button
			this.enabled = false;
		}

		GUI.EndGroup();
	}
}