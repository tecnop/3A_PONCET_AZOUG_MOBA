﻿using UnityEngine;
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
	protected bool iAmAWeapon;		// True if the item is a weapon, false otherwise

	protected Item(string name,
		string description,
		string modelPath,
		string iconPath,
		uint recyclingXP,
		uint level,
		uint buffID)
	{
		this.name = name;
		this.description = description;
		this.modelPath = modelPath;
		this.iconPath = iconPath;
		this.recyclingXP = recyclingXP;
		this.level = level;
		this.buffID = buffID;
	}

	public string getName()
	{
		return this.name;
	}

	public string getDesc()
	{
		return this.description;
	}

	public string getModel()
	{
		return this.modelPath;
	}

	public string getIcon()
	{
		return this.iconPath;
	}

	public uint getRecyclingXP()
	{
		return this.recyclingXP;
	}

	public uint getLevel()
	{
		return this.level;
	}

	public Buff getBuff()
	{
		return DataTables.getBuff(this.buffID);
	}

	public bool isWeapon()
	{
		return this.iAmAWeapon;
	}
}
