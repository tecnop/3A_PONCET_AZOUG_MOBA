using UnityEngine;
using System.Collections;

public class ArmorSet : WikiEntry
{
	private uint buffID;			// Index of the entry in the buff table completing this set enables

	private uint setSize;			// Number of items in this set
	private bool autoSize;			// If true, "setSize" will increase automatically as items are created using this set

	public ArmorSet(string name,
		string description = null,
		string description2 = null,
		string iconPath = null,
		Quality quality = Quality.COMMON,
		uint buffID = 0,
		uint setSize = 0)
		: base(name, description, description2, null, iconPath, quality)
	{
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

	public Buff GetBuff()
	{
		return DataTables.GetBuff(this.buffID);
	}

	public uint GetSize()
	{
		return this.setSize;
	}
}
