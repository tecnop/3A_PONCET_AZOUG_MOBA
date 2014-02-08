using UnityEngine;
using System.Collections;

public enum ProjectileTrajectory { Straight, Throw };

public class Projectile
{
	private string name;						// Projectile name
	private string modelPath;					// Projectile model
	private string effectPath;					// Effect to display around the model
	private string impactEffectPath;			// Effect to play upon impact

	private float damage;						// Damage to deal on impact
	private float speed;						// Projectile's speed
	private float impactRadius;					// Radius of the area around impact where entities will be affected
	private uint buffID;						// Index of the entry in the buff table that should be inflicted upon impact
	private float buffDuration;					// Duration of the buff

	//private float missileRadius;
	private Vector3 hitboxSize;					// Hitbox
	private ProjectileTrajectory trajectory;	// Trajectory type

	public Projectile(string name = "Projectile",
		string modelPath = null,
		string effectPath = null,
		string impactEffectPath = null,
		float damage = 0.0f,
		float speed = 1.0f,
		float impactRadius = 0.0f,
		uint buffID = 0,
		float buffDuration = 0,
		Vector3 hitboxSize = new Vector3(),
		ProjectileTrajectory trajectory = ProjectileTrajectory.Straight)
	{
		this.name = name;
		this.modelPath = modelPath;
		this.effectPath = effectPath;
		this.impactEffectPath = effectPath;
		this.damage = damage;
		this.speed = speed;
		this.impactRadius = impactRadius;
		this.buffID = buffID;
		this.buffDuration = buffDuration;
		this.hitboxSize = hitboxSize;
		this.trajectory = trajectory;
	}

	public string GetName() { return this.name; }
	public string GetModelPath() { return this.modelPath; }
	public string GetEffectPath() { return this.effectPath; }
	public string GetImpactEffectPath() { return this.impactEffectPath; }
	public float GetDamage() { return this.damage; }
	public float GetSpeed() { return this.speed; }
	public float GetImpactRadius() { return this.impactRadius; }
	public uint GetBuffID() { return this.buffID; }
	public Buff GetBuff() { return DataTables.GetBuff(this.buffID); } // Adding them both in this case
	public float GetBuffDuration() { return this.buffDuration; }
}
