using UnityEngine;
using System.Collections;

public class DetectionSphereScript : MonoBehaviour
{
	private ArrayList entities;	// Array of entities we've already hit

	private float startTime;	// Time at which we spawned

	//private CharacterManager owner;

	private DamageInstance damageInstance;

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

	public void SetUp(CharacterManager inflictor, Vector3 position, float radius, int layer, DamageInstance damageInstance)
	{
		if (!self)
		{
			self = this.gameObject;
		}

		self.layer = layer;
		//this.owner = inflictor;

		this.damageInstance = damageInstance;

		_transform.localScale *= radius;
	}

	private void ApplyToCharacter(CharacterManager target)
	{
		target.GetCombatScript().ApplyDamageInstance(this.damageInstance);
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
			ApplyToCharacter(phys.GetManager());
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
