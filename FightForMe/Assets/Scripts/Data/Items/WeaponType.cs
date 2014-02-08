using UnityEngine;
using System.Collections;

public class WeaponType
{
	private string name;			// A descriptive name of the weapon type
	private string hitBoxPath;		// Path to the hitbox used by this weapon type
	private string idleAnimPath;	// Path to the humanoid idle animation used by this weapon type
	private string attackAnimPath;	// Path to the humanoid attack animation used by this weapon type
	private bool isRanged;			// If true, the weapon should not attempt to hit melee-ranged enemies

	public WeaponType(string name,
		string hitBoxPath = null,
		string idleAnimPath = null,
		string attackAnimPath = null,
		bool isRanged = false)
	{
		this.name = name;
		this.hitBoxPath = hitBoxPath;
		this.idleAnimPath = idleAnimPath;
		this.attackAnimPath = attackAnimPath;
		this.isRanged = isRanged;
	}

	public string GetName() { return this.name; }
	public string GetHitBoxPath() { return this.hitBoxPath; }
	public string GetIdleAnim() { return this.idleAnimPath; }
	public string GetAttackAnim() { return this.attackAnimPath; }
	public bool IsRanged() { return this.isRanged; }
}
