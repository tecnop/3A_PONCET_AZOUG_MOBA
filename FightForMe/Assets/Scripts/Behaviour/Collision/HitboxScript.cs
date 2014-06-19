using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitboxScript : MonoBehaviour
{
	private List<Collider> entities;	// Array of entities we've already hit

	private float timeToLive;	// Time left to live

	private CharacterManager owner;

	private Spell collisionSpell;	// Spell to execute on each hit target

	private GameObject self;

	[SerializeField]
	private Transform _transform;

	[SerializeField]
	private ParticleSystem _particles;

	[SerializeField]
	private AudioSource _audio;

	private bool disabled; // Once the spell has been executed, this switches to true

	void Start()
	{
		entities = new List<Collider>();
		if (!self)
		{
			self = this.gameObject;
			this.timeToLive = 0.2f;
		}
	}

	public void SetUp(CharacterManager inflictor, float radius, int layer, uint collisionSpellID, float timeToLive, bool makeParent, bool particles = false)
	{
		if (!self)
		{
			self = this.gameObject;
		}

		self.layer = layer;
		this.owner = inflictor;
		_transform.localScale *= radius;
		this.timeToLive = timeToLive;
		if (makeParent)
		{
			_transform.parent = inflictor.transform;
		}

		if (particles)
		{
			_particles.Play();
		}

		this.collisionSpell = DataTables.GetSpell(collisionSpellID);
	}

	void OnTriggerEnter(Collider col)
	{
		if (disabled)
		{
			return;
		}

		if (entities.Contains(col))
		{
			return;
		}

		entities.Add(col);

		CharacterPhysicsScript phys = col.GetComponent<CharacterPhysicsScript>();
		if (phys)
		{
			CharacterManager hisManager = phys.GetManager();
			if (this.collisionSpell != null)
			{
				this.collisionSpell.Execute(this.owner, _transform.position, hisManager);

				Utils.PlaySpellSoundOnSource(this.collisionSpell, _audio);
			}
		}
	}

	void Update()
	{
		if (!GameData.gamePaused)
		{
			timeToLive -= Time.deltaTime;
			if (timeToLive <= 0)
			{ // Time's up
				if (disabled)
				{ // For good this time
					Destroy(this.gameObject);
				}
				else
				{ // Give us some time to finish playing particle effects and such
					disabled = true;
					_particles.Stop();
					timeToLive = 3.0f;
				}
			}
		}
	}
}
