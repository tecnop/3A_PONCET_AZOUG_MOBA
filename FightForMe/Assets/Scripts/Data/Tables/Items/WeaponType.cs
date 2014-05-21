using UnityEngine;
using System.Collections;

public class WeaponType : WikiEntry
{
	private string hitBoxPath;		// Path to the hitbox used by this weapon type
	private string idleAnimPath;	// Path to the humanoid idle animation used by this weapon type
	private string attackAnimPath;	// Path to the humanoid attack animation used by this weapon type
	private bool isRanged;			// If true, the weapon should not attempt to hit melee-ranged enemies
	private bool isTwoHanded;		// If true, the weapon may not be used with another weapon

	public override WikiCategory category
	{
		get
		{
			return WikiCategory.WEAPON_TYPES;
		}
	}

	public WeaponType(Metadata metadata,
		string hitBoxPath = null,
		string idleAnimPath = null,
		string attackAnimPath = null,
		bool isRanged = false,
		bool isTwoHanded = true)
		: base(metadata)
	{
		this.hitBoxPath = hitBoxPath;
		this.idleAnimPath = idleAnimPath;
		this.attackAnimPath = attackAnimPath;
		this.isRanged = isRanged;
		this.isTwoHanded = isTwoHanded;
	}

	public string GetHitBoxPath() { return this.hitBoxPath; }
	public string GetIdleAnim() { return this.idleAnimPath; }
	public string GetAttackAnim() { return this.attackAnimPath; }
	public bool IsRanged() { return this.isRanged; }
	public bool IsTwoHanded() { return this.isTwoHanded; }

	public override void DrawDataWindow(float width, float height)
	{
		base.DrawDataWindow(width, height);
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);
	}
}
