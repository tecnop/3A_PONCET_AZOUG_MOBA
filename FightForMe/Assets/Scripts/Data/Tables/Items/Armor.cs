using UnityEngine;
using System.Collections;

public enum ArmorSlot { HEAD, BODY, HANDS, FEET };

public class Armor : Item
{
	private ArmorSlot slot;		// Inventory slot this armor fits in
	private uint setID;			// Armor set this item is a part of (0 means none)

	private Stats stats;		// Stats granted by this piece of armor

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

		GUI.Label(new Rect(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 3.0f), "Emplacement: " + Armor.slotNames[(int)this.slot], FFMStyles.centeredText_wrapped);
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);
	}
}
