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

	//private float damageAmplifier;		// Percentage of damage to inflict on impact (default: 1.0f)
	private float damage;				// Damage to inflict on impact
	private float damageRadius;			// Radius of the area in which to inflict damage
	private uint buffID;				// Index of the entry in the buff table that should be inflicted upon collision
	private float buffDuration;			// Duration of the buff

	private CharacterManager owner;		// Character responsible for our actions

	private CharacterCombatScript _ownerCombat;

	private GameObject self;

	private bool paused;

	Vector3 velocity;

	public void StoreData(CharacterManager owner, int layer, float velocity, float damage = 0.0f, float damageRadius = 0.0f, uint buffID = 0, float buffDuration = 500, uint damageFlags = 0)
	{
		if (!self)
		{
			self = this.gameObject;
			_ownerCombat = owner.GetCombatScript();
		}

		self.layer = layer;

		this.damage = damage;
		this.damageRadius = damageRadius;
		this.buffID = buffID;
		this.buffDuration = buffDuration;
		_rigidBody.velocity = _transform.rotation * Vector3.forward * velocity;
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
			_ownerCombat.AreaOfEffect(_transform.position, _transform.rotation, this.damageRadius, this.damage, this.buffID);
		}
		else
		{
			Collider collider = collision.collider;
			CharacterPhysicsScript phys = collider.GetComponent<CharacterPhysicsScript>();

			if (phys)
			{ // If we collided with it then it's got to be an enemy
				CharacterManager hisManager = phys.GetManager();

				_ownerCombat.Damage(hisManager, damage, collider.ClosestPointOnBounds(_transform.position), 0);
				_ownerCombat.InflictBuff(hisManager, this.buffID, this.buffDuration);
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
