using UnityEngine;
using System.Collections;

public class HUDDroppedItemDisplay : HUDComponent
{
	private CharacterStatsScript _stats;
	private CharacterInventoryScript _inventory;
	private PlayerMiscDataScript _misc;

	public HUDDroppedItemDisplay(Rect frame, HUDContainer parent)
		: base("HUD_dropped_item_display", frame, parent: parent)
	{
		this._stats = GameData.activePlayer.GetStatsScript();
		this._inventory = GameData.activePlayer.GetInventoryScript();
		this._misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(frame);

		Rect itemView = new Rect(0.0f, 0.0f, w, 0.8f * h);
		localRect = new Rect(0.0f, 0.0f, w, 0.8f * h);

		GUI.BeginGroup(itemView);

		uint itemID = HUDRenderer.GetSelectedItem().GetItemID();
		Item item = DataTables.GetItem(itemID);
		if (item != null)
		{ // TODO: DataView
			GUI.Label(localRect, item.GetName(), FFMStyles.centeredText);
		}
		else
		{
			GUI.Label(localRect, "<Unknown item>", FFMStyles.centeredText);
		}

		GUI.EndGroup();

		Rect buttons = new Rect(0.0f, 0.8f * h, w, 0.2f * h);
		localRect = new Rect(0.0f, 0.0f, w, 0.2f * h);

		GUI.BeginGroup(buttons);

		if (GUI.Button(new Rect(0.0f, 0.0f, 0.3f * w, 0.2f * h), "Equiper"))
		{
			HUDRenderer.GetSelectedItem().OnPickUp(_inventory);
			HUDRenderer.SetSelectedItem(null);
		}
		if (item.IsWeapon())
		{
			Weapon weapon2 = _inventory.GetWeapon();
			if (weapon2 != null)
			{
				WeaponType type1 = weapon2.GetWeaponType();
				WeaponType type2 = ((Weapon)item).GetWeaponType();
				if (type1 != null && !type1.IsTwoHanded() &&
					type2 != null && !type2.IsTwoHanded())
				{ // Our current weapon is not two handed and neither is the one on the floor
					if (GUI.Button(new Rect(0.35f * w, 0.0f, 0.3f * w, 0.2f * h), "Equiper en main gauche"))
					{
						//HUDRenderer.GetSelectedItem().OnPickUp(_inventory);
						//HUDRenderer.SetSelectedItem(null);
					}
				}
			}
		}
		if (GUI.Button(new Rect(0.7f * w, 0.0f, 0.3f * w, 0.2f * h), "Recycler"))
		{
			HUDRenderer.GetSelectedItem().OnRecycle(_misc);
			HUDRenderer.SetSelectedItem(null);
		}

		GUI.EndGroup();

		GUI.EndGroup();
	}
}
