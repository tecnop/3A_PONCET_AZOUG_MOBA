using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDInventory : HUDComponent
{
	PlayerInputScript _input;

	Vector2 scrollPos;

	public HUDInventory(Rect frame, HUDContainer parent)
		: base("HUD_inventory", frame, false, parent)
	{
		_input = (PlayerInputScript)GameData.activePlayer.GetInputScript();
	}

	public override void Render()
	{ // Temporary version
		float w = frame.width;
		float h = frame.height;
		//Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "inventory_local");
		bool inRect = _input.MouseIsInRect(this.GetAbsoluteRect());

		GUI.Box(frame, GUIContent.none);

		GUI.BeginGroup(frame);

		List<Item> items = GameData.activePlayer.GetInventoryScript().GetItems();
		uint i = 0;

		scrollPos = GUI.BeginScrollView(SRect.Make(0.0f, 0.0f, w, 0.9f * h), scrollPos, SRect.Make(0.0f, 0.0f, w - 20.0f, 40.0f * items.Count), false, true);

		foreach (Item item in items)
		{
			Rect itemRect = SRect.Make(0.0f, 40.0f * i, w - 20.0f, 40.0f, "inventory_item" + i);
			Vector2 absPos = this.GetAbsolutePos() + new Vector2(itemRect.x, itemRect.y) - scrollPos;
			Rect absItemRect = SRect.Make(absPos.x, absPos.y, itemRect.width, itemRect.height, "inventory_item" + i + "_abs", true);

			if (GUI.Button(itemRect, item.GetName()))
			{
				GameData.activePlayer.GetInventoryScript().DropItem(i);
			}
			if (inRect && _input.MouseIsInRect(absItemRect))
			{
				HUDRenderer.SetDataViewObject(item);
			}
			i++;
		}

		GUI.EndScrollView(true);

		if (GUI.Button(SRect.Make(0.25f * w, 0.9f * h, 0.5f * w, 0.1f * h, "inventory_close"), "Fermer"))
		{ // Exit button
			this.enabled = false;
		}

		GUI.EndGroup();
	}
}