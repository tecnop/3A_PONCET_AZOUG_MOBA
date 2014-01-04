using UnityEngine;
using System.Collections;

public enum ArmorSlot { HEAD, TORSO, HANDS, LEGS, FEET };

public class Armor : Item
{
	private ArmorSlot slot;		// Inventory slot this armor fits in
	private uint setID;			// Armor set this item is a part of (0 means none)

	Stats stats;				// Stats granted by this piece of armor

	// Test constructor
	public Armor(string name) : base()
	{
		this.name = name;
		this.slot = ArmorSlot.HEAD;
		this.setID = 0;
		this.stats = new Stats(5, 5, 5);
	}

	public ArmorSlot GetSlot() { return this.slot; }
	public uint GetSetID() { return this.setID; }
}
