using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InflictedBuff
{
	private uint buffID;					// ID of the entry in the Buff table that this buff was created from
	private float endTime;					// Time at which this buff will run out

	// TODO: Intensity? Other %-based stats?

	private CharacterManager inflictor;		// Character responsible for this buff

	private Buff _buff;						// Associated buff in the buff table (retrieved using buffID)

	public InflictedBuff(uint buffID,
		float duration,
		CharacterManager inflictor)
	{
		this.buffID = buffID;
		this.endTime = Time.time + duration;
		this.inflictor = inflictor;

		this._buff = DataTables.GetBuff(this.buffID);
	}

	public void AddToDuration(float time)
	{
		this.endTime += time;
	}

	public string GetName()
	{
		return _buff.GetName();
	}

	public List<Effect> GetEffects()
	{
		List<uint> curEffects = _buff.GetEffects();
		List<Effect> res = new List<Effect>(curEffects.Count);
		foreach (uint effect in curEffects)
		{
			res.Add(DataTables.GetEffect(effect));
		}
		return res;
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
