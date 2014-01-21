using UnityEngine;
using System.Collections;

public enum ArmorSlot { HEAD, TORSO, HANDS, LEGS, FEET };

public class Armor : Item
{
	private ArmorSlot slot;		// Inventory slot this armor fits in
	private uint setID;			// Armor set this item is a part of (0 means none)

	Stats stats;				// Stats granted by this piece of armor

	public Armor(string name = "Armure",
		string description = null,
		string modelPath = null,
		string iconPath = null,
		uint recyclingXP = 0,
		uint level = 0,
		uint skillID = 0,
		ArmorSlot slot = ArmorSlot.TORSO,
		uint setID = 0,
		Stats stats = null)
		: base(name, description, modelPath, iconPath, recyclingXP, level, skillID)
	{
		this.slot = slot;
		this.setID = setID;

		if (setID != 0)
		{ // Notify the item set that a new item is now a part of it
			DataTables.getArmorSet(setID).IncreaseSetSize();
		}

		if (stats != null)
			this.stats = stats;
		else
			this.stats = new Stats();

		this.iAmAWeapon = false;
	}

	public ArmorSlot GetSlot() { return this.slot; }
	public uint GetSetID() { return this.setID; }
}
