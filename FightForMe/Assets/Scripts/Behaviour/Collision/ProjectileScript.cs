using UnityEngine;
using System.Collections;

public class ProjectileScript : VisibleEntity
{
	[SerializeField]
	private Rigidbody _rigidBody;

	[SerializeField]
	private ParticleSystem _particles;

	[SerializeField]
	private TrailRenderer _trail;

	[SerializeField]
	private AudioSource _audio;

	private Spell impactSpell;			// Spell to execute on impact

	private CharacterManager owner;			// Character responsible for our actions

	private GameObject self;

	private bool paused;			// If true, the projectile is frozen
	private Vector3 velocity;		// If the projectile is frozen, this is its stored velocity
	private bool hadGravity;		// If the projectile is frozen, this indicates whether it was falling or not
	private float timeToLive;		// Time left to live (infinite if 0)

	private bool disabled; // Once the spell has been executed, this switches to true

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
	{ // TODO: Make this a part of the projectile set up
		_rigidBody.useGravity = true;

		Vector3 diff = pos - _transform.position;
		// NOTE: Some manipulation needs to be done here to properly account for height difference
		float flightDuration = diff.magnitude / _rigidBody.velocity.magnitude; // I don't want to get the projectile's stats again

		if (this.timeToLive > 0 && flightDuration > this.timeToLive)
		{ // Cap it
			flightDuration = this.timeToLive;
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

		if (disabled)
		{
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

			Utils.PlaySpellSoundOnSource(this.impactSpell, _audio);
		}

		disabled = true;
		timeToLive = 3.0f;
		_particles.Stop();
		_trail.enabled = false;
		this.RemoveFromGrid();
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
				if (!disabled)
				{
					if (TimeSinceLastUpdate() > 0.2f)
					{ // Update our visibility
						UpdatePositionOnGrid();
					}
				}

				if (timeToLive > 0)
				{
					timeToLive -= Time.deltaTime;
					if (timeToLive <= 0)
					{ // Time's up
						if (disabled)
						{ // We're done
							Destroy(this.gameObject);
						}
						else
						{
							_rigidBody.useGravity = true;
						}
					}
				}
				else
				{ // TODO: If we're out of bounds, kill us
					if (_rigidBody.useGravity)
					{
						timeToLive -= Time.deltaTime;
						if (timeToLive <= -5.0f)
						{ // We should have died 5 seconds ago
							Destroy(this.gameObject);
						}
					}
				}
			}
		}
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
}
