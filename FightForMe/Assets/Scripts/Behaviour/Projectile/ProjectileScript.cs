using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
	[SerializeField]
	private GraphicsLoader _graphics;

	[SerializeField]
	private Transform _transform;

	[SerializeField]
	private Rigidbody _rigidBody;

	private DamageInstance damageInstance;	// Damage instance to apply to hit targets
	private float damageRadius;				// Radius of the area in which to inflict damage

	private CharacterManager owner;			// Character responsible for our actions

	private CharacterCombatScript _ownerCombat;

	private GameObject self;

	private bool paused;

	Vector3 velocity;

	public void SetUp(CharacterManager owner, uint projectileID)
	{
		Projectile projectile = DataTables.GetProjectile((uint)projectileID);

		if (projectile == null)
		{ // Hmmm...
			Debug.LogWarning("Tried to setup unknown projectile " + projectileID + " on entity " + this.name);
			return;
		}

		self = this.gameObject;

		_ownerCombat = owner.GetCombatScript();

		this.name = projectile.GetName();
		float damage = projectile.GetDamage() + owner.GetStatsScript().GetDamage();
		this.damageRadius = projectile.GetImpactRadius();
		_rigidBody.velocity = _transform.rotation * Vector3.forward * projectile.GetSpeed();
		_graphics.LoadModel(projectile.GetModel());
		self.layer = owner.GetLayer();

		// Here: get more data from the guy's stats

		this.damageInstance = new DamageInstance(owner, damage, projectile.GetBuffID(), projectile.GetBuffDuration());
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!self)
		{
			Debug.LogError("ERROR: Projectile has not been initialized properly!");
			Destroy(this.gameObject);
			return;
		}

		if (this.damageRadius > 0.0f)
		{ // Even if we hit someone in particular, just run the area of effect
			_ownerCombat.AreaOfEffect(_transform.position, _transform.rotation, this.damageRadius, this.damageInstance);
		}
		else
		{
			Collider collider = collision.collider;
			CharacterPhysicsScript phys = collider.GetComponent<CharacterPhysicsScript>();

			if (phys)
			{ // If we collided with it then it's got to be an enemy
				CharacterManager hisManager = phys.GetManager();

				//_ownerCombat.Damage(hisManager, damage, collider.ClosestPointOnBounds(_transform.position), 0);
				//_ownerCombat.InflictBuff(hisManager, this.buffID, this.buffDuration);
				hisManager.GetCombatScript().ApplyDamageInstance(this.damageInstance);
			}
		}

		Destroy(self);
	}

	void Update()
	{
		if (GameData.gamePaused)
		{
			paused = true;
			velocity = _rigidBody.velocity;
		}
		else if (paused)
		{
			_rigidBody.velocity = velocity;
			paused = false;
		}
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
}
