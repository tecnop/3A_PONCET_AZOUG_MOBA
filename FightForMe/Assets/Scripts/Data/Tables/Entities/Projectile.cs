using UnityEngine;
using System.Collections;

public enum ProjectileTrajectory { Straight, Throw };
public enum ProjectileCollisionType { None, Players, World, Everything };

public class Projectile : WikiEntry
{
	private string effectPath;					// Effect to display around the model
	private string impactEffectPath;			// Effect to play upon impact

	private float damage;						// Damage to deal on impact (used only for weapon projectiles)
	private uint impactSpell;					// ID of the spell to be executed on impact
	private float speed;						// Projectile's speed

	//private float missileRadius;
	private Vector3 hitboxSize;					// Hitbox
	private float range;						// Maximum range the projectile can travel (infinite if <= 0)
	private float lifeTime;						// Maximum time in milliseconds the projectile can exist (infinite if <= 0)
	private ProjectileTrajectory trajectory;	// Trajectory type (note: with "Throw", range is used as the distance to travel and lifeTime as the time to spend in the air)
	private ProjectileCollisionType collision;	// Things we want to collide with

	public Projectile(Metadata metadata,
		string effectPath = null,
		string impactEffectPath = null,
		float damage = 0.0f,
		float speed = 1.0f,
		uint impactAbility = 0,
		Vector3 hitboxSize = new Vector3(), // Not sure why this even works
		float range = 0,
		float lifeTime = 0,
		ProjectileTrajectory trajectory = ProjectileTrajectory.Straight,
		ProjectileCollisionType collision = ProjectileCollisionType.Everything)
		: base(metadata)
	{
		this.effectPath = effectPath;
		this.impactEffectPath = effectPath;
		this.damage = damage;
		this.speed = speed;
		this.impactSpell = impactAbility;
		if (hitboxSize == Vector3.zero)
		{
			this.hitboxSize = new Vector3(1.0f, 1.0f, 1.0f);
		}
		else
		{
			this.hitboxSize = hitboxSize;
		}
		this.range = range;
		this.lifeTime = lifeTime;
		this.trajectory = trajectory;
		this.collision = collision;
	}

	public string GetEffectPath() { return this.effectPath; }
	public string GetImpactEffectPath() { return this.impactEffectPath; }
	public float GetDamage() { return this.damage; }
	public float GetSpeed() { return this.speed; }
	public uint GetImpactSpellID() { return this.impactSpell; }

	public Vector3 GetHitBoxSize() { return this.hitboxSize; }
	public float GetRange() { return this.range; }
	public float GetLifeTime() { return this.lifeTime; }
	public ProjectileTrajectory GetTrajectory() { return this.trajectory; }
	public ProjectileCollisionType GetCollisionType() { return this.collision; }
}
