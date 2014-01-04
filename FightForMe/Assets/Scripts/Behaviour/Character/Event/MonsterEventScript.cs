using UnityEngine;
using System.Collections;

public class MonsterEventScript : CharacterEventScript
{
	private NPCAIScript _ai;

	void Start()
	{
		_ai = (NPCAIScript)_manager.GetInputScript();
	}

	public override void OnPain(float damage)
	{

	}

	public override void OnDeath(CharacterManager killer)
	{ // Drop our items

	}

	public override void OnSpotEntity(GameObject entity)
	{
		Debug.Log("Monster spotted an entity: " + entity.name);
		if (entity.tag == "Player" && _ai.IsSearchingEnemy())
		{
			_ai.SetTarget(entity);
		}
	}

	public override void OnCollision(Collider collider)
	{ // TODO: Pathfinding stuff
		if (collider.tag == "WayPoint")
		{
			WayPointScript wp = collider.GetComponent<WayPointScript>();
			_ai.OnWayPointCollision(wp.GetID());
		}
	}
}
