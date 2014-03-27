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

	private bool paused;

	Vector3 velocity;

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
		_rigidBody.velocity = _transform.rotation * Vector3.forward * projectile.GetSpeed();
		self.layer = owner.GetLayer();

		// TODO: Load more stuff from the guy's stats (misc effects and such)
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
	{ // TODO: When implemented, extend our lifetime when the game is paused
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
		}
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
}
