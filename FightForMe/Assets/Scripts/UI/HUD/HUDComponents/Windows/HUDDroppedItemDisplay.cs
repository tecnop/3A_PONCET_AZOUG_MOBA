﻿using UnityEngine;
using System.Collections;

public class HUDDroppedItemDisplay : HUDComponent
{
	//private CharacterStatsScript _stats;
	private CharacterInventoryScript _inventory;
	private PlayerMiscDataScript _misc;

	public HUDDroppedItemDisplay(Rect frame, HUDContainer parent)
		: base("HUD_dropped_item_display", frame, parent: parent)
	{
		//this._stats = GameData.activePlayer.GetStatsScript();
		this._inventory = GameData.activePlayer.GetInventoryScript();
		this._misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "dropped_item_local");

		GUI.BeginGroup(frame);

		Rect itemView = SRect.Make(0.0f, 0.0f, w, 0.8f * h, "dropped_item_view");

		GUI.BeginGroup(itemView);

		uint itemID = HUDRenderer.GetSelectedItem().GetItemID();
		Item item = DataTables.GetItem(itemID);
		if (item != null)
		{
			item.DrawDataWindow(itemView.width, itemView.height);
		}
		else
		{
			localRect = SRect.Make(0.0f, 0.0f, w, 0.8f * h, "dropped_item_view_local");
			GUI.Label(localRect, "<Unknown item>", FFMStyles.centeredText);
		}

		GUI.EndGroup();

		Rect buttons = SRect.Make(0.0f, 0.8f * h, w, 0.2f * h, "dropped_item_buttons");
		localRect = SRect.Make(0.0f, 0.0f, w, 0.2f * h, "dropped_item_buttons_local");

		GUI.BeginGroup(buttons);

		if (GUI.Button(SRect.Make(0.0f, 0.0f, 0.3f * w, 0.2f * h, "dropped_item_buttons_equip"), "Equiper"))
		{
			HUDRenderer.GetSelectedItem().OnPickUp(_inventory);
			HUDRenderer.SetSelectedItem(null);
		}
		if (item != null && item.IsWeapon())
		{
			Weapon weapon2 = _inventory.GetWeapon();
			if (weapon2 != null)
			{ // Don't allow equipping something in left hand if we have nothing in right hand
				WeaponType type1 = weapon2.GetWeaponType();
				WeaponType type2 = ((Weapon)item).GetWeaponType();
				if (type1 != null && !type1.IsTwoHanded() &&
					type2 != null && !type2.IsTwoHanded())
				{ // Our current weapon is not two handed and neither is the one on the floor
					/*if (GUI.Button(SRect.Make(0.35f * w, 0.0f, 0.3f * w, 0.2f * h, "dropped_item_buttons_equip2"), "Equiper en main gauche"))
					{ // Removed until it's properly implemented
						//HUDRenderer.GetSelectedItem().OnPickUp(_inventory);
						//HUDRenderer.SetSelectedItem(null);
					}*/
				}
			}
		}
		if (GUI.Button(SRect.Make(0.7f * w, 0.0f, 0.3f * w, 0.2f * h, "dropped_item_buttons_recycle"), "Recycler"))
		{
			HUDRenderer.GetSelectedItem().OnRecycle(_misc);
			HUDRenderer.SetSelectedItem(null);
		}

		GUI.EndGroup();

		GUI.EndGroup();
	}
}
