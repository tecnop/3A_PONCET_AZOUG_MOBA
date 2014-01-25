using UnityEngine;
using System.Collections;

public class ArmorSet
{
	private string name;			// Descriptive name of the set

	private uint buffID;			// Index of the entry in the buff table completing this set enables

	private uint setSize;			// Number of items in this set
	private bool autoSize;			// If true, "setSize" will increase automatically as items are created using this set

	public ArmorSet(string name,
		uint buffID = 0,
		uint setSize = 0)
	{
		this.name = name;

		this.buffID = buffID;

		this.setSize = setSize;
		this.autoSize = (setSize == 0);
	}

	public void IncreaseSetSize()
	{
		if (this.autoSize)
		{
			this.setSize++;
		}
	}

	public string GetName()
	{
		return this.name;
	}

	public Buff GetBuff()
	{
		return DataTables.getBuff(this.buffID);
	}

	public uint GetSize()
	{
		return this.setSize;
	}
}
