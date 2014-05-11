using UnityEngine;
using System.Collections;

public abstract class Item : WikiEntry
{
	protected uint recyclingXP;		// Experience reward for recycling this item
	protected uint buffID;			// ID of the entry in the buff table carrying this item enables

	private bool isWeapon;			// True if the item is a weapon, false if it is an armor

	protected Item(Metadata metadata,
		uint recyclingXP,
		uint buffID,
		bool isWeapon)
		: base(metadata)
	{
		this.recyclingXP = recyclingXP;
		this.buffID = buffID;
		this.isWeapon = isWeapon;
	}

	public uint GetRecyclingXP()
	{
		return this.recyclingXP;
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
