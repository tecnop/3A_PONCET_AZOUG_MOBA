using UnityEngine;
using System.Collections;

public class HitboxScript : MonoBehaviour
{ // TODO: Make a datatable of hitboxes?
	private ArrayList entities;	// Array of entities we've already hit

	private float startTime;	// Time at which we spawned

	private CharacterManager owner;

	private Spell collisionSpell;	// Spell to execute on each hit target

	private GameObject self;

	[SerializeField]
	private Transform _transform;

	void Start()
	{
		startTime = Time.time;
		entities = new ArrayList();
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

	void LateUpdate()
	{
		if (Time.time < startTime + 0.2f)
		{ // No time left
			return;
		}

		Destroy(this.gameObject);
	}
}
