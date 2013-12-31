using UnityEngine;
using System.Collections;

public enum ArmorSlot { HEAD, TORSO, HANDS, LEGS, FEET };

public class Armor : Item
{
	ArmorSlot slot;				// Inventory slot this armor fits in
	uint setID;					// Armor set this item is a part of (0 means none)

	// TODO: Stats

	// Test constructor
	public Armor(string name) : base()
	{
		this.name = name;
		this.slot = ArmorSlot.HEAD;
		this.setID = 0;
	}

	public ArmorSlot GetSlot() { return this.slot; }
	public uint GetSetID() { return this.setID; }
}
