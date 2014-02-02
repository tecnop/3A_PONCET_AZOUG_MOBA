using UnityEngine;
using System.Collections;

public class DetectionSphereScript : MonoBehaviour
{
	private ArrayList entities;	// Array of entities we've already hit

	private float startTime;	// Time at which we spawned

	private CharacterManager inflictor;
	private float damage;

	private GameObject self;
	private Transform _transform;

	void Start()
	{
		startTime = Time.time;
		entities = new ArrayList();
		if (!self)
		{
			self = this.gameObject;
			_transform = self.transform;
		}
	}

	public void storeData(CharacterManager inflictor, Vector3 position, float radius, int layer, float damage = 0.0f, uint buffID = 0, uint damageFlags = 0)
	{
		if (!self)
		{
			self = this.gameObject;
			_transform = self.transform;
		}

		self.layer = layer;
		this.inflictor = inflictor;
		this.damage = damage;
		_transform.localScale *= radius;
	}

	private void applyToCharacter(CharacterManager target)
	{
		inflictor.GetCombatScript().Damage(target, damage);
	}

	void OnTriggerEnter(Collider col)
	{
		if (entities.Contains(col))
		{
			return;
		}

		entities.Add(col);

		applyToCharacter(col.GetComponent<CharacterPhysicsScript>().GetManager());
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
