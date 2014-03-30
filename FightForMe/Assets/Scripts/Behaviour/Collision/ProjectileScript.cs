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

	private Spell impactSpell;			// Spell to execute on impact

	private CharacterManager owner;			// Character responsible for our actions

	private GameObject self;

	private bool paused;			// If true, the projectile is frozen
	private Vector3 velocity;		// If the projectile is frozen, this is its stored velocity
	private float timeToLive;		// Time left to live (infinite if 0)

	public void SetUp(CharacterManager owner, uint projectileID, uint impactSpellOverride = 0)
	{
		Projectile projectile = DataTables.GetProjectile((uint)projectileID);

		if (projectile == null)
		{ // Hmmm...
			Debug.LogWarning("Tried to setup unknown projectile " + projectileID + " on entity " + this.name);
			return;
		}

		self = this.gameObject;
		this.owner = owner;

		this.name = projectile.GetName();
		if (impactSpellOverride != 0)
		{
			this.impactSpell = DataTables.GetSpell(impactSpellOverride);
		}
		else
		{
			this.impactSpell = DataTables.GetSpell(projectile.GetImpactSpellID());
		}
		_graphics.LoadModel(projectile.GetModel());

		// TODO: Use the new fields in Projectile (collision, range, lifeTime, trajectory)
		float lifeTime = projectile.GetLifeTime();

		if (projectile.GetRange() != 0)
		{
			float lifeTime2 = projectile.GetRange() / projectile.GetSpeed();
			if (lifeTime == 0 || lifeTime2 < lifeTime)
			{
				lifeTime = lifeTime2;
			}
		}

		this.timeToLive = lifeTime;

		_rigidBody.velocity = _transform.rotation * Vector3.forward * projectile.GetSpeed();
		self.layer = owner.GetLayer();

		// TODO: Load more stuff from the guy's stats (misc effects and such)
	}

	public void ThrowAt(Vector3 pos)
	{ // Pretend we're facing it already
		_rigidBody.useGravity = true;
		float flightDuration = this.timeToLive * 0.95f; // Maybe? I don't think this will be used much anyway
		if (flightDuration == 0)
		{ // We have a variable life time
			Vector3 diff = pos - _transform.position;
			flightDuration = diff.magnitude / _rigidBody.velocity.magnitude; // I don't want to get the projectile's stats again
		}
		_rigidBody.velocity = _rigidBody.velocity - Physics.gravity * flightDuration / 2.0f;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!self)
		{
			Debug.LogError("ERROR: Projectile has not been initialized properly!");
			Destroy(this.gameObject);
			return;
		}

		Collider collider = collision.collider;
		CharacterPhysicsScript phys = collider.GetComponent<CharacterPhysicsScript>();
		CharacterManager hisManager = null;

		if (phys)
		{ // If we collided with it then it's got to be an enemy
			hisManager = phys.GetManager();
		}

		if (this.impactSpell != null)
		{
			this.impactSpell.Execute(owner, _transform.position, hisManager);
		}

		Destroy(self);
	}

	void Update()
	{
		if (GameData.gamePaused)
		{
			if (!paused)
			{ // Store our current velocity
				paused = true;
				velocity = _rigidBody.velocity;
			}
		}
		else
		{
			if (paused)
			{ // Restore our velocity
				_rigidBody.velocity = velocity;
				paused = false;
			}
			else
			{ // Actual update
				if (timeToLive > 0)
				{
					timeToLive -= Time.deltaTime;
					if (timeToLive <= 0)
					{ // Time's up
						_rigidBody.useGravity = true;
					}
				}
			}
		}
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
}
