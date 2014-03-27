using UnityEngine;
using System.Collections;

public class HUDInventory : HUDComponent
{
	public HUDInventory(Rect frame, HUDContainer parent)
		: base("HUD_inventory", frame, false, parent)
	{

	}

	public override void Render()
	{ // Temporary version
		float w = frame.width;
		float h = frame.height;

		GUI.Box(frame, GUIContent.none);

		GUI.BeginGroup(frame);

		ArrayList objects = GameData.activePlayer.GetInventoryScript().GetItems();
		uint i = 0;
		foreach (Item item in objects)
		{
			if (GUI.Button(new Rect(0.0f, 40.0f * i, w, 40.0f), item.GetName()))
			{
				GameData.activePlayer.GetInventoryScript().DropItem(i);
			}
			i++;
		}
		
		if (GUI.Button(new Rect(0.25f * w, 0.9f * h, 0.5f * w, 0.1f * h), "Fermer"))
		{ // Exit button
			this.enabled = false;
		}

		GUI.EndGroup();
	}
}