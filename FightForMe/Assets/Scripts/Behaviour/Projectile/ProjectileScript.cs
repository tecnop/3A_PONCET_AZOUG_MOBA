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
	private Ability impactAbility;			// Ability to execute on impact

	private CharacterManager owner;			// Character responsible for our actions

	//private CharacterCombatScript _ownerCombat;

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

		//_ownerCombat = owner.GetCombatScript();

		this.name = projectile.GetName();
		float damage = owner.GetStatsScript().GetProjDamage();
		this.impactAbility = DataTables.GetAbility(projectile.GetImpactAbilityID());
		_rigidBody.velocity = _transform.rotation * Vector3.forward * projectile.GetSpeed();
		_graphics.LoadModel(projectile.GetModel());
		self.layer = owner.GetLayer();

		// TODO: Load more stuff from the guy's stats
		// Also use the new fields in Projectile (collision, range, lifeTime, trajectory)

		this.damageInstance = new DamageInstance(owner, damage);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!self)
		{
			Debug.LogError("ERROR: Projectile has not been initialized properly!");
			Destroy(this.gameObject);
			return;
		}

		if (this.impactAbility != null)
		{
			this.impactAbility.Execute();
		}

		Collider collider = collision.collider;
		CharacterPhysicsScript phys = collider.GetComponent<CharacterPhysicsScript>();

		if (phys)
		{ // If we collided with it then it's got to be an enemy
			CharacterManager hisManager = phys.GetManager();

			//_ownerCombat.Damage(hisManager, damage, collider.ClosestPointOnBounds(_transform.position), 0);
			//_ownerCombat.InflictBuff(hisManager, this.buffID, this.buffDuration);
			hisManager.GetCombatScript().ApplyDamageInstance(this.damageInstance);
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
