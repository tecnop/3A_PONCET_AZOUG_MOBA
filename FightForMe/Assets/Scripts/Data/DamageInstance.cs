using UnityEngine;
using System.Collections;

public class DamageInstance
{
	private CharacterManager inflictor;		// If null, damage was inflicted by the environment (I guess)
	private CharacterManager target;		// If null, the damage has not been applied yet

	private float damage;			// Amount of damage to inflict (may be negative for heals?)
	private uint buffID;			// ID of the buff to be inflicted
	private float buffDuration;			// Duration of the buff to be inflicted

	public DamageInstance(CharacterManager inflictor = null,
		float damage = 0,
		uint buffID = 0,
		float buffDuration = 0)
	{
		this.inflictor = inflictor;
		this.target = null;
		this.damage = damage;
		this.buffID = buffID;
		this.buffDuration = buffDuration;
	}

	private DamageInstance(DamageInstance obj)
	{
		this.inflictor = obj.inflictor;
		//this.target = obj.target;
		this.damage = obj.damage;
		this.buffID = obj.buffID;
		this.buffDuration = obj.buffDuration;
	}

	public DamageInstance ApplyToTarget(CharacterManager target)
	{
		DamageInstance newInstance = new DamageInstance(this);
		newInstance.target = target;
		return newInstance;
	}

	public void ApplyEffects()
	{
		if (!target)
		{ // Need someone to apply us to
			return;
		}

		if (inflictor)
		{ // Do something when this is not true?
			//Debug.Log(inflictor.name + " hit " + target.name + " for " + damage + " damage");
			inflictor.GetCombatScript().Damage(target, damage);
		}

		target.GetCombatScript().ReceiveBuff(inflictor, this.buffID, this.buffDuration);
	}

	public bool isPending
	{
		get
		{
			return (this.target == null);
		}
	}
}
