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
	// TODO: Debuffs

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
		this.hitboxSize = hitboxSize;
		this.trajectory = trajectory;
	}
}
