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

	[SerializeField]
	private ParticleSystem _particles;

	[SerializeField]
	private TrailRenderer _trail;

	private Spell impactSpell;			// Spell to execute on impact

	private CharacterManager owner;			// Character responsible for our actions

	private GameObject self;

	private bool paused;			// If true, the projectile is frozen
	private Vector3 velocity;		// If the projectile is frozen, this is its stored velocity
	private bool hadGravity;		// If the projectile is frozen, this indicates whether it was falling or not
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

		if (projectile.GetCollisionType() == ProjectileCollisionType.None)
		{ // Leave the default layer
		}
		else if (projectile.GetCollisionType() == ProjectileCollisionType.World)
		{
			self.layer = LayerMask.NameToLayer("WorldProj");
		}
		else if (projectile.GetCollisionType() == ProjectileCollisionType.Players)
		{ // TODO
		}
		else if (projectile.GetCollisionType() == ProjectileCollisionType.Everything)
		{
			if (LayerMask.LayerToName(owner.GetLayer()) == "Team1Entity")
			{
				self.layer = LayerMask.NameToLayer("Team1Proj");
			}
			else if (LayerMask.LayerToName(owner.GetLayer()) == "Team2Entity")
			{
				self.layer = LayerMask.NameToLayer("Team2Proj");
			}
			else if (LayerMask.LayerToName(owner.GetLayer()) == "NeutralEntity")
			{
				self.layer = LayerMask.NameToLayer("NeutralProj");
			}
		}

		_transform.localScale = projectile.GetHitBoxSize();

		// Ready to go!
		_rigidBody.velocity = _transform.rotation * Vector3.forward * projectile.GetSpeed();
	}

	public void ThrowAt(Vector3 pos)
	{ // Pretend we're facing it already
		_rigidBody.useGravity = true;
		float flightDuration = this.timeToLive * 0.95f; // Maybe? I don't think this will be used much anyway
		if (flightDuration == 0)
		{ // We have a variable life time (NOTE: Some manipulation needs to be done here to account for height difference)
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
				hadGravity = _rigidBody.useGravity;
				_rigidBody.velocity = Vector3.zero;
				_rigidBody.useGravity = false;
			}
		}
		else
		{
			if (paused)
			{ // Restore our velocity
				_rigidBody.velocity = velocity;
				_rigidBody.useGravity = hadGravity;
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
				else
				{ // If we're out of bounds, kill us

				}
			}
		}
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
}
