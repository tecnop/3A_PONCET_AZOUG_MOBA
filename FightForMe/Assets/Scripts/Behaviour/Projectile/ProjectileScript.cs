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
	private NetworkView _networkView;

	private DamageInstance damage;		// Damage instance to apply to hit targets
	private float damageRadius;			// Radius of the area in which to inflict damage

	private CharacterManager owner;		// Character responsible for our actions

	private CharacterCombatScript _ownerCombat;

	private GameObject self;

	private bool paused;

	Vector3 velocity;

	// BAD
	uint pendingSetup = 0;
	int pendingLayer = 0;
	float pendingDamage = 0;

	void Start()
	{ // This is needed because Unity's network is a piece of crap
		self = this.gameObject;
		if (pendingSetup != 0)
		{
			DoSetUp((int)pendingSetup);
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			int test = (int)pendingSetup;
			stream.Serialize(ref test);
			int test2 = pendingLayer;
			stream.Serialize(ref test2);
			float test4 = pendingDamage;
			stream.Serialize(ref test4);
		}
		else
		{
			int test = 0;
			stream.Serialize(ref test);
			pendingSetup = (uint)test;
			int test2 = 0;
			stream.Serialize(ref test2);
			pendingLayer = test2;
			float test4 = 0.0f;
			stream.Serialize(ref test4);
			pendingDamage = test4;
		}
	}

	[RPC]
	private void DoSetUp(int projectileID)
	{
		if (!self)
		{
			pendingSetup = (uint)projectileID;
			return;
		}

		Projectile projectile = DataTables.GetProjectile((uint)projectileID);

		if (projectile == null)
		{ // Hmmm...
			Debug.LogWarning("Tried to setup unknown projectile " + projectileID + " on entity " + this.name);
			return;
		}

		this.name = projectile.GetName();
		this.damage = new DamageInstance(owner, projectile.GetDamage(), projectile.GetBuffID(), projectile.GetBuffDuration());
		this.damageRadius = projectile.GetImpactRadius();
		_rigidBody.velocity = _transform.rotation * Vector3.forward * projectile.GetSpeed();
		_graphics.LoadModel(projectile.GetModelPath());

		if (pendingLayer != 0)
		{
			self.layer = pendingLayer;
		}

		if (pendingDamage != 0)
		{
			//this.damage += pendingDamage; // TODO
		}
	}

	public void SetUpFromProjectile(uint projectileID)
	{
		/*if (GameData.isOnline)
		{ // No longer used as this object is entirely run by prediction
			_networkView.RPC("DoSetUp", RPCMode.All, (int)projectileID);
		}
		else*/
		{
			DoSetUp((int)projectileID);
		}
	}

	public void StoreData(CharacterManager owner, int layer, float extraDamage)
	{
		_ownerCombat = owner.GetCombatScript();

		if (!self)
		{ // Make this pending too?
			pendingDamage = extraDamage;
			pendingLayer = layer;
			return;
		}

		self.layer = layer;
		//this.damage += extraDamage; // TODO
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

				//_ownerCombat.Damage(hisManager, damage, collider.ClosestPointOnBounds(_transform.position), 0);
				//_ownerCombat.InflictBuff(hisManager, this.buffID, this.buffDuration);
				hisManager.GetCombatScript().ApplyDamageInstance(this.damage);
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
