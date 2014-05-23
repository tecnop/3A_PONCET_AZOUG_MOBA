using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ArmorSlot { HEAD, BODY, HANDS, FEET };

public class Armor : Item
{
	private ArmorSlot slot;		// Inventory slot this armor fits in
	private uint setID;			// Armor set this item is a part of (0 means none)

	private Stats stats;		// Stats granted by this piece of armor

	public override WikiCategory category
	{
		get
		{
			return WikiCategory.ITEMS;
		}
	}

	public static string[] slotNames
	{
		get
		{
			return new string[] { "Tête", "Corps", "Mains", "Pieds" };
		}
	}

	public Armor(Metadata metadata,
		uint recyclingXP = 100,
		uint buffID = 0,
		ArmorSlot slot = ArmorSlot.BODY,
		uint setID = 0,
		Stats stats = null)
		: base(metadata, recyclingXP, buffID, false)
	{
		this.slot = slot;
		this.setID = setID;

		if (setID != 0)
		{ // Notify the item set that a new item is now a part of it
			DataTables.GetArmorSet(setID).IncreaseSetSize();
		}

		if (stats != null)
			this.stats = stats;
		else
			this.stats = new Stats();
	}

	public ArmorSlot GetSlot() { return this.slot; }
	public uint GetSetID() { return this.setID; }
	public Stats GetStats() { return this.stats; }

	public override void DrawDataWindow(float width, float height)
	{
		base.DrawDataWindow(width, height);

		GUI.Label(SRect.Make(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 5.0f, "data_window_armor_slot"), "Emplacement: " + Armor.slotNames[(int)this.slot], FFMStyles.centeredText_wrapped);

		if (this.setID != 0)
		{
			GUI.BeginGroup(SRect.Make(2.0f * width / 3.0f, height / 5.0f, width / 3.0f, 2.0f * height / 5.0f, "data_window_armor_set"));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Panoplie:");
			WikiManager.DrawReferenceInLayout(DataTables.GetArmorSet(this.setID));
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}

		// TODO: Bake this
		List<string> list = new List<string>();
		if (stats.GetStrength() != 0) list.Add(stats.GetStrength() + " à l'endurance");
		if (stats.GetAgility() != 0) list.Add(stats.GetAgility() + " à la puissance");
		if (stats.GetIntelligence() != 0) list.Add(stats.GetIntelligence() + " à l'intelligence");

		GUI.Label(SRect.Make(10.0f, 0.45f * height, width - 20.0f, 0.55f * height - 40.0f, "data_window_armor_sets"), string.Join(", ", list.ToArray()));
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);
	}
}
