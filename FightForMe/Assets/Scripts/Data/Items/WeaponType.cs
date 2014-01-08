using UnityEngine;
using System.Collections;

public class WeaponType
{
	private string name;			// A descriptive name of the weapon type
	private string hitBoxPath;		// Path to the hitbox used by this weapon type
	private string idleAnimPath;	// Path to the humanoid idle animation used by this weapon type
	private string attackAnimPath;	// Path to the humanoid attack animation used by this weapon type

	public WeaponType(string name,
		string hitBoxPath = null,
		string idleAnimPath = null,
		string attackAnimPath = null)
	{
		this.name = name;
		this.hitBoxPath = hitBoxPath;
		this.idleAnimPath = idleAnimPath;
		this.attackAnimPath = attackAnimPath;
	}

	public string getName() { return this.name; }
	public string getHitBoxPath() { return this.hitBoxPath; }
	public string getIdleAnim() { return this.idleAnimPath; }
	public string getAttackAnim() { return this.attackAnimPath; }
}
