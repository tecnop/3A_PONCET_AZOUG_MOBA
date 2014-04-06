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

	void Start()
	{
		timeToLive = 0.2f;
		entities = new List<Collider>();
		if (!self)
		{
			self = this.gameObject;
		}
	}

	public void SetUp(CharacterManager inflictor, float radius, int layer, uint collisionSpellID)
	{
		if (!self)
		{
			self = this.gameObject;
		}

		self.layer = layer;
		this.owner = inflictor;
		_transform.localScale *= radius;

		this.collisionSpell = DataTables.GetSpell(collisionSpellID);
	}

	void OnTriggerEnter(Collider col)
	{
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
				Destroy(this.gameObject);
			}
		}
	}
}
