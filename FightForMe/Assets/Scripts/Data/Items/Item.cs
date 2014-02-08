using UnityEngine;
using System.Collections;

public abstract class Item
{
	protected string name;			// Item name
	protected string description;	// Item descrition
	protected string modelPath;		// Path to the item's model
	protected string iconPath;		// Path to the item's icon
	protected uint recyclingXP;		// Experience reward for recycling this item
	protected uint level;			// Quality level of this item
	protected uint buffID;			// ID of the entry in the buff table carrying this item enables

	private bool isWeapon;			// True if the item is a weapon, false if it is an armor

	protected Item(string name,
		string description,
		string modelPath,
		string iconPath,
		uint recyclingXP,
		uint level,
		uint buffID,
		bool isWeapon)
	{
		this.name = name;
		this.description = description;
		this.modelPath = modelPath;
		this.iconPath = iconPath;
		this.recyclingXP = recyclingXP;
		this.level = level;
		this.buffID = buffID;
		this.isWeapon = isWeapon;
	}

	public string GetName()
	{
		return this.name;
	}

	public string GetDesc()
	{
		return this.description;
	}

	public string GetModel()
	{
		return this.modelPath;
	}

	public string GetIcon()
	{
		return this.iconPath;
	}

	public uint GetRecyclingXP()
	{
		return this.recyclingXP;
	}

	public uint GetLevel()
	{
		return this.level;
	}

	public Buff GetBuff()
	{
		return DataTables.GetBuff(this.buffID);
	}

	public bool IsWeapon()
	{
		return this.isWeapon;
	}
}
