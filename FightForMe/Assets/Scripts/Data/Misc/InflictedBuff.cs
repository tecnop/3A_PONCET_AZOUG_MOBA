using UnityEngine;
using System.Collections;

public class InflictedBuff
{
	private uint buffID;					// ID of the entry in the Buff table that this buff was created from
	private float endTime;					// Time at which this buff will run out

	// TODO:
	//	Intensity? Other %-based stats?
	//	last tick (for periodic effects)

	private CharacterManager inflictor;		// Character responsible for this buff

	private Buff _buff;						// Associated buff in the buff table (retrieved using buffID)

	public InflictedBuff(uint buffID,
		float duration,
		CharacterManager inflictor)
	{
		this.buffID = buffID;
		this.endTime = Time.time + duration;
		this.inflictor = inflictor;

		this._buff = DataTables.getBuff(this.buffID);
	}

	public string GetName()
	{
		return _buff.GetName();
	}

	public ArrayList GetEffects()
	{
		return _buff.GetEffects();
	}

	public float GetTimeLeft()
	{
		return endTime - Time.time;
	}

	public CharacterManager GetInflictor()
	{
		return this.inflictor;
	}
}
