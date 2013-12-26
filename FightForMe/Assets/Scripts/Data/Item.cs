using UnityEngine;
using System.Collections;

public abstract class Item {

	protected string name;			// Item name
	protected string description;	// Item descrition
	protected string modelPath;		// Path to the item's model
	protected string iconPath;		// Path to the item's icon
	protected uint recyclingXP;		// Experience reward for recycling this item
	protected uint level;			// Quality level of this item

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
}
